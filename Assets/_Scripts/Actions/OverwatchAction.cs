using UnityEngine;
using HeartOfIron.Enums;

public class OverwatchAction : DefendAction
{
    public OverwatchAction(MechController sourceMech) : base(ActionType.Overwatch, sourceMech)
    {
    }

    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.LogWarning($"Cannot execute OverwatchAction due to failed CanExecute check: {canExecuteResult}");
            return canExecuteResult;
        }

        SourceMech.UpdateMechState(MechState.Overwatch);
        Debug.Log($"{SourceMech.Mech.Name}: Executing OverwatchAction. Mech state updated to Overwatch.");
        return ActionResult.Success;
    }

    public override ActionResult CanExecute()
    {
        if (SourceMech == null)
        {
            Debug.LogWarning("Cannot execute OverwatchAction because the mech is null.");
            return ActionResult.NullInput;
        } else if (SourceMech.MechState == MechState.Dead)
        {
            Debug.LogWarning("Cannot execute OverwatchAction because the mech is dead.");
            return ActionResult.Failure;
        } else if (SourceMech.MechState == MechState.Overwatch)
        {
            Debug.LogWarning($"{SourceMech.Mech.Name}: Cannot execute OverwatchAction because the mech is already in overwatch.");
            return ActionResult.Failure;
        }

        return ActionResult.Success;
    }
}
