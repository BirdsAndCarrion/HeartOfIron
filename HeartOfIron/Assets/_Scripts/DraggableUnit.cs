using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class DraggableUnit : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private GameObject _ghostUnitPrefab;
    [SerializeField] private Material _validPlacementMaterial;
    [SerializeField] private Material _invalidPlacementMaterial;
    private GameObject _ghostUnitInstance;
    private Vector3 _offset;
    private Mech _mechData;

    public void Initialize(Mech mechData, GameObject ghostUnitPrefab, Material validPlacementMaterial, Material invalidPlacementMaterial)
    {
        _mechData = mechData;
        _ghostUnitPrefab = ghostUnitPrefab;
        _validPlacementMaterial = validPlacementMaterial;
        _invalidPlacementMaterial = invalidPlacementMaterial;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Create a ghost unit when the player starts dragging
        _ghostUnitInstance = Instantiate(_ghostUnitPrefab, transform.position, Quaternion.identity);
        _ghostUnitInstance.transform.localScale = new Vector3(1, 1, 1); // Adjust the scale of the ghost unit as needed
        _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_ghostUnitInstance != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            // Raycast should ignore the Ignore Raycast layer
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")) && hit.collider.GetComponent<Terrain>() != null)
            {
                Vector3 newPosition = hit.point /*+ _offset*/;
                _ghostUnitInstance.transform.position = newPosition;
                _ghostUnitInstance.GetComponent<Renderer>().material = CombatManager.Instance.IsValidPlacementPosition(newPosition) ? _validPlacementMaterial : _invalidPlacementMaterial;  

            }
            else
            {
                Debug.LogWarning("Raycast did not hit the terrain. Cannot drag.");
            }
        }
        else
        {
            Debug.LogWarning("Ghost unit instance is null. Cannot drag.");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_ghostUnitInstance != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")) && hit.collider.GetComponent<Terrain>() != null)
            {
                Vector3 finalPosition = hit.point /*+ _offset*/;
                if (CombatManager.Instance.TryPlaceUnitAtPosition(_mechData, finalPosition))
                {
                    // Successfully placed the unit
                    Debug.Log("Unit deployed successfully at position: " + finalPosition);
                    Destroy(_ghostUnitInstance);
                    Destroy(gameObject); // Destroy the original unit after placement
                }
                else
                {
                    // Invalid placement, destroy the ghost unit and reset position
                    Destroy(_ghostUnitInstance);
                    Debug.LogWarning("Invalid placement. Unit not deployed.");
                }
            }
            else
            {
                Debug.LogWarning("Raycast did not hit the terrain. Cannot place unit.");
                Destroy(_ghostUnitInstance);
            }
        }
    }
}

