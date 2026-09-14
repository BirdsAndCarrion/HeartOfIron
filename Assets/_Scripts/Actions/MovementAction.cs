using UnityEngine;
using HeartOfIron.Enums;


public abstract class MovementAction : Action
{
    private Vector3 _destination;
    public Vector3 Destination => _destination;

    public MovementAction(ActionType type, MechController sourceMech, Vector3 destination, MechController targetMech = null) : base(type, sourceMech, targetMech)
    {
        _destination = destination;
    }

    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }
}
