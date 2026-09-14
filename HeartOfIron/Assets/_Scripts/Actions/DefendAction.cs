using UnityEngine;
using System;
using HeartOfIron.Enums;

public abstract class DefendAction : Action
{
    public DefendAction(ActionType type, MechController sourceMech) : base(type, sourceMech)
    {
    }
}
