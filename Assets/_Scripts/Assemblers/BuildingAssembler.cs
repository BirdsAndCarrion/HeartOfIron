using UnityEngine;
using System.Collections.Generic;

public class BuildingAssembler : MonoBehaviour
{

    [SerializeField] private List<GameObject> _buildingWallAttachments;
    [SerializeField] private List<GameObject> _buildingRoofAttachments;

    public GameObject AssembleBuilding(GameObject buildingBase)
    {
        Queue<Transform> wallAttachmentPoints = new Queue<Transform>();
        List<Transform> roofAttachmentPoints = new List<Transform>();

        foreach (Transform child in buildingBase.transform)
        {
            if (child.CompareTag("WallAttachmentPoint"))
                wallAttachmentPoints.Enqueue(child);
            else if (child.CompareTag("GenericAttachmentPoint"))
            {
                wallAttachmentPoints.Enqueue(child);
                roofAttachmentPoints.Add(child);
            }
        }
        while (wallAttachmentPoints.Count > 0 && !CheckIfOnlyRoofAttachments(wallAttachmentPoints))
        {
            Transform attachmentPoint = wallAttachmentPoints.Dequeue();
            GameObject wallAttachment = _buildingWallAttachments[UnityEngine.Random.Range(0, _buildingWallAttachments.Count)];
            Instantiate(wallAttachment, attachmentPoint.position, attachmentPoint.rotation, buildingBase.transform);
        }
        Vector2 roofDimensions = ComputeRoofDimensions(roofAttachmentPoints);
        if (roofDimensions != Vector2.zero)
        {
            GameObject roofAttachment = _buildingRoofAttachments[UnityEngine.Random.Range(0, _buildingRoofAttachments.Count)];
            Instantiate(roofAttachment, new Vector3(0, buildingBase.transform.position.y, 0.5f), Quaternion.identity, wallAttachmentPoints.Count > 0 ? wallAttachmentPoints.Peek().parent : buildingBase.transform);
        }
        return buildingBase;
    }
    private bool CheckIfOnlyRoofAttachments(Queue<Transform> wallAttachmentPoints)
    {
        if (wallAttachmentPoints == null || wallAttachmentPoints.Count == 0)
            return true;
        float height = 0f;
        foreach (Transform point in wallAttachmentPoints)
        {
            if (height == 0f)
                height = point.position.y;
            if (point.position.y != height)
                return false;
            if (point.CompareTag("WallAttachmentPoint"))
                return false;
            
        }
        return true;
    }
    private Vector2 ComputeRoofDimensions(List<Transform> roofAttachmentPoints)
    {
        if (roofAttachmentPoints == null || roofAttachmentPoints.Count == 0)
            return Vector2.zero;
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        foreach (Transform point in roofAttachmentPoints)
        {
            Vector3 pos = point.position;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.z < minZ) minZ = pos.z;
            if (pos.z > maxZ) maxZ = pos.z;
        }
        return new Vector2(maxX - minX, maxZ - minZ);
    }
}
