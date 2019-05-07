using UnityEditor;
using UnityEngine;

public class CopyrightWindow : EditorWindow
{
    Texture2D texture;
    Material mat;
    float factor;

    Rect texRect = Rect.zero;

    [MenuItem("Tools/CopyRight")]
    static void CopyRight()
    {
        EditorWindow.GetWindowWithRect(typeof(CopyrightWindow), new Rect(0, 0, 280, 300), true, "CopyRight");
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
            factor += Time.deltaTime * 0.4f;
            factor = Mathf.Clamp(factor, 0, 0.5f);
            if (factor <= 0.5f)
            {
                mat.SetFloat("_DistortFactor", factor);
                Repaint();
            }
        }
    }

    void OnGUI()
    {
        if (texture != null)
        {
            EditorGUI.DrawPreviewTexture(texRect, texture, mat);
        }
        EditorGUI.LabelField(new Rect(10, 280, 200, 20), "copyright@huailiang");
    }

}


public abstract class LgWindow : EditorWindow
{
    protected static GUIStyle boldLableStyle, textureStyle, labelBtnStyle;
    private int width, height;
    private bool inited = false;

    private static void InitStyle()
    {
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
        if (labelBtnStyle == null)
        {
            labelBtnStyle = new GUIStyle(EditorStyles.miniButton);
        }
    }

    public void Set(int w, int h)
    {
        width = w;
        height = h;
    }


    public static EditorWindow GetWindow<T>(int width, int height, string title) where T : LgWindow
    {
        EditorWindow window = EditorWindow.GetWindowWithRect(typeof(T), new Rect(0, 0, width, height + 30), true, title);
        (window as LgWindow).Set(width, height);
        InitStyle();
        return window;
    }


    private void OnGUI()
    {
        if (!inited)
        {
            inited = true;
            LgInit();
        }
        LgGUI();
        EditorGUI.LabelField(new Rect(width - 120, height + 10, 100, 30), "copyright @2019");
    }

    protected abstract void LgGUI();

    protected virtual void LgInit() { }

}
