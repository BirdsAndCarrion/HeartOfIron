using UnityEngine;
using System;
using System.Collections.Generic;
using HeartOfIron.Enums;

[Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeReference] private List<Item> _playerInventory = new List<Item>();
    public List<Item> PlayerInventory => _playerInventory;
    [SerializeField] private List<ItemBlueprint> _availableResearch = new List<ItemBlueprint>();
    public List<ItemBlueprint> AvailableResearch => _availableResearch;

    [SerializeField] private float _playerBolts;
    public float Bolts => _playerBolts;
    [SerializeField] private float _gameTime;
    public float GameTime => _gameTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (_playerInventory == null)
            {
                _playerInventory = new List<Item>();
            }
            if (_availableResearch == null)
            {
                _availableResearch = new List<ItemBlueprint>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        _gameTime += Time.deltaTime;
        if (_gameTime >= 1f)
        {
            _gameTime = 0f;
            // TimePasses(1); // Call TimePasses with 1 time unit every second
        }
    }

    public void SaveGame()
    {
        SaveManager.Init();
        SaveObject saveObject = new SaveObject(_gameTime, _playerBolts, _playerInventory, _availableResearch, UnitManager.Instance.PlayerMechs, UnitManager.Instance.PlayerPilots);
        string serializedData = saveObject.Serialize();
        Debug.Log($"Game saved. Serialized data: {serializedData}");
        SaveManager.SaveGame(serializedData);
    }

    public void LoadGame()
    {
        SaveObject saveObject = SaveManager.LoadGame();
        if (saveObject != null)
        {
            _gameTime = saveObject.GameTime;
            _playerBolts = saveObject.PlayerBolts;
            _playerInventory = saveObject.PlayerInventory;
            _availableResearch = saveObject.AvailableResearch;
            List<Mech> loadedMechs = saveObject.PlayerMechs;
            List<Pilot> loadedPilots = saveObject.PlayerPilots;
            foreach (Mech mech in loadedMechs)
            {
                if (!UnitManager.Instance.PlayerMechs.Contains(mech))
                {
                    UnitManager.Instance.PlayerMechs.Add(mech);
                }
            }
            foreach (Pilot pilot in loadedPilots)
            {
                if (!UnitManager.Instance.PlayerPilots.Contains(pilot))
                {
                    foreach (Mech mech in UnitManager.Instance.PlayerMechs)
                    {
                        if (mech.Pilot != null && mech.Pilot.Id == pilot.Id)
                        {
                            pilot.AssignedMech = mech; // Assign the loaded mech to the pilot
                            break;
                        }
                    }
                    if (pilot.AssignedMech == null)
                    {
                        Debug.LogWarning($"Pilot {pilot.Name} has no assigned mech.");
                    }
                    UnitManager.Instance.PlayerPilots.Add(pilot);
                }
            }
        } else
        {
            Debug.LogWarning("No save data found. Starting a new game.");
            StartNewGame();
        }
        ResourceManager.Instance.LoadGame();
    }

    void StartNewGame()
    {
        _gameTime = 0f;
        _playerBolts = 0f;
        _playerInventory.Clear();
        _availableResearch.Clear();
        UnitManager.Instance.PlayerMechs.Clear();
        UnitManager.Instance.PlayerPilots.Clear();

        // Create a default mech and pilot for the new game
        Mech defaultMech = new Mech("Default Mech", MechType.Light, Faction.Player);
        UnitManager.Instance.AddMech(defaultMech);
        Pilot defaultPilot = new Pilot("Default Pilot", Faction.Player);
        UnitManager.Instance.AddPilot(defaultPilot);
        defaultMech.AddPilot(defaultPilot); // Assign the default pilot to the default mech
        defaultPilot.AssignedMech = defaultMech; // Assign the default mech to the default pilot

        // Create a default weapon and add it to the player's inventory
        Weapon defaultWeapon = new Weapon("Default Weapon", Weight.Light, 100, 30, 10, 30, 1f, 0.25f, 0.5f, 5, 1, 1);
        _playerInventory.Add(defaultWeapon);
        defaultMech.EquipWeapon(defaultWeapon); // Equip the default weapon to the default mech
        defaultWeapon.SetOwner(defaultMech); // Set the owner of the default weapon to the default mech
        
    }

    void TimePasses(int timeUnits)
    {
        Debug.Log($"Time passes: {timeUnits} time units.");
        UnitManager.Instance.TimePasses(timeUnits);
    }

    public void AddItemToInventory(Item item)
    {
        if (!_playerInventory.Contains(item))
        {
            _playerInventory.Add(item);
            Debug.Log($"Item {item.Name} added to player's inventory. Total items now: {_playerInventory.Count}");
        }
        else
        {
            Debug.LogWarning($"Item {item.Name} is already in the player's inventory.");
        }
    }

    public void RemoveItemFromInventory(Item item)
    {
        if (_playerInventory.Contains(item))
        {
            _playerInventory.Remove(item);
        }
        else
        {
            Debug.LogWarning($"Item {item.Name} is not in the player's inventory.");
        }
    }

    public void ClearInventory()
    {
        _playerInventory.Clear();
    }

    public bool HasItem(Item item)
    {
        return _playerInventory.Contains(item);
    }

    public Weapon GetWeaponById(Guid id)
    {
        foreach (Item item in _playerInventory)
        {
            if (item is Weapon weapon && weapon.Id == id)
            {
                return weapon;
            }
        }
        return null; // Return null if the weapon is not found
    }

    public List<Weapon> GetAllWeapons()
    {
        List<Weapon> weapons = new List<Weapon>();
        foreach (Item item in _playerInventory)
        {
            if (item is Weapon weapon)
            {
                weapons.Add(weapon);
            }
        }
        return weapons;
    }

    public Armor GetArmorById(Guid id)
    {
        foreach (Item item in _playerInventory)
        {
            if (item is Armor armor && armor.Id == id)
            {
                return armor;
            }
        }
        return null; // Return null if the armor is not found
    }

    public List<Armor> GetAllArmors()
    {
        List<Armor> armors = new List<Armor>();
        foreach (Item item in _playerInventory)
        {
            if (item is Armor armor)
            {
                armors.Add(armor);
            }
        }
        return armors;
    }

    public EnergyShield GetEnergyShieldById(Guid id)
    {
        foreach (Item item in _playerInventory)
        {
            if (item is EnergyShield shield && shield.Id == id)
            {
                return shield;
            }
        }
        return null; // Return null if the energy shield is not found
    }

    public List<EnergyShield> GetAllEnergyShields()
    {
        List<EnergyShield> shields = new List<EnergyShield>();
        foreach (Item item in _playerInventory)
        {
            if (item is EnergyShield shield)
            {
                shields.Add(shield);
            }
        }
        return shields;
    }

    public void AddAvailableResearch(ItemBlueprint blueprint)
    {
        if (!_availableResearch.Contains(blueprint))
        {
            _availableResearch.Add(blueprint);
        }
        else
        {
            Debug.LogWarning($"Blueprint {blueprint.ItemName} is already in the available research list.");
        }
    }

    public void RemoveAvailableResearch(ItemBlueprint blueprint)
    {
        if (_availableResearch.Contains(blueprint))
        {
            _availableResearch.Remove(blueprint);
        }
        else
        {
            Debug.LogWarning($"Blueprint {blueprint.ItemName} is not in the available research list.");
        }
    }

    ItemBlueprint GetBlueprintByName(string itemName)
    {
        foreach (ItemBlueprint blueprint in _availableResearch)
        {
            if (blueprint.ItemName == itemName)
            {
                Debug.Log($"Found blueprint: {blueprint.ItemName}");
                return blueprint;
            }
        }
        Debug.LogWarning($"Blueprint with name {itemName} not found.");
        return null;
    }

    public void ClearAvailableResearch()
    {
        _availableResearch.Clear();
    }

    public void ResearchItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot research a null item.");
            return;
        }
        ItemBlueprint blueprint = GetBlueprintByName(item.Name);
        if (blueprint != null)
        {
            blueprint.SetModified(true);
            if (blueprint.IncreaseStats(item))
            {
                RemoveItemFromInventory(item);
            }
            else
            {
                Debug.LogWarning($"Failed to increase stats for item: {item.Name}");
            }
        }
    }

    public void AddBolts(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot add a negative amount of bolts.");
            return;
        }
        _playerBolts += amount;
        Debug.Log($"Added {amount} bolts. Total bolts now: {_playerBolts}");
    }

    public void SpendBolts(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot spend a negative amount of bolts.");
            return;
        }
        if (amount > _playerBolts)
        {
            Debug.LogWarning("Not enough bolts to spend.");
            return;
        }
        _playerBolts -= amount;
        Debug.Log($"Spent {amount} bolts. Total bolts now: {_playerBolts}");
    }
}
