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

    private int residulePad1_1, residuleConv1_1, residuleNormal1_1, residuleInst1_1, residulePad1_2, residuleConv1_2, residuleNormal1_2, residuleInst1_2;
    private int decodeExpand1, decodeConv1, decodeNormal1, decodeInstance1, decodeExpand2, decodeConv2, decodeNormal2, decodeInstance2;
    private int decodeExpand3, decodeConv3, decodeNormal3, decodeInstance3, decodeExpand4, decodeConv4, decodeNormal4, decodeInstance4, decodeExpand5, decodeConv5;

    private const int width = 256;
    private bool drawGui = true;

    void Start()
    {
        InitEncoder();
        InitDecoder();
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
        residulePad1_1 = decoderShader.FindKernel("ResidulePad1_1");
        residuleConv1_1 = decoderShader.FindKernel("ResiduleConv1_1");
        residuleNormal1_1 = decoderShader.FindKernel("ResiduleNormal1_1");
        residuleInst1_1 = decoderShader.FindKernel("ResiduleInst1_1");
        residulePad1_2 = decoderShader.FindKernel("ResidulePad1_2");
        residuleConv1_2 = decoderShader.FindKernel("ResiduleConv1_2");
        residuleNormal1_2 = decoderShader.FindKernel("ResiduleNormal1_2");
        residuleInst1_2 = decoderShader.FindKernel("ResiduleInst1_2");
        decodeExpand1 = decoderShader.FindKernel("DecoderExpand1");
        decodeConv1 = decoderShader.FindKernel("DecoderConv1");
        decodeNormal1 = decoderShader.FindKernel("DecoderNormal1");
        decodeInstance1 = decoderShader.FindKernel("DecoderInstance1");
        decodeExpand2 = decoderShader.FindKernel("DecoderExpand2");
        decodeConv2 = decoderShader.FindKernel("DecoderConv2");
        decodeNormal2 = decoderShader.FindKernel("DecoderNormal2");
        decodeInstance2 = decoderShader.FindKernel("DecoderInstance2");
        decodeExpand3 = decoderShader.FindKernel("DecoderExpand3");
        decodeConv3 = decoderShader.FindKernel("DecoderConv3");
        decodeNormal3 = decoderShader.FindKernel("DecoderNormal3");
        decodeInstance3 = decoderShader.FindKernel("DecoderInstance3");
        decodeExpand4 = decoderShader.FindKernel("DecoderExpand4");
        decodeConv4 = decoderShader.FindKernel("DecoderConv4");
        decodeNormal4 = decoderShader.FindKernel("DecoderNormal4");
        decodeInstance4 = decoderShader.FindKernel("DecoderInstance4");
        decodeExpand5 = decoderShader.FindKernel("DecoderPad5");
        decodeConv5 = decoderShader.FindKernel("DecoderConv5");
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

    private void Update() { if (Input.GetKeyUp(KeyCode.Space)) drawGui = !drawGui; }

    private void OnGUI()
    {
        if (!drawGui) return;
        if (GUI.Button(new Rect(20, 20, 80, 40), "Run"))
        {
            DrawEncoder();
            DrawResidule();
            DrawDecoder();
        }
        if (GUI.Button(new Rect(20, 80, 80, 40), "Decode"))
        {
            float[] layer = checkpoint.LoadLayer("encoder_c3");
            BufferPool.Get("encoder_conv3").SetData(layer);
            encoderShader.Dispatch(enStyleConv4, 40 / 8, 40 / 8, 1);
            encoderShader.Dispatch(enStyleNorm4, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance4, 40 / 8, 40 / 8, 128 / 4);
            encoderShader.Dispatch(enStyleConv5, 16 / 8, 16 / 8, 1);
            encoderShader.Dispatch(enStyleNorm5, 1, 1, 1);
            encoderShader.Dispatch(enStyleInstance5, 16 / 8, 16 / 8, 256 / 4);
            DrawResidule();
            DrawDecoder();
        }
        if (GUI.Button(new Rect(20, 140, 80, 40), "Output"))
        {
            float[] layer = checkpoint.LoadLayer("decoder_d4");
            BufferPool.Get("decoder_conv4").SetData(layer);
            DrawRender();
        }
    }

    private void DrawEncoder()
    {
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
    }

    private void DrawResidule()
    {
        decoderShader.Dispatch(residulePad1_1, 24 / 8, 24 / 8, 256 / 4);
        decoderShader.Dispatch(residuleConv1_1, 16 / 8, 16 / 8, 1);
        decoderShader.Dispatch(residuleNormal1_1, 1, 1, 1);
        decoderShader.Dispatch(residuleInst1_1, 16 / 8, 16 / 8, 256 / 4);
        decoderShader.Dispatch(residulePad1_2, 24 / 8, 24 / 8, 256 / 4);
        decoderShader.Dispatch(residuleConv1_2, 16 / 8, 16 / 8, 1);
        decoderShader.Dispatch(residuleNormal1_2, 1, 1, 1);
        decoderShader.Dispatch(residuleInst1_2, 16 / 8, 16 / 8, 256 / 4);
    }

    private void DrawDecoder()
    {
        decoderShader.Dispatch(decodeExpand1, 16 / 8, 16 / 8, 256 / 4);
        decoderShader.Dispatch(decodeConv1, 32 / 8, 32 / 8, 1);
        decoderShader.Dispatch(decodeNormal1, 1, 1, 1);
        decoderShader.Dispatch(decodeInstance1, 32 / 8, 32 / 8, 256 / 4);
        decoderShader.Dispatch(decodeExpand2, 32 / 8, 32 / 8, 256 / 4);
        decoderShader.Dispatch(decodeConv2, 64 / 8, 64 / 8, 1);
        decoderShader.Dispatch(decodeNormal2, 1, 1, 1);
        decoderShader.Dispatch(decodeInstance2, 64 / 8, 64 / 8, 128 / 4);
        decoderShader.Dispatch(decodeExpand3, 64 / 8, 64 / 8, 128 / 4);
        decoderShader.Dispatch(decodeConv3, 128 / 8, 128 / 8, 1);
        decoderShader.Dispatch(decodeNormal3, 1, 1, 1);
        decoderShader.Dispatch(decodeInstance3, 128 / 8, 128 / 8, 64 / 4);
        decoderShader.Dispatch(decodeExpand4, 128 / 8, 128 / 8, 64 / 4);
        decoderShader.Dispatch(decodeConv4, 256 / 8, 256 / 8, 1);
        decoderShader.Dispatch(decodeNormal4, 1, 1, 1);
        decoderShader.Dispatch(decodeInstance4, 256 / 8, 256 / 8, 32 / 4);
        DrawRender();
    }

    private void DrawRender()
    {
        decoderShader.Dispatch(decodeExpand5, 264 / 8, 264 / 8, 32 / 4);
        decoderShader.Dispatch(decodeConv5, 256 / 8, 256 / 8, 1);
        tempRender.sharedMaterial.SetTexture("_MainTex", tempDestination);
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
                SetDecoderBuffer(item.Key, buffer, decodeConv1, residuleNormal1_1, residuleInst1_1, residuleNormal1_2, residuleInst1_2, decodeNormal1, decodeInstance1, decodeConv2, decodeNormal2, decodeInstance2,
                    decodeConv3, decodeNormal3, decodeInstance3, decodeConv4, decodeNormal4, decodeInstance4);
                if (item.Key == "decoder_g_pred_c_Conv_weights") decoderShader.SetBuffer(decodeConv5, "decoder_g_pred_c_Conv_weights", buffer);
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
                SetDecoderBuffer(item.Key, buffer, residuleConv1_1, residuleNormal1_1, residuleConv1_2, residuleNormal1_2, decodeConv2, decodeNormal2, decodeConv3,
                    decodeConv1, decodeNormal3, decodeConv4, decodeNormal4);
            }
        }
        encoderShader.SetTexture(enConv, "source", mainTexture);
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
        encoderShader.SetBuffer(enConv, name, cb);
        encoderShader.SetBuffer(enNorm, name, cb);
        encoderShader.SetBuffer(enInst, name, cb);
        encoderShader.SetBuffer(stylePad, name, cb);

        name = "encoder_conv0";
        cb = BufferPool.Get<float>(name, 286, 286, 3);
        encoderShader.SetBuffer(stylePad, name, cb);
        encoderShader.SetBuffer(enStyleConv1, name, cb);

        name = "encoder_conv0_statistic";
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
        SetDecoderBuffer(name, cb, residulePad1_1, residuleConv1_1, residuleNormal1_1, residuleInst1_2, residulePad1_2, residuleConv1_2, residuleNormal1_2);

        name = "input_writable";
        cb = BufferPool.Get<float>(name, 16, 16, 256);
        SetDecoderBuffer(name, cb, residuleNormal1_1, residuleConv1_1, residuleInst1_1, decodeExpand1, residulePad1_2, residuleNormal1_2, residuleConv1_2, residuleInst1_2);

        name = "decoder_conv0_statistic";
        cb = BufferPool.Get<float>(name, 512);
        SetDecoderBuffer(name, cb, residulePad1_1, residuleConv1_1, residuleNormal1_1, residuleInst1_1, residuleConv1_2, residulePad1_2, residuleNormal1_2, residuleInst1_2);

        name = "decoder_conv0";
        cb = BufferPool.Get<float>(name, 18, 18, 256);
        SetDecoderBuffer(name, cb, residulePad1_1, residuleConv1_1, residuleNormal1_1, residuleNormal1_2, residulePad1_2, residuleConv1_2, residuleInst1_2);

        name = "decoder_conv0_conved";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, residuleNormal1_1, residuleConv1_1, decodeConv1, decodeExpand1);

        name = "decoder_conv1";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, residuleConv1_1, decodeNormal1, decodeConv1, decodeInstance1, decodeExpand2);

        name = "decoder_conv1_conved";
        cb = BufferPool.Get<float>(name, 32, 32, 256);
        SetDecoderBuffer(name, cb, residuleNormal1_1, residuleConv1_1, decodeConv1, decodeExpand1);

        name = "decoder_conv1_statistic";
        cb = BufferPool.Get<float>(name, 512);
        SetDecoderBuffer(name, cb, residuleNormal1_1, residuleConv1_1, decodeNormal1, decodeConv1, decodeInstance1);

        name = "decoder_conv2";
        cb = BufferPool.Get<float>(name, 64, 64, 128);
        SetDecoderBuffer(name, cb, residuleConv1_2, decodeExpand3, decodeNormal2, decodeConv2, decodeInstance2);

        name = "decoder_conv2_conved";
        cb = BufferPool.Get<float>(name, 64, 64, 256);
        SetDecoderBuffer(name, cb, decodeConv2, decodeExpand2);

        name = "decoder_conv2_statistic";
        cb = BufferPool.Get<float>(name, 256);
        SetDecoderBuffer(name, cb, residuleConv1_2, residuleNormal1_2, decodeConv2, decodeNormal2, decodeInstance2);

        name = "decoder_conv3";
        cb = BufferPool.Get<float>(name, 128, 128, 64);
        SetDecoderBuffer(name, cb, decodeExpand4, decodeConv3, decodeNormal3, decodeInstance3);

        name = "decoder_conv3_conved";
        cb = BufferPool.Get<float>(name, 128, 128, 128);
        SetDecoderBuffer(name, cb, decodeConv3, decodeExpand3);

        name = "decoder_conv3_statistic";
        cb = BufferPool.Get<float>(name, 128);
        SetDecoderBuffer(name, cb, decodeNormal3, decodeConv3, decodeInstance3);

        name = "decoder_conv4";
        cb = BufferPool.Get<float>(name, 256, 256, 32);
        SetDecoderBuffer(name, cb, decodeExpand5, decodeConv4, decodeNormal4, decodeInstance4);

        name = "decoder_conv4_conved";
        cb = BufferPool.Get<float>(name, 256, 256, 64);
        SetDecoderBuffer(name, cb, decodeConv4, decodeExpand4, decodeExpand5);

        name = "decoder_conv4_statistic";
        cb = BufferPool.Get<float>(name, 64);
        SetDecoderBuffer(name, cb, decodeNormal4, decodeConv4, decodeInstance4);

        name = "decoder_conv5_pad";
        cb = BufferPool.Get<float>(name, 262, 262, 32);
        SetDecoderBuffer(name, cb, decodeExpand5, decodeConv5);

        decoderShader.SetTexture(decodeConv5, "decoder_destination", tempDestination);
    }
}