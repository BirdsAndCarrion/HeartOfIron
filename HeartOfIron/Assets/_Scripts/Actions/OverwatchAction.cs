using UnityEngine;
using System;
using HeartOfIron.Enums;

public class OverwatchAction : DefendAction
{
    public OverwatchAction(MechController sourceMech) : base(ActionType.Overwatch, sourceMech)
    {
    }

    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.LogWarning($"Cannot execute OverwatchAction due to failed CanExecute check.");
            return false;
        }

        SourceMech.UpdateMechState(MechState.Overwatch);
        Debug.Log($"{SourceMech.Mech.Name}: Executing OverwatchAction. Mech state updated to Overwatch.");
        return true;
    }

    public override bool CanExecute()
    {
        if (SourceMech == null || SourceMech.MechState == MechState.Dead)
        {
            Debug.LogWarning("Cannot execute OverwatchAction because the mech is null or dead.");
            return false;
        }

        if (SourceMech.MechState == MechState.Overwatch)
        {
            Debug.LogWarning($"{SourceMech.Mech.Name}: Cannot execute OverwatchAction because the mech is already in overwatch.");
            return false;
        }

        return true;
    }
}
