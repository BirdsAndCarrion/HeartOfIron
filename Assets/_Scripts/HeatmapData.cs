using UnityEngine;

[CreateAssetMenu(fileName = "HeatmapData", menuName = "Scriptable Objects/HeatmapData")]
public class HeatmapData : ScriptableObject
{
    public int height;
    public int width;

    [HideInInspector]
    public float[] values;

    public float GetValue(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            Debug.LogError("Index out of bounds");
            return 0f;
        }
        return values[y * width + x];
    }
}
