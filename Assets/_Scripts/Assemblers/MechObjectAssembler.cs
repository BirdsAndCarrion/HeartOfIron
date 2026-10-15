using UnityEngine;
using System.Collections.Generic;
using HeartOfIron.Enums;

public class MechObjectAssembler : MonoBehaviour
{
    
    public static MechObjectAssembler Instance { get; private set; }

    [SerializeField] private GameObject _mechPrefab;
    [SerializeField] private GameObject _mechPrefabWController; // New prefab with MechController attached
    [SerializeField] private float MECH_SCALE = 1.0f; // Adjust the scale as needed
    [SerializeField] private WeaponPrefabsList _weaponPrefabsList;
    

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

    public MechController AssembleMechObject(Mech mechData, Vector3 position, Quaternion rotation)
    {
        GameObject mechObject = Instantiate(_mechPrefabWController, position, rotation);
        Material[] materials = mechObject.GetComponent<Renderer>().materials;
        materials[0].color = mechData.Color; // Apply color to the primary material of the mech
        if (materials.Length > 1)
        {
            materials[1].color = mechData.Pilot.Color; // Apply color to the secondary material of the mech
        }
        MechController mechController = mechObject.GetComponent<MechController>();
        mechController.Initialize(mechData);
        mechObject.name = $"Mech_{mechData.Name}";
        List<ShootProjectile> shootProjectiles = new List<ShootProjectile>();
        for (int i = 0; i < mechData.Inventory.Weapons.Length; i++)
        {
            Weapon weapon = mechData.Inventory.Weapons[i];
            if (weapon == null) continue;
            // Get prefabs from fields of MechObjectAssembler
            GameObject weaponPrefab = _weaponPrefabsList.GetPrefabByWeaponName(weapon.WeaponName);
            if (weaponPrefab == null) continue;
            // Find ShootProjectile component in the mechObject's children and add them to the list
            ShootProjectile shootProjectileComponent = weaponPrefab.GetComponentInChildren<ShootProjectile>();
            // Add the ShootProjectile that should be found for all ranged weapons
            if (shootProjectileComponent != null)
            {
                shootProjectiles.Add(shootProjectileComponent);
            }
            weaponPrefab.name = weapon.Name; // Name the attached weapon prefab based on the weapon's name
            // Pass prefabs into the equipping module for proper attachment to the mech
            if (weapon.SlotsRequired == 1)
            {
                mechController.EquippingModule.EquipOneHandedItem(weaponPrefab, i == (int)Hand.Right);
            } else if (weapon.SlotsRequired == 2)
            {
                mechController.EquippingModule.EquipTwoHandedItem(weaponPrefab, weapon.IsMelee);
            }
        }
        mechController.SetShootProjectiles(shootProjectiles.ToArray());
        mechObject.transform.localScale = Vector3.one * MECH_SCALE; // Adjust the scale as needed
        Debug.Log($"Assembled Mech: {mechData.Name} at position {position}");
        return mechController;
    }
}
