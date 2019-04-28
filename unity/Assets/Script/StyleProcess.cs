using UnityEngine;

public class StyleProcess : MonoBehaviour
{
    public ComputeShader encoderShader, decoderShader;
    public Renderer tempRender = null;
    private LoadCheckpoint checkpoint;
    private bool drawGui = true;
    private Model model;

    void Start()
    {
        if (tempRender.sharedMaterial.mainTexture == null)
        {
            tempRender.sharedMaterial.mainTexture = Resources.Load<Texture>("app1");
        }
        model = new Model(encoderShader, decoderShader);
        model.BindRender(tempRender);
        checkpoint = new LoadCheckpoint();
        checkpoint.Load(model.Process);
    }


    private void OnDestroy()
    {
        model.Dispose();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            drawGui = !drawGui;
        }
    }

    private void OnGUI()
    {
        if (!drawGui) return;
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