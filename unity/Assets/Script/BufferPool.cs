using System.Collections.Generic;
using UnityEngine;


public struct Buffer
{
    public int[] shape;
    public ComputeBuffer cb;

    public int count
    {
        get
        {
            int v = 1;
            if (shape != null)
                for (int i = 0; i < shape.Length; i++)
                    v *= shape[i];
            return v;
        }
    }
}

public class BufferPool
{

    private static Dictionary<string, Buffer> buffer = new Dictionary<string, Buffer>();


    private static int GetStride<T>()
    {
        if (typeof(T) == typeof(Matrix3X3))
        {
            return 9 * sizeof(float);
        }
        return sizeof(float);
    }

    public static ComputeBuffer Get<T>(string name, params int[] shape)
    {
        var cb = Get(name);
        if (cb == null)
        {
            int count = shape[0];
            for (int i = 1; i < shape.Length; i++)
            {
                count *= shape[i];
            }
            cb = new ComputeBuffer(count, GetStride<T>());
            Buffer b = new Buffer() { cb = cb, shape = shape };
            buffer.Add(name, b);
        }
        return cb;
    }

    public static ComputeBuffer Get(string name)
    {
        if (buffer.ContainsKey(name))
        {
            return buffer[name].cb;
        }
        return null;
    }

    public static float[] GetData(string name)
    {
        if (buffer.ContainsKey(name))
        {
            int count = buffer[name].count;
            float[] rst = new float[count];
            buffer[name].cb.GetData(rst);
            return rst;
        }
        return null;
    }

    public static bool TryGet(string name, out Buffer b)
    {
        return buffer.TryGetValue(name, out b);
    }

    public static ComputeBuffer Inference(string from, string to)
    {
        var br = buffer[from];
        if (!buffer.ContainsKey(to)) buffer.Add(to, br);
        return br.cb;
    }

    public static void Release(string name)
    {
        if (buffer.ContainsKey(name))
        {
            buffer[name].cb.Release();
            buffer.Remove(name);
        }
    }

    public static void Release(ComputeBuffer cb)
    {
        foreach (var it in buffer)
        {
            if (it.Value.cb == cb)
            {
                cb.Release();
                buffer.Remove(it.Key);
                break;
            }
        }
    }

    public static void ReleaseAll()
    {
        foreach (var item in buffer)
        {
            item.Value.cb.Release();
        }
        buffer.Clear();
    }

}
