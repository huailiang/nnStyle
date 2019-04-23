using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BufferProfile
{

    private static StringBuilder sb = new StringBuilder();

    public static void Print(string name)
    {
        Buffer buffer;
        if (BufferPool.TryGet(name, out buffer))
        {
            Print(buffer.cb, name, buffer.shape);
        }
        else
        {
            Debug.LogError("Not found buffer " + name);
        }
    }

    public static void Print(ComputeBuffer cb, string name, params int[] shape)
    {
        int dftX = shape[0] / 2;
        Print(cb, dftX, name, shape);
    }

    public static void CheckZero(string name)
    {
        Buffer buffer;
        if (BufferPool.TryGet(name, out buffer))
        {
            ComputeBuffer cb = buffer.cb;
            int[] shape = buffer.shape;
            if (shape.Length == 3)
            {
                int x = shape[0], y = shape[1], z = shape[2];
                float[] array = new float[x * y * z];
                cb.GetData(array);
                int counter = 0;
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                        for (int k = 0; k < z; k++)
                        {
                            int idx = i * y * z + j * z + k;
                            if (array[idx] > 1e-4) counter++;
                        }
                Debug.Log(name + " has none-zero counter: " + counter);
            }
        }
    }

    public static void Print(ComputeBuffer buffer, int dftX, string name, params int[] shape)
    {
        sb.Length = 0;
        int len = shape.Length;
        sb.Append("[GPU] ");
        sb.Append(name);
        if (len == 3)
        {
            int x = shape[0];
            int y = shape[1];
            int z = shape[2];
            int max_y = z <= 8 ? 80 : 20;
            int max_z = z <= 8 ? z : 14;
            float[] array = new float[x * y * z];
            buffer.GetData(array);
            sb.AppendFormat("({0}x{1}x{2})  indx:{3}\n", x, y, z, x / 2);
            for (int j = 0; j < Mathf.Min(max_y, y); j++)
            {
                sb.Append("[" + j + "] ");
                for (int k = 0; k < Mathf.Min(max_z, z); k++)
                {
                    sb.Append("\t");
                    sb.Append(array[dftX * y * z + j * z + k].ToString("f4"));
                }
                sb.Append("\n");
            }
            Debug.Log(sb);
        }
        else if (len == 1)
        {
            int max = Mathf.Min(shape[0], 40);
            sb.AppendFormat(" shape:{0}", shape[0]);
            float[] array = new float[shape[0]];
            buffer.GetData(array);
            for (int i = 0; i < max; i = i + 2)
            {
                sb.AppendFormat("\n[{0}]\t{1}\t{2}", i / 2, array[i].ToString("f4"), array[i + 1].ToString("f4"));
            }
            Debug.Log(sb);
        }
        else
        {
            Debug.LogWarning("yet not support shape not equal " + len + " tensor output!");
        }
    }


    private static float[] TransfColor(Color color)
    {
        float[] rst = new float[3];
        rst[0] = ((color.r) * 255) / 127.5f - 1.0f;
        rst[1] = ((color.g) * 255) / 127.5f - 1.0f;
        rst[2] = ((color.b) * 255) / 127.5f - 1.0f;
        return rst;
    }

    public static void NormalInst(Texture2D texture)
    {
        int width = 256, height = 256, depth = 3;
        float[] buffer = new float[width * height * depth];
        float[] statistic = new float[depth * 2];
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var color = TransfColor(texture.GetPixel(i, height - j));
                buffer[j * height * depth + i * depth] = color[0];
                buffer[j * height * depth + i * depth + 1] = color[1];
                buffer[j * height * depth + i * depth + 2] = color[2];
                statistic[0] += color[0];
                statistic[1] += Mathf.Pow(color[0], 2);
                statistic[2] += color[1];
                statistic[3] += Mathf.Pow(color[1], 2);
                statistic[4] += color[2];
                statistic[5] += Mathf.Pow(color[2], 2);
            }
        }

        int x = 128;
        string str = string.Empty;
        for (int i = 0; i < 80; i++)
        {
            int idx = x * width * depth + i * depth;
            str += string.Format("[ {0} ] \t{1}\t{2}\t{3}\n", i, buffer[idx].ToString("f3"), buffer[idx + 1].ToString("f3"), buffer[idx + 2].ToString("f3"));
        }
        for (int i = 0; i < 3; i++)
        {
            int idx = i * 2;
            int len = width * height;
            float mean = statistic[idx] / len;
            statistic[idx] = mean;
            statistic[idx + 1] = statistic[idx + 1] / len - Mathf.Pow(mean, 2);
        }
        Debug.Log(string.Format("[CPU] statistic:{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n{6}", statistic[0].ToString("f3"),
            statistic[1].ToString("f4"),
            statistic[2].ToString("f4"),
            statistic[3].ToString("f4"),
            statistic[4].ToString("f4"),
            statistic[5].ToString("f4"), str));
        float EPSILON = 1e-5f;
        float[] x_offset = { 0.14529803f, -0.21076468f, -0.06774432f };
        float[] x_scale = { 0.6957419f, 0.8511919f, 1.3949857f };
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var color = TransfColor(texture.GetPixel(i, height - j));
                int idx = j * width * depth + i * depth;
                float mean = statistic[0];
                float variance = statistic[1];
                float inv = 1f / Mathf.Sqrt(variance + EPSILON);
                float normalized = (color[0] - mean) * inv;
                float scale = x_scale[0];
                float offset = x_offset[0];
                buffer[idx] = scale * normalized + offset;

                idx++;
                mean = statistic[2];
                variance = statistic[3];
                inv = 1f / Mathf.Sqrt(variance + EPSILON);
                normalized = (color[1] - mean) * inv;
                scale = x_scale[1];
                offset = x_offset[1];
                buffer[idx] = scale * normalized + offset;

                idx++;
                mean = statistic[4];
                variance = statistic[5];
                inv = 1f / Mathf.Sqrt(variance + EPSILON);
                normalized = (color[2] - mean) * inv;
                scale = x_scale[2];
                offset = x_offset[2];
                buffer[idx] = scale * normalized + offset;
            }
        }

        x = 256 / 2;
        str = "style transf arg:\n";
        for (int i = 0; i < 80; i++)
        {
            int idx = x * width * depth + i * depth;
            str += string.Format("[ {0} ] \t{1}\t{2}\t{3}\n", i, buffer[idx].ToString("f3"), buffer[idx + 1].ToString("f3"), buffer[idx + 2].ToString("f3"));
        }
        Debug.Log(str);
    }

    private static int OrderIndex(int x, int y, int z, uint width, uint depth)
    {
        return (int)(width * depth * x + y * depth + z);
    }

    public static float[] OrderSeq(int x, int y, int z, uint width, uint depth, ref float[] layer)
    {
        float[] m = new float[9];
        m[0] = layer[OrderIndex(x, y, z, width, depth)];
        m[1] = layer[OrderIndex(x + 1, y, z, width, depth)];
        m[2] = layer[OrderIndex(x + 2, y, z, width, depth)];
        m[3] = layer[OrderIndex(x, y + 1, z, width, depth)];
        m[4] = layer[OrderIndex(x + 1, y + 1, z, width, depth)];
        m[5] = layer[OrderIndex(x + 2, y + 1, z, width, depth)];
        m[6] = layer[OrderIndex(x, y + 2, z, width, depth)];
        m[7] = layer[OrderIndex(x + 1, y + 2, z, width, depth)];
        m[8] = layer[OrderIndex(x + 2, y + 2, z, width, depth)];
        return m;
    }

    //inpput 284x284x32 weights: 32x32x3x3
    public static void Conv2(float[] layer, Matrix3X3[] weights)
    {
        uint input = 284, output = 141, depth1 = 32, depth2 = 32, stride = 2;
        float[] array = new float[output * output * depth2];
        for (int i = 0; i < output; i++)
            for (int j = 0; j < output; j++)
                for (int d2 = 0; d2 < depth2; d2++)
                {
                    float v = 0f;
                    for (int d1 = 0; d1 < depth1; d1++)
                    {
                        float[] seq = OrderSeq((int)(i * stride), (int)(j * stride), d1, input, depth1, ref layer);
                        Matrix3X3 matx1 = new Matrix3X3(seq);
                        Matrix3X3 matx2 = weights[depth2 * d1 + d2];
                        v += (matx1 * matx2).Sum();
                    }
                    long indx = output * depth2 * i + depth2 * j + d2;
                    array[indx] = v;
                }

        sb.Length = 0;
        sb.Append("output:\n");
        for (int j = 0; j < 20; j++)
        {
            sb.Append("[" + j + "] ");
            for (int k = 0; k < 12; k++)
            {
                sb.Append("\t");
                sb.Append(array[70 * output * depth2 + j * depth2 + k].ToString("f4"));
            }
            sb.Append("\n");
        }
        Debug.Log(sb);
        CalcuteNormal(array, (int)output, (int)depth2);
    }

    public static void CalcuteNormal(float[] array, int nwidth, int depth)
    {
        long len = nwidth * nwidth;
        float mean = 0;
        for (int i = 0; i < nwidth; i++)
            for (int j = 0; j < nwidth; j++)
            {
                int idx = (int)(i * nwidth * depth + j * depth);
                mean += array[idx];
            }
        
        Debug.Log(string.Format("mean length:{0} mean:{1}\n", len, mean));
    }

    public static void CalNormal(float[] array)
    {
        uint nwidth = 141, width = 32, depth = 32, scale = nwidth / width;
        float mean = 0;
        for (uint y = 0; y < width; y++)
        {
            int nix = (int)(y * depth);
            uint itvl = y < nwidth % width ? scale + 1 : scale;
            float g_cache = 0;
            for (uint i = 0; i < nwidth; i++)
            {
                for (uint j = 0; j < itvl; j++)
                {
                    uint idx = i * nwidth * depth + (y + width * j) * depth;
                    g_cache += array[idx];
                }
            }
            mean += g_cache;
        }
        Debug.Log(string.Format("[{0}]\t{1}\n", 0, mean.ToString("f4")));
    }

    static void DebugList(List<int> list, float[] array)
    {
        list.Sort();
        sb.Length = 0;
        sb.AppendFormat("list len:{0}", list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            if (i % 12 == 0) sb.AppendFormat("\n[{0}]\t", (i / 12));
            sb.Append(list[i]);
            sb.Append("\t");
        }
        Debug.Log(sb);
    }
}