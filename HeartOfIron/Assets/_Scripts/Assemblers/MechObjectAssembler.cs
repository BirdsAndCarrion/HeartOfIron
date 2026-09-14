using UnityEngine;

public class MechObjectAssembler : MonoBehaviour
{
    
    public static MechObjectAssembler Instance { get; private set; }

    [SerializeField] private GameObject _mechPrefab;
    [SerializeField] private GameObject _mechPrefabWController; // New prefab with MechController attached
    [SerializeField] private float MECH_SCALE = 1.0f; // Adjust the scale as needed

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
        mechObject.GetComponent<Renderer>().material.color = mechData.Color; // TODO: Apply color to specific parts of the mech instead of the whole object
        // TODO: Apply pilot color to secondary parts of the mech if applicable
        // TODO: Apply spawning shader effects to the mechObject if applicable
        MechController mechController = mechObject.GetComponent<MechController>();
        mechController.Initialize(mechData);
        mechObject.name = $"Mech_{mechData.Name}";
        // TODO: Add logic for attaching weapon prefabs to the mech based on the mechData's equipped weapons
        // Attach prefabs for equipped weapons to the mechObject based on the mechData's equipped weapons
        // Name the attached weapon prefabs based on the weapon's name for easier identification
        // TODO: Remove this logic in favor of the new weapon attachment system once implemented
        MechVision mechVision = mechObject.transform.Find("Vision")?.GetComponent<MechVision>();
        ShootProjectile[] shootProjectiles = mechObject.GetComponentsInChildren<ShootProjectile>();
        mechController.SetShootProjectiles(shootProjectiles);
        OverwatchScript overwatchScript = mechObject.transform.Find("Overwatch")?.GetComponent<OverwatchScript>();
        if (overwatchScript != null)
        {
            overwatchScript.Initialize(mechController);
        }
        else
        {
            Debug.LogError("OverwatchScript not found in the mech prefab.");
        }

        if (mechVision != null)
        {
            mechVision.SetVisionParameters(mechData.VisionRange, mechData.VisionAngle);
        }
        else
        {
            Debug.LogError("MechVision component not found in the mech prefab.");
        }
        mechObject.transform.localScale = Vector3.one * MECH_SCALE; // Adjust the scale as needed
        Debug.Log($"Assembled Mech: {mechData.Name} at position {position}");
        return mechController;
    }
}
