using HeartOfIron.Enums;
using System.Collections.Generic;

public abstract class AttackAction : Action
{
    private List<Attack> _attacks;
    public List<Attack> Attacks => _attacks;

    public AttackAction(ActionType type, MechController sourceMech, MechController targetMech, IEnumerable<Attack> attacks ) : base(type, sourceMech, targetMech)
    {
        this._attacks = new List<Attack>(attacks);
    }
}
