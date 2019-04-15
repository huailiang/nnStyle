using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class BufferProfile
{

    private static StringBuilder sb = new StringBuilder();

    public static void Print(string name, params int[] shape)
    {
        var cb = BufferPool.Get(name);
        Print(cb, name, shape);
    }

    public static void Print(ComputeBuffer buffer, string name, params int[] shape)
    {
        sb.Length = 0;
        int len = shape.Length;
        sb.Append(name);
        if (len == 3)
        {
            int x = shape[0];
            int y = shape[1];
            int z = shape[2];
            int max_y = z <= 8 ? 80 : 20;
            int max_z = z <= 8 ? z : 10;
            float[] array = new float[x * y * z];
            buffer.GetData(array);
            sb.AppendFormat(" shape: {0}x{1}x{2} indx:{3}\n", x, y, z, x / 2);

            for (int j = 0; j < Mathf.Min(max_y, y); j++)
            {
                sb.Append("[" + j + "] ");
                for (int k = 0; k < Mathf.Min(max_z, z); k++)
                {
                    sb.Append("\t");
                    sb.Append(array[x / 2 * y * z + j * z + k].ToString("f3"));
                }
                sb.Append("\n");
            }
            Debug.Log(sb);
        }
        else
        {
            Debug.LogWarning("yet not support shape not equal 3 tensor output!");
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
        // Debug.Log(texture.GetPixel(2, 2));
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
        string str = "orig\n";
        for (int i = 0; i < 80; i++)
        {
            int idx = x * width * depth + i * depth;
            str += string.Format("[ {0} ] \t{1}\t{2}\t{3}\n", i, buffer[idx].ToString("f3"), buffer[idx + 1].ToString("f3"), buffer[idx + 2].ToString("f3"));
        }
        Debug.Log(str);

        for (int i = 0; i < 3; i++)
        {
            int idx = i * 2;
            int len = width * height;
            float mean = statistic[idx] / len;
            statistic[idx] = mean;
            statistic[idx + 1] = statistic[idx + 1] / len - Mathf.Pow(mean, 2);
        }

        float EPSILON = 1e-5f;
        float[] x_offset = { 0.12092815f, -0.11713269f, 0.17570892f };
        float[] x_scale = { 1.0396817f, 0.94713473f, 0.9952491f };
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var color = TransfColor(texture.GetPixel(i, height - j));
                int idx = j * width * depth + i * depth;
                float mean = statistic[0];
                float variance = statistic[1];
                float inv = 1f / Mathf.Sqrt(variance + EPSILON);
                float orig = (color[0] - mean) * inv;
                float scale = x_scale[0];
                float offset = x_offset[0];
                buffer[idx] = scale * color[0] + offset;

                idx++;
                mean = statistic[2];
                variance = statistic[3];
                inv = 1f / Mathf.Sqrt(variance + EPSILON);
                orig = (color[1] - mean) * inv;
                scale = x_scale[1];
                offset = x_offset[1];
                buffer[idx] = scale * color[1] + offset;

                idx++;
                mean = statistic[4];
                variance = statistic[5];
                inv = 1f / Mathf.Sqrt(variance + EPSILON);
                orig = (color[2] - mean) * inv;
                scale = x_scale[2];
                offset = x_offset[2];
                buffer[idx] = scale * color[2] + offset;
            }
        }

        x = 256 / 2;
        str = "transf\n";
        for (int i = 0; i < 80; i++)
        {
            int idx = x * width * depth + i * depth;
            str += string.Format("[ {0} ] \t{1}\t{2}\t{3}\n", i, buffer[idx].ToString("f3"), buffer[idx + 1].ToString("f3"), buffer[idx + 2].ToString("f3"));
        }
        Debug.Log(str);
    }



}
