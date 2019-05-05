using UnityEngine;

public class StyleProcess : MonoBehaviour
{
    public ComputeShader encoderShader, decoderShader;
    public Renderer tempRender = null;
    public bool realtimeRender = true;
    private LoadCheckpoint checkpoint;
    private bool drawGui = true;
    private Model model;

    void Start()
    {
        if (tempRender.sharedMaterial.mainTexture == null)
        {
            tempRender.sharedMaterial.mainTexture = Resources.Load<Texture>("app2");
        }
        model = new Model(encoderShader, decoderShader);
        model.BindRender(tempRender, realtimeRender);
        checkpoint = new LoadCheckpoint();
        checkpoint.Load(model.Process);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst);
        if (realtimeRender && model != null)
        {
            model.RebindSource(src);
            model.DrawEncoder();
            model.DrawResidule();
            model.DrawDecoder();
        }
    }

    private void OnDestroy()
    {
        model.Dispose();
    }

    private void Update()
    {
        if (!realtimeRender && Input.GetKeyUp(KeyCode.Space))
        {
            drawGui = !drawGui;
        }
    }

    private void OnGUI()
    {
        if (!drawGui || realtimeRender) return;
        if (GUI.Button(new Rect(20, 20, 80, 40), "Run"))
        {
            model.DrawEncoder();
            model.DrawResidule();
            model.DrawDecoder();
        }
        if (GUI.Button(new Rect(20, 80, 80, 40), "Decode"))
        {
            float[] layer = checkpoint.LoadLayer("encoder_c5");
            BufferPool.Get("input_initial").SetData(layer);
            model.DrawResidule();
            model.DrawDecoder();
        }
        if (GUI.Button(new Rect(20, 140, 80, 40), "Output"))
        {
            float[] layer = checkpoint.LoadLayer("decoder_d4");
            BufferPool.Get("decoder_conv4").SetData(layer);
            model.DrawRenderTexure();
        }
    }


}