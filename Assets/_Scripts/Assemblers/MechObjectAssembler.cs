using UnityEngine;
using System.Collections.Generic;

public class MechObjectAssembler : MonoBehaviour
{
    
    public static MechObjectAssembler Instance { get; private set; }

    [SerializeField] private GameObject _mechPrefab;
    [SerializeField] private GameObject _mechPrefabWController; // New prefab with MechController attached
    [SerializeField] private float MECH_SCALE = 1.0f; // Adjust the scale as needed

    // TODO: Add fields for all weapon prefabs (list?)

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

    // TODO: Update this method based on the new MechController initialization logic
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
        EquippingModule equippingModule = mechObject.GetComponent<EquippingModule>();
        List<ShootProjectile> shootProjectiles = new List<ShootProjectile>();
        // TODO: Add logic for attaching weapon prefabs to the mech based on the mechData's equipped weapons
            // Get prefabs from fields of MechObjectAssembler
            // Find ShootProjectile component in the mechObject's children and add them to the list
            ShootProjectile shootProjectileComponent = mechObject.GetComponentInChildren<ShootProjectile>();
            // Add the ShootProjectile that should be found for all ranged weapons
            if (shootProjectileComponent != null)
            {
                shootProjectiles.Add(shootProjectileComponent);
            }
            // Name the attached weapon prefabs based on the weapon's name for easier identification
            // Pass prefabs into the equipping module for proper attachment to the mech
        mechController.SetShootProjectiles(shootProjectiles.ToArray());
        mechObject.transform.localScale = Vector3.one * MECH_SCALE; // Adjust the scale as needed
        Debug.Log($"Assembled Mech: {mechData.Name} at position {position}");
        return mechController;
    }
}
