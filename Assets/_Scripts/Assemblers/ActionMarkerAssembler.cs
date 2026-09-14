using UnityEngine;
using System;

[Serializable]
public class ActionMarkerAssembler : MonoBehaviour
{
    public static ActionMarkerAssembler Instance { get; private set; }

    [SerializeField] private GameObject _actionMarkerPrefab;
    [SerializeField] private float ACTION_MARKER_SCALE = 0.5f; // Adjust the scale as needed

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

    public virtual ActionMarker AssembleActionMarker(Vector3 position, MechController sourceMech, bool addToMech = true)
    {
        GameObject actionMarkerObject = Instantiate(_actionMarkerPrefab, position, Quaternion.identity);
        ActionMarker actionMarker = actionMarkerObject.GetComponent<ActionMarker>();
        actionMarker.Initialize(position, sourceMech);
        actionMarkerObject.name = $"ActionMarker_{actionMarker.Id}";
        actionMarkerObject.transform.position += new Vector3(0, 0.1f, 0); // Slightly above the ground to avoid clipping
        actionMarkerObject.transform.localScale = Vector3.one * ACTION_MARKER_SCALE; // Adjust the scale as needed
        if (sourceMech != null && sourceMech.ActionMarkers != null)
        {
            if (addToMech)
            {
                sourceMech.AddActionMarker(actionMarker);
            }
            if (sourceMech.Mech.Color != null)
            {
                actionMarkerObject.GetComponent<Renderer>().material.color = sourceMech.Mech.Color; // Assuming the mech's color is used for the action marker
            } else
            {
                actionMarkerObject.GetComponent<Renderer>().material.color = Color.white; // Default color if no color is defined
                Debug.LogWarning($"{sourceMech.Mech.Name} has no color defined. Defaulting action marker color to white.");
            }
        }
        Debug.Log($"Assembled ActionMarker: {actionMarker.Id} at position {position} for Mech: {sourceMech.Mech.Name}");
        return actionMarker;
    }
}
