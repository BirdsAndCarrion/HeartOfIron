using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;

[Serializable]
public class EquippingModule : MonoBehaviour
{
    [SerializeField] private Transform shoulderLocation;
    [SerializeField] private Transform handLocationR;
    [SerializeField] private Transform handLocationL;

    [SerializeField] private Rig RArmRig;
    [SerializeField] private TwoBoneIKConstraint RArmIK;
    [SerializeField] private Rig LArmRig;
    [SerializeField] private TwoBoneIKConstraint LArmIK;
    [SerializeField] private Rig AimRig;
    [SerializeField] private MultiPositionConstraint AimConstraint;
    [SerializeField] private MultiAimConstraint AimAimConstraint;
    [SerializeField] private Rig RHandAimRig;
    [SerializeField] private MultiParentConstraint RHandAimConstraint;
    [SerializeField] private Rig LHandAimRig;
    [SerializeField] private MultiParentConstraint LHandAimConstraint;

    // TODO: Equipping melee weapons?

    public void EquipOneHandedItem(GameObject item, bool isRightHand)
    {
        if (isRightHand)
        {
            foreach (Transform child in handLocationR)
            {
                Destroy(child.gameObject);
            }
            item.transform.SetParent(handLocationR);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
        else
        {
            foreach (Transform child in handLocationL)
            {
                Destroy(child.gameObject);
            }
            item.transform.SetParent(handLocationL);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        RArmIK.data.target = null;
        LArmIK.data.target = null;
        AimConstraint.data.constrainedObject = null;
        AimAimConstraint.data.constrainedObject = null;

        if (isRightHand)
        {
            RHandAimConstraint.data.constrainedObject = handLocationR;
        }
        else
        {
            LHandAimConstraint.data.constrainedObject = handLocationL;
        }

        RArmRig.weight = 0f;
        LArmRig.weight = 0f;
        AimRig.weight = 0f;
        RHandAimRig.weight = 1f;
        LHandAimRig.weight = 1f;
    }

    public void EquipTwoHandedItem(GameObject item)
    {
        foreach (Transform child in handLocationR)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in handLocationL)
        {
            Destroy(child.gameObject);
        }

        item.transform.SetParent(shoulderLocation);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        RArmIK.data.target = item.transform.Find("Primary");
        LArmIK.data.target = item.transform.Find("Secondary");

        AimConstraint.data.constrainedObject = item.transform;
        AimAimConstraint.data.constrainedObject = item.transform;

        RHandAimConstraint.data.constrainedObject = null;
        LHandAimConstraint.data.constrainedObject = null;

        RArmRig.weight = 1f;
        LArmRig.weight = 1f;
        AimRig.weight = 1f;
        RHandAimRig.weight = 0f;
        LHandAimRig.weight = 0f;
    }
}
