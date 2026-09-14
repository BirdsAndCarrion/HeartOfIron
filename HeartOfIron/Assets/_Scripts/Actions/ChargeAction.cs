using UnityEngine;
using System;
using HeartOfIron.Enums;

public class ChargeAction : MovementAction
{

    private Weapon _weapon;

    public ChargeAction(MechController sourceMech, MechController targetMech, Weapon weapon) : base(ActionType.Charge, sourceMech, targetMech != null ? targetMech.transform.position : Vector3.zero, targetMech)
    {
        _weapon = weapon;
    }

    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.LogWarning($"Cannot execute ChargeAction due to failed CanExecute check.");
            return false;
        }
        if (SourceMech.MechState == MechState.Charging)
        {
            // TODO: Check if the mech would turn more than a certain angle to face the target mech, and if so, cancel the charge action
            SetDestination(TargetMech.transform.position);
            SourceMech.MoveTo(Destination);
            SourceMech.UpdateMechState(MechState.Charging);
            Debug.Log($"{SourceMech.Mech.Name}: Executing ChargeAction towards {TargetMech.Mech.Name} at position {Destination}.");
            return true;
        }
        else
        {
            ActionMarker actionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(Destination, SourceMech);
            if (actionMarker != null)
            {
                actionMarker.Initialize(SourceMech, TargetMech, _weapon);
                SourceMech.InsertActionMarkerAt(actionMarker, 0); // Insert the action marker at the front of the queue
                SourceMech.UpdateMechState(MechState.Charging);
                Debug.Log($"{SourceMech.Mech.Name}: ChargeAction initialized. ActionMarker created for charging towards {TargetMech.Mech.Name} at position {Destination}.");
                Debug.Log($"{SourceMech.Mech.Name}: ChargeAction executed. Mech state updated to Charging. ActionMarker ID: {actionMarker.Id}.");
                return true;
            }
            else
            {
                Debug.LogError($"{SourceMech.Mech.Name}: Failed to create ActionMarker for ChargeAction.");
                return false;
            }
        }
    }

    public override bool CanExecute()
    {
        if (SourceMech == null ||SourceMech.MechState == MechState.Dead)
        {
            Debug.LogWarning($"Cannot execute ChargeAction because the mech is null or dead.");
            return false;
        }

        if (TargetMech == null || TargetMech.MechState == MechState.Dead)
        {
            Debug.LogWarning($"Cannot execute ChargeAction because the target mech is null or dead.");
            return false;
        }

        if (_weapon == null)
        {
            Debug.LogWarning($"Cannot execute ChargeAction because the weapon is null.");
            return false;
        }

        return true;
    }
}
