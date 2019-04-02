using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCompute : MonoBehaviour
{

    struct PBuffer
    {
        public float scale;
        public float axi;
    }

    public ComputeShader shader;
    private RenderTexture midDestination = null;
    private RenderTexture tempDestination = null;
    public Renderer tempRender = null;
    public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public float scale = 1;
    public float axi = 1;
    private ComputeBuffer buffer, buffer2;
    private int handleActiveMain;
    private int handleLastMain;
    private PBuffer[] values;

    void Start()
    {
        handleActiveMain = shader.FindKernel("ActiveMain");
        handleLastMain = shader.FindKernel("LastMain");
        if (handleActiveMain < 0 || handleLastMain < 0)
        {
            Debug.Log("Initialization failed ");
            enabled = false;
        }

        buffer = new ComputeBuffer(1, 2 * sizeof(float), ComputeBufferType.Append);
        values = new PBuffer[1];
        values[0].scale = scale;
        values[0].axi = axi;
        buffer.SetData(values);
        int len = 256;
        buffer2 = new ComputeBuffer(len, sizeof(float));
        buffer2.SetData(new float[len]);
    }

    void Update()
    {
        if (values != null)
        {
            values[0].scale = scale;
            values[0].axi = axi;
            buffer.SetData(values);
        }
    }

    void OnDestroy()
    {
        if (tempDestination != null)
        {
            tempDestination.Release();
            tempDestination = null;
        }
        if (midDestination != null)
        {
            midDestination.Release();
            midDestination = null;
        }
        if (buffer != null)
        {
            buffer.Release();
            buffer = null;
        }
        if (buffer2 != null)
        {
            buffer2.Release();
            buffer2 = null;
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

                midDestination = new RenderTexture(256, 256, 0);
                midDestination.enableRandomWrite = true;
                midDestination.Create();
            }

            shader.SetTexture(handleLastMain, "Destination", tempDestination);
            shader.SetTexture(handleActiveMain, "Destination", tempDestination);
            shader.SetTexture(handleLastMain, "Middletion", midDestination);
            shader.SetTexture(handleActiveMain, "Middletion", midDestination);
            shader.SetVector("Color", (Vector4)color);
            shader.SetBuffer(handleActiveMain, "buffer", buffer);
            shader.SetBuffer(handleActiveMain, "buffer2", buffer2);
            shader.SetBuffer(handleLastMain, "buffer2", buffer2);
            shader.Dispatch(handleActiveMain, tempDestination.width / 8, tempDestination.height / 8, 1);
            shader.Dispatch(handleLastMain, tempDestination.width / 8, tempDestination.height / 8, 1);
            tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
        }
    }


}