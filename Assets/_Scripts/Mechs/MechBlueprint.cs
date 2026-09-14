using UnityEngine;
using HeartOfIron.Enums;

[CreateAssetMenu(fileName = "MechBlueprint", menuName = "Scriptable Objects/MechBlueprint")]
public class MechBlueprint : ItemBlueprint
{
    [SerializeField] private MechType _mechType;
    public MechType MechType => _mechType;
    [SerializeField] private float[] _maxHealth;
    public float[] Health => _maxHealth;
    [SerializeField] private float _maxSpeed;
    public float MaxSpeed => _maxSpeed;
    
}
