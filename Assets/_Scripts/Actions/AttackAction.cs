using HeartOfIron.Enums;
using System.Collections.Generic;

/// <summary>
/// Represents an attack action performed by a mech, which involves a series of attacks against a target mech.
/// This class serves as a base for specific types of attack actions, encapsulating the common properties and behavior related to attacks.
/// </summary>
public abstract class AttackAction : Action
{
    /// <summary>
    /// The list of attacks that are part of this attack action.
    /// </summary>
    private List<Attack> _attacks;
    public List<Attack> Attacks => _attacks;

    public AttackAction(ActionType type, MechController sourceMech, MechController targetMech, IEnumerable<Attack> attacks ) : base(type, sourceMech, targetMech)
    {
        this._attacks = new List<Attack>(attacks);
    }
}
