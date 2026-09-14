using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseParams))]
public class NoiseParamsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            TerrainGenerator terrainGenerator = FindFirstObjectByType<TerrainGenerator>();
            NoiseParams noiseParams = (NoiseParams)target;
            if (terrainGenerator != null)
            {
                terrainGenerator.GenerateHeightmap(noiseParams);
            }
        }
    }
}
