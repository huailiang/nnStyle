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
        int max = 100;
        if (len == 3)
        {
            int x = shape[0];
            int y = shape[1];
            int z = shape[2];
            float[] array = new float[x * y * z];
            buffer.GetData(array);
            sb.AppendFormat(" shape: {0}x{1}x{2}\n", x, y, z);
            for (int j = 0; j < Mathf.Min(max * 2, y); j++)
            {
                sb.Append("[" + j + "] ");
                for (int k = 0; k < Mathf.Min(max, z); k++)
                {
                    sb.Append("\t");
                    sb.Append(array[j * z + k].ToString("f3"));
                }
                sb.Append("\n");
            }
            Debug.Log(sb);
        }
        else if (shape.Length == 2)
        {
            int x = shape[0];
            int y = shape[1];
            float[] array = new float[x * y];
            buffer.GetData(array);
            sb.AppendFormat(" shape: {0}x{1}\n", x, y);
            for (int j = 0; j < Mathf.Min(max * 2, x); j++)
            {
                string str = string.Empty;
                sb.Append("[" + j + "] ");
                for (int k = 0; k < Mathf.Min(max, y); k++)
                {
                    sb.Append("\t");
                    sb.Append(array[j * y + k].ToString("f3"));
                }
                sb.Append("\n");
            }
            Debug.Log(sb);
        }
    }

}
