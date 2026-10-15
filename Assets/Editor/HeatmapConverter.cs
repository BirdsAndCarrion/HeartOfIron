#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class HeatmapConverter : EditorWindow
{
    [MenuItem("Tools/Convert Heatmap Texture to Asset")]
    public static void ConvertSelectedTexture()
    {
        Texture2D selectedTexture = Selection.activeObject as Texture2D;
        if (selectedTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "No texture selected.", "OK");
            return;
        }

        try
        {
            Color[] pixels = selectedTexture.GetPixels();

            HeatmapData dataAsset = ScriptableObject.CreateInstance<HeatmapData>();
            dataAsset.width = selectedTexture.width;
            dataAsset.height = selectedTexture.height;
            dataAsset.values = new float[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                dataAsset.values[i] = pixels[i].r;
            }

            string path = AssetDatabase.GetAssetPath(selectedTexture);
            string newPath = Path.ChangeExtension(path, ".asset");

            AssetDatabase.CreateAsset(dataAsset, newPath);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", "Heatmap asset created at:\n" + newPath, "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Failed to convert texture to heatmap asset.\n" + e.Message, "OK");
        }
    }
}
#endif
