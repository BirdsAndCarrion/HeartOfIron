using UnityEngine;
using System;
using HeartOfIron.Enums;

[Serializable, RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] private BodyPart bodyPart;

    public void SetLocationIndex(int index)
    {
        bodyPart = (BodyPart)index;
    }

    public int GetLocationIndex()
    {
        return (int)bodyPart;
    }
}
