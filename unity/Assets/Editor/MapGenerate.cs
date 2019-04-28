using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArgData))]
public class MapGenerate : Editor
{
    private ArgData data;
    private Model model;
    private List<Kernel> list;
    

    [MenuItem("Tools/GenerateMap")]
    static void CreateFaceData()
    {
        string path = "Assets/Resources/map.asset";
        if (!File.Exists(path))
        {
            ArgData asset = ScriptableObject.CreateInstance<ArgData>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.Refresh();
        }
        else
        {
            EditorUtility.DisplayDialog("tip", "asset exists", "ok");
        }
    }

    private void OnEnable()
    {
        data = target as ArgData;
        SortData();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Analysis", GUILayout.MaxWidth(100)))
        {
            Analysis();
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save", GUILayout.MaxWidth(100)))
        {
            data.OnSave();
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
    }

    private void Analysis()
    {
        var enc = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Shader/StyleEncoder.compute");
        var dec = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Shader/StyleDecoder.compute");
        model = new Model(enc, dec);
        AnalyModel();
        LoadCheckpoint cpkt = new LoadCheckpoint();
        cpkt.Load(Preprocess);
        SortData();
    }


    private void Preprocess(Dictionary<string, float[]> v1, Dictionary<string, Matrix3X3[]> v3)
    {
        data.datas = new BaseData[v1.Count + v3.Count];
        var itr = v1.GetEnumerator();
        int ix = 0;
        while (itr.MoveNext())
        {
            var item = itr.Current;
            data.datas[ix] = new BaseData();
            data.datas[ix].buffer = item.Key;
            data.datas[ix].kernel = Process(item.Key, true);
            ix++;
        }
        var itr2 = v3.GetEnumerator();
        while (itr2.MoveNext())
        {
            var item = itr2.Current;
            data.datas[ix] = new BaseData();
            data.datas[ix].buffer = item.Key;
            data.datas[ix].kernel = Process(item.Key, false);
            ix++;
        }
    }


    private void AnalyModel()
    {
        if (model != null)
        {
            if (list == null) list = new List<Kernel>();
            else list.Clear();
            Type type = model.GetType();
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            foreach (FieldInfo info in type.GetFields(flag))
            {
                if (info.FieldType == typeof(Kernel))
                {
                    Kernel kernel = (Kernel)info.GetValue(model);
                    list.Add(kernel);
                }
            }
        }
    }

    private Kernel FindKernel(string n)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == n)
            {
                return list[i];
            }
        }
        return null;
    }


    private Kernel Process(string buffer, bool v1)
    {
        if (buffer.StartsWith("encoder"))
        {
            string prex = "encoder_g_e";
            string str = buffer.Substring(prex.Length, 1);
            if (v1)
            {
                return FindKernel("StyleInstance" + str);
            }
            else
            {
                return FindKernel("StyleConv" + str);
            }
        }
        else if (buffer.StartsWith("decoder_g_r"))
        {
            string prex = "decoder_g_r";
            string str = buffer.Substring(prex.Length, 5);
            if (v1)
            {
                return FindKernel("ResiduleInst" + str.Replace("bn", ""));
            }
            else
            {
                return FindKernel("ResiduleConv" + str.Replace("c", ""));
            }
        }
        else if (buffer.StartsWith("decoder"))
        {
            string prex = "decoder_g_e"; 
            string str = buffer.Substring(prex.Length, 1);
            if (v1)
            {
                if (buffer == "decoder_g_pred_c_Conv_weights")
                    return FindKernel("DecoderConv5");
                return FindKernel("DecoderInstance" + str);
            }
            else
            {
                return FindKernel("DecoderConv" + str);
            }
        }
        else
        {
            Debug.LogError("not handle " + buffer);
        }
        return null;
    }


    private void SortData()
    {
        if (data.datas != null)
        {
            List<BaseData> datas = new List<BaseData>(data.datas);
            datas.Sort(RegSort);
            data.datas = datas.ToArray();
        }
    }

    private int RegSort(BaseData x, BaseData y)
    {
        return string.Compare(x.buffer, y.buffer);
    }
}