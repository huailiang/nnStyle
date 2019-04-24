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
    private int enConv, enNorm, enInst, stylePad, enStyleConv1, enStyleNorm1, enStyleInstance1, enStyleConv2, enStyleNorm2, enStyleInstance2;
    private int enStyleConv3, enStyleNorm3, enStyleInstance3, enStyleConv4, enStyleNorm4, enStyleInstance4, enStyleConv5, enStyleNorm5, enStyleInstance5;

    private int deResidulePad1_1, deResiduleConv1_1, deResiduleNormal1_1, deResiduleInst1_1, deResidulePad1_2, deResiduleConv1_2, deResiduleNormal1_2, deResiduleInst1_2;
    private int decoderExpand1, decoderConv1, decoderNormal1, decoderInstance1, decoderExpand2, decoderConv2, decoderNormal2, decoderInstance2;
    private int decoderExpand3, decoderConv3, decoderNormal3, decoderInstance3, decoderExpand4, decoderConv4, decoderNormal4, decoderInstance4, decoderExpand5, decoderConv5;

    private const int width = 256;
    private bool drawGui = true;

    void Start()
    {
        InitEncoder();
        InitDecoder();
        if (stylePad < 0 || enStyleConv1 < 0 || enStyleNorm1 < 0 || enStyleInstance1 < 0)
        {
            Debug.Log("Initialization Failed");
            return;
        }
        if (tempRender.sharedMaterial.mainTexture == null)
        {
            tempRender.sharedMaterial.mainTexture = Resources.Load<Texture>("app1");
        }
        mainTexture = tempRender.sharedMaterial.mainTexture;
        tempDestination = new RenderTexture(width, width, 0);
        tempDestination.enableRandomWrite = true;
        tempDestination.Create();
        checkpoint = new LoadCheckpoint();
        checkpoint.Load(Process);
    }

    private void InitEncoder()
    {
        enConv = encoderShader.FindKernel("StyleEncoderConv");
        enNorm = encoderShader.FindKernel("StyleEncoderNormal");
        enInst = encoderShader.FindKernel("StyleEncoderInstance");
        stylePad = encoderShader.FindKernel("StylePad");
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
        deResiduleInst1_1 = decoderShader.FindKernel("ResiduleInst1_1");
        deResidulePad1_2 = decoderShader.FindKernel("ResidulePad1_2");
        deResiduleConv1_2 = decoderShader.FindKernel("ResiduleConv1_2");
        deResiduleNormal1_2 = decoderShader.FindKernel("ResiduleNormal1_2");
        deResiduleInst1_2 = decoderShader.FindKernel("ResiduleInst1_2");
        decoderExpand1 = decoderShader.FindKernel("DecoderExpand1");
        decoderConv1 = decoderShader.FindKernel("DecoderConv1");
        decoderNormal1 = decoderShader.FindKernel("DecoderNormal1");
        decoderInstance1 = decoderShader.FindKernel("DecoderInstance1");
        decoderExpand2 = decoderShader.FindKernel("DecoderExpand2");
        decoderConv2 = decoderShader.FindKernel("DecoderConv2");
        decoderNormal2 = decoderShader.FindKernel("DecoderNormal2");
        decoderInstance2 = decoderShader.FindKernel("DecoderInstance2");
        decoderExpand3 = decoderShader.FindKernel("DecoderExpand3");
        decoderConv3 = decoderShader.FindKernel("DecoderConv3");
        decoderNormal3 = decoderShader.FindKernel("DecoderNormal3");
        decoderInstance3 = decoderShader.FindKernel("DecoderInstance3");
        decoderExpand4 = decoderShader.FindKernel("DecoderExpand4");
        decoderConv4 = decoderShader.FindKernel("DecoderConv4");
        decoderNormal4 = decoderShader.FindKernel("DecoderNormal4");
        decoderInstance4 = decoderShader.FindKernel("DecoderInstance4");
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
        if (!drawGui) return;
        if (GUI.Button(new Rect(20, 20, 80, 40), "Run"))
        {
            //encoder
            encoderShader.Dispatch(enConv, width / 8, width / 8, 1);
            encoderShader.Dispatch(enNorm, 1, 1, 1);
            encoderShader.Dispatch(enInst, 256 / 8, 256 / 8, 1);
            encoderShader.Dispatch(stylePad, 288 / 8, 288 / 8, 1);
            encoderShader.Dispatch(enStyleConv1, 288 / 8, 288 / 8, 1);
            encoderShader.Dispatch(enStyleNorm1, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance1, 288 / 8, 288 / 8, 32 / 4);
            encoderShader.Dispatch(enStyleConv2, 144 / 8, 144 / 8, 1);
            encoderShader.Dispatch(enStyleNorm2, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance2, 144 / 8, 144 / 8, 32 / 4);
            encoderShader.Dispatch(enStyleConv3, 72 / 8, 72 / 8, 1);
            encoderShader.Dispatch(enStyleNorm3, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance3, 72 / 8, 72 / 8, 64 / 4);
            encoderShader.Dispatch(enStyleConv4, 40 / 8, 40 / 8, 1);
            encoderShader.Dispatch(enStyleNorm4, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance4, 40 / 8, 40 / 8, 128 / 4);
            encoderShader.Dispatch(enStyleConv5, 16 / 8, 16 / 8, 1);
            encoderShader.Dispatch(enStyleNorm5, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance5, 16 / 8, 16 / 8, 256 / 4);

            //decoder
            decoderShader.Dispatch(deResidulePad1_1, 24 / 8, 24 / 8, 256 / 4);
            decoderShader.Dispatch(deResiduleConv1_1, 16 / 8, 16 / 8, 1);
            decoderShader.Dispatch(deResiduleNormal1_1, 1, 1, 1);
            decoderShader.Dispatch(deResiduleInst1_1, 16 / 8, 16 / 8, 256 / 4);
            decoderShader.Dispatch(deResidulePad1_2, 24 / 8, 24 / 8, 256 / 4);
            decoderShader.Dispatch(deResiduleConv1_2, 16 / 8, 16 / 8, 1);
            decoderShader.Dispatch(deResiduleNormal1_2, 1, 1, 1);
            decoderShader.Dispatch(deResiduleInst1_2, 16 / 8, 16 / 8, 256 / 4);
            decoderShader.Dispatch(decoderExpand1, 16 / 8, 16 / 8, 256 / 4);
            decoderShader.Dispatch(decoderConv1, 32 / 8, 32 / 8, 1);
            decoderShader.Dispatch(decoderNormal1, 1, 1, 1);
            decoderShader.Dispatch(decoderInstance1, 16 / 8, 16 / 8, 256 / 4);
            decoderShader.Dispatch(decoderExpand2, 32 / 8, 32 / 8, 256 / 4);
            decoderShader.Dispatch(decoderConv2, 64 / 8, 64 / 8, 1);
            decoderShader.Dispatch(decoderNormal2, 1, 1, 1);
            decoderShader.Dispatch(decoderInstance2, 64 / 8, 64 / 8, 128 / 4);
            decoderShader.Dispatch(decoderExpand3, 64 / 8, 64 / 8, 128 / 4);
            decoderShader.Dispatch(decoderConv3, 128 / 8, 128 / 8, 1);
            decoderShader.Dispatch(decoderNormal3, 1, 1, 1);
            decoderShader.Dispatch(decoderInstance3, 128 / 8, 128 / 8, 64 / 4);
            decoderShader.Dispatch(decoderExpand4, 128 / 8, 128 / 8, 64 / 4);
            decoderShader.Dispatch(decoderConv4, 256 / 8, 256 / 8, 1);
            decoderShader.Dispatch(decoderNormal4, 1, 1, 1);
            decoderShader.Dispatch(decoderInstance4, 256 / 8, 256 / 8, 32 / 4);
            decoderShader.Dispatch(decoderExpand5, 264 / 8, 264 / 8, 32 / 4);
            decoderShader.Dispatch(decoderConv5, 256 / 8, 256 / 8, 1);
            tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
        }
        if (GUI.Button(new Rect(20, 80, 80, 40), "Decoder_r1"))
        {
            float[] layer = checkpoint.LoadLayer("decoder_r0");
            BufferPool.Get("input_initial").SetData(layer);
            BufferProfile.CheckZero("encoder_conv5");
            BufferProfile.CheckZero("input_initial");
        }
        if (GUI.Button(new Rect(20, 140, 80, 40), "Encoder_v1"))
        {
            encoderShader.Dispatch(enConv, width / 8, width / 8, 1);
            encoderShader.Dispatch(enNorm, 1, 1, 1);
            encoderShader.Dispatch(enInst, 256 / 8, 256 / 8, 1);
            BufferProfile.Print("encoder_inst_statistic");
            BufferProfile.Print("encoder_inst");
        }
        if (GUI.Button(new Rect(20, 200, 80, 40), "Encoder_v4"))
        {
            float[] layer = checkpoint.LoadLayer("encoder_c1");
            BufferPool.Get("encoder_conv1").SetData(layer);
            encoderShader.Dispatch(enStyleConv2, 144 / 8, 144 / 8, 1);
            BufferProfile.Print("encoder_conv2");
            encoderShader.Dispatch(enStyleNorm2, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance2, 144 / 8, 144 / 8, 32 / 4);
            BufferProfile.Print("encoder_conv2_statistic");
            BufferProfile.Print("encoder_conv2");
        }
        if (GUI.Button(new Rect(120, 20, 80, 40), "Texture"))
        {
            var texture = Resources.Load<Texture2D>("app1");
            BufferProfile.NormalInst(texture);
        }
        if (GUI.Button(new Rect(120, 80, 80, 40), "Decoder"))
        {
            float[] layer = checkpoint.LoadLayer("decoder_d1");
            BufferPool.Get("decoder_conv1").SetData(layer);
            //BufferProfile.Print("decoder_residule");
            //decoderShader.Dispatch(decoderExpand1, 16 / 8, 16 / 8, 256 / 4);
            //decoderShader.Dispatch(decoderConv1, 32 / 8, 32 / 8, 1);
            //decoderShader.Dispatch(decoderNormal1, 1, 1, 4);
            //decoderShader.Dispatch(decoderInstance1, 32 / 8, 32 / 8, 256 / 4);
            decoderShader.Dispatch(decoderExpand2, 32 / 8, 32 / 8, 256 / 4);
            decoderShader.Dispatch(decoderConv2, 64 / 8, 64 / 8, 1);
            decoderShader.Dispatch(decoderNormal2, 1, 1, 2);
            decoderShader.Dispatch(decoderInstance2, 64 / 8, 64 / 8, 128 / 4);
            decoderShader.Dispatch(decoderExpand3, 64 / 8, 64 / 8, 128 / 4);
            decoderShader.Dispatch(decoderConv3, 128 / 8, 128 / 8, 1);
            decoderShader.Dispatch(decoderNormal3, 1, 1, 1);
            decoderShader.Dispatch(decoderInstance3, 128 / 8, 128 / 8, 64 / 4);
            decoderShader.Dispatch(decoderExpand4, 128 / 8, 128 / 8, 64 / 4);
            decoderShader.Dispatch(decoderConv4, 256 / 8, 256 / 8, 1);
            decoderShader.Dispatch(decoderNormal4, 1, 1, 1);
            decoderShader.Dispatch(decoderInstance4, 256 / 8, 256 / 8, 32 / 4);
            decoderShader.Dispatch(decoderExpand5, 264 / 8, 264 / 8, 32 / 4);
            decoderShader.Dispatch(decoderConv5, 256 / 8, 256 / 8, 1);
            BufferProfile.Print("decoder_conv5_pad");
            tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
            drawGui = false;
        }
        if (GUI.Button(new Rect(120, 140, 80, 40), "Output"))
        {
            float[] layer = checkpoint.LoadLayer("decoder_d4");
            BufferPool.Get("decoder_conv4").SetData(layer);
            BufferProfile.PrintH("decoder_conv4",2);
            decoderShader.Dispatch(decoderExpand5, 264 / 8, 264 / 8, 32 / 4);
            decoderShader.Dispatch(decoderConv5, 256 / 8, 256 / 8, 1);
            BufferProfile.PrintH("decoder_conv5_pad", 2);
            tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
            drawGui = false;
        }
    }

    private void Process(Dictionary<string, float[]> v1, Dictionary<string, Matrix3X3[]> v3)
    {
        foreach (var item in v1)
        {
            if (item.Key.StartsWith("encoder"))
            {
                buffer = BufferPool.Get<float>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                encoderShader.SetBuffer(enInst, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance1, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance2, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance3, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance4, item.Key, buffer);
                encoderShader.SetBuffer(enStyleInstance5, item.Key, buffer);
            }
            else if (item.Key.StartsWith("decoder"))
            {
                buffer = BufferPool.Get<float>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                SetDecoderBuffer(item.Key, buffer, decoderConv1, deResiduleNormal1_1, deResiduleInst1_1, deResiduleNormal1_2, deResiduleInst1_2, decoderNormal1, decoderInstance1, decoderConv2, decoderNormal2, decoderInstance2,
                    decoderConv3, decoderNormal3, decoderInstance3, decoderConv4, decoderNormal4, decoderInstance4);
                if (item.Key == "decoder_g_pred_c_Conv_weights") decoderShader.SetBuffer(decoderConv5, "decoder_g_pred_c_Conv_weights", buffer);
            }
        }
        foreach (var item in v3)
        {
            if (item.Key.StartsWith("encoder"))
            {
                buffer = BufferPool.Get<Matrix3X3>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                encoderShader.SetBuffer(stylePad, item.Key, buffer);
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
                buffer = BufferPool.Get<Matrix3X3>(item.Key, item.Value.Length);
                buffer.SetData(item.Value);
                SetDecoderBuffer(item.Key, buffer, deResiduleConv1_1, deResiduleNormal1_1, deResiduleConv1_2, deResiduleNormal1_2, decoderConv2, decoderNormal2, decoderConv3,
                    decoderConv1, decoderNormal3, decoderConv4, decoderNormal4);
            }
        }
        encoderShader.SetTexture(enConv, "source", mainTexture);
        encoderShader.SetTexture(enInst, "source", mainTexture);
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
        string name = "encoder_conv0";
        var cb = BufferPool.Get<float>(name, 286, 286, 3);
        encoderShader.SetBuffer(stylePad, name, cb);
        encoderShader.SetBuffer(enStyleConv1, name, cb);

        name = "encoder_inst";
        cb = BufferPool.Get<float>(name, 256, 256, 3);
        encoderShader.SetBuffer(enConv, name, cb);
        encoderShader.SetBuffer(enNorm, name, cb);
        encoderShader.SetBuffer(enInst, name, cb);
        encoderShader.SetBuffer(stylePad, name, cb);

        name = "encoder_inst_statistic";
        cb = BufferPool.Get<float>(name, 6);
        encoderShader.SetBuffer(enNorm, name, cb);
        encoderShader.SetBuffer(enInst, name, cb);

        name = "encoder_conv1";
        cb = BufferPool.Get<float>(name, 284, 284, 32);
        encoderShader.SetBuffer(enStyleConv1, name, cb);
        encoderShader.SetBuffer(enStyleConv2, name, cb);
        encoderShader.SetBuffer(enStyleInstance1, name, cb);
        encoderShader.SetBuffer(enStyleNorm1, name, cb);

        name = "encoder_conv2";
        cb = BufferPool.Get<float>(name, 141, 141, 32);
        encoderShader.SetBuffer(enStyleConv2, name, cb);
        encoderShader.SetBuffer(enStyleConv3, name, cb);
        encoderShader.SetBuffer(enStyleInstance2, name, cb);
        encoderShader.SetBuffer(enStyleNorm2, name, cb);

        name = "encoder_conv3";
        cb = BufferPool.Get<float>(name, 70, 70, 64);
        encoderShader.SetBuffer(enStyleConv3, name, cb);
        encoderShader.SetBuffer(enStyleConv4, name, cb);
        encoderShader.SetBuffer(enStyleInstance3, name, cb);
        encoderShader.SetBuffer(enStyleNorm3, name, cb);

        name = "encoder_conv4";
        cb = BufferPool.Get<float>(name, 34, 34, 128);
        encoderShader.SetBuffer(enStyleConv4, name, cb);
        encoderShader.SetBuffer(enStyleConv5, name, cb);
        encoderShader.SetBuffer(enStyleInstance4, name, cb);
        encoderShader.SetBuffer(enStyleNorm4, name, cb);

        name = "encoder_conv5";
        cb = BufferPool.Get<float>(name, 16, 16, 256);
        encoderShader.SetBuffer(enStyleConv5, name, cb);
        encoderShader.SetBuffer(enStyleInstance5, name, cb);
        encoderShader.SetBuffer(enStyleNorm5, name, cb);

        name = "encoder_conv1_statistic";
        cb = BufferPool.Get<float>(name, 32 * 2);
        encoderShader.SetBuffer(enStyleConv1, name, cb);
        encoderShader.SetBuffer(enStyleNorm1, name, cb);
        encoderShader.SetBuffer(enStyleInstance1, name, cb);

        name = "encoder_conv2_statistic";
        cb = BufferPool.Get<float>(name, 32 * 2);
        encoderShader.SetBuffer(enStyleConv2, name, cb);
        encoderShader.SetBuffer(enStyleNorm2, name, cb);
        encoderShader.SetBuffer(enStyleInstance2, name, cb);

        name = "encoder_conv3_statistic";
        cb = BufferPool.Get<float>(name, 64 * 2);
        encoderShader.SetBuffer(enStyleConv3, name, cb);
        encoderShader.SetBuffer(enStyleNorm3, name, cb);
        encoderShader.SetBuffer(enStyleInstance3, name, cb);

        name = "encoder_conv4_statistic";
        cb = BufferPool.Get<float>(name, 128 * 2);
        encoderShader.SetBuffer(enStyleConv4, name, cb);
        encoderShader.SetBuffer(enStyleNorm4, name, cb);
        encoderShader.SetBuffer(enStyleInstance4, name, cb);

        name = "encoder_conv5_statistic";
        cb = BufferPool.Get<float>(name, 256 * 2);
        encoderShader.SetBuffer(enStyleConv5, name, cb);
        encoderShader.SetBuffer(enStyleNorm5, name, cb);
        encoderShader.SetBuffer(enStyleInstance5, name, cb);
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
        SetDecoderBuffer(name, cb, deResidulePad1_1, deResiduleConv1_1, deResiduleNormal1_1, deResiduleInst1_2, deResidulePad1_2, deResiduleConv1_2, deResiduleNormal1_2);

        name = "input_writable";
        cb = BufferPool.Get<float>(name, 16, 16, 256);
        SetDecoderBuffer(name, cb, deResiduleNormal1_1, deResiduleConv1_1, deResiduleInst1_1, deResidulePad1_2, deResiduleNormal1_2, deResiduleConv1_2, deResiduleInst1_2);

        name = "input_statistic";
        cb = BufferPool.Get<float>(name, 512);
        SetDecoderBuffer(name, cb, deResidulePad1_1, deResiduleConv1_1, deResiduleNormal1_1, deResiduleInst1_1, deResiduleConv1_2, deResidulePad1_2, deResiduleNormal1_2, deResiduleInst1_2);

        name = "decoder_residule";
        cb = BufferPool.Get<float>(name, 16, 16, 256);
        SetDecoderBuffer(name, cb, deResidulePad1_1, deResiduleConv1_1, deResiduleNormal1_1, deResiduleNormal1_2, deResidulePad1_2, deResiduleConv1_2, decoderExpand1, deResiduleInst1_2);

        name = "decoder_conv0_conved";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, deResiduleNormal1_1, deResiduleConv1_1, decoderConv1, decoderExpand1);

        name = "decoder_conv1";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, deResiduleConv1_1, decoderNormal1, decoderConv1, decoderInstance1, decoderExpand2);

        name = "decoder_conv1_conved";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, deResiduleNormal1_1, deResiduleConv1_1, decoderConv1, decoderExpand1);

        name = "decoder_conv1_statistic";
        cb = BufferPool.Get<float>(name, 512);
        SetDecoderBuffer(name, cb, deResiduleNormal1_1, deResiduleConv1_1, decoderNormal1, decoderConv1, decoderInstance1);

        name = "decoder_conv2";
        cb = BufferPool.Get<float>(name, 64, 64, 128);
        SetDecoderBuffer(name, cb, deResiduleConv1_2, decoderExpand3, decoderNormal2, decoderConv2, decoderInstance2);

        name = "decoder_conv2_conved";
        cb = BufferPool.Get<float>(name, 64, 64, 256);
        SetDecoderBuffer(name, cb, decoderConv2, decoderExpand2);

        name = "decoder_conv2_statistic";
        cb = BufferPool.Get<float>(name, 256);
        SetDecoderBuffer(name, cb, deResiduleConv1_2, deResiduleNormal1_2, decoderConv2, decoderNormal2, decoderInstance2);

        name = "decoder_conv3";
        cb = BufferPool.Get<float>(name, 128, 128, 64);
        SetDecoderBuffer(name, cb, decoderExpand4, decoderConv3, decoderNormal3, decoderInstance3);

        name = "decoder_conv3_conved";
        cb = BufferPool.Get<float>(name, 128, 128, 128);
        SetDecoderBuffer(name, cb, decoderConv3, decoderExpand3);

        name = "decoder_conv3_statistic";
        cb = BufferPool.Get<float>(name, 128);
        SetDecoderBuffer(name, cb, decoderNormal3, decoderConv3, decoderInstance3);

        name = "decoder_conv4";
        cb = BufferPool.Get<float>(name, 256, 256, 32);
        SetDecoderBuffer(name, cb, decoderExpand5, decoderConv4, decoderNormal4, decoderInstance4);

        name = "decoder_conv4_conved";
        cb = BufferPool.Get<float>(name, 256, 256, 64);
        SetDecoderBuffer(name, cb, decoderConv4, decoderExpand4, decoderExpand5);

        name = "decoder_conv4_statistic";
        cb = BufferPool.Get<float>(name, 64);
        SetDecoderBuffer(name, cb, decoderNormal4, decoderConv4, decoderInstance4);

        name = "decoder_conv5_pad";
        cb = BufferPool.Get<float>(name, 262, 262, 32);
        SetDecoderBuffer(name, cb, decoderExpand5, decoderConv5);

        decoderShader.SetTexture(decoderConv5, "decoder_destination", tempDestination);
    }
}