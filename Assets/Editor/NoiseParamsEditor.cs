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
                terrainGenerator.PlaceObjectsOnTerrain(noiseParams.CoverObjects, noiseParams.BuildingObjects, terrainGenerator.GetHeatMapData(), noiseParams.CoverDensity);
            }
        }

        if (GUILayout.Button("Clear Placed Objects"))
        {
            TerrainGenerator terrainGenerator = FindFirstObjectByType<TerrainGenerator>();
            if (terrainGenerator != null)
            {
                foreach (GameObject coverObject in terrainGenerator.CoverObjectParent)
                {
                    DestroyImmediate(coverObject);
                }
                terrainGenerator.CoverObjectParent.Clear();

                foreach (GameObject buildingObject in terrainGenerator.BuildingObjectParent)
                {
                    DestroyImmediate(buildingObject);
                }
                terrainGenerator.BuildingObjectParent.Clear();
            }
        }
    }
}
