using System.Collections.Generic;
using UnityEngine;

public class ActiveCompute : MonoBehaviour
{
    public ComputeShader shader;
    public Color tintColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private RenderTexture tempDestination = null;
    private int handleActiveMain, handleBufferMain;
    private ComputeBuffer appendBuffer, consumeBuffer, countBuffer;
    private Renderer tempRender = null;


    void Start()
    {
        tempRender = GetComponent<Renderer>();
        handleActiveMain = shader.FindKernel("ActiveMain");
        handleBufferMain = shader.FindKernel("BufferMain");
        if (handleActiveMain < 0 || handleBufferMain < 0)
        {
            Debug.Log("Initialization failed ");
            enabled = false;
        }
        appendBuffer = new ComputeBuffer(64, sizeof(float), ComputeBufferType.Append);
        appendBuffer.SetCounterValue(0);
        consumeBuffer = new ComputeBuffer(64, sizeof(int), ComputeBufferType.Append);
        consumeBuffer.SetCounterValue(0);
        consumeBuffer.SetData(new int[] { 97, 98, 99 });
        consumeBuffer.SetCounterValue(3);
        shader.SetBuffer(handleBufferMain, "appendBuffer", appendBuffer);
        shader.SetBuffer(handleBufferMain, "consumeBuffer", consumeBuffer);
    }


    void OnDestroy()
    {
        if (tempDestination != null)
        {
            tempDestination.Release();
            tempDestination = null;
        }
        if (appendBuffer != null)
        {
            appendBuffer.Release();
            appendBuffer = null;
        }
        if (consumeBuffer != null)
        {
            consumeBuffer.Release();
            consumeBuffer = null;
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 140, 40), "Active function"))
        {
            if (null == shader || handleActiveMain < 0) return;
            if (null == tempDestination)
            {
                if (null != tempDestination)
                {
                    tempDestination.Release();
                }
                tempDestination = new RenderTexture(256, 256, 0);
                tempDestination.enableRandomWrite = true;
                tempDestination.Create();
            }

            shader.SetTexture(handleBufferMain, "Destination", tempDestination);
            shader.SetTexture(handleActiveMain, "Destination", tempDestination);
            shader.SetVector("Color", (Vector4)tintColor);
            shader.SetBuffer(handleActiveMain, "buffer2", appendBuffer);
            shader.SetBuffer(handleBufferMain, "buffer2", appendBuffer);
            shader.Dispatch(handleActiveMain, tempDestination.width / 8, tempDestination.height / 8, 1);
            shader.Dispatch(handleBufferMain, tempDestination.width / 8, tempDestination.height / 8, 1);
            tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
        }
        if (GUI.Button(new Rect(20, 100, 140, 40), "Append Consume"))
        {
            shader.Dispatch(handleBufferMain, 8 / 8, 1, 1);
            var countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
            ComputeBuffer.CopyCount(appendBuffer, countBuffer, 0);
            int[] counter = new int[1] { 0 };
            countBuffer.GetData(counter);
            int count = counter[0];
            Debug.Log("append buffer count: " + count);
            var data = new int[count];
            appendBuffer.GetData(data);
            Debug.Log("data length: " + data.Length);
            string str = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                str += " " + data[i];
            }
            Debug.Log(str);
            ComputeBuffer.CopyCount(consumeBuffer, countBuffer, 0);
            countBuffer.GetData(counter);
            Debug.Log("consume buffer count: " + counter[0]);
            countBuffer.Dispose();
        }
    }


}