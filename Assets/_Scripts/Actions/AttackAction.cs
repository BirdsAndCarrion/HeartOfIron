using HeartOfIron.Enums;
using System.Collections.Generic;

public abstract class AttackAction : Action
{
    private List<Weapon> _weapons;
    public List<Weapon> Weapons => _weapons;

    public AttackAction(ActionType type, MechController sourceMech, MechController targetMech, List<Weapon> weapons) : base(type, sourceMech, targetMech)
    {
        this._weapons = weapons;
    }
}
