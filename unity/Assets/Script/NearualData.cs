using System;
using UnityEngine;

public enum Nearual
{
    Encoder,
    Decoder
}


[Serializable]
public class BaseData
{
    public string buffer;

    public int kernel;

    public Nearual nearual;
}


public class NearualData : ScriptableObject
{
    public BaseData[] datas;

    public void OnSave()
    {
        Debug.Log("saved on editor");
    }

}