using UnityEngine;
using System;
using HeartOfIron.Enums;

public class HunkerAction : DefendAction
{
    public HunkerAction(MechController sourceMech) : base(ActionType.Hunker, sourceMech)
    {
    }

    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.Log($"Cannot execute HunkerAction due to failed CanExecute check.");
            return false;
        }

        SourceMech.UpdateMechState(MechState.Hunkering);
        Debug.Log($"{SourceMech.Mech.Name}: Executing HunkerAction. Mech state updated to Hunkering.");
        return true;
    }

    public override bool CanExecute()
    {
        if (SourceMech == null)
        {
            Debug.LogWarning("HunkerAction cannot be executed because SourceMech is null.");
            return false;
        }
        if (SourceMech.MechState == MechState.Dead)
        {
            Debug.Log($"{SourceMech.Mech.Name}: Cannot execute HunkerAction because the mech is dead.");
            return false;
        }
        if (SourceMech.MechState == MechState.Hunkering)
        {
            Debug.Log($"{SourceMech.Mech.Name}: Cannot execute HunkerAction because the mech is already hunkering.");
            return false;
        }

        return true;
    }
}
