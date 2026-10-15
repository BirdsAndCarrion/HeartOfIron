using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using HeartOfIron.Structs;

[RequireComponent(typeof(SphereCollider)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
public class MechVision : MonoBehaviour
{
    private SphereCollider visionCollider;
    [SerializeField] private float visionRadius = 10f; // Vision range in units
    [SerializeField] private float visionAngle = 90f; // Field of view angle in degrees
    private List<MechController> detectedMechs = new List<MechController>();
    //private LayerMask targetLayerMask;

    public UnityEvent<MechController, MechController> OnMechSpotted;
    public UnityEvent<MechController, MechController> OnMechLost;

    [SerializeField] private float meshResolution = 1f; // Resolution of the vision mesh
    [SerializeField] private MeshFilter visionMeshFilter;
    [SerializeField] private int edgeResolveIterations = 4; // Number of iterations to refine the edge detection
    [SerializeField] private float edgeDistanceThreshold = 0.5f; // Threshold distance to consider an edge detected
    private Mesh visionMesh;

    private void Awake()
    {
        visionCollider = GetComponent<SphereCollider>();
        visionCollider.isTrigger = true;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            meshRenderer.material.color = new Color(1f, 1f, 1f, 0.3f); // Set the material color to a semi-transparent white
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Disable shadows for the vision mesh
            meshRenderer.receiveShadows = false; // Disable receiving shadows for the vision mesh
        }
        visionMeshFilter = GetComponent<MeshFilter>();
        if (visionMeshFilter != null)
        {
            visionMesh = new Mesh();
            visionMesh.name = "Vision Mesh";
            visionMeshFilter.mesh = visionMesh;
        }
        //targetLayerMask = LayerMask.GetMask("Player"); // Assuming the player is on the "Player" layer
    }

    void Start()
    {
        EventRouter.RegisterWithHub(this);
    }

    void LateUpdate()
    {
        DrawVisionMesh();
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

    public void Init(float radius, float angle)
    {
        SetVisionParameters(radius, angle);
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

    private void DrawVisionMesh()
    {
        Debug.Log("Drawing vision mesh.");
        int stepCount = Mathf.RoundToInt(visionAngle * meshResolution);
        float stepAngleSize = visionAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo previousViewCast = new ViewCastInfo(false, Vector3.zero, 0f, 0f);
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -visionAngle / 2f + stepAngleSize * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            ViewCastInfo viewCastInfo = ViewCast(angle);

            if (i > 0 && (previousViewCast.hit != viewCastInfo.hit || (previousViewCast.hit && viewCastInfo.hit && Vector3.Distance(previousViewCast.point, viewCastInfo.point) > edgeDistanceThreshold)))
            {
                EdgeInfo edge = FindEdge(previousViewCast, viewCastInfo);
                if (edge.pointA != Vector3.zero)
                {
                    viewPoints.Add(edge.pointA);
                }
                if (edge.pointB != Vector3.zero)
                {
                    viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(viewCastInfo.point);
            Debug.DrawLine(transform.position, viewCastInfo.point, Color.green);
            previousViewCast = viewCastInfo;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = minViewCast.point;
        Vector3 maxPoint = maxViewCast.point;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2f;
            ViewCastInfo viewCast = ViewCast(angle);
            bool edgeHit = viewCast.hit;
            if (edgeHit == minViewCast.hit)
            {
                minAngle = angle;
                minPoint = viewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = viewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = Quaternion.Euler(0, globalAngle, 0) * transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, visionRadius))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + direction * visionRadius, visionRadius, globalAngle);
        }
    }
}
