using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using HeartOfIron.Enums;
using System.Collections.Generic;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour
{

    public static TerrainGenerator Instance { get; private set; }

    [Header("--- Terrain Configuration ---")]
    [Tooltip("The type of terrain to generate.")]
    [SerializeField] private TerrainType _terrainType;
    [SerializeField] private bool _debugMode = false;

    [Space]
    public float terrainSize = 1000f;
    public float scale = 10f;
    public int resolution = 513;
    public float heightMultiplier = 100f;

    private float[,] _heightMap;
    private float _seedOffset;

    [Header("--- Colorization ---")]
    [Tooltip("Gradient to use for coloring the terrain based on height.")]
    [SerializeField] private Gradient _terrainGradient;

    [Space]
    [SerializeField] private Terrain _terrain;
    public Terrain TerrainData => _terrain;

    [Header("--- Cover Heatmaps ---")]
    [SerializeField] private List<HeatmapData> _coverHeatmap;
    [SerializeField] private int MAX_COVER_OBJECTS = 100;
    public List<GameObject> CoverObjectParent;
    public List<GameObject> BuildingObjectParent;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (_terrain == null)
        {
            Debug.LogError("Terrain reference is not set in the inspector.");
            return;
        }

        _seedOffset = UnityEngine.Random.Range(0f, 100000f);

        if (Application.isEditor && _debugMode)
        {
            GenerateHeightmap(null, true);
        } else
        {
            GenerateHeightmap();
        }

        NavMeshSurface navMeshSurface = _terrain.gameObject.GetComponent<NavMeshSurface>();
        if (navMeshSurface != null)
        {
            navMeshSurface.collectObjects = CollectObjects.Children;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh built successfully.");
        }
        else
        {
            Debug.LogError("NavMeshSurface component not found on the terrain GameObject.");
        }

        if (!ResourceManager.Instance.TryGetParametersFor(_terrainType, out var noiseParams))
        {
            Debug.LogError($"Noise parameters for terrain type {_terrainType} not found.");
            return;
        }

        PlaceObjectsOnTerrain(noiseParams.CoverObjects, noiseParams.BuildingObjects, _coverHeatmap[UnityEngine.Random.Range(0, _coverHeatmap.Count)], noiseParams.CoverDensity);

        // ColorizeTerrain();


    }

    public void GenerateHeightmap(NoiseParams noiseParams = null, bool debugMode = false)
    {
        if (_terrain == null)
        {
            Debug.LogError("Terrain reference is not set in the inspector.");
            return;
        }

        TerrainData terrainData = _terrain.terrainData;
        terrainData.size = new Vector3(terrainData.size.x, heightMultiplier, terrainData.size.z);
        if (terrainData == null)
        {
            Debug.LogError("TerrainData is not set on the Terrain component.");
            return;
        }

        if (noiseParams == null && !ResourceManager.Instance.TryGetParametersFor(_terrainType, out noiseParams))
        {
            Debug.LogError($"Noise parameters for terrain type {_terrainType} not found.");
            return;
        }

        int heightmapResolution = terrainData.heightmapResolution;
        if (heightmapResolution != resolution)
        {
            Debug.LogWarning($"Configured resolution ({resolution}) does not match TerrainData.heightmapResolution ({heightmapResolution}). Using TerrainData's resolution.");
        }

        _heightMap = new float[heightmapResolution, heightmapResolution];

        scale = noiseParams.Scale; // Use the scale from NoiseParams

        if (scale <= 0)
        {
            scale = 0.0001f; // Prevent division by zero or negative scale
        }

        if (noiseParams == null)
        {
            Debug.LogError("Noise parameters are not set.");
            return;
        }
        else
        {
            Debug.Log($"Generating heightmap for terrain type: {_terrainType} with parameters: Scale={noiseParams.Scale}, Frequency={noiseParams.Frequency}, Octaves={noiseParams.Octaves}, Lacunarity={noiseParams.Lacunarity}, Persistence={noiseParams.Persistence}, Amplitude={noiseParams.Amplitude}");
        }

        Vector2[] offsets = new Vector2[noiseParams.Octaves];
        for (int i = 0; i < noiseParams.Octaves; i++)
        {
            float offsetX = UnityEngine.Random.Range(-100000f, 100000f) + _seedOffset;
            float offsetY = UnityEngine.Random.Range(-100000f, 100000f) + _seedOffset;
            offsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        float halfWidth = heightmapResolution / 2f;
        float halfHeight = heightmapResolution / 2f;

        for (int y = 0; y < heightmapResolution; y++)
        {
            for (int x = 0; x < heightmapResolution; x++)
            {

                float amplitude = noiseParams.Amplitude;
                float frequency = noiseParams.Frequency;
                float noiseHeight = 0f;

                for (int octave = 0; octave < noiseParams.Octaves; octave++)
                {
                    float xCoord = ((float)x - halfWidth) / scale * frequency + offsets[octave].x;
                    float yCoord = ((float)y - halfHeight) / scale * frequency + offsets[octave].y;
                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                    /*if (perlinValue < noiseParams.MinHeight)
                    {
                        perlinValue = 0f;
                    } else
                    {
                        perlinValue -= noiseParams.MinHeight;
                    }*/
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= noiseParams.Persistence;
                    frequency *= noiseParams.Lacunarity;
                }
                if (debugMode)
                {
                    _heightMap[y, x] = 0f;
                } else 
                {
                    _heightMap[y, x] = noiseHeight;
                }
                if (noiseHeight > maxHeight) maxHeight = noiseHeight;
                if (noiseHeight < minHeight) minHeight = noiseHeight;
            }
        }

        for (int y = 0; y < heightmapResolution; y++)
        {
            for (int x = 0; x < heightmapResolution; x++)
            {
                _heightMap[y, x] = Mathf.InverseLerp(minHeight, maxHeight, _heightMap[y, x]);
            }
        }

        // Debug.Log($"Heightmap generated. Min height: {minHeight}, Max height: {maxHeight}");

        // Update the InputManager's maximum camera height based on the new terrain height
        InputManager inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            inputManager.UpdateMaximumCameraHeight(maxHeight * heightMultiplier);
        }

        terrainData.SetHeights(0, 0, _heightMap);
    }

    public void PlaceObjectsOnTerrain(List<CoverObjectDetails> coverObjects, List<BuildingObjectDetails> buildingObjects, HeatmapData heatmap, float threshold)
    {
        if (coverObjects == null || coverObjects.Count == 0)
        {
            Debug.LogWarning("No cover objects provided for placement on the terrain.");
            return;
        }
        if (buildingObjects == null)
        {
            Debug.LogWarning("No building objects provided for placement on the terrain.");
            return;
        }
        if (heatmap == null)
        {
            Debug.LogWarning("No heatmap provided for object placement.");
            return;
        }

        List<Vector2> placedObjectPositions = new List<Vector2>();

        int heatmapWidth = heatmap.width;
        int heatmapHeight = heatmap.height;

        int placedObjectsCount = 0;
        while (placedObjectsCount < MAX_COVER_OBJECTS)
        {

            int randX = UnityEngine.Random.Range(0, heatmapWidth);
            int randY = UnityEngine.Random.Range(0, heatmapHeight);
            Vector2 selectedPosition2D = new Vector2(randX, randY);
            int attempts = 0;
            while (placedObjectPositions.Contains(selectedPosition2D) && attempts < 100)
            {
                randX = UnityEngine.Random.Range(0, heatmapWidth);
                randY = UnityEngine.Random.Range(0, heatmapHeight);
                selectedPosition2D = new Vector2(randX, randY);
                if (attempts >= 100)
                {
                    Debug.LogWarning("Failed to find a suitable position for the cover object after 100 attempts.");
                    break;
                }
                attempts++;
            }
            placedObjectPositions.Add(selectedPosition2D);
            
            float selectedPixel = heatmap.GetValue(randX, randY);

            if (selectedPixel > threshold)
            {
                // Decide between structure types
                float structureChoice = UnityEngine.Random.value;
                StructureType selectedStructure = structureChoice < 0.95f ? StructureType.Cover : buildingObjects.Count > 0 ? StructureType.Building : StructureType.Cover;
                // Select formation if appropriate
                if (selectedStructure == StructureType.Cover)
                {
                    float formationChoice = UnityEngine.Random.value;
                    CoverFormation selectedFormation = formationChoice < 0.5f ? CoverFormation.Wall : (formationChoice < 0.8f ? CoverFormation.Cluster : CoverFormation.Single);
                    CoverObjectDetails selectedCover = coverObjects[UnityEngine.Random.Range(0, coverObjects.Count)];
                    if (!TryPlaceCoverOnTerrain(selectedCover, new Vector3(selectedPosition2D.x, GetHeightAtPosition(selectedPosition2D), selectedPosition2D.y), selectedFormation, new List<Vector3>()))
                    {
                        Debug.LogWarning("Failed to place cover on terrain at position: " + selectedPosition2D);
                        continue;
                    }
                }
                else
                {
                    BuildingObjectDetails selectedBuilding = buildingObjects[UnityEngine.Random.Range(0, buildingObjects.Count)];
                    if (!TryPlaceBuildingOnTerrain(selectedBuilding, new Vector3(selectedPosition2D.x, GetHeightAtPosition(selectedPosition2D), selectedPosition2D.y)))
                    {
                        Debug.LogWarning("Failed to place building on terrain at position: " + selectedPosition2D);
                        continue;
                    }
                }
            }
            placedObjectsCount++;
        }

    }

    private bool TryPlaceCoverOnTerrain(CoverObjectDetails coverObject, Vector3 position, CoverFormation formation, List<Vector3> placedObjectPositions, float recurranceValue = 1)
    {
        RaycastHit hit;
        Vector3 worldSize = Vector3.zero;
        Debug.DrawRay(position + Vector3.up * 100f, Vector3.down * 200f, Color.red, 10f);
        if (Physics.Raycast(position + Vector3.up * 100f, Vector3.down, out hit, 200f))
        {
            position = hit.point;
            if (!hit.collider.gameObject.GetComponent<Terrain>())
            {
                Debug.LogWarning("Raycast hit a non-terrain object: " + hit.collider.name);
                return false;
            }
            worldSize = coverObject.CoverObject.GetComponent<Renderer>().bounds.size;
            Collider[] colliders = Physics.OverlapSphere(position, (worldSize.magnitude / 2f) + coverObject.DistanceNeeded);
            if (colliders.Length > 0)
            {
                foreach (Collider collider in colliders)
                {
                    if (!collider.gameObject.GetComponent<Terrain>())
                    {
                        Debug.LogWarning("Hit: " + collider.name);
                        return false;
                    }
                }
            }
            GameObject coverInstance = Instantiate(coverObject.CoverObject, position, Quaternion.identity);
            CoverObjectParent.Add(coverInstance);
            placedObjectPositions.Add(position);
            coverInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward); // Adjust rotation as needed
            coverInstance.transform.position = position; // Ensure the cover is placed at the correct position
        } else
        {
            Debug.LogWarning("Raycast did not hit any terrain.");
            return false;
        }
        if (UnityEngine.Random.value < recurranceValue)
        {
            switch (formation)
            {
                case CoverFormation.Wall:
                    if (placedObjectPositions.Count > 1)
                    {
                        Vector3 direction = (placedObjectPositions[placedObjectPositions.Count - 1] - placedObjectPositions[placedObjectPositions.Count - 2]).normalized;
                        Vector3 wallPosition = placedObjectPositions[placedObjectPositions.Count - 1] + direction * (worldSize.magnitude + coverObject.DistanceNeeded);
                        wallPosition.y = GetHeightAtPosition(wallPosition.x, wallPosition.z);
                        Debug.Log("Attempting to place wall cover at position: " + wallPosition);
                        TryPlaceCoverOnTerrain(coverObject, wallPosition, formation, placedObjectPositions, recurranceValue * coverObject.SiblingChance);
                    } else
                    {
                        Vector3 direction = UnityEngine.Random.onUnitSphere * (worldSize.magnitude + coverObject.DistanceNeeded);
                        Vector3 wallPosition = placedObjectPositions[placedObjectPositions.Count - 1] + direction;
                        wallPosition.y = GetHeightAtPosition(wallPosition.x, wallPosition.z);
                        Debug.Log("Attempting to place wall cover at position: " + wallPosition);
                        TryPlaceCoverOnTerrain(coverObject, wallPosition, formation, placedObjectPositions, recurranceValue * coverObject.SiblingChance);
                    }
                    break;
                case CoverFormation.Cluster:
                    Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * coverObject.ClusteringRadius;
                    Vector3 clusterPosition = new Vector3(randomPoint.x, GetHeightAtPosition(randomPoint.x, randomPoint.y), randomPoint.y);
                    clusterPosition.y = GetHeightAtPosition(clusterPosition.x, clusterPosition.z);
                    Debug.Log("Attempting to place cluster cover at position: " + clusterPosition);
                    TryPlaceCoverOnTerrain(coverObject, clusterPosition, formation, placedObjectPositions, recurranceValue * coverObject.SiblingChance);
                    break;
                default:
                    // Handle unexpected formation types here
                    break;
            }
        }
        return true;
    }

    private bool TryPlaceBuildingOnTerrain(BuildingObjectDetails buildingObject, Vector3 position)
    {
        // Flatten the terrain around the building position
        FlattenTerrain(position, buildingObject.DistanceNeeded + buildingObject.FlatteningRadius);
        // Update the terrain with the modified heightmap
        _terrain.terrainData.SetHeights(0, 0, _heightMap);
        // Apply the changes to the terrain
        _terrain.Flush();
        // Instantiate the building object at the specified position
        GameObject buildingInstance = Instantiate(buildingObject.BuildingObject, position, Quaternion.identity);
        BuildingObjectParent.Add(buildingInstance);
        return true;

    }

    private void FlattenTerrain(Vector3 position, float radius)
    {
        TerrainData terrainData = _terrain.terrainData;
        int heightmapResolution = terrainData.heightmapResolution;

        float normalizedX = Mathf.InverseLerp(0, terrainData.size.x, position.x);
        float normalizedZ = Mathf.InverseLerp(0, terrainData.size.z, position.z);

        int mapX = Mathf.Clamp(Mathf.RoundToInt(normalizedX * (heightmapResolution - 1)), 0, heightmapResolution - 1);
        int mapZ = Mathf.Clamp(Mathf.RoundToInt(normalizedZ * (heightmapResolution - 1)), 0, heightmapResolution - 1);

        int radiusInPixels = Mathf.RoundToInt(radius / terrainData.size.x * heightmapResolution);

        for (int z = mapZ - radiusInPixels; z <= mapZ + radiusInPixels; z++)
        {
            for (int x = mapX - radiusInPixels; x <= mapX + radiusInPixels; x++)
            {
                if (x >= 0 && x < heightmapResolution && z >= 0 && z < heightmapResolution)
                {
                    _heightMap[z, x] = _heightMap[mapZ, mapX];
                }
            }
        }
    }

    void ColorizeTerrain()
    {
        // Implementation for colorizing the terrain goes here
    }

    public float GetHeightAtPosition(Vector2 position2D)
    {
        return GetHeightAtPosition(position2D.x, position2D.y);
    }

    public float GetHeightAtPosition(float x, float z)
    {
        if (_heightMap == null)
        {
            Debug.LogError("Heightmap is not generated yet.");
            return 0f;
        }

        TerrainData terrainData = _terrain.terrainData;
        int heightmapResolution = terrainData.heightmapResolution;

        float normalizedX = Mathf.InverseLerp(0, terrainData.size.x, x);
        float normalizedZ = Mathf.InverseLerp(0, terrainData.size.z, z);

        int mapX = Mathf.Clamp(Mathf.RoundToInt(normalizedX * (heightmapResolution - 1)), 0, heightmapResolution - 1);
        int mapZ = Mathf.Clamp(Mathf.RoundToInt(normalizedZ * (heightmapResolution - 1)), 0, heightmapResolution - 1);

        return _heightMap[mapZ, mapX] * heightMultiplier;
    }

    public HeatmapData GetHeatMapData()
    {
        return _coverHeatmap[UnityEngine.Random.Range(0, _coverHeatmap.Count)];
    }

}
