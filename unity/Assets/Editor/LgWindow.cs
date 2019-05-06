using UnityEditor;
using UnityEngine;

public class LgWindow : EditorWindow
{

    Texture2D texture;
    Material mat;
    float factor;

    Rect texRect = Rect.zero;

    [MenuItem("Tools/CopyRight")]
    static void CopyRight()
    {
        EditorWindow.GetWindowWithRect(typeof(LgWindow), new Rect(0, 0, 280, 300), true, "");
    }

    private void OnEnable()
    {
        factor = 0;
        texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/EditorRes/author.jpg");
        mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Editor/EditorRes/fade.mat");
        texRect = new Rect(10, 10, 256, 256);
    }

    private void OnInspectorUpdate()
    {
        if (mat != null)
        {
            factor += Time.deltaTime * 0.1f;
            factor = Mathf.Clamp(factor, 0, 0.5f);
            Debug.Log(factor);
            mat.SetFloat("_DistortFactor", factor);
        }
    }

    void OnGUI()
    {
        if (texture != null)
        {
            EditorGUI.DrawPreviewTexture(texRect, texture, mat);


        }
        EditorGUI.LabelField(new Rect(10, 280, 200, 20), "copyright");

    }


}
