using UnityEngine;
using System;
using HeartOfIron.Enums;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NoiseParams", menuName = "Scriptable Objects/NoiseParams")]
public class NoiseParams : ScriptableObject
{
    public TerrainType TerrainType;
    [Tooltip("Changes the overall size of the terrain features. Lower values will create more extreme terrain, while higher values will create flatter terrain.")]
    public float Scale;
    [Tooltip("Changes the x axis of the curve, changing the frequency of the terrain. Lower values will create more extreme terrain, while higher values will create flatter terrain.")]
    public float Frequency;
    [Tooltip("The number of curves to combine. More octaves will create more detail, but will also increase computation time.")]
    public int Octaves;
    [Tooltip("The frequency multiplier for each octave. Higher values increases the number of small features.")]
    public float Lacunarity;
    [Tooltip("The amplitude multiplier for each octave. Higher values increases the impact of smaller features.")]
    [Range(0f, 1f)]
    public float Persistence;
    [Tooltip("Sets the minimum acceptable value at which height is considered. Any height below this will be clamped to zero.")]
    [Range(0f, 1f)]
    public float MinHeight;
    [Tooltip("Changes the y axis of the curve, changing the severity of the terrain. Lower values will create more extreme terrain, while higher values will create flatter terrain.")]
    public float Amplitude;
    [Tooltip("The density of the cover objects (trees, rocks, etc.) on the terrain. Lower values will create more sparse cover, while higher values will create denser cover.")]
    [Range(0f, 1f)]
    public float CoverDensity;
    [Tooltip("The list of cover objects (trees, rocks, etc.) that can appear on the terrain.")]
    public List<CoverObjectDetails> CoverObjects;
    [Tooltip("The list of building objects that can appear on the terrain.")]
    public List<BuildingObjectDetails> BuildingObjects;

    private void OnValidate()
    {
        Scale = Mathf.Max(0.0001f, Scale);
        Frequency = Mathf.Max(0.0001f, Frequency);
        Octaves = Mathf.Max(1, Octaves);
        Lacunarity = Mathf.Max(1f, Lacunarity);
        Persistence = Mathf.Clamp01(Persistence);
        Amplitude = Mathf.Max(0.0001f, Amplitude);
        CoverDensity = Mathf.Clamp01(CoverDensity);
    }
}

[Serializable]
public class CoverObjectDetails
{
    public GameObject CoverObject;
    [Tooltip("The minimum distance required between this cover object and other objects.")]
    [Min(0f)]
    public float DistanceNeeded;
    [Range(0f, 1f)]
    public float SiblingChance;
    [Tooltip("The radius within which any the next sibling cover object can appear.")]
    [Min(0f)]
    public float ClusteringRadius;
}

[Serializable]
public class BuildingObjectDetails
{
    public GameObject BuildingObject;
    [Tooltip("The minimum distance required between this building object and other objects.")]
    [Min(0f)]
    public float DistanceNeeded;
    [Tooltip("The radius around the building that will be flattened.")]
    [Min(0f)]
    public float FlatteningRadius;
}
