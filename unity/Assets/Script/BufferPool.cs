using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferPool
{
    private static List<ComputeBuffer> buffer = new List<ComputeBuffer>();

    public static ComputeBuffer Get(int count, int stride)
    {
        var cb = new ComputeBuffer(count, stride);
        buffer.Add(cb);
        return cb;
    }

    public static void Release(ComputeBuffer cb)
    {
        if (buffer.Contains(cb))
        {
            buffer.Remove(cb);
        }
        cb.Release();
        cb = null;
    }

    public static void ReleaseAll()
    {
        for (int i = 0; i < buffer.Count; i++)
        {
            buffer[i].Release();
        }
        buffer.Clear();
    }

}
