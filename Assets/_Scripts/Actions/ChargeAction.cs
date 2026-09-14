using UnityEngine;
using System.Collections.Generic;
using HeartOfIron.Enums;

public class ChargeAction : MovementAction
{

    private List<Weapon> _weapons;

    public ChargeAction(MechController sourceMech, MechController targetMech, List<Weapon> weapons) : base(ActionType.Charge, sourceMech, targetMech != null ? targetMech.transform.position : Vector3.zero, targetMech)
    {
        _weapons = weapons;
    }

    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.LogWarning($"Cannot execute ChargeAction due to failed CanExecute check: {canExecuteResult}");
            return canExecuteResult;
        }
        if (SourceMech.MechState == MechState.Charging)
        {
            // TODO: Check if the mech would turn more than a certain angle to face the target mech, and if so, cancel the charge action
            SetDestination(TargetMech.transform.position);
            SourceMech.MoveTo(Destination);
            SourceMech.UpdateMechState(MechState.Charging);
            Debug.Log($"{SourceMech.Mech.Name}: Executing ChargeAction towards {TargetMech.Mech.Name} at position {Destination}.");
            return ActionResult.Success;
        }
        else
        {
            ActionMarker actionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(Destination, SourceMech);
            if (actionMarker != null)
            {
                actionMarker.Initialize(SourceMech, TargetMech, _weapons);
                SourceMech.InsertActionMarkerAt(actionMarker, 0); // Insert the action marker at the front of the queue
                SourceMech.UpdateMechState(MechState.Charging);
                Debug.Log($"{SourceMech.Mech.Name}: ChargeAction initialized. ActionMarker created for charging towards {TargetMech.Mech.Name} at position {Destination}.");
                Debug.Log($"{SourceMech.Mech.Name}: ChargeAction executed. Mech state updated to Charging. ActionMarker ID: {actionMarker.Id}.");
                return ActionResult.Success;
            }
            else
            {
                Debug.LogError($"{SourceMech.Mech.Name}: Failed to create ActionMarker for ChargeAction.");
                return ActionResult.Failure;
            }
        }
    }

    public override ActionResult CanExecute()
    {
        if (SourceMech == null || TargetMech == null || _weapons == null)
        {
            Debug.LogWarning($"Cannot execute ChargeAction because the source mech, target mech, or weapons list is null.");
            return ActionResult.NullInput; 
        }

        if (SourceMech.MechState == MechState.Dead || TargetMech.MechState == MechState.Dead)
        {
            Debug.LogWarning($"Cannot execute ChargeAction because the source mech or target mech is dead.");
            return ActionResult.Failure;
        }

        return ActionResult.Success;
    }
}
