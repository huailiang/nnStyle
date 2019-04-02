using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleProcess : MonoBehaviour
{
    public ComputeShader encoderShader, decoderShader;
    public Renderer tempRender = null;
    private RenderTexture tempDestination = null;
    private Texture mainTexture = null;
    private LoadCheckpoint checkpoint;
    private ComputeBuffer buffer;
    private int styleMain;
    private int enStyleConv1, enStyleNorm1, enStyleInstance1;
    private int enStyleConv2, enStyleNorm2, enStyleInstance2;
    private int enStyleConv3, enStyleNorm3, enStyleInstance3;
    private int enStyleConv4, enStyleNorm4, enStyleInstance4;
    private int enStyleConv5, enStyleNorm5, enStyleInstance5;

    private int deResidulePad1_1, deResiduleConv1_1, deResiduleNormal1_1;
    private int deResidulePad1_2, deResiduleConv1_2, deResuleNormal1_2;
    private int decoderExpand1, decoderConv1, decoderNormal1;
    private int decoderExpand2, decoderConv2, decoderNormal2;
    private int decoderExpand3, decoderConv3, decoderNormal3;
    private int decoderExpand4, decoderConv4, decoderNormal4;
    private int decoderExpand5, decoderConv5;

    private const int width = 256;

    void Start()
    {
        InitEncoder();
        InitDecoder();
        if (styleMain < 0 || enStyleConv1 < 0 || enStyleNorm1 < 0 || enStyleInstance1 < 0)
        {
            Debug.Log("Initialization failed ");
            return;
        }

        mainTexture = tempRender.sharedMaterial.mainTexture;
        tempDestination = new RenderTexture(width, width, 0);
        tempDestination.enableRandomWrite = true;
        tempDestination.Create();
        checkpoint = new LoadCheckpoint();
    }

    private void InitEncoder()
    {
        styleMain = encoderShader.FindKernel("StyleMain");
        enStyleConv1 = encoderShader.FindKernel("StyleConv1");
        enStyleNorm1 = encoderShader.FindKernel("StyleNormal1");
        enStyleInstance1 = encoderShader.FindKernel("StyleInstance1");
        enStyleConv2 = encoderShader.FindKernel("StyleConv2");
        enStyleNorm2 = encoderShader.FindKernel("StyleNormal2");
        enStyleInstance2 = encoderShader.FindKernel("StyleInstance2");
        enStyleConv3 = encoderShader.FindKernel("StyleConv3");
        enStyleNorm3 = encoderShader.FindKernel("StyleNormal3");
        enStyleInstance3 = encoderShader.FindKernel("StyleInstance3");
        enStyleConv4 = encoderShader.FindKernel("StyleConv4");
        enStyleNorm4 = encoderShader.FindKernel("StyleNormal4");
        enStyleInstance4 = encoderShader.FindKernel("StyleInstance4");
        enStyleConv5 = encoderShader.FindKernel("StyleConv5");
        enStyleNorm5 = encoderShader.FindKernel("StyleNormal5");
        enStyleInstance5 = encoderShader.FindKernel("StyleInstance5");
    }

    private void InitDecoder()
    {
        deResidulePad1_1 = decoderShader.FindKernel("ResidulePad1_1");
        deResiduleConv1_1 = decoderShader.FindKernel("ResiduleConv1_1");
        deResiduleNormal1_1 = decoderShader.FindKernel("ResiduleNormal1_1");
        deResidulePad1_2 = decoderShader.FindKernel("ResidulePad1_2");
        deResiduleConv1_2 = decoderShader.FindKernel("ResiduleConv1_2");
        deResuleNormal1_2 = decoderShader.FindKernel("ResiduleNormal1_2");
        decoderExpand1 = decoderShader.FindKernel("DecoderExpand1");
        decoderConv1 = decoderShader.FindKernel("DecoderConv1");
        decoderNormal1 = decoderShader.FindKernel("DecoderNormal1");
        decoderExpand2 = decoderShader.FindKernel("DecoderExpand2");
        decoderConv2 = decoderShader.FindKernel("DecoderConv2");
        decoderNormal2 = decoderShader.FindKernel("DecoderNormal2");
        decoderExpand3 = decoderShader.FindKernel("DecoderExpand3");
        decoderConv3 = decoderShader.FindKernel("DecoderConv3");
        decoderNormal3 = decoderShader.FindKernel("DecoderNormal3");
        decoderExpand4 = decoderShader.FindKernel("DecoderExpand4");
        decoderConv4 = decoderShader.FindKernel("DecoderConv4");
        decoderNormal4 = decoderShader.FindKernel("DecoderNormal4");
        decoderExpand5 = decoderShader.FindKernel("DecoderPad5");
        decoderConv5 = decoderShader.FindKernel("DecoderConv5");
    }

    private void OnDestroy()
    {
        if (tempDestination != null)
        {
            tempDestination.Release();
            tempDestination = null;
        }
        BufferPool.ReleaseAll();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 40, 20), "Load"))
        {
            checkpoint.Load(Process);
        }
        if (GUI.Button(new Rect(20, 50, 40, 20), "Run"))
        {
            //encoder
            encoderShader.Dispatch(styleMain, 288 / 8, 288 / 8, 1);
            encoderShader.Dispatch(enStyleConv1, 288 / 8, 288 / 8, 1);
            encoderShader.Dispatch(enStyleNorm1, 32 / 8, 1, 1);
            encoderShader.Dispatch(enStyleInstance1, 288 / 8, 288 / 8, 32 / 4);
            encoderShader.Dispatch(enStyleConv2, 144 / 8, 144 / 8, 1);
            encoderShader.Dispatch(enStyleNorm2, 32 / 8, 1, 1);
            encoderShader.Dispatch(enStyleInstance2, 144 / 8, 144 / 8, 32 / 4);
            encoderShader.Dispatch(enStyleConv3, 72 / 8, 72 / 8, 1);
            encoderShader.Dispatch(enStyleNorm3, 64 / 8, 1, 1);
            encoderShader.Dispatch(enStyleInstance3, 72 / 8, 72 / 8, 64 / 4);
            encoderShader.Dispatch(enStyleConv4, 40 / 8, 40 / 8, 1);
            encoderShader.Dispatch(enStyleNorm4, 128 / 8, 1, 1);
            encoderShader.Dispatch(enStyleInstance4, 40 / 8, 40 / 8, 128 / 4);
            encoderShader.Dispatch(enStyleConv5, 16 / 8, 16 / 8, 1);
            encoderShader.Dispatch(enStyleNorm5, 256 / 8, 1, 1);
            encoderShader.Dispatch(enStyleInstance5, 16 / 8, 16 / 8, 256 / 4);
            tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
            //decoder
            decoderShader.Dispatch(deResidulePad1_1, 24 / 8, 24 / 8, 256 / 4);
            decoderShader.Dispatch(deResiduleConv1_1, 16 / 8, 16 / 8, 1);
            decoderShader.Dispatch(deResiduleNormal1_1, 16 / 8, 16 / 8, 256 / 4);
            decoderShader.Dispatch(deResidulePad1_2, 24 / 8, 24 / 8, 256 / 4);
            decoderShader.Dispatch(deResiduleConv1_2, 16 / 8, 16 / 8, 1);
            decoderShader.Dispatch(deResuleNormal1_2, 16 / 8, 16 / 8, 256 / 4);
            decoderShader.Dispatch(decoderExpand1, 16 / 8, 16 / 8, 256 / 4);
            decoderShader.Dispatch(decoderConv1, 32 / 8, 32 / 8, 1);
            decoderShader.Dispatch(decoderNormal1, 32 / 8, 32 / 8, 256 / 4);
            decoderShader.Dispatch(decoderExpand2, 32 / 8, 32 / 8, 256 / 4);
            decoderShader.Dispatch(decoderConv2, 64 / 8, 64 / 8, 1);
            decoderShader.Dispatch(decoderNormal2, 64 / 8, 64 / 8, 128 / 4);
            decoderShader.Dispatch(decoderExpand3, 64 / 8, 64 / 8, 128 / 4);
            decoderShader.Dispatch(decoderConv3, 128 / 8, 128 / 8, 1);
            decoderShader.Dispatch(decoderNormal3, 128 / 8, 128 / 8, 64 / 4);
            decoderShader.Dispatch(decoderExpand4, 128 / 8, 128 / 8, 64 / 4);
            decoderShader.Dispatch(decoderConv4, 256 / 8, 256 / 8, 1);
            decoderShader.Dispatch(decoderNormal4, 256 / 8, 256 / 8, 32 / 4);
            decoderShader.Dispatch(decoderExpand5, 256 / 8, 256 / 8, 32 / 4);
            decoderShader.Dispatch(decoderConv5, 256 / 8, 256 / 8, 1);
        }
    }

    void Process(Dictionary<string, float[]> v1, Dictionary<string, Matrix3X3[]> v3)
    {
        foreach (var item in v1)
        {
            if (item.Key.StartsWith("encoder"))
            {
                buffer = BufferPool.Get(item.Value.Length, sizeof(float));
                buffer.SetData(item.Value);
                encoderShader.SetBuffer(enStyleInstance1, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance2, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance3, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance4, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance5, item.Key, buffer);
            }
            else if (item.Key.StartsWith("decoder"))
            {
                buffer = BufferPool.Get(item.Value.Length, sizeof(float));
                buffer.SetData(item.Value);
                decoderShader.SetBuffer(deResiduleNormal1_1, item.Key, buffer);
                decoderShader.SetBuffer(deResuleNormal1_2, item.Key, buffer);
                decoderShader.SetBuffer(decoderNormal2, item.Key, buffer);
                decoderShader.SetBuffer(decoderNormal3, item.Key, buffer);
                decoderShader.SetBuffer(decoderNormal4, item.Key, buffer);
            }
        }
        foreach (var item in v3)
        {
            if (item.Key.StartsWith("encoder"))
            {
                buffer = BufferPool.Get(item.Value.Length, 9 * sizeof(float));
                buffer.SetData(item.Value);
                encoderShader.SetBuffer(styleMain, item.Key, buffer);
                encoderShader.SetBuffer(enStyleConv1, item.Key, buffer);
                encoderShader.SetBuffer(enStyleNorm1, item.Key, buffer);
                encoderShader.SetBuffer(enStyleConv2, item.Key, buffer);
                encoderShader.SetBuffer(enStyleNorm2, item.Key, buffer);
                encoderShader.SetBuffer(enStyleConv3, item.Key, buffer);
                encoderShader.SetBuffer(enStyleNorm3, item.Key, buffer);
                encoderShader.SetBuffer(enStyleConv4, item.Key, buffer);
                encoderShader.SetBuffer(enStyleNorm4, item.Key, buffer);
                encoderShader.SetBuffer(enStyleConv5, item.Key, buffer);
                encoderShader.SetBuffer(enStyleNorm5, item.Key, buffer);
            }
            else if (item.Key.StartsWith("decoder"))
            {
                buffer = BufferPool.Get(item.Value.Length, 9 * sizeof(float));
                buffer.SetData(item.Value);
                decoderShader.SetBuffer(deResiduleConv1_1, item.Key, buffer);
                decoderShader.SetBuffer(deResiduleNormal1_1, item.Key, buffer);
                decoderShader.SetBuffer(deResiduleConv1_2, item.Key, buffer);
                decoderShader.SetBuffer(deResuleNormal1_2, item.Key, buffer);
                decoderShader.SetBuffer(decoderConv2, item.Key, buffer);
                decoderShader.SetBuffer(decoderNormal2, item.Key, buffer);
                decoderShader.SetBuffer(decoderConv3, item.Key, buffer);
                decoderShader.SetBuffer(decoderNormal3, item.Key, buffer);
                decoderShader.SetBuffer(decoderConv4, item.Key, buffer);
                decoderShader.SetBuffer(decoderNormal4, item.Key, buffer);
            }
        }
        encoderShader.SetTexture(styleMain, "source", mainTexture);
        encoderShader.SetTexture(styleMain, "destination", tempDestination);
        Debug.Log("process network args finish");
        ProcessNet();
        Debug.Log("Process neural network finsih");
    }

    void ProcessNet()
    {
        ProcessEncoder();
        ProcessDecoder();
    }

    private void ProcessEncoder()
    {
        int count = 286 * 286 * 3;
        var cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        encoderShader.SetBuffer(styleMain, "encoder_conv0", cb);
        encoderShader.SetBuffer(enStyleConv1, "encoder_conv0", cb);

        count = 284 * 284 * 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        encoderShader.SetBuffer(enStyleConv1, "encoder_conv1", cb);
        encoderShader.SetBuffer(enStyleConv2, "encoder_conv1", cb);
        encoderShader.SetBuffer(enStyleInstance1, "encoder_conv1", cb);


        count = 141 * 141 * 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        encoderShader.SetBuffer(enStyleConv2, "encoder_conv2", cb);
        encoderShader.SetBuffer(enStyleConv3, "encoder_conv2", cb);
        encoderShader.SetBuffer(enStyleInstance2, "encoder_conv2", cb);


        count = 70 * 70 * 64;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        encoderShader.SetBuffer(enStyleConv3, "encoder_conv3", cb);
        encoderShader.SetBuffer(enStyleConv4, "encoder_conv3", cb);
        encoderShader.SetBuffer(enStyleInstance3, "encoder_conv3", cb);

        count = 34 * 34 * 128;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        encoderShader.SetBuffer(enStyleConv4, "encoder_conv4", cb);
        encoderShader.SetBuffer(enStyleConv5, "encoder_conv4", cb);
        encoderShader.SetBuffer(enStyleInstance4, "encoder_conv4", cb);

        count = 16 * 16 * 256;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        encoderShader.SetBuffer(enStyleConv5, "encoder_conv5", cb);
        encoderShader.SetBuffer(enStyleInstance5, "encoder_conv5", cb);

        count = 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count * 2);
        encoderShader.SetBuffer(enStyleConv1, "encoder_conv1_statistic", cb);
        encoderShader.SetBuffer(enStyleNorm1, "encoder_conv1_statistic", cb);
        encoderShader.SetBuffer(enStyleInstance1, "encoder_conv1_statistic", cb);

        count = 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count * 2);
        encoderShader.SetBuffer(enStyleConv2, "encoder_conv2_statistic", cb);
        encoderShader.SetBuffer(enStyleNorm2, "encoder_conv2_statistic", cb);
        encoderShader.SetBuffer(enStyleInstance2, "encoder_conv2_statistic", cb);

        count = 64;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count * 2);
        encoderShader.SetBuffer(enStyleConv3, "encoder_conv3_statistic", cb);
        encoderShader.SetBuffer(enStyleNorm3, "encoder_conv3_statistic", cb);
        encoderShader.SetBuffer(enStyleInstance3, "encoder_conv3_statistic", cb);

        count = 128;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count * 2);
        encoderShader.SetBuffer(enStyleConv4, "encoder_conv4_statistic", cb);
        encoderShader.SetBuffer(enStyleNorm4, "encoder_conv4_statistic", cb);
        encoderShader.SetBuffer(enStyleInstance4, "encoder_conv4_statistic", cb);

        count = 256;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count * 2);
        encoderShader.SetBuffer(enStyleConv5, "encoder_conv5_statistic", cb);
        encoderShader.SetBuffer(enStyleNorm5, "encoder_conv5_statistic", cb);
        encoderShader.SetBuffer(enStyleInstance5, "encoder_conv5_statistic", cb);
        encoderShader.SetTexture(enStyleInstance5, "destination", tempDestination);
    }

    private void ProcessDecoder()
    {

        int count = 16 * 16 * 256;
        var cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResidulePad1_1, "input_initial", cb);

        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResiduleConv1_1, "input_writable", cb);

        count = 2 * 256;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResidulePad1_1, "input_statistic", cb);
        decoderShader.SetBuffer(deResiduleConv1_1, "input_statistic", cb);

        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResidulePad1_1, "decoder_residule", cb);
        decoderShader.SetBuffer(deResiduleConv1_1, "decoder_residule", cb);

        count = 32 * 32 * 256;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResiduleConv1_1, "decoder_conv1", cb);

        count = 32 * 32 * 256;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResiduleConv1_1, "decoder_conv1_conved", cb);
        decoderShader.SetBuffer(deResiduleNormal1_1, "decoder_conv1_conved", cb);

        count = 2 * 256;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResiduleConv1_1, "decoder_conv1_statistic", cb);
        decoderShader.SetBuffer(deResiduleNormal1_1, "decoder_conv1_statistic", cb);

        count = 64 * 64 * 128;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResiduleConv1_2, "decoder_conv2", cb);
        decoderShader.SetBuffer(decoderConv2, "decoder_conv2", cb);
        decoderShader.SetBuffer(decoderExpand2, "decoder_conv2", cb);

        count = 64 * 64 * 128;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResiduleConv1_2, "decoder_conv2_conved", cb);
        decoderShader.SetBuffer(deResiduleNormal1_1, "decoder_conv2_conved", cb);
        decoderShader.SetBuffer(decoderConv2, "decoder_conv2_conved", cb);
        decoderShader.SetBuffer(decoderNormal2, "decoder_conv2_conved", cb);

        count = 2 * 128;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(deResiduleConv1_2, "decoder_conv2_statistic", cb);
        decoderShader.SetBuffer(deResuleNormal1_2, "decoder_conv2_statistic", cb);
        decoderShader.SetBuffer(decoderConv2, "decoder_conv2_statistic", cb);
        decoderShader.SetBuffer(decoderNormal2, "decoder_conv2_statistic", cb);

        count = 128 * 128 * 64;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(decoderExpand3, "decoder_conv3", cb);
        decoderShader.SetBuffer(decoderConv3, "decoder_conv3", cb);

        count = 128 * 128 * 64;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(decoderConv3, "decoder_conv3_conved", cb);
        decoderShader.SetBuffer(decoderNormal3, "decoder_conv3_conved", cb);

        count = 2 * 64;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(decoderConv3, "decoder_conv3_statistic", cb);
        decoderShader.SetBuffer(decoderNormal3, "decoder_conv3_statistic", cb);

        count = 256 * 256 * 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(decoderExpand4, "decoder_conv4", cb);
        decoderShader.SetBuffer(decoderConv4, "decoder_conv4", cb);

        count = 256 * 256 * 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(decoderConv4, "decoder_conv4_conved", cb);
        decoderShader.SetBuffer(decoderNormal4, "decoder_conv4_conved", cb);

        count = 2 * 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(decoderConv4, "decoder_conv4_statistic", cb);
        decoderShader.SetBuffer(decoderNormal4, "decoder_conv4_statistic", cb);

        count = 262 * 262 * 32;
        cb = BufferPool.Get(count, sizeof(float));
        cb.SetCounterValue((uint)count);
        decoderShader.SetBuffer(decoderExpand5, "decoder_conv5_pad", cb);
        decoderShader.SetBuffer(decoderConv5, "decoder_conv5_pad", cb);
        decoderShader.SetTexture(decoderConv5, "decoder_destination", tempDestination);
    }

}
