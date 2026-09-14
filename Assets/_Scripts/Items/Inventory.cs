using UnityEngine;
using System;
using HeartOfIron.Enums;

[Serializable]
public class Inventory
{
    [SerializeReference] private Weapon[] _weapons;
    public Weapon[] Weapons => _weapons;
    [SerializeReference] private Armor[] _armors;
    public Armor[] Armors => _armors;
    [SerializeReference] private EnergyShield _energyShield;
    public EnergyShield EnergyShield => _energyShield;

    public Inventory()
    {
        _weapons = new Weapon[2];
        _armors = new Armor[4]; // Assuming 4 armor slots for head, torso, arms, and legs
        _energyShield = null; // Initialize energy shield to null
    }

    public Inventory(Weapon weapon1, Weapon weapon2, Armor headArmor, Armor torsoArmor, Armor armArmor, Armor legArmor, EnergyShield energyShield)
    {
        _weapons = new Weapon[2];
        _weapons[(int)Hand.Right] = weapon1;
        _weapons[(int)Hand.Left] = weapon2;
        _armors = new Armor[4];
        _armors[(int)ArmorType.Head] = headArmor;
        _armors[(int)ArmorType.Torso] = torsoArmor;
        _armors[(int)ArmorType.Arms] = armArmor;
        _armors[(int)ArmorType.Legs] = legArmor;
        _energyShield = energyShield;
    }

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
        else if (item is EnergyShield shield)
        {
            if (_energyShield != null && _energyShield.Id == shield.Id)
            {
                return true;
            }
        }
        return false;
    }

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
                return true;
            }
            else if (_weapons[(int)Hand.Left] == null)
            {
                _weapons[(int)Hand.Left] = weapon;
                weapon.SetOwner(owner); // Set the owner of the weapon when equipped
                return true;
            }
        } else
        {
            Debug.LogWarning("Cannot equip weapon: Not enough available slots.");
            return false;
        }
        return false;
    }

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
    }

    public bool EquipArmor(Armor armor, Mech owner)
    {
        if (!CanEquipItem(armor))
        {
            return false;
        }
        int armorIndex = (int)armor.ArmorType;
        if (armorIndex >= 0 && armorIndex < _armors.Length)
        {
            if (_armors[armorIndex] != null)
            {
                Debug.LogWarning($"Cannot equip armor: Slot for {armor.ArmorType} is already occupied.");
                return false;
            }
            _armors[armorIndex] = armor;
            armor.SetOwner(owner); // Set the owner of the armor when equipped
            return true;
        }
        else
        {
            Debug.LogWarning("Invalid armor type.");
            return false;
        }
    }

    public void UnequipArmor(Armor armor)
    {
        int armorIndex = (int)armor.ArmorType;
        if (armorIndex >= 0 && armorIndex < _armors.Length)
        {
            if (_armors[armorIndex] == armor)
            {
                _armors[armorIndex] = null;
                armor.ClearOwner(); // Clear the owner of the armor when unequipped
            }
            else
            {
                Debug.LogWarning($"Armor of type {armor.ArmorType} is not currently equipped.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid armor type.");
        }
    } 

    public bool EquipEnergyShield(EnergyShield energyShield, Mech owner)
    {
        if (!CanEquipItem(energyShield))
        {
            return false;
        }
        if (_energyShield != null)
        {
            Debug.LogWarning("Cannot equip energy shield: An energy shield is already equipped.");
            return false;
        }
        _energyShield = energyShield;
        energyShield.SetOwner(owner); // Set the owner of the energy shield when equipped
        return true;
    }

    public void UnequipEnergyShield(EnergyShield energyShield)
    {
        if (_energyShield == null)
        {
            Debug.LogWarning("Cannot unequip energy shield: No energy shield is currently equipped.");
            return;
        }
        if (_energyShield != energyShield)
        {
            Debug.LogWarning("Cannot unequip energy shield: The specified energy shield is not currently equipped.");
            return;
        }
        _energyShield.ClearOwner(); // Clear the owner of the energy shield when unequipped
        _energyShield = null;
    }

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

    public Weapon GetIdealRangedWeapon(float distance)
    {
        Weapon idealWeapon = null;
        foreach (Weapon weapon in _weapons)
        {
            if (weapon != null && !weapon.IsMelee && distance > weapon.MeleeRange)
            {
                if (idealWeapon == null)
                {
                    idealWeapon = weapon;
                } else if (weapon.CanAttack(distance) && !idealWeapon.CanAttack(distance))
                {
                    idealWeapon = weapon;
                } else if (weapon.CanAttack(distance) && weapon.GetAverageDamage(distance) > idealWeapon.GetAverageDamage(distance))
                {
                    idealWeapon = weapon;
                }
            }
        }
        return idealWeapon;
    }

    public Weapon GetIdealMeleeWeapon()
    {
        Weapon idealWeapon = null;
        foreach (Weapon weapon in _weapons)
        {
            if (weapon != null)
            {
                if (idealWeapon == null)
                {
                    idealWeapon = weapon;
                } else if (weapon.GetAverageDamage(0) > idealWeapon.GetAverageDamage(0))
                {
                    idealWeapon = weapon;
                }
            }
        }
        return idealWeapon;
    }

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
                info += $"- {armor.Name} ({armor.ArmorType})\n";
            }
            else
            {
                info += "- Empty Slot\n";
            }
        }
        info += "Energy Shield:\n";
        if (_energyShield != null)
        {
            info += $"- {_energyShield.Name}\n";
        }
        else
        {
            info += "- None\n";
        }
        return info;
    }
}
