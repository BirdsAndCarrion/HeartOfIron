using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class MechVision : MonoBehaviour
{
    private SphereCollider visionCollider;
    [SerializeField] private float visionRadius = 10f; // Vision range in units
    [SerializeField] private float visionAngle = 90f; // Field of view angle in degrees
    private List<MechController> detectedMechs = new List<MechController>();
    //private LayerMask targetLayerMask;

    public UnityEvent<MechController, MechController> OnMechSpotted;
    public UnityEvent<MechController, MechController> OnMechLost;

    private void Awake()
    {
        visionCollider = GetComponent<SphereCollider>();
        visionCollider.isTrigger = true;
        //targetLayerMask = LayerMask.GetMask("Player"); // Assuming the player is on the "Player" layer
    }

    void Start()
    {
        EventRouter.RegisterWithHub(this);
    }

    void OnDestroy()
    {
        if (this == null)
        {
            Debug.LogError("MechVision OnDestroy called on a null instance.");
            return;
        }
        EventRouter.UnregisterFromHub(this);
    }

    public void SetVisionParameters(float radius, float angle)
    {
        visionRadius = radius;
        visionAngle = angle;
        if (visionCollider != null)
        {
            visionCollider.radius = visionRadius;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if other object has a MechController component
        MechController mechController = other.GetComponent<MechController>();
        if (mechController != null)
        {
            // Check if the mech is within the vision angle
            Vector3 directionToMech = (other.transform.position - transform.position).normalized;
            float angleToMech = Vector3.Angle(transform.forward, directionToMech);

            if (angleToMech <= visionAngle / 2f && !detectedMechs.Contains(mechController))
            {
                // Mech is within vision range and angle
                Debug.Log("Mech detected: " + mechController.Mech.Name);
                // You can add additional logic here, such as alerting the mech or triggering an event
                detectedMechs.Add(mechController);
                OnMechSpotted?.Invoke(this.transform.parent.GetComponent<MechController>(), mechController);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if other object has a MechController component
        MechController mechController = other.GetComponent<MechController>();
        if (mechController != null)
        {
            // Check if the mech is within the vision angle
            Vector3 directionToMech = (other.transform.position - transform.position).normalized;
            float angleToMech = Vector3.Angle(transform.forward, directionToMech);

            if (angleToMech <= visionAngle / 2f && !detectedMechs.Contains(mechController))
            {
                // Mech is within vision range and angle
                Debug.Log("Mech still in sight: " + mechController.Mech.Name);
                // You can add additional logic here, such as alerting the mech or triggering an event
                OnMechSpotted?.Invoke(this.transform.parent.GetComponent<MechController>(), mechController);
                detectedMechs.Add(mechController);
            } else if (angleToMech > visionAngle / 2f && detectedMechs.Contains(mechController))
            {
                // Mech has moved out of the vision angle
                Debug.Log("Mech lost sight: " + mechController.Mech.Name);
                // You can add additional logic here, such as alerting the mech or triggering an event
                OnMechLost?.Invoke(this.transform.parent.GetComponent<MechController>(), mechController);
                detectedMechs.Remove(mechController);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if other object has a MechController component
        MechController mechController = other.GetComponent<MechController>();
        if (mechController != null && detectedMechs.Contains(mechController))
        {
            // Mech has exited the vision range
            Debug.Log("Mech exited vision range: " + mechController.Mech.Name);
            // You can add additional logic here, such as alerting the mech or triggering an event
            OnMechLost?.Invoke(this.transform.parent.GetComponent<MechController>(), mechController);
            detectedMechs.Remove(mechController);
        }
    }
}
