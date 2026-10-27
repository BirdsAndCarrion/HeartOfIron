using UnityEngine;
using System;
using System.Collections.Generic;
using HeartOfIron.Enums;

/// <summary>
/// Represents the inventory of a mech, containing weapons, armors, an energy shield, and their associated attacks.
/// Provides methods for setting timers and updating cooldowns for the attacks.
/// Provides methods for managing the inventory, including adding and removing items.
/// Also designates attacks based on the equipped weapons.
/// </summary>
/// <remarks>
/// The inventory manages most of the relationships between the mech and its equipped item, including attack designation and cooldown management.
/// Not all of the inventory's slots are meant to be used on every mech (i.e. some mechs may not have an energy shield or may not have armor).
/// </remarks>
[Serializable]
public class Inventory
{
    [SerializeReference] private Weapon[] _weapons;
    public Weapon[] Weapons => _weapons;
    [SerializeReference] private Armor[] _armors;
    public Armor[] Armors => _armors;
    [SerializeReference] private EnergyShield _energyShield;
    public EnergyShield EnergyShield => _energyShield;

    private List<RangedAttack> _rangedAttacks;
    public List<RangedAttack> RangedAttacks => _rangedAttacks;
    private List<MeleeAttack> _meleeAttacks;
    public List<MeleeAttack> MeleeAttacks => _meleeAttacks;

    private Dictionary<Weapon, RangedAttack> _weaponToRangedAttackMap;
    public Dictionary<Weapon, RangedAttack> WeaponToRangedAttackMap => _weaponToRangedAttackMap;
    private Dictionary<Weapon, MeleeAttack> _weaponToMeleeAttackMap;
    public Dictionary<Weapon, MeleeAttack> WeaponToMeleeAttackMap => _weaponToMeleeAttackMap;

    public Inventory()
    {
        _weapons = new Weapon[2];
        _armors = new Armor[4]; // Initialize armors array with 4 slots
        _energyShield = null; // Initialize energy shield to null
        _rangedAttacks = new List<RangedAttack>();
        _meleeAttacks = new List<MeleeAttack>();
        _weaponToRangedAttackMap = new Dictionary<Weapon, RangedAttack>();
        _weaponToMeleeAttackMap = new Dictionary<Weapon, MeleeAttack>();
    }

    /// <summary>
    /// Sets the timer for the specified weapon's attacks.
    /// </summary>
    /// <param name="weapon">The weapon whose attacks' timers are to be set.</param>
    /// <param name="timerValue">The value to set the timer to.</param>
    public void SetTimer(Weapon weapon,float timerValue)
    {
        _weaponToRangedAttackMap[weapon]?.SetTimer(timerValue);
        _weaponToMeleeAttackMap[weapon]?.SetTimer(timerValue);
    }

    /// <summary>
    /// Updates the cooldown timers for all ranged and melee attacks in the inventory.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateCooldowns(float deltaTime)
    {
        foreach (RangedAttack rA in _rangedAttacks)
        {
            if (rA != null)
            {
                rA.UpdateTimer(deltaTime);
            }
        }
        foreach (MeleeAttack mA in _meleeAttacks)
        {
            if (mA != null)
            {
                mA.UpdateTimer(deltaTime);
            }
        }
    }

    /// <summary>
    /// Designates the attacks for the currently equipped weapons, updating the lists and maps accordingly.
    /// This method should be called whenever the equipped weapons change to ensure the attack lists and maps are accurate.
    /// </summary>
    private void DesignateAttacks()
    {
        if (_weapons[(int)Hand.Right] == null && _weapons[(int)Hand.Left] == null)
        {
            _rangedAttacks.Clear();
            _meleeAttacks.Clear();
        } else if (_weapons[(int)Hand.Right] != null && _weapons[(int)Hand.Left] == null)
        {
            RangedAttack rA = _weapons[(int)Hand.Right].GetRangedAttack();
            if (rA != null)
            {
                _rangedAttacks.Add(rA);
                _weaponToRangedAttackMap[_weapons[(int)Hand.Right]] = rA;
            }
            MeleeAttack mA = _weapons[(int)Hand.Right].GetMeleeAttack();
            if (mA != null)
            {
                _meleeAttacks.Add(mA);
                _weaponToMeleeAttackMap[_weapons[(int)Hand.Right]] = mA;
            }
        } else if (_weapons[(int)Hand.Right] == null && _weapons[(int)Hand.Left] != null)
        {
            RangedAttack rA = _weapons[(int)Hand.Left].GetRangedAttack();
            if (rA != null)
            {
                _rangedAttacks.Add(rA);
                _weaponToRangedAttackMap[_weapons[(int)Hand.Left]] = rA;
            }
            MeleeAttack mA = _weapons[(int)Hand.Left].GetMeleeAttack();
            if (mA != null)
            {
                _meleeAttacks.Add(mA);
                _weaponToMeleeAttackMap[_weapons[(int)Hand.Left]] = mA;
            }
        }
        else if (_weapons[(int)Hand.Right] != null && _weapons[(int)Hand.Left] != null)
        {
            if (_weapons[(int)Hand.Right].IsMelee)
            {
                MeleeAttack mA = _weapons[(int)Hand.Right].GetMeleeAttack();
                if (mA != null)
                {
                    _meleeAttacks.Add(mA);
                    _weaponToMeleeAttackMap[_weapons[(int)Hand.Right]] = mA;
                }
                RangedAttack rA = _weapons[(int)Hand.Left].GetRangedAttack();
                if (rA != null)
                {
                    _rangedAttacks.Add(rA);
                    _weaponToRangedAttackMap[_weapons[(int)Hand.Left]] = rA;
                }
            }
            else
            {
                RangedAttack rA = _weapons[(int)Hand.Right].GetRangedAttack();
                if (rA != null)
                {
                    _rangedAttacks.Add(rA);
                    _weaponToRangedAttackMap[_weapons[(int)Hand.Right]] = rA;
                }
                MeleeAttack mA = _weapons[(int)Hand.Left].GetMeleeAttack();
                if (mA != null)
                {
                    _meleeAttacks.Add(mA);
                    _weaponToMeleeAttackMap[_weapons[(int)Hand.Left]] = mA;
                }
            }
        }
    }

    public Inventory(Mech mech, Weapon weapon1, Weapon weapon2, Armor[] armors, EnergyShield energyShield)
    {
        _weapons = new Weapon[2];
        EquipWeapon(weapon1, mech);
        EquipWeapon(weapon2, mech);
        _armors = new Armor[4]; // Initialize armors array with 4 slots
        foreach (Armor armor in armors)
        {
            EquipArmor(armor, mech);
        }
        EquipEnergyShield(energyShield, mech);
        _rangedAttacks = new List<RangedAttack>();
        _meleeAttacks = new List<MeleeAttack>();
        _weaponToRangedAttackMap = new Dictionary<Weapon, RangedAttack>();
        _weaponToMeleeAttackMap = new Dictionary<Weapon, MeleeAttack>();
        DesignateAttacks();
    }

    public Inventory(Mech mech, Weapon weapon1, Weapon weapon2, Armor armor1, Armor armor2, Armor armor3, Armor armor4, EnergyShield energyShield)
    {
        _weapons = new Weapon[2];
        EquipWeapon(weapon1, mech);
        EquipWeapon(weapon2, mech);
        _armors = new Armor[4];
        EquipArmor(armor1, mech);
        EquipArmor(armor2, mech);
        EquipArmor(armor3, mech);
        EquipArmor(armor4, mech);
        EquipEnergyShield(energyShield, mech);
        _rangedAttacks = new List<RangedAttack>();
        _meleeAttacks = new List<MeleeAttack>();
        _weaponToRangedAttackMap = new Dictionary<Weapon, RangedAttack>();
        _weaponToMeleeAttackMap = new Dictionary<Weapon, MeleeAttack>();
        DesignateAttacks();
    }

    /// <summary>
    /// Checks if the inventory contains the specified item.
    /// </summary>
    /// <param name="item">The item to check for in the inventory.</param>
    /// <returns>True if the item is present in the inventory, false otherwise.</returns>
    public bool HasItem(Item item)
    {
        if (item is Weapon weapon)
        {
            foreach (Weapon w in _weapons)
            {
                if (w != null && w.Id == weapon.Id)
                {
                    return true;
                }
            }
        }
        else if (item is Armor armor)
        {
            foreach (Armor a in _armors)
            {
                if (a != null && a.Id == armor.Id)
                {
                    return true;
                }
            }
        }
        else if (item is EnergyShield energyShield)
        {
            if (_energyShield != null && _energyShield.Id == energyShield.Id)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Attempts to equip the specified weapon to the inventory for the given owner mech.
    /// </summary>
    /// <param name="weapon">The weapon to equip.</param>
    /// <param name="owner">The mech that owns the inventory.</param>
    /// <returns>True if the weapon was successfully equipped, false otherwise.</returns>
    public bool EquipWeapon(Weapon weapon, Mech owner)
    {
        if (!CanEquipItem(weapon))
        {
            return false;
        }
        if (_weapons[(int)Hand.Right] == null && _weapons[(int)Hand.Left] == null && weapon.SlotsRequired <= 2)
        {
            for (int i = 0; i < weapon.SlotsRequired; i++)
            {
                // Debug.Log($"{weapon.Name} requires {weapon.SlotsRequired} slots. Equipping in slot {i}");
                if (i < _weapons.Length)
                {
                    Debug.Log($"Equipping weapon {weapon.Name} in slot {i}");
                    _weapons[i] = weapon;
                    if (i == weapon.SlotsRequired - 1)
                    {
                        weapon.SetOwner(owner); // Set the owner of the weapon when equipped
                        DesignateAttacks();
                        return true; // Successfully equipped all required slots
                    }
                }
            }
        } else if ((_weapons[(int)Hand.Right] == null || _weapons[(int)Hand.Left] == null) && weapon.SlotsRequired <= 1)
        {
            if (_weapons[(int)Hand.Right] == null)
            {
                _weapons[(int)Hand.Right] = weapon;
                weapon.SetOwner(owner); // Set the owner of the weapon when equipped
                DesignateAttacks();
                return true;
            }
            else if (_weapons[(int)Hand.Left] == null)
            {
                _weapons[(int)Hand.Left] = weapon;
                weapon.SetOwner(owner); // Set the owner of the weapon when equipped
                DesignateAttacks();
                return true;
            }
        } else
        {
            Debug.LogWarning("Cannot equip weapon: Not enough available slots.");
            return false;
        }
        return false;
    }

    /// <summary>
    /// Unequips the specified weapon from the inventory.
    /// </summary>
    /// <param name="weapon">The weapon to unequip.</param>
    public void UnequipWeapon(Weapon weapon)
    {
        if (_weapons[(int)Hand.Right] == weapon)
        {
            _weapons[(int)Hand.Right] = null;
            weapon.ClearOwner(); // Clear the owner of the weapon when unequipped
        }
        if (_weapons[(int)Hand.Left] == weapon)
        {
            _weapons[(int)Hand.Left] = null;
            weapon.ClearOwner(); // Clear the owner of the weapon when unequipped
        }
        DesignateAttacks(); // Update attacks after unequipping a weapon
    }

    /// <summary>
    /// Attempts to equip the specified armor to the inventory for the given owner mech.
    /// </summary>
    /// <param name="armor">The armor to equip.</param>
    /// <param name="owner">The mech that owns the inventory.</param>
    /// <returns>True if the armor was successfully equipped, false otherwise.</returns>
    public bool EquipArmor(Armor armor, Mech owner)
    {
        if (!CanEquipItem(armor) && !CanEquipArmor(armor))
        {
            return false;
        }
        for (int i = 0; i < _armors.Length; i++)
        {
            if (_armors[i] == null)
            {
                _armors[i] = armor;
                armor.SetOwner(owner); // Set the owner of the armor when equipped
                return true;
            }
        }
        Debug.LogWarning("Cannot equip armor: No available slots.");
        return false;
    }

    /// <summary>
    /// Checks if the specified armor can be equipped in the inventory.
    /// </summary>
    /// <param name="armor">The armor to check.</param>
    /// <returns>True if the armor can be equipped, false otherwise.</returns>
    private bool CanEquipArmor(Armor armor)
    {
        if (armor == null)
        {
            Debug.LogWarning("Cannot check equip status for a null armor.");
            return false;
        }
        if (_armors.Length == 0)
        {
            Debug.LogWarning("Cannot equip armor: No armor slots available.");
            return false;
        }
        if (_energyShield != null && armor.Weight != _energyShield.Weight)
        {
            Debug.LogWarning("Cannot equip armor: Incompatible with the equipped energy shield.");
            return false;
        }
        for (int i = 0; i < _armors.Length; i++)
        {
            if (_armors[i] == null)
            {
                return true; // There is at least one available slot
            }
        }
        Debug.LogWarning("Cannot equip armor: All armor slots are occupied.");
        return false;
    }

    /// <summary>
    /// Unequips the specified armor from the inventory.
    /// </summary>
    /// <param name="armor">The armor to unequip.</param>
    public void UnequipArmor(Armor armor)
    {
        for (int i = 0; i < _armors.Length; i++)
        {
            if (_armors[i] == armor)
            {
                _armors[i].ClearOwner(); // Clear the owner of the armor when unequipped
                _armors[i] = null;
                return;
            }
        }
        Debug.LogWarning("Cannot unequip armor: The specified armor is not currently equipped.");
    } 

    /// <summary>
    /// Attempts to equip the specified energy shield to the inventory for the given owner mech.
    /// </summary>
    /// <param name="energyShield">The energy shield to equip.</param>
    /// <param name="owner">The mech that owns the inventory.</param>
    /// <returns>True if the energy shield was successfully equipped, false otherwise.</returns>
    public bool EquipEnergyShield(EnergyShield energyShield, Mech owner)
    {
        if (!CanEquipItem(energyShield) && !CanEquipEnergyShield(energyShield))
        {
            return false;
        }
        if (_energyShield == null)
        {
            _energyShield = energyShield;
            energyShield.SetOwner(owner); // Set the owner of the energy shield when equipped
            return true;
        }
        Debug.LogWarning("Cannot equip energy shield: Slot is already occupied.");
        return false;
    }

    /// <summary>
    /// Checks if the specified energy shield can be equipped in the inventory.
    /// </summary>
    /// <param name="energyShield">The energy shield to check.</param>
    /// <returns>True if the energy shield can be equipped, false otherwise.</returns>
    private bool CanEquipEnergyShield(EnergyShield energyShield)
    {
        if (energyShield == null)
        {
            Debug.LogWarning("Cannot check equip status for a null energy shield.");
            return false;
        }
        if (_energyShield != null)
        {
            Debug.LogWarning("Cannot equip energy shield: Slot is already occupied.");
            return false;
        }
        for (int i = 0; i < _armors.Length; i++)
        {
            if (_armors[i] != null && _armors[i].Weight != energyShield.Weight)
            {
                Debug.LogWarning("Cannot equip energy shield: Incompatible with the equipped armor.");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Unequips the specified energy shield from the inventory.
    /// </summary>
    /// <param name="energyShield">The energy shield to unequip.</param>
    public void UnequipEnergyShield(EnergyShield energyShield)
    {
        if (_energyShield == energyShield)
        {
            _energyShield.ClearOwner(); // Clear the owner of the energy shield when unequipped
            _energyShield = null;
            return;
        }
        Debug.LogWarning("Cannot unequip energy shield: The specified energy shield is not currently equipped.");
    }

    /// <summary>
    /// Checks if the specified item can be equipped in the inventory.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item can be equipped, false otherwise.</returns>
    public bool CanEquipItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot check equip status for a null item.");
            return false;
        } else if (item.Owner != null)
        {
            Debug.LogWarning($"Cannot equip item: {item.Name} is already owned by {item.Owner.Name}.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Gets the ideal attack based on the specified distance.
    /// </summary>
    /// <param name="distance">The distance to the target.</param>
    /// <returns>The ideal attack for the given distance.</returns>
    public Attack GetIdealAttack(float distance)
    {
        MeleeAttack idealMeleeAttack = GetIdealMeleeAttack(distance);
        RangedAttack idealRangedAttack = GetIdealRangedAttack(distance);

        if (idealMeleeAttack == null) return idealRangedAttack;
        if (idealRangedAttack == null) return idealMeleeAttack;

        return idealMeleeAttack.GetAverageDamage(distance) > idealRangedAttack.GetAverageDamage(distance) ? (Attack)idealMeleeAttack : (Attack)idealRangedAttack;
    }

    /// <summary>
    /// Gets the ideal ranged attack based on the specified distance.
    /// </summary>
    /// <param name="distance">The distance to the target.</param>
    /// <returns>The ideal ranged attack for the given distance.</returns>
    public RangedAttack GetIdealRangedAttack(float distance)
    {
        RangedAttack idealRangedAttack = _rangedAttacks.Count > 0 ? _rangedAttacks[0] : null;

        for (int i = 1; i < _rangedAttacks.Count; i++)
        {
            if (_rangedAttacks[i].GetAverageDamage(distance) > idealRangedAttack.GetAverageDamage(distance))
            {
                idealRangedAttack = _rangedAttacks[i];
            }
        }
        
        return idealRangedAttack;
    }

    /// <summary>
    /// Gets the ideal melee attack based on the specified distance.
    /// </summary>
    /// <param name="distance">The distance to the target.</param>
    /// <returns>The ideal melee attack for the given distance.</returns>
    public MeleeAttack GetIdealMeleeAttack(float distance)
    {
        MeleeAttack idealMeleeAttack = null;

        for (int i = 0; i < _meleeAttacks.Count; i++)
        {
            if (idealMeleeAttack == null || _meleeAttacks[i].GetAverageDamage(distance) > idealMeleeAttack.GetAverageDamage(distance))
            {
                idealMeleeAttack = _meleeAttacks[i];
            }
        }
        
        return idealMeleeAttack;
    }

    /// <summary>
    /// Gets a string representation of the inventory, including weapons, armors, and energy shields.
    /// </summary>
    /// <returns>A string containing the inventory information.</returns>   
    public string GetInfo()
    {
        string info = "Inventory:\n";
        info += "Weapons:\n";
        foreach (Weapon weapon in _weapons)
        {
            if (weapon != null)
            {
                info += $"- {weapon.Name}\n";
            }
            else
            {
                info += "- Empty Slot\n";
            }
        }
        info += "Armors:\n";
        foreach (Armor armor in _armors)
        {
            if (armor != null)
            {
                info += $"- {armor.Name}\n";
            }
            else
            {
                info += "- Empty Slot\n";
            }
        }
        info += "Energy Shields:\n";
        if (_energyShield != null)
        {
            info += $"- {_energyShield.Name}\n";
        }
        else
        {
            info += "- Empty Slot\n";
        }
        return info;
    }
}
