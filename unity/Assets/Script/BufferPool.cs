using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferPool
{
    private static Dictionary<string, ComputeBuffer> buffer = new Dictionary<string, ComputeBuffer>();

    public static ComputeBuffer Get(string name, int count, int stride)
    {
        var cb = new ComputeBuffer(count, stride);
        buffer.Add(name, cb);
        return cb;
    }

    public static ComputeBuffer Get(string name)
    {
        if (buffer.ContainsKey(name))
        {
            return buffer[name];
        }
        return null;
    }

    public static void Release(string name)
    {
        if (buffer.ContainsKey(name))
        {
            buffer[name].Release();
            buffer.Remove(name);
        }
    }

    public static void Release(ComputeBuffer cb)
    {
        foreach (var it in buffer)
        {
            if (it.Value == cb)
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
            item.Value.Release();
        }
        buffer.Clear();
    }

}
