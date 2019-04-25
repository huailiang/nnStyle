using System;
using UnityEngine;

public struct Matrix3X3
{
    public float m00, m01, m02, m10, m11, m12, m20, m21, m22;

    public Matrix3X3(float arg00, float arg01, float arg02, float arg10, float arg11, float arg12, float arg20, float arg21, float arg22)
    {
        m00 = arg00; m01 = arg01; m02 = arg02;
        m10 = arg10; m11 = arg11; m12 = arg12;
        m20 = arg20; m21 = arg21; m22 = arg22;
    }

    public Matrix3X3(float[] args)
    {
        if (args == null || args.Length != 9)
        {
            throw new Exception("Matrix3X3 construct error");
        }
        m00 = args[0]; m01 = args[1]; m02 = args[2];
        m10 = args[3]; m11 = args[4]; m12 = args[5];
        m20 = args[6]; m21 = args[7]; m22 = args[8];
    }

    public static readonly Matrix3X3 identity = new Matrix3X3(1, 0, 0, 0, 1, 0, 0, 0, 1);

    public static readonly Matrix3X3 zero = new Matrix3X3(0, 0, 0, 0, 0, 0, 0, 0, 0);

    public override string ToString()
    {
        return string.Format("{0}\t{1}\t{2}\n{3}\t{4}\t{5}\n{6}\t{7}\t{8}\n",
            m00.ToString("f4"), m01.ToString("f4"), m02.ToString("f4"),
            m10.ToString("f4"), m11.ToString("f4"), m12.ToString("f4"),
            m20.ToString("f4"), m21.ToString("f4"), m22.ToString("f4"));
    }

    public Matrix3X3 transpose
    {
        get { return new Matrix3X3(m00, m10, m20, m01, m11, m21, m02, m12, m22); }
    }

    public float Sum()
    {
        return m00 + m01 + m02 + m10 + m11 + m12 + m20 + m21 + m22;
    }

    public Vector3 GetColumn(int i)
    {
        Vector3 v3 = Vector3.zero;
        if (i == 0)
        {
            v3.x = m00; v3.y = m10; v3.z = m20;
        }
        else if (i == 1)
        {
            v3.x = m01; v3.y = m11; v3.z = m21;
        }
        else if (i == 2)
        {
            v3.x = m02; v3.y = m12; v3.z = m22;
        }
        else
        {
            throw new Exception("Matrix3X3 GetColumn index overrange");
        }
        return v3;
    }

    public Vector3 GetRow(int i)
    {
        Vector3 v3 = Vector3.zero;
        if (i == 0)
        {
            v3.x = m00; v3.y = m01; v3.z = m02;
        }
        else if (i == 1)
        {
            v3.x = m10; v3.y = m11; v3.z = m12;
        }
        else if (i == 2)
        {
            v3.x = 20; v3.y = m21; v3.z = m22;
        }
        else
        {
            throw new Exception("Matrix3X3 GetRow index overrange");
        }
        return v3;
    }

    public static Matrix3X3 operator *(Matrix3X3 x, Matrix3X3 y)
    {
        return new Matrix3X3(x.m00 * y.m00, x.m01 * y.m01, x.m02 * y.m02,
                             x.m10 * y.m10, x.m11 * y.m11, x.m12 * y.m12,
                             x.m20 * y.m20, x.m21 * y.m21, x.m22 * y.m22);
    }

    public static Matrix3X3 operator +(Matrix3X3 x, Matrix3X3 y)
    {
        return new Matrix3X3(x.m00 + y.m00, x.m01 + y.m01, x.m02 + y.m02,
                             x.m10 + y.m10, x.m11 + y.m11, x.m12 + y.m12,
                             x.m20 + y.m20, x.m21 + y.m21, x.m22 + y.m22);
    }

    public float this[int x, int y]
    {
        get
        {
            if (x >= 3 || y >= 3)
            {
                throw new Exception("Matrix3X3 this index overrange");
            }
            float rst = m00;
            if (x == 0 && y == 0) rst = m00;
            else if (x == 0 && y == 1) rst = m01;
            else if (x == 0 && y == 2) rst = m02;
            else if (x == 1 && y == 0) rst = m10;
            else if (x == 1 && y == 1) rst = m11;
            else if (x == 1 && y == 2) rst = m12;
            else if (x == 2 && y == 0) rst = m20;
            else if (x == 2 && y == 1) rst = m21;
            else if (x == 2 && y == 2) rst = m22;
            return rst;
        }
        set
        {
            if (x >= 3 || y >= 3)
            {
                throw new Exception("Matrix3X3 this index overrange");
            }
            if (x == 0 && y == 0) m00 = value;
            else if (x == 0 && y == 1) m01 = value;
            else if (x == 0 && y == 2) m02 = value;
            else if (x == 1 && y == 0) m10 = value;
            else if (x == 1 && y == 1) m11 = value;
            else if (x == 1 && y == 2) m12 = value;
            else if (x == 2 && y == 0) m20 = value;
            else if (x == 2 && y == 1) m21 = value;
            else if (x == 2 && y == 2) m22 = value;
        }
    }

}
