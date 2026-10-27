using UnityEngine;
using HeartOfIron.Enums;

/// <summary>
/// Represents an overwatch action performed by a mech, putting it into an overwatch state.
/// </summary>
public class OverwatchAction : DefendAction
{
    public OverwatchAction(MechController sourceMech) : base(ActionType.Overwatch, sourceMech)
    {
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
