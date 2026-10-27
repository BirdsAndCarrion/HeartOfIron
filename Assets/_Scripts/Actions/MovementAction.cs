using UnityEngine;
using HeartOfIron.Enums;

/// <summary>
/// Represents a movement action that involves a mech moving to a specific destination.
/// </summary>
public abstract class MovementAction : Action
{
    /// <summary>
    /// The destination the mech is moving towards.
    /// </summary>
    private Vector3 _destination;
    public Vector3 Destination => _destination;

    public MovementAction(ActionType type, MechController sourceMech, Vector3 destination, MechController targetMech = null) : base(type, sourceMech, targetMech)
    {
        _destination = destination;
    }

    /// <summary>
    /// Sets the destination the mech should move towards.
    /// </summary>
    /// <param name="destination">The new destination for the mech.</param>
    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }
}
