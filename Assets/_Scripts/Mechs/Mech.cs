using UnityEngine;
using System;
using HeartOfIron.Enums;
using HeartOfIron.Structs;

[Serializable]
public class Mech
{

    private readonly int LIGHT_HEALTH_INCREASE = 1;
    private readonly int MEDIUM_HEALTH_INCREASE = 2;
    private readonly int HEAVY_HEALTH_INCREASE = 3;
    private readonly int LIGHT_SPEED_INCREASE = 3;
    private readonly int MEDIUM_SPEED_INCREASE = 2;
    private readonly int HEAVY_SPEED_INCREASE = 1;
    private readonly int LIGHT_VISION_RANGE_INCREASE = 3;
    private readonly int MEDIUM_VISION_RANGE_INCREASE = 2;
    private readonly int HEAVY_VISION_RANGE_INCREASE = 1;
    private readonly int LIGHT_VISION_ANGLE_INCREASE = 3;
    private readonly int MEDIUM_VISION_ANGLE_INCREASE = 2;
    private readonly int HEAVY_VISION_ANGLE_INCREASE = 1;

    [SerializeField] private Guid _id;
    public Guid Id => _id;
    [SerializeField] private string _name;
    public string Name => _name;
    [SerializeField] private MechType _type;
    public MechType Type => _type;
    [SerializeField] private int _level;
    public int Level => _level;
    [SerializeField] private int _experience;
    public int Experience => _experience;
    [SerializeField] private float[] _health;
    public float[] Health => _health;
    [SerializeField] private float[] _maxHealth;
    public float[] MaxHealth => _maxHealth;
    [SerializeField] private float _maxSpeed;
    public float MaxSpeed => _maxSpeed;
    [SerializeField] private float _visionRange;
    public float VisionRange => _visionRange;
    [SerializeField] private float _visionAngle;
    public float VisionAngle => _visionAngle;
    [SerializeField] private Inventory _inventory;
    public Inventory Inventory => _inventory;
    [SerializeField] private Faction _faction;
    public Faction Faction => _faction;
    [SerializeField] private Color _color;
    public Color Color => _color;
    [SerializeField] private Pilot _pilot;
    public Pilot Pilot => _pilot;

    // FOR TESTING PURPOSES ONLY
    public Mech(string name, MechType type, Faction faction)
    {
        this._id = Guid.NewGuid();
        this._name = name;
        this._type = type;
        this._level = 1;
        this._experience = 0;
        this._health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        this._maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        this._maxSpeed = 10f;
        this._inventory = new Inventory();
        this._faction = faction;
        this._color = Color.gray;
        this._pilot = null;
        this._visionRange = 10f; // Default vision range
        this._visionAngle = 90f; // Default vision angle
    }

    public Mech(string name, MechType type, int level, int experience, float[] health, float[] maxHealth, float maxSpeed, float visionRange, float visionAngle, Inventory inventory, Faction faction, Color color, Pilot pilot)
    {
        this._id = Guid.NewGuid();
        this._name = name;
        this._type = type;
        this._level = level;
        this._experience = experience;
        this._health = health;
        this._maxHealth = maxHealth;
        this._maxSpeed = maxSpeed;
        this._visionRange = visionRange;
        this._visionAngle = visionAngle;
        this._inventory = inventory;
        this._faction = faction;
        this._color = color;
        this._pilot = pilot;
    }

    public Mech(Guid id, string name, MechType type, int level, int experience, float[] health, float[] maxHealth, float maxSpeed, float visionRange, float visionAngle, Inventory inventory, Faction faction, Color color, Pilot pilot)
    {
        this._id = id;
        this._name = name;
        this._type = type;
        this._level = level;
        this._experience = experience;
        this._health = health;
        this._maxHealth = maxHealth;
        this._maxSpeed = maxSpeed;
        this._visionRange = visionRange;
        this._visionAngle = visionAngle;
        this._inventory = inventory;
        this._faction = faction;
        this._color = color;
        this._pilot = pilot;
    }

    public DamageReport TakeDamage(float damageAmount, int location, bool radiateDamage = false)
    {
        if (location < 0 || location >= _health.Length)
        {
            Debug.LogError($"Invalid location index {location} for mech {_name}. Damage not applied.");
            return new DamageReport(0f, 0f);
        }
        float remainingDamage = damageAmount;
        float flag = 0f;
        if (_inventory.EnergyShield != null && remainingDamage > 0)
        {
            remainingDamage = _inventory.EnergyShield.TakeDamage(remainingDamage, location);
            flag = 2f;
        }
        if (_inventory.Armors[location] != null && remainingDamage > 0)
        {
            remainingDamage = _inventory.Armors[location].TakeDamage(remainingDamage, location);
            flag = 1f;
        }
        float overflowDamage = 0f;
        if (remainingDamage > 0)
        {
            _health[location] -= remainingDamage;
            flag = 0f;
            overflowDamage = -_health[location];
            if (_health[location] < 0)
            {
                int nextLocation = (location + 1) % _health.Length;
                while (overflowDamage > 0 && !radiateDamage && nextLocation != location)
                {
                    float damageToNextLocation = UnityEngine.Random.Range(0, overflowDamage);
                    overflowDamage -= damageToNextLocation;
                    overflowDamage += TakeDamage(damageToNextLocation, nextLocation, true).remainingDamage;
                    nextLocation = (nextLocation + 1) % _health.Length;
                }
                _health[location] = 0;
            }
        }
        Debug.Log($"Mech {_name} took {damageAmount} damage to location {location}. Remaining health: {_health[location]}");
        if (radiateDamage)
        {
            return new DamageReport(overflowDamage, flag);
        } else
        {
            return new DamageReport(overflowDamage, flag);
        }
        
    }

    public void RepairAll(float repairAmount)
    {
        for (int i = 0; i < _health.Length; i++)
        {
            Repair(repairAmount, i);
        }
    }

    public void Repair(float repairAmount, int location = 0)
    {
        if (location < 0 || location >= _health.Length)
        {
            Debug.LogError($"Invalid location index {location} for mech {_name}. Repair not applied.");
            return;
        }
        if (_health[location] == 0)
        {
            Debug.LogWarning($"Mech {_name} is destroyed at location {location}. Cannot repair.");
            return;
        }
        _health[location] += repairAmount;
        if (_health[location] > _maxHealth[location])
        {
            _health[location] = _maxHealth[location];
        }
        Armor[] armors = _inventory.Armors;
        if (armors != null )
        {
            switch (location)
            {
                case 0:
                    if (armors[0] != null)
                    {
                        armors[0].Repair(repairAmount);
                    }
                    break;
                case 1:
                    if (armors[1] != null)
                    {
                        armors[1].Repair(repairAmount);
                    }
                    break;
                case 2:
                case 3:
                    if (armors[2] != null)
                    {
                        armors[2].Repair(repairAmount, location % 2);
                    }
                    break;
                case 4:
                case 5:
                    if (armors[3] != null)
                    {
                        armors[3].Repair(repairAmount, location % 2);
                    }
                    break;
                default:
                    break;
            }
        }
        EnergyShield energyShield = _inventory.EnergyShield;
        if (energyShield != null)
        {
            energyShield.Repair(repairAmount);
        }
        Debug.Log($"Mech {_name} repaired {repairAmount} health at location {location}. Current health: {_health[location]}");
    }

    public bool IsLowHealth()
    {
        float effectiveHealth = 0f;
        float effectiveMaxHealth = 0f;
        // Debug.Log($"Mech {_name} effective health calculation started. Initial effective health: {effectiveHealth}, effective max health: {effectiveMaxHealth}");
        Armor[] armors = _inventory.Armors;
        for (int i = 0; i < _health.Length; i++)
        {
            float armorHealth = 0f;
            float armorMaxHealth = 0f;
            if (armors != null)
            {
                switch (i)
                {
                    case 0:
                        if (armors[0] != null)
                        {
                            armorHealth = armors[0].GetDurability();
                            armorMaxHealth = armors[0].MaxDurability;
                        }
                        break;
                    case 1:
                        if (armors[1] != null)
                        {
                            armorHealth = armors[1].GetDurability();
                            armorMaxHealth = armors[1].MaxDurability;
                        }
                        break;
                    case 2:
                    case 3:
                        if (armors[2] != null)
                        {
                            armorHealth = armors[2].GetDurability(i % 2);
                            armorMaxHealth = armors[2].MaxDurability;
                        }
                        break;
                    case 4:
                    case 5:
                        if (armors[3] != null)
                        {
                            armorHealth = armors[3].GetDurability(i % 2);
                            armorMaxHealth = armors[3].MaxDurability;
                        }
                        break;
                    default:
                        break;
                }
            }
            effectiveHealth += armorHealth;
            effectiveMaxHealth += armorMaxHealth;
        }
        // Debug.Log($"Mech {_name} effective health armor calculation completed. Effective health: {effectiveHealth}, effective max health: {effectiveMaxHealth}");
        EnergyShield energyShield = _inventory.EnergyShield;
        if (energyShield != null)
        {
            effectiveHealth += energyShield.CurrentCapacity;
            effectiveMaxHealth += energyShield.MaxCapacity;
        }
        // Debug.Log($"Mech {_name} effective health shield calculation completed. Effective health: {effectiveHealth}, effective max health: {effectiveMaxHealth}");
        for (int i = 0; i < _health.Length; i++)
        {
            // Debug.Log($"Mech {_name} effective health before location {i}: effectiveHealth: {effectiveHealth}, effective max health: {effectiveMaxHealth}");
            // Debug.Log($"Mech {_name} health at location {i}: {_health[i]}/{_maxHealth[i]}");
            effectiveHealth += _health[i];
            effectiveMaxHealth += _maxHealth[i];
            // Debug.Log($"Mech {_name} effective health after location {i}: {effectiveHealth}, effective max health: {effectiveMaxHealth}");
        }
        // Debug.Log($"Mech {_name} effective health: {effectiveHealth}, effective max health: {effectiveMaxHealth}, low health threshold: {effectiveMaxHealth * 0.25f}");
        return (effectiveHealth / effectiveMaxHealth) < 0.25f; // Returns true if effective health is below 25% of effective max health
    }

    public void GainExperience(int experienceAmount)
    {
        _experience += experienceAmount;
        Debug.Log($"Mech {_name} gained {experienceAmount} experience. Total experience: {_experience}");
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int requiredExperience = _level * 100; // Example leveling formula
        while (_experience >= requiredExperience)
        {
            _experience -= requiredExperience;
            LevelUp();
            requiredExperience = _level * 100; // Update required experience for next level
        }
    }

    private void LevelUp()
    {
        _level++;
        Debug.Log($"Mech {_name} leveled up! New level: {_level}");
        switch (_type)
        {
            case MechType.Light:
                for(int i = 0; i < _maxHealth.Length; i++)
                {
                    _maxHealth[i] += LIGHT_HEALTH_INCREASE;
                }
                _maxSpeed += LIGHT_SPEED_INCREASE;
                _visionRange += LIGHT_VISION_RANGE_INCREASE;
                _visionAngle += LIGHT_VISION_ANGLE_INCREASE;
                break;
            case MechType.Medium:
                for(int i = 0; i < _maxHealth.Length; i++)
                {
                    _maxHealth[i] += MEDIUM_HEALTH_INCREASE;
                }
                _maxSpeed += MEDIUM_SPEED_INCREASE;
                _visionRange += MEDIUM_VISION_RANGE_INCREASE;
                _visionAngle += MEDIUM_VISION_ANGLE_INCREASE;
                break;
            case MechType.Heavy:
                for(int i = 0; i < _maxHealth.Length; i++)
                {
                    _maxHealth[i] += HEAVY_HEALTH_INCREASE;
                }
                _maxSpeed += HEAVY_SPEED_INCREASE;
                _visionRange += HEAVY_VISION_RANGE_INCREASE;
                _visionAngle += HEAVY_VISION_ANGLE_INCREASE;
                break;
            default:
                break;
        }
    }

    public void AddPilot(Pilot pilot)
    {
        if (pilot == null)
        {
            Debug.LogError("Attempted to add a null pilot to the mech.");
            return;
        }
        if (_pilot != null)
        {
            Debug.LogWarning($"Mech {_name} already has a pilot assigned.");
            return;
        } else if (pilot.Faction != _faction)
        {
            Debug.LogWarning($"Pilot {pilot.Name} belongs to a different faction ({pilot.Faction}) than the mech ({_faction}).");
            return;
        }
        _pilot = pilot;
        // Debug.Log($"Pilot {pilot.Name} has been assigned to Mech {_name}.");
    }

    public void RemovePilot()
    {
        if (_pilot == null)
        {
            Debug.LogWarning($"Mech {_name} does not have a pilot to remove.");
            return;
        }
        // Debug.Log($"Pilot {_pilot.Name} has been removed from Mech {_name}.");
        _pilot = null;
    }

    public bool EquipWeapon(Weapon weapon)
    {
        if (_inventory == null)
        {
            Debug.LogError($"Mech {_name} does not have an inventory to equip weapons.");
            return false;
        }
        return _inventory.EquipWeapon(weapon, this);
    }

    public void UnequipWeapon(Weapon weapon)
    {
        if (_inventory == null)
        {
            Debug.LogError($"Mech {_name} does not have an inventory to unequip weapons.");
            return;
        }
        _inventory.UnequipWeapon(weapon);
    }

    public bool EquipArmor(Armor armor)
    {
        if (_inventory == null)
        {
            Debug.LogError($"Mech {_name} does not have an inventory to equip armor.");
            return false;
        }
        return _inventory.EquipArmor(armor, this);
    }

    public void UnequipArmor(Armor armor)
    {
        if (_inventory == null)
        {
            Debug.LogError($"Mech {_name} does not have an inventory to unequip armor.");
            return;
        }
        _inventory.UnequipArmor(armor);
    }

    public bool EquipEnergyShield(EnergyShield energyShield)
    {
        if (_inventory == null)
        {
            Debug.LogError($"Mech {_name} does not have an inventory to equip an energy shield.");
            return false;
        }
        return _inventory.EquipEnergyShield(energyShield, this);
    }

    public void UnequipEnergyShield(EnergyShield energyShield)
    {
        if (_inventory == null)
        {
            Debug.LogError($"Mech {_name} does not have an inventory to unequip an energy shield.");
            return;
        }
        _inventory.UnequipEnergyShield(energyShield);
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            Debug.LogWarning("Attempted to set an empty or whitespace name for the mech.");
            return;
        }
        _name = newName;
        Debug.Log($"Mech name updated to {_name}.");
    }

    public void SetMechColor(Color newColor)
    {
        _color = newColor;
        // Debug.Log($"Mech {_name}'s color updated to: {_color}");
    }

    public string GetInfo()
    {
        string info = $"Mech Name: {_name}\n" +
                      $"Type: {_type}\n" +
                      $"Level: {_level}\n" +
                      $"Experience: {_experience}\n" +
                      $"Max Speed: {_maxSpeed}\n" +
                      $"Faction: {_faction}\n" +
                      $"Pilot: {(_pilot != null ? _pilot.Name : "None")}\n" +
                      $"Health: Head({_health[(int)BodyPart.Head]}/{_maxHealth[(int)BodyPart.Head]}), Torso({_health[(int)BodyPart.Torso]}/{_maxHealth[(int)BodyPart.Torso]}), " +
                      $"Right Arm({_health[(int)BodyPart.RightArm]}/{_maxHealth[(int)BodyPart.RightArm]}), Left Arm({_health[(int)BodyPart.LeftArm]}/{_maxHealth[(int)BodyPart.LeftArm]}), " +
                      $"Right Leg({_health[(int)BodyPart.RightLeg]}/{_maxHealth[(int)BodyPart.RightLeg]}), Left Leg({_health[(int)BodyPart.LeftLeg]}/{_maxHealth[(int)BodyPart.LeftLeg]})\n" +
                      $"Inventory: {(_inventory != null ? _inventory.GetInfo() : "None")}";
        return info;
    }

    public bool IsDead()
    {
        return _health[(int)BodyPart.Head] <= 0 || _health[(int)BodyPart.Torso] <= 0; // Mech is considered dead if head or torso are destroyed
    }
}
