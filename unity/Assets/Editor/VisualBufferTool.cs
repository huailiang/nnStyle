using UnityEditor;
using UnityEngine;

public class VisualBufferTool : EditorWindow
{
    GUIStyle boldLableStyle, textureStyle;
    int[] shape;
    float[] layers;
    int fslider, pslider;
    Texture2D texture;
    const float e = 2.7182818f;
    LoadCheckpoint checkpoint;

    int tx = 20, ty = 80, tw = 360;
    Rect texRect = Rect.zero;
    string[] p_layer = { "encoder_c1", "encoder_c2", "encoder_c3", "encoder_c4",
                        "decoder_d1", "decoder_d2", "decoder_d3", "decoder_d4", "decoder_r1" };
    int select = 0, p_select = -1;

    [MenuItem("Tools/LayerVisual")]
    static void VisualTool()
    {
        EditorWindow.GetWindowWithRect(typeof(VisualBufferTool), new Rect(0, 0, 400, 600), true, "Visual Buffer");
    }

    private void Init()
    {
        if (checkpoint == null)
        {
            checkpoint = new LoadCheckpoint();
        }
        if (boldLableStyle == null)
        {
            boldLableStyle = new GUIStyle(EditorStyles.label);
            boldLableStyle.fontSize = 22;
            boldLableStyle.fontStyle = FontStyle.Bold;
        }
        if (textureStyle == null)
        {
            textureStyle = new GUIStyle(EditorStyles.objectField);
        }
        texRect.x = tx;
        texRect.y = ty;
        texRect.width = tw;
        texRect.height = tw;
    }


    void OnGUI()
    {
        Init();
        GUILayout.Space(10);

        GUILayout.BeginVertical();
        GUILayout.Label("Visual NN Layer Buffer", boldLableStyle);
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        select = EditorGUILayout.Popup(select, p_layer);
        if (p_select != select)
        {
            string str = p_layer[select];
            if (!string.IsNullOrEmpty(str))
            {
                fslider = 0;
                layers = checkpoint.LoadLayer(str, out shape);
                DrawTexture();
            }
            p_select = select;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(380);

        if (texture != null)
        {
            EditorGUI.DrawPreviewTexture(texRect, texture);

            if (shape != null)
            {
                GUILayout.Space(8);
                fslider = EditorGUILayout.IntSlider(fslider, 0, depth - 1);
                if (fslider != pslider)
                {
                    z = fslider;
                    UpdateTexture();
                    pslider = fslider;
                }
                EditorGUILayout.LabelField(string.Format("shape:     {0}x{1}x{2}", shape[0], shape[1], shape[2]));
                float v = 0; int xx, yy;
                if (sample(Event.current.mousePosition, out v, out xx, out yy))
                {
                    EditorGUILayout.LabelField(string.Format("current:  {0} coord:({1},{2})", v, xx, yy));
                }
            }
        }
        GUILayout.EndVertical();
    }


    int z = 0, width = 0, height = 0, depth = 0;
    void DrawTexture()
    {
        width = shape[0];
        height = shape[1];
        depth = shape[2];
        texture = new Texture2D(width, height);
        UpdateTexture();
    }


    void UpdateTexture()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int idx = i * width * depth + j * depth + z;
                float v = idx < layers.Length ? sigmod(layers[idx]) : 0;
                Color color = new Color(v, v, v, 1);
                texture.SetPixel(j, height - 1 - i, color);
            }
        }
        texture.Apply();
    }


    float sigmod(float x)
    {
        return 1 / (1 + Mathf.Pow(e, -x));
    }

    bool sample(Vector2 pos, out float v, out int xx, out int yy)
    {
        float x = pos.x - tx;
        float y = pos.y - ty;
        yy = Mathf.FloorToInt(x * width / (float)tw);  //low dimension
        xx = height - Mathf.FloorToInt(y * height / (float)tw); //high dimension 
        v = 0f;
        if (xx < width && yy < height)
        {
            v = sample(xx, yy);
            return true;
        }
        return false;
    }

    float sample(int x, int y)
    {
        int idx = x * width * depth + y * depth + z;
        if (idx < layers.Length && idx >= 0)
        {
            return layers[idx];
        }
        return 0f;
    }

}
