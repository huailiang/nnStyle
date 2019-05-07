using UnityEditor;
using UnityEngine;

public class VisualBufferWindow : LgWindow
{
    int[] shape;
    float[] layer;
    int fslider, pslider;
    int select = 0, p_select = -1;
    int tx = 20, ty = 95, tw = 360;
    Texture2D texture;
    Material mat;
    const float e = 2.7182818f;
    LoadCheckpoint checkpoint;
    bool showgrid;
    Rect texRect = Rect.zero;
    string[] p_layer = { "encoder_c1", "encoder_c2", "encoder_c3", "encoder_c4",
                        "decoder_d1", "decoder_d2", "decoder_d3", "decoder_d4", "decoder_r1" };


    [MenuItem("Tools/LayerVisual")]
    static void VisualTool()
    {
        LgWindow.GetWindow<VisualBufferWindow>(400, 600, "LayerWindow");
    }

    protected override void LgInit()
    {
        checkpoint = new LoadCheckpoint();
        showgrid = true;
        mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Editor/EditorRes/grid.mat");
        texRect.x = tx;
        texRect.y = ty;
        texRect.width = tw;
        texRect.height = tw;
    }


    protected override void LgGUI()
    {
        GUILayout.Space(10);

        GUILayout.BeginVertical();
        GUILayout.Label("Visual Nearual Layer", boldLableStyle);
        GUILayout.Space(8);

        select = EditorGUILayout.Popup(select, p_layer);
        showgrid = EditorGUILayout.Toggle("show grid", showgrid);
        if (p_select != select)
        {
            string str = p_layer[select];
            if (!string.IsNullOrEmpty(str))
            {
                fslider = 0;
                layer = null;
                if (checkpoint.LoadLayer(str, out shape, out layer))
                    DrawTexture();
            }
            p_select = select;
        }
        GUILayout.Space(30 + tw);

        if (texture != null)
        {
            EditorGUI.DrawPreviewTexture(texRect, texture, showgrid ? mat : null);

            if (shape != null)
            {
                EditorGUILayout.LabelField("drag slider to view different depth in selected layer");

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
                float v = idx < layer.Length ? sigmod(layer[idx]) : 0;
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
        xx = Mathf.FloorToInt(y * height / (float)tw); //high dimension 
        v = 0f;
        if (xx >= 0 && xx < width && yy >= 0 && yy < height)
        {
            v = sample(xx, yy);
            return true;
        }
        return false;
    }

    float sample(int x, int y)
    {
        int idx = x * width * depth + y * depth + z;
        if (idx < layer.Length)
        {
            return layer[idx];
        }
        return 0f;
    }

}
