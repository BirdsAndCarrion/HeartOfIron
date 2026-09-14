using UnityEngine;
using System;
using HeartOfIron.Enums;

public abstract class AttackAction : Action
{
    private Weapon _weapon;
    public Weapon Weapon => _weapon;

    public AttackAction(ActionType type, MechController sourceMech, MechController targetMech, Weapon weapon) : base(type, sourceMech, targetMech)
    {
        this._weapon = weapon;
    }
}
