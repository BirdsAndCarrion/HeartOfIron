using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using HeartOfIron.Enums;

public class TerrainGenerator : MonoBehaviour
{

    public static TerrainGenerator Instance { get; private set; }

    [Header("--- Terrain Configuration ---")]
    [Tooltip("The type of terrain to generate.")]
    [SerializeField] private TerrainType _terrainType;

    [Space]
    public float terrainSize = 1000f;
    public float scale = 10f;
    public int resolution = 513;
    public float heightMultiplier = 100f;

    [Header("--- Object Placement Configuration ---")]
    [Tooltip("The prefab to place on the terrain.")]
    [SerializeField] private GameObject _objectPrefab;

    private float[,] _heightMap;
    private float _seedOffset;

    [Header("--- Colorization ---")]
    [Tooltip("Gradient to use for coloring the terrain based on height.")]
    [SerializeField] private Gradient _terrainGradient;

    [Space]
    [SerializeField] private Terrain _terrain;
    public Terrain TerrainData => _terrain;

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

        GenerateHeightmap();

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

        if (_objectPrefab != null)
        {
            // PlaceObjectsOnTerrain();
        }
        else
        {
            Debug.LogWarning("Object prefab is not set. No objects will be placed on the terrain.");
        }

        // ColorizeTerrain();


    }

    public void GenerateHeightmap(NoiseParams noiseParams = null)
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
                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) /** 2 - 1*/; // Adjust Perlin noise to range [-1, 1]
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= noiseParams.Persistence;
                    frequency *= noiseParams.Lacunarity;
                }

                _heightMap[y, x] = noiseHeight;
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

    void ColorizeTerrain()
    {
    
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

}
