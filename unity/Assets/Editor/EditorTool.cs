using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorTool
{
    public static string basepath
    {
        get
        {
            string path = Application.dataPath;
            path = path.Remove(path.IndexOf("/Assets"));
            return path;
        }
    }

    [MenuItem("Tools/MakeRT")]
    static void MakeRT()
    {
        int width = 4, height = 2;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture2D.filterMode = FilterMode.Point;
        texture2D.SetPixel(0, 0, new Color(1, 0, 0));
        texture2D.SetPixel(1, 0, new Color(0, 1, 0));
        texture2D.SetPixel(2, 0, new Color(0, 0, 1));
        texture2D.SetPixel(3, 0, new Color(1, 1, 0));
        
        texture2D.SetPixel(0, 1, new Color(1, 1, 0));
        texture2D.SetPixel(1, 1, new Color(1, 1, 0));
        texture2D.SetPixel(2, 1, new Color(1, 1, 1));
        texture2D.SetPixel(3, 1, new Color(1, 1, 1));
        texture2D.Apply(false);
        byte[] bytes = texture2D.EncodeToJPG(100);
        string path = Application.dataPath + "/Resources/RT.jpg";
        if (File.Exists(path)) File.Delete(path);
        FileStream file = File.Open(path, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        writer.Close();
        file.Close();
        EditorUtility.DisplayDialog("tip", "texture done", "ok");
        Open(path);
    }


    public static void Open(string path)
    {
        if (File.Exists(path))
        {
            path = Path.GetDirectoryName(path);
        }
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
#if UNITY_EDITOR_OSX
        string shell = basepath + "/Shell/open.sh";
        string arg = path;
        string ex = shell + " " + arg;
        System.Diagnostics.Process.Start("/bin/bash", ex);
#elif UNITY_EDITOR_WIN
        path = path.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", path);
#endif
    }
}
