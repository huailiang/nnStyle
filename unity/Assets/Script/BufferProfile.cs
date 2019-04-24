﻿using System.Text;
using UnityEngine;
using System;

public class BufferProfile
{

    private static StringBuilder sb = new StringBuilder();

    private static void LookupPool(string name, Action<ComputeBuffer, string, int[]> callback)
    {
        Buffer buffer;
        if (BufferPool.TryGet(name, out buffer))
        {
            callback(buffer.cb, name, buffer.shape);
        }
        else
        {
            Debug.LogError("Not found buffer " + name);
        }
    }

    public static void Print(string name)
    {
        LookupPool(name, Print);
    }

    public static void Print(ComputeBuffer cb, string name, params int[] shape)
    {
        int dftX = shape[0] / 2;
        Print(cb, dftX, name, shape);
    }
    
    public static void Print(ComputeBuffer buffer, int dftX, string name, params int[] shape)
    {
        HandleLog(name, (x, y, z) =>
        {
            int max_y = z <= 8 ? 80 : 20;
            int max_z = z <= 8 ? z : 14;
            float[] array = new float[x * y * z];
            buffer.GetData(array);
            sb.AppendFormat("({0}x{1}x{2})  indx:{3}\n", x, y, z, x / 2);
            for (int i = 0; i < Mathf.Min(max_y, y); i++)
            {
                sb.Append("[" + i + "] ");
                for (int j = 0; j < Mathf.Min(max_z, z); j++)
                {
                    sb.Append("\t");
                    sb.Append(array[dftX * y * z + i * z + j].ToString("f4"));
                }
                sb.Append("\n");
            }
        }, (x) => LogV1(x, buffer), shape);
    }
    
    public static void PrintH(string name)
    {
        LookupPool(name, PrintH);
    }

    public static void PrintH(string name, int dftZ)
    {
        LookupPool(name, (buffer, na, shape) => PrintH(buffer, name, dftZ, shape));
    }

    public static void PrintH(ComputeBuffer buffer, string name, params int[] shape)
    {
        PrintH(buffer, name, -1, shape);
    }

    public static void PrintH(ComputeBuffer buffer, string name, int dftZ, params int[] shape)
    {
        HandleLog(name, (x, y, z) =>
        {
            int max_x = 20;
            int max_y = 14;
            if (dftZ == -1) dftZ = z / 2;
            float[] array = new float[x * y * z];
            buffer.GetData(array);
            sb.AppendFormat("({0}x{1}x{2})  indx:{3}\n", x, y, z, dftZ);
            for (int i = 0; i < Mathf.Min(max_x, x); i++)
            {
                sb.Append("[" + i + "] ");
                for (int j = 0; j < Mathf.Min(max_y, y); j++)
                {
                    sb.Append("\t");
                    sb.Append(array[i * y * z + j * z + dftZ].ToString("f4"));
                }
                sb.Append("\n");
            }
        }, (x) => LogV1(x, buffer), shape);
    }

    private static void HandleLog(string name, Action<int, int, int> v3, Action<int> v1, params int[] shape)
    {
        sb.Length = 0;
        int len = shape.Length;
        sb.Append("[GPU] ");
        sb.Append(name);
        if (len == 3)
        {
            v3(shape[0], shape[1], shape[2]);
            Debug.Log(sb);
        }
        else if (len == 1)
        {
            v1(shape[0]);
            Debug.Log(sb);
        }
        else
        {
            Debug.LogWarning("yet not support shape not equal " + len + " tensor output!");
        }
    }

    private static void LogV1(int x, ComputeBuffer buffer)
    {
        int max = Mathf.Min(x, 40);
        sb.AppendFormat(" shape:{0}", x);
        float[] array = new float[x];
        buffer.GetData(array);
        for (int i = 0; i < max; i = i + 2)
        {
            sb.AppendFormat("\n[{0}]\t{1}\t{2}", i / 2, array[i].ToString("f4"), array[i + 1].ToString("f4"));
        }
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
    
    public static void CalcuteNormal(float[] array, int width, int depth)
    {
        long len = width * width;
        float[] statistic = new float[depth * 2];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < width; j++)
                for (int k = 0; k < depth; k++)
                {
                    int idx = (int)(i * width * depth + j * depth + k);
                    statistic[k * 2] += array[idx];
                    statistic[k * 2 + 1] += Mathf.Pow(array[idx], 2);
                }

        sb.Length = 0;
        sb.AppendFormat("statistic length:{0}\n", len);
        for (int k = 0; k < depth; k++)
        {
            float mean = statistic[k * 2] / len;
            float qrt = statistic[k * 2 + 1] / len - mean * mean;
            sb.AppendFormat("[{0}]\t{1}\t{2}\n", k, mean.ToString("f4"), qrt.ToString("f4"));
        }
        Debug.Log(sb);
    }
}