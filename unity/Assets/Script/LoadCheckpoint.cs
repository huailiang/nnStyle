using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadCheckpoint
{
    Dictionary<string, float[]> v1_map = new Dictionary<string, float[]>();
    Dictionary<string, Matrix3X3[]> v3_map = new Dictionary<string, Matrix3X3[]>();

    public void Load(Action<Dictionary<string, float[]>, Dictionary<string, Matrix3X3[]>> callback)
    {
        string path = Application.dataPath + "/Resources/args.bytes";
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        var reader = new BinaryReader(fs);
        Read(reader);
        reader.Close();
        fs.Close();
        if (callback != null)
        {
            callback(v1_map, v3_map);
        }
    }


    private void Read(BinaryReader reader)
    {
        while (true)
        {
            short shape = reader.ReadInt16();
            if (shape == 1)
            {
                Read_v1(reader);
            }
            else if (shape == 4)
            {
                Read_v4(reader);
            }
            else
            {
                Debug.Log(shape == 0 ? "read end with success!" : "not parse shape" + shape);
                break;
            }
        }
    }

    private string ReadString(BinaryReader reader)
    {
        short len = reader.ReadInt16();
        char[] key = reader.ReadChars(len);
        return new string(key);
    }

    private void Read_v1(BinaryReader reader)
    {
        string key = ReadString(reader);
        short len = reader.ReadInt16();
        v1_map.Add(key, new float[len]);
        for (int i = 0; i < len; i++)
        {
            v1_map[key][i] = reader.ReadSingle();
        }
    }

    private void Read_v4(BinaryReader reader)
    {
        string key = ReadString(reader);
        short input = reader.ReadInt16();
        short output = reader.ReadInt16();
        short height = reader.ReadInt16();
        short width = reader.ReadInt16();

        if (height == 3 && width == 3)
        {
            v3_map.Add(key, new Matrix3X3[input * output]);
            for (int i = 0; i < input; i++)
            {
                for (int j = 0; j < output; j++)
                {
                    Matrix3X3 mat = Matrix3X3.zero;
                    for (int k = 0; k < height; k++)
                    {
                        for (int l = 0; l < width; l++)
                        {
                            mat[l, k] = reader.ReadSingle();
                        }
                    }
                    v3_map[key][i * output + j] = mat;
                }
            }
        }
        else //7X7
        {
            int indx = 0;
            float[] array = new float[input * output * height * width];
            for (int i = 0; i < input; i++) //input
            {
                for (int j = 0; j < output; j++) //output
                {
                    for (int k = 0; k < height; k++)
                    {
                        for (int l = 0; l < width; l++)
                        {
                            array[indx++] = reader.ReadSingle();
                        }
                    }
                    v1_map[key] = array;
                }
            }
            // Debug.LogWarning(key + " is not handle v4 data with kernel shape: " + s3 + "x" + s4);
        }
    }


}
