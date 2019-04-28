using System;
using System.Collections.Generic;
using UnityEngine;

public struct Kernel
{
    public int iv;
    public string sv;
    
    public static implicit operator int(Kernel kt)
    {
        return kt.iv;
    }

    public static implicit operator string(Kernel kt)
    {
        return kt.sv;
    }

    public static implicit operator Kernel(string str)
    {
        Kernel kt = new Kernel();
        kt.sv = str;
        return kt;
    }
    
}

public class Model : IDisposable
{
    private Kernel StyleConv0, StyleNormal0, StyleInstance0, StylePad, StyleConv1, StyleNormal1, StyleInstance1, StyleConv2, StyleNormal2, StyleInstance2;
    private Kernel StyleConv3, StyleNormal3, StyleInstance3, StyleConv4, StyleNormal4, StyleInstance4, StyleConv5, StyleNormal5, StyleInstance5;
    private Kernel ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleInst1_1, ResidulePad1_2, ResiduleConv1_2, ResiduleNormal1_2, ResiduleInst1_2;
    private Kernel DecoderExpand1, DecoderConv1, DecoderNormal1, DecoderInstance1, DecoderExpand2, DecoderConv2, DecoderNormal2, DecoderInstance2;
    private Kernel DecoderExpand3, DecoderConv3, DecoderNormal3, DecoderInstance3, DecoderExpand4, DecoderConv4, DecoderNormal4, DecoderInstance4, DecoderPad5, DecoderConv5;

    private ComputeShader encoderShader, decoderShader;
    private ComputeBuffer buffer;
    private Renderer tempRender = null;
    private RenderTexture tempDestination = null;
    private Texture mainTexture = null;
    private const int width = 256;

    private Kernel BindKernel(ComputeShader shader, string name)
    {
        Kernel kernel = name;
        kernel.iv = shader.FindKernel(name);
        return kernel;
    }

    public Model(ComputeShader encoder, ComputeShader decoder)
    {
        encoderShader = encoder;
        decoderShader = decoder;
        InitEncoder();
        InitDecoder();
    }

    public void BindRender(Renderer temp)
    {
        tempRender = temp;
        mainTexture = tempRender.sharedMaterial.mainTexture;
        tempDestination = new RenderTexture(width, width, 0);
        tempDestination.enableRandomWrite = true;
        tempDestination.Create();
    }

    public void Dispose()
    {
        if (tempDestination != null)
        {
            tempDestination.Release();
            tempDestination = null;
        }
        BufferPool.ReleaseAll();
    }

    private void InitEncoder()
    {
        StyleConv0 = BindKernel(encoderShader, "StyleConv0");
        StyleNormal0 = BindKernel(encoderShader, "StyleNormal0");
        StyleInstance0 = BindKernel(encoderShader, "StyleInstance0");
        StylePad = BindKernel(encoderShader, "StylePad");
        StyleConv1 = BindKernel(encoderShader, "StyleConv1");
        StyleNormal1 = BindKernel(encoderShader, "StyleNormal1");
        StyleInstance1 = BindKernel(encoderShader, "StyleInstance1");
        StyleConv2 = BindKernel(encoderShader, "StyleConv2");
        StyleNormal2 = BindKernel(encoderShader, "StyleNormal2");
        StyleInstance2 = BindKernel(encoderShader, "StyleInstance2");
        StyleConv3 = BindKernel(encoderShader, "StyleConv3");
        StyleNormal3 = BindKernel(encoderShader, "StyleNormal3");
        StyleInstance3 = BindKernel(encoderShader, "StyleInstance3");
        StyleConv4 = BindKernel(encoderShader, "StyleConv4");
        StyleNormal4 = BindKernel(encoderShader, "StyleNormal4");
        StyleInstance4 = BindKernel(encoderShader, "StyleInstance4");
        StyleConv5 = BindKernel(encoderShader, "StyleConv5");
        StyleNormal5 = BindKernel(encoderShader, "StyleNormal5");
        StyleInstance5 = BindKernel(encoderShader, "StyleInstance5");
    }

    private void InitDecoder()
    {
        ResidulePad1_1 = BindKernel(decoderShader, "ResidulePad1_1");
        ResiduleConv1_1 = BindKernel(decoderShader, "ResiduleConv1_1");
        ResiduleNormal1_1 = BindKernel(decoderShader, "ResiduleNormal1_1");
        ResiduleInst1_1 = BindKernel(decoderShader, "ResiduleInst1_1");
        ResidulePad1_2 = BindKernel(decoderShader, "ResidulePad1_2");
        ResiduleConv1_2 = BindKernel(decoderShader, "ResiduleConv1_2");
        ResiduleNormal1_2 = BindKernel(decoderShader, "ResiduleNormal1_2");
        ResiduleInst1_2 = BindKernel(decoderShader, "ResiduleInst1_2");
        DecoderExpand1 = BindKernel(decoderShader, "DecoderExpand1");
        DecoderConv1 = BindKernel(decoderShader, "DecoderConv1");
        DecoderNormal1 = BindKernel(decoderShader, "DecoderNormal1");
        DecoderInstance1 = BindKernel(decoderShader, "DecoderInstance1");
        DecoderExpand2 = BindKernel(decoderShader, "DecoderExpand2");
        DecoderConv2 = BindKernel(decoderShader, "DecoderConv2");
        DecoderNormal2 = BindKernel(decoderShader, "DecoderNormal2");
        DecoderInstance2 = BindKernel(decoderShader, "DecoderInstance2");
        DecoderExpand3 = BindKernel(decoderShader, "DecoderExpand3");
        DecoderConv3 = BindKernel(decoderShader, "DecoderConv3");
        DecoderNormal3 = BindKernel(decoderShader, "DecoderNormal3");
        DecoderInstance3 = BindKernel(decoderShader, "DecoderInstance3");
        DecoderExpand4 = BindKernel(decoderShader, "DecoderExpand4");
        DecoderConv4 = BindKernel(decoderShader, "DecoderConv4");
        DecoderNormal4 = BindKernel(decoderShader, "DecoderNormal4");
        DecoderInstance4 = BindKernel(decoderShader, "DecoderInstance4");
        DecoderPad5 = BindKernel(decoderShader, "DecoderPad5");
        DecoderConv5 = BindKernel(decoderShader, "DecoderConv5");
    }

    public void DrawEncoder()
    {
        encoderShader.Dispatch(StyleConv0, width / 8, width / 8, 1);
        encoderShader.Dispatch(StyleNormal0, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance0, 256 / 8, 256 / 8, 1);
        encoderShader.Dispatch(StylePad, 288 / 8, 288 / 8, 1);
        encoderShader.Dispatch(StyleConv1, 288 / 8, 288 / 8, 1);
        encoderShader.Dispatch(StyleNormal1, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance1, 288 / 8, 288 / 8, 32 / 4);
        encoderShader.Dispatch(StyleConv2, 144 / 8, 144 / 8, 1);
        encoderShader.Dispatch(StyleNormal2, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance2, 144 / 8, 144 / 8, 32 / 4);
        encoderShader.Dispatch(StyleConv3, 72 / 8, 72 / 8, 1);
        encoderShader.Dispatch(StyleNormal3, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance3, 72 / 8, 72 / 8, 64 / 4);
        encoderShader.Dispatch(StyleConv4, 40 / 8, 40 / 8, 1);
        encoderShader.Dispatch(StyleNormal4, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance4, 40 / 8, 40 / 8, 128 / 4);
        encoderShader.Dispatch(StyleConv5, 16 / 8, 16 / 8, 1);
        encoderShader.Dispatch(StyleNormal5, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance5, 16 / 8, 16 / 8, 256 / 4);
    }

    public void DrawResidule()
    {
        decoderShader.Dispatch(ResidulePad1_1, 24 / 8, 24 / 8, 256 / 4);
        decoderShader.Dispatch(ResiduleConv1_1, 16 / 8, 16 / 8, 1);
        decoderShader.Dispatch(ResiduleNormal1_1, 1, 1, 1);
        decoderShader.Dispatch(ResiduleInst1_1, 16 / 8, 16 / 8, 256 / 4);
        decoderShader.Dispatch(ResidulePad1_2, 24 / 8, 24 / 8, 256 / 4);
        decoderShader.Dispatch(ResiduleConv1_2, 16 / 8, 16 / 8, 1);
        decoderShader.Dispatch(ResiduleNormal1_2, 1, 1, 1);
        decoderShader.Dispatch(ResiduleInst1_2, 16 / 8, 16 / 8, 256 / 4);
    }

    public void DrawDecoder()
    {
        decoderShader.Dispatch(DecoderExpand1, 16 / 8, 16 / 8, 256 / 4);
        decoderShader.Dispatch(DecoderConv1, 32 / 8, 32 / 8, 1);
        decoderShader.Dispatch(DecoderNormal1, 1, 1, 1);
        decoderShader.Dispatch(DecoderInstance1, 32 / 8, 32 / 8, 256 / 4);
        decoderShader.Dispatch(DecoderExpand2, 32 / 8, 32 / 8, 256 / 4);
        decoderShader.Dispatch(DecoderConv2, 64 / 8, 64 / 8, 1);
        decoderShader.Dispatch(DecoderNormal2, 1, 1, 1);
        decoderShader.Dispatch(DecoderInstance2, 64 / 8, 64 / 8, 128 / 4);
        decoderShader.Dispatch(DecoderExpand3, 64 / 8, 64 / 8, 128 / 4);
        decoderShader.Dispatch(DecoderConv3, 128 / 8, 128 / 8, 1);
        decoderShader.Dispatch(DecoderNormal3, 1, 1, 1);
        decoderShader.Dispatch(DecoderInstance3, 128 / 8, 128 / 8, 64 / 4);
        decoderShader.Dispatch(DecoderExpand4, 128 / 8, 128 / 8, 64 / 4);
        decoderShader.Dispatch(DecoderConv4, 256 / 8, 256 / 8, 1);
        decoderShader.Dispatch(DecoderNormal4, 1, 1, 1);
        decoderShader.Dispatch(DecoderInstance4, 256 / 8, 256 / 8, 32 / 4);
        DrawRenderTexure();
    }

    public void DrawRenderTexure()
    {
        decoderShader.Dispatch(DecoderPad5, 264 / 8, 264 / 8, 32 / 4);
        decoderShader.Dispatch(DecoderConv5, 256 / 8, 256 / 8, 1);
        tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
    }

    public void Process(Dictionary<string, float[]> v1, Dictionary<string, Matrix3X3[]> v3)
    {
        foreach (var item in v1)
        {
            if (item.Key.StartsWith("encoder"))
            {
                buffer = BufferPool.Get<float>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                encoderShader.SetBuffer(StyleInstance0, item.Key, buffer);
                encoderShader.SetBuffer(StyleInstance1, item.Key, buffer);
                encoderShader.SetBuffer(StyleInstance2, item.Key, buffer);
                encoderShader.SetBuffer(StyleInstance3, item.Key, buffer);
                encoderShader.SetBuffer(StyleInstance4, item.Key, buffer);
                encoderShader.SetBuffer(StyleInstance5, item.Key, buffer);
            }
            else if (item.Key.StartsWith("decoder"))
            {
                buffer = BufferPool.Get<float>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                SetDecoderBuffer(item.Key, buffer, ResiduleInst1_1, ResiduleInst1_2, DecoderInstance1, DecoderInstance2, DecoderInstance3, DecoderInstance4);
                if (item.Key == "decoder_g_pred_c_Conv_weights") decoderShader.SetBuffer(DecoderConv5, "decoder_g_pred_c_Conv_weights", buffer);
            }
        }
        foreach (var item in v3)
        {
            if (item.Key.StartsWith("encoder"))
            {
                buffer = BufferPool.Get<Matrix3X3>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                encoderShader.SetBuffer(StyleConv1, item.Key, buffer);
                encoderShader.SetBuffer(StyleConv2, item.Key, buffer);
                encoderShader.SetBuffer(StyleConv3, item.Key, buffer);
                encoderShader.SetBuffer(StyleConv4, item.Key, buffer);
                encoderShader.SetBuffer(StyleConv5, item.Key, buffer);
            }
            else if (item.Key.StartsWith("decoder"))
            {
                buffer = BufferPool.Get<Matrix3X3>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                SetDecoderBuffer(item.Key, buffer, ResiduleConv1_1, ResiduleConv1_2, DecoderConv2, DecoderConv3, DecoderConv1, DecoderConv4);
            }
        }
        encoderShader.SetTexture(StyleConv0, "source", mainTexture);
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
        string name = "encoder_inst";
        var cb = BufferPool.Get<float>(name, 256, 256, 3);
        encoderShader.SetBuffer(StyleConv0, name, cb);
        encoderShader.SetBuffer(StyleNormal0, name, cb);
        encoderShader.SetBuffer(StyleInstance0, name, cb);
        encoderShader.SetBuffer(StylePad, name, cb);

        name = "encoder_conv0";
        cb = BufferPool.Get<float>(name, 286, 286, 3);
        encoderShader.SetBuffer(StylePad, name, cb);
        encoderShader.SetBuffer(StyleConv1, name, cb);

        name = "encoder_conv0_statistic";
        cb = BufferPool.Get<float>(name, 6);
        encoderShader.SetBuffer(StyleNormal0, name, cb);
        encoderShader.SetBuffer(StyleInstance0, name, cb);

        name = "encoder_conv1";
        cb = BufferPool.Get<float>(name, 284, 284, 32);
        encoderShader.SetBuffer(StyleConv1, name, cb);
        encoderShader.SetBuffer(StyleConv2, name, cb);
        encoderShader.SetBuffer(StyleInstance1, name, cb);
        encoderShader.SetBuffer(StyleNormal1, name, cb);

        name = "encoder_conv2";
        cb = BufferPool.Get<float>(name, 141, 141, 32);
        encoderShader.SetBuffer(StyleConv2, name, cb);
        encoderShader.SetBuffer(StyleConv3, name, cb);
        encoderShader.SetBuffer(StyleInstance2, name, cb);
        encoderShader.SetBuffer(StyleNormal2, name, cb);

        name = "encoder_conv3";
        cb = BufferPool.Get<float>(name, 70, 70, 64);
        encoderShader.SetBuffer(StyleConv3, name, cb);
        encoderShader.SetBuffer(StyleConv4, name, cb);
        encoderShader.SetBuffer(StyleInstance3, name, cb);
        encoderShader.SetBuffer(StyleNormal3, name, cb);

        name = "encoder_conv4";
        cb = BufferPool.Get<float>(name, 34, 34, 128);
        encoderShader.SetBuffer(StyleConv4, name, cb);
        encoderShader.SetBuffer(StyleConv5, name, cb);
        encoderShader.SetBuffer(StyleInstance4, name, cb);
        encoderShader.SetBuffer(StyleNormal4, name, cb);

        name = "encoder_conv5";
        cb = BufferPool.Get<float>(name, 16, 16, 256);
        encoderShader.SetBuffer(StyleConv5, name, cb);
        encoderShader.SetBuffer(StyleInstance5, name, cb);
        encoderShader.SetBuffer(StyleNormal5, name, cb);

        name = "encoder_conv1_statistic";
        cb = BufferPool.Get<float>(name, 32 * 2);
        encoderShader.SetBuffer(StyleConv1, name, cb);
        encoderShader.SetBuffer(StyleNormal1, name, cb);
        encoderShader.SetBuffer(StyleInstance1, name, cb);

        name = "encoder_conv2_statistic";
        cb = BufferPool.Get<float>(name, 32 * 2);
        encoderShader.SetBuffer(StyleConv2, name, cb);
        encoderShader.SetBuffer(StyleNormal2, name, cb);
        encoderShader.SetBuffer(StyleInstance2, name, cb);

        name = "encoder_conv3_statistic";
        cb = BufferPool.Get<float>(name, 64 * 2);
        encoderShader.SetBuffer(StyleConv3, name, cb);
        encoderShader.SetBuffer(StyleNormal3, name, cb);
        encoderShader.SetBuffer(StyleInstance3, name, cb);

        name = "encoder_conv4_statistic";
        cb = BufferPool.Get<float>(name, 128 * 2);
        encoderShader.SetBuffer(StyleConv4, name, cb);
        encoderShader.SetBuffer(StyleNormal4, name, cb);
        encoderShader.SetBuffer(StyleInstance4, name, cb);

        name = "encoder_conv5_statistic";
        cb = BufferPool.Get<float>(name, 256 * 2);
        encoderShader.SetBuffer(StyleConv5, name, cb);
        encoderShader.SetBuffer(StyleNormal5, name, cb);
        encoderShader.SetBuffer(StyleInstance5, name, cb);
    }

    private void SetDecoderBuffer(string name, ComputeBuffer cb, params int[] kernels)
    {
        for (int i = 0; i < kernels.Length; i++)
        {
            decoderShader.SetBuffer(kernels[i], name, cb);
        }
    }

    private void ProcessDecoder()
    {
        string name = "input_initial";
        var cb = BufferPool.Inference("encoder_conv5", name);
        SetDecoderBuffer(name, cb, ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleInst1_2, ResidulePad1_2, ResiduleConv1_2, ResiduleNormal1_2);

        name = "input_writable";
        cb = BufferPool.Get<float>(name, 16, 16, 256);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, ResiduleInst1_1, DecoderExpand1, ResidulePad1_2, ResiduleNormal1_2, ResiduleConv1_2, ResiduleInst1_2);

        name = "decoder_conv0_statistic";
        cb = BufferPool.Get<float>(name, 512);
        SetDecoderBuffer(name, cb, ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleInst1_1, ResiduleConv1_2, ResidulePad1_2, ResiduleNormal1_2, ResiduleInst1_2);

        name = "decoder_conv0";
        cb = BufferPool.Get<float>(name, 18, 18, 256);
        SetDecoderBuffer(name, cb, ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleNormal1_2, ResidulePad1_2, ResiduleConv1_2, ResiduleInst1_2);

        name = "decoder_conv0_conved";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, DecoderConv1, DecoderExpand1);

        name = "decoder_conv1";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, ResiduleConv1_1, DecoderNormal1, DecoderConv1, DecoderInstance1, DecoderExpand2);

        name = "decoder_conv1_conved";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, DecoderConv1, DecoderExpand1);

        name = "decoder_conv1_statistic";
        cb = BufferPool.Get<float>(name, 512);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, DecoderNormal1, DecoderConv1, DecoderInstance1);

        name = "decoder_conv2";
        cb = BufferPool.Get<float>(name, 64, 64, 128);
        SetDecoderBuffer(name, cb, ResiduleConv1_2, DecoderExpand3, DecoderNormal2, DecoderConv2, DecoderInstance2);

        name = "decoder_conv2_conved";
        cb = BufferPool.Get<float>(name, 64, 64, 256);
        SetDecoderBuffer(name, cb, DecoderConv2, DecoderExpand2);

        name = "decoder_conv2_statistic";
        cb = BufferPool.Get<float>(name, 256);
        SetDecoderBuffer(name, cb, ResiduleConv1_2, ResiduleNormal1_2, DecoderConv2, DecoderNormal2, DecoderInstance2);

        name = "decoder_conv3";
        cb = BufferPool.Get<float>(name, 128, 128, 64);
        SetDecoderBuffer(name, cb, DecoderExpand4, DecoderConv3, DecoderNormal3, DecoderInstance3);

        name = "decoder_conv3_conved";
        cb = BufferPool.Get<float>(name, 128, 128, 128);
        SetDecoderBuffer(name, cb, DecoderConv3, DecoderExpand3);

        name = "decoder_conv3_statistic";
        cb = BufferPool.Get<float>(name, 128);
        SetDecoderBuffer(name, cb, DecoderNormal3, DecoderConv3, DecoderInstance3);

        name = "decoder_conv4";
        cb = BufferPool.Get<float>(name, 256, 256, 32);
        SetDecoderBuffer(name, cb, DecoderPad5, DecoderConv4, DecoderNormal4, DecoderInstance4);

        name = "decoder_conv4_conved";
        cb = BufferPool.Get<float>(name, 256, 256, 64);
        SetDecoderBuffer(name, cb, DecoderConv4, DecoderExpand4, DecoderPad5);

        name = "decoder_conv4_statistic";
        cb = BufferPool.Get<float>(name, 64);
        SetDecoderBuffer(name, cb, DecoderNormal4, DecoderConv4, DecoderInstance4);

        name = "decoder_conv5_pad";
        cb = BufferPool.Get<float>(name, 262, 262, 32);
        SetDecoderBuffer(name, cb, DecoderPad5, DecoderConv5);

        decoderShader.SetTexture(DecoderConv5, "decoder_destination", tempDestination);
    }
}


[Serializable]
public class BaseData
{
    public string buffer;
    public string kernel;
}


public class ArgData : ScriptableObject
{
    public BaseData[] datas;

    public void OnSave()
    {
        Debug.Log("saved on editor");
    }

}

