using UnityEngine;
using HeartOfIron.Enums;

/// <summary>
/// Represents a hunker action performed by a mech, putting it into a defensive hunkering state.
/// </summary>
public class HunkerAction : DefendAction
{
    public HunkerAction(MechController sourceMech) : base(ActionType.Hunker, sourceMech)
    {
    }

    /// <inheritdoc/>
    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.Log($"Cannot execute HunkerAction due to failed CanExecute check: {canExecuteResult}");
            return canExecuteResult;
        }

        SourceMech.UpdateMechState(MechState.Hunkering);
        Debug.Log($"{SourceMech.Mech.Name}: Executing HunkerAction. Mech state updated to Hunkering.");
        return ActionResult.Success;
    }

    /// <inheritdoc/>
    public override ActionResult CanExecute()
    {
        if (SourceMech == null)
        {
            Debug.LogWarning("HunkerAction cannot be executed because SourceMech is null.");
            return ActionResult.NullInput;
        }
        if (SourceMech.MechState == MechState.Dead)
        {
            Debug.Log($"{SourceMech.Mech.Name}: Cannot execute HunkerAction because the mech is dead.");
            return ActionResult.Failure;
        }
        if (SourceMech.MechState == MechState.Hunkering)
        {
            Debug.Log($"{SourceMech.Mech.Name}: Cannot execute HunkerAction because the mech is already hunkering.");
            return ActionResult.Failure;
        }

        return ActionResult.Success;
    }
}
