using UnityEngine;
using System;
using System.Collections.Generic;

public class ActionMarker : MonoBehaviour, ISelectable
{
    private Guid _id;
    public Guid Id => _id;
    private ActionPair _actionPair;
    public ActionPair ActionPair => _actionPair;

    public MechController SourceMech => _actionPair.MovementAction.SourceMech;

    public void Awake()
    {
        EventRouter.RegisterWithHub(this);
    }

    public void OnDestroy()
    {
        EventRouter.UnregisterFromHub(this);
    }

    public void Initialize(Vector3 position, MechController sourceMech)
    {
        _id = Guid.NewGuid();
        MoveAction moveAction = new MoveAction(sourceMech, position);
        _actionPair = new ActionPair(moveAction);
    }

    public void Initialize(MechController sourceMech, MechController targetMech, List<MeleeAttack> attacks)
    {
        _id = Guid.NewGuid();
        ChargeAction chargeAction = new ChargeAction(sourceMech, targetMech, attacks);
        MeleeAction meleeAction = new MeleeAction(sourceMech, targetMech, attacks);
        _actionPair = new ActionPair(chargeAction, meleeAction);
    }

    public void SetSecondaryAction(Action secondaryAction)
    {
        _actionPair.SetSecondaryAction(secondaryAction);
    }

    public string OnSelected()
    {
        EventRouter.OnActionMarkerSelected.Invoke(this);
        Debug.Log($"ActionMarker selected: {_id}, Source Mech: {_actionPair.MovementAction.SourceMech.Mech.Name}");
        return $"ActionMarker ID: {_id}, Source Mech: {_actionPair.MovementAction.SourceMech.Mech.Name}";
    }
}
