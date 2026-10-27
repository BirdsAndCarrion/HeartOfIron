using HeartOfIron.Enums;

/// <summary>
/// Represents a defensive action performed by a mech.
/// </summary>
public abstract class DefendAction : Action
{
    public DefendAction(ActionType type, MechController sourceMech) : base(type, sourceMech)
    {
    }
}
