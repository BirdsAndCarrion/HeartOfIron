using UnityEngine;
using HeartOfIron.Enums;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "WeaponPrefabsList", menuName = "Scriptable Objects/WeaponPrefabsList")]
public class WeaponPrefabsList : ScriptableObject
{
    public List<WeaponPrefabEntry> WeaponPrefabs;

    public GameObject GetPrefabByWeaponName(WeaponName weaponName)
    {
        foreach (var entry in WeaponPrefabs)
        {
            if (entry.WeaponName == weaponName)
            {
                return entry.Prefab;
            }
        }
        return null;
    }
}

[Serializable]
public class WeaponPrefabEntry
{
    public WeaponName WeaponName;
    public GameObject Prefab;
}


