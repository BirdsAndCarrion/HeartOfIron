using UnityEngine;
using System;
using HeartOfIron.Enums;

public abstract class Action
{
    private Guid _id;
    public Guid Id => _id;
    private ActionType _actionType;
    public ActionType ActionType => _actionType;
    private MechController _sourceMech;
    public MechController SourceMech => _sourceMech;
    private MechController _targetMech;
    public MechController TargetMech => _targetMech;

    public Action(ActionType actionType, MechController sourcheMech, MechController targetMech = null)
    {
        this._id = Guid.NewGuid();
        this._actionType = actionType;
        this._sourceMech = sourcheMech;
        this._targetMech = targetMech;
    }

    public abstract bool Execute();

    public abstract bool CanExecute();
}
