using UnityEngine;
using System.Collections.Generic;
using HeartOfIron.Enums;

public class ResourceManager : MonoBehaviour
{

    public static ResourceManager Instance { get; private set; }

    public GameObject ProjectilePrefab; // Assign this in the Unity Inspector

    List<NoiseParams> _noiseParamsList = new List<NoiseParams>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_noiseParamsList == null || _noiseParamsList.Count == 0)
        {
            _noiseParamsList = new List<NoiseParams>(Resources.LoadAll<NoiseParams>("NoiseParams"));
        }
    }

    public void LoadGame()
    {
        // Implement load game logic here

        List<ItemBlueprint> research = new List<ItemBlueprint>();
        research = CrossReferenceResources(research);
        GameManager.Instance.ClearAvailableResearch();
        foreach (ItemBlueprint itemBlueprint in research)
        {
            GameManager.Instance.AvailableResearch.Add(itemBlueprint);
        }
        // FOR TESTING PURPOSES ONLY: Display the available research items in the EngiBayUIManager
        // EngiBayUIManager.Instance.ShowEngiBayOptions();
    }

    List<ItemBlueprint> LoadResources()
    {
        List<ItemBlueprint> resources = new List<ItemBlueprint>();
        List<WeaponBlueprint> weapons = new List<WeaponBlueprint>(Resources.LoadAll<WeaponBlueprint>("Items/Weapons"));
        List<ArmorBlueprint> armors = new List<ArmorBlueprint>(Resources.LoadAll<ArmorBlueprint>("Items/Armors"));
        List<EnergyShieldBlueprint> energyShields = new List<EnergyShieldBlueprint>(Resources.LoadAll<EnergyShieldBlueprint>("Items/EnergyShields"));
        List<MechBlueprint> mechs = new List<MechBlueprint>(Resources.LoadAll<MechBlueprint>("Items/Mechs"));
        foreach (WeaponBlueprint weapon in weapons)
        {
            resources.Add(weapon);
        }
        foreach (ArmorBlueprint armor in armors)
        {
            resources.Add(armor);
        }
        foreach (EnergyShieldBlueprint energyShield in energyShields)
        {
            resources.Add(energyShield);
        }
        foreach (MechBlueprint mech in mechs)
        {
            resources.Add(mech);
        }
        return resources;
    }

    List<ItemBlueprint> CrossReferenceResources(List<ItemBlueprint> research)
    {
        List<ItemBlueprint> resources = LoadResources();
        foreach (ItemBlueprint resource in resources)
        {
            if (!research.Contains(resource))
            {
                research.Add(resource);
            }
        }
        return research;
    }

    public bool TryGetParametersFor(TerrainType type, out NoiseParams parameters)
    {
        Debug.Log($"Total NoiseParams loaded: {_noiseParamsList.Count}");
        foreach (NoiseParams param in _noiseParamsList)
        {
            if (param.TerrainType == type)
            {
                parameters = param;
                return true;
            }
        }
        parameters = null;
        return false;
    }
}
