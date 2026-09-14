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

    public Action(ActionType actionType, MechController sourceMech, MechController targetMech = null)
    {
        _id = Guid.NewGuid();
        _actionType = actionType;
        _sourceMech = sourceMech;
        _targetMech = targetMech;
    }

    public abstract ActionResult Execute();

    public abstract ActionResult CanExecute();
}
