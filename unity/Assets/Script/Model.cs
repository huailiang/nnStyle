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
    private Kernel StyleConv0, StyleNormal0, StyleInstance0, StylePad, StyleConv1, StyleNormal1, StyleInstance1;
    private Kernel StyleConv2, StyleNormal2, StyleInstance2, StyleConv3, StyleNormal3, StyleInstance3;
    private Kernel ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleInst1_1, ResidulePad1_2, ResiduleConv1_2, ResiduleNormal1_2, ResiduleInst1_2;
    private Kernel DecoderExpand1, DecoderConv1, DecoderNormal1, DecoderInstance1, DecoderExpand2, DecoderConv2, DecoderNormal2, DecoderInstance2, DecoderExpand3, DecoderConv3;

    private ComputeShader encoderShader, decoderShader;
    private ComputeBuffer buffer;
    private Renderer tempRender = null;
    private RenderTexture tempDestination = null;
    private Texture mainTexture = null;
    private const int width = 256;
    private Dictionary<string, BaseData> map;

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

    public void BindRender(Renderer temp, bool realtime)
    {
        tempRender = temp;
        if (realtime)
        {
            encoderShader.SetInt("alpha", Screen.height / 256);
        }
        else
        {
            mainTexture = tempRender.sharedMaterial.mainTexture;
            encoderShader.SetInt("alpha", 1);
        }
        tempDestination = new RenderTexture(width, width, 0);
        tempDestination.enableRandomWrite = true;
        tempDestination.Create();
    }

    private void BuildMap()
    {
        if (map == null)
        {
            map = new Dictionary<string, BaseData>();
            var data = Resources.Load<NearualData>("map");
            for (int i = 0; i < data.datas.Length; i++)
            {
                var it = data.datas[i];
                map.Add(it.buffer, it);
            }
        }
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
    }

    public void DrawEncoder()
    {
        encoderShader.Dispatch(StyleConv0, width / 8, width / 8, 1);
        encoderShader.Dispatch(StyleNormal0, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance0, 256 / 8, 256 / 8, 1);
        encoderShader.Dispatch(StylePad, 264 / 8, 264 / 8, 1);
        encoderShader.Dispatch(StyleConv1, 136 / 8, 136 / 8, 1);
        encoderShader.Dispatch(StyleNormal1, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance1, 136 / 8, 136 / 8, 32 / 4);
        encoderShader.Dispatch(StyleConv2, 72 / 8, 72 / 8, 1);
        encoderShader.Dispatch(StyleNormal2, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance2, 72 / 8, 72 / 8, 32 / 4);
        encoderShader.Dispatch(StyleConv3, 32 / 8, 32 / 8, 1);
        encoderShader.Dispatch(StyleNormal3, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance3, 32 / 8, 32 / 8, 64 / 4);
    }

    public void DrawResidule()
    {
        decoderShader.Dispatch(ResidulePad1_1, 40 / 8, 40 / 8, 64 / 4);
        decoderShader.Dispatch(ResiduleConv1_1, 32 / 8, 32 / 8, 1);
        decoderShader.Dispatch(ResiduleNormal1_1, 1, 1, 1);
        decoderShader.Dispatch(ResiduleInst1_1, 32 / 8, 32 / 8, 64 / 4);
        decoderShader.Dispatch(ResidulePad1_2, 40 / 8, 40 / 8, 64 / 4);
        decoderShader.Dispatch(ResiduleConv1_2, 32 / 8, 32 / 8, 1);
        decoderShader.Dispatch(ResiduleNormal1_2, 1, 1, 1);
        decoderShader.Dispatch(ResiduleInst1_2, 32 / 8, 32 / 8, 64 / 4);
    }

    public void DrawDecoder()
    {
        decoderShader.Dispatch(DecoderExpand1, 32 / 8, 32 / 8, 64 / 4);
        decoderShader.Dispatch(DecoderConv1, 64 / 8, 64 / 8, 1);
        decoderShader.Dispatch(DecoderNormal1, 1, 1, 1);
        decoderShader.Dispatch(DecoderInstance1, 64 / 8, 64 / 8, 64 / 4);
        decoderShader.Dispatch(DecoderExpand2, 64 / 8, 64 / 8, 64 / 4);
        decoderShader.Dispatch(DecoderConv2, 128 / 8, 128 / 8, 1);
        decoderShader.Dispatch(DecoderNormal2, 1, 1, 1);
        decoderShader.Dispatch(DecoderInstance2, 128 / 8, 128 / 8, 32 / 4);
        decoderShader.Dispatch(DecoderExpand3, 128 / 8, 128 / 8, 32 / 4);
        decoderShader.Dispatch(DecoderConv3, 256 / 8, 256 / 8, 1);
        tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
    }

    public void Test()
    {
        encoderShader.Dispatch(StyleConv1, 136 / 8, 136 / 8, 1);
        encoderShader.Dispatch(StyleNormal1, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance1, 136 / 8, 136 / 8, 32 / 4);
        encoderShader.Dispatch(StyleConv2, 72 / 8, 72 / 8, 1);
        encoderShader.Dispatch(StyleNormal2, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance2, 72 / 8, 72 / 8, 32 / 4);
        encoderShader.Dispatch(StyleConv3, 32 / 8, 32 / 8, 1);
        encoderShader.Dispatch(StyleNormal3, 1, 1, 1);
        encoderShader.Dispatch(StyleInstance3, 32 / 8, 32 / 8, 64 / 4);
        DrawResidule();
        DrawDecoder();
    }


    public void Process(Dictionary<string, float[]> v1, Dictionary<string, Matrix3X3[]> v3)
    {
        if (map == null) BuildMap();
        ComputeShader[] shaders = { encoderShader, decoderShader };
        var itr = v1.GetEnumerator();
        while (itr.MoveNext())
        {
            var item = itr.Current;
            var shader = shaders[(int)map[item.Key].nearual];
            buffer = BufferPool.Get<float>(item.Key, item.Value.Length);
            buffer.SetData(item.Value);
            shader.SetBuffer(map[item.Key].kernel, item.Key, buffer);
        }
        var itr2 = v3.GetEnumerator();
        while (itr2.MoveNext())
        {
            var item = itr2.Current;
            var shader = shaders[(int)map[item.Key].nearual];
            buffer = BufferPool.Get<Matrix3X3>(item.Key, item.Value.Length);
            buffer.SetData(item.Value);
            shader.SetBuffer(map[item.Key].kernel, item.Key, buffer);
        }
        map.Clear();
        if (mainTexture != null) encoderShader.SetTexture(StyleConv0, "source", mainTexture);
        ProcessNearual();
        Debug.Log("Process neural network finsih");
    }

    public void RebindSource(RenderTexture rt)
    {
        encoderShader.SetTexture(StyleConv0, "source", rt);
    }

    void ProcessNearual()
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
        cb = BufferPool.Get<float>(name, 264, 264, 3);
        encoderShader.SetBuffer(StylePad, name, cb);
        encoderShader.SetBuffer(StyleConv1, name, cb);

        name = "encoder_conv0_statistic";
        cb = BufferPool.Get<float>(name, 6);
        encoderShader.SetBuffer(StyleNormal0, name, cb);
        encoderShader.SetBuffer(StyleInstance0, name, cb);

        name = "encoder_conv1";
        cb = BufferPool.Get<float>(name, 131, 131, 32);
        encoderShader.SetBuffer(StyleConv1, name, cb);
        encoderShader.SetBuffer(StyleConv2, name, cb);
        encoderShader.SetBuffer(StyleInstance1, name, cb);
        encoderShader.SetBuffer(StyleNormal1, name, cb);

        name = "encoder_conv2";
        cb = BufferPool.Get<float>(name, 65, 65, 32);
        encoderShader.SetBuffer(StyleConv2, name, cb);
        encoderShader.SetBuffer(StyleConv3, name, cb);
        encoderShader.SetBuffer(StyleInstance2, name, cb);
        encoderShader.SetBuffer(StyleNormal2, name, cb);

        name = "encoder_conv3";
        cb = BufferPool.Get<float>(name, 32, 32, 64);
        encoderShader.SetBuffer(StyleConv3, name, cb);
        encoderShader.SetBuffer(StyleInstance3, name, cb);
        encoderShader.SetBuffer(StyleNormal3, name, cb);


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
        var cb = BufferPool.Inference("encoder_conv3", name);
        SetDecoderBuffer(name, cb, ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleInst1_2, ResidulePad1_2, ResiduleConv1_2, ResiduleNormal1_2);

        name = "input_writable";
        cb = BufferPool.Get<float>(name, 32, 32, 64);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, ResiduleInst1_1, DecoderExpand1, ResidulePad1_2, ResiduleNormal1_2, ResiduleConv1_2, ResiduleInst1_2);

        name = "decoder_conv0_statistic";
        cb = BufferPool.Get<float>(name, 128);
        SetDecoderBuffer(name, cb, ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleInst1_1, ResiduleConv1_2, ResidulePad1_2, ResiduleNormal1_2, ResiduleInst1_2);

        name = "decoder_conv0";
        cb = BufferPool.Get<float>(name, 34, 34, 64);
        SetDecoderBuffer(name, cb, ResidulePad1_1, ResiduleConv1_1, ResiduleNormal1_1, ResiduleNormal1_2, ResidulePad1_2, ResiduleConv1_2, ResiduleInst1_2);

        name = "decoder_conv0_conved";
        cb = BufferPool.Get<float>(name, 32, 32, 64);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, DecoderConv1, DecoderExpand1);

        name = "decoder_conv1";
        cb = BufferPool.Get<float>(name, 64, 64, 64);
        SetDecoderBuffer(name, cb, ResiduleConv1_1, DecoderNormal1, DecoderConv1, DecoderInstance1, DecoderExpand2);

        name = "decoder_conv1_conved";
        cb = BufferPool.Get<float>(name, 64, 64, 64);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, DecoderConv1, DecoderExpand1);

        name = "decoder_conv1_statistic";
        cb = BufferPool.Get<float>(name, 128);
        SetDecoderBuffer(name, cb, ResiduleNormal1_1, ResiduleConv1_1, DecoderNormal1, DecoderConv1, DecoderInstance1);

        name = "decoder_conv2";
        cb = BufferPool.Get<float>(name, 128, 128, 32);
        SetDecoderBuffer(name, cb, ResiduleConv1_2, DecoderExpand3, DecoderNormal2, DecoderConv2, DecoderInstance2);

        name = "decoder_conv2_conved";
        cb = BufferPool.Get<float>(name, 128, 128, 64);
        SetDecoderBuffer(name, cb, DecoderConv2, DecoderExpand2);

        name = "decoder_conv2_statistic";
        cb = BufferPool.Get<float>(name, 64);
        SetDecoderBuffer(name, cb, ResiduleConv1_2, ResiduleNormal1_2, DecoderConv2, DecoderNormal2, DecoderInstance2);

        name = "decoder_conv3";
        cb = BufferPool.Get<float>(name, 256, 256, 3);
        SetDecoderBuffer(name, cb, DecoderConv3);

        name = "decoder_conv3_conved";
        cb = BufferPool.Get<float>(name, 256, 256, 32);
        SetDecoderBuffer(name, cb, DecoderConv3, DecoderExpand3);

        decoderShader.SetTexture(DecoderConv3, "decoder_destination", tempDestination);
    }
}

