using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct Matrix3X3
{
    public float m00, m01, m02, m10, m11, m12, m20, m21, m22;

    public Matrix3X3(float arg00, float arg01, float arg02, float arg10, float arg11, float arg12, float arg20, float arg21, float arg22)
    {
        m00 = arg00; m01 = arg01; m02 = arg02;
        m10 = arg10; m11 = arg11; m12 = arg12;
        m20 = arg20; m21 = arg21; m22 = arg22;
    }

    public static readonly Matrix3X3 identity = new Matrix3X3(1, 0, 0, 0, 1, 0, 0, 0, 1);

    public static readonly Matrix3X3 zero = new Matrix3X3(0, 0, 0, 0, 0, 0, 0, 0, 0);

    public override string ToString()
    {
        return string.Format("{0}\t{1}\t{2}\n{3}\t{4}\t{5}\n{6}\t{7}\t{8}",
            m00.ToString("f3"), m01.ToString("f3"), m02.ToString("f3"),
            m10.ToString("f3"), m11.ToString("f3"), m12.ToString("f3"),
            m20.ToString("f3"), m21.ToString("f3"), m22.ToString("f3"));
    }

    public Matrix3X3 transpose
    {
        get { return new Matrix3X3(m00, m10, m20, m01, m11, m21, m02, m12, m22); }
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

    public static Matrix3X3 operator *(Matrix3X3 m1, Matrix3X3 m2)
    {
        return new Matrix3X3(m1.m00 * m2.m00, m1.m01 * m2.m01, m1.m02 * m2.m02,
                             m1.m10 * m2.m10, m1.m11 * m2.m11, m1.m12 * m2.m12,
                             m1.m20 * m2.m20, m1.m21 * m2.m21, m1.m22 * m2.m22);
    }

    public static Matrix3X3 operator +(Matrix3X3 m1, Matrix3X3 m2)
    {
        return new Matrix3X3(m1.m00 + m2.m00, m1.m01 + m2.m01, m1.m02 + m2.m02,
                             m1.m10 + m2.m10, m1.m11 + m2.m11, m1.m12 + m2.m12,
                             m1.m20 + m2.m20, m1.m21 + m2.m21, m1.m22 + m2.m22);
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
