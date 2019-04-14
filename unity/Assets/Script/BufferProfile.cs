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
            sb.AppendFormat(" shape: {0}x{1}x{2} indx\n", x, y, z, x / 2);

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

}
