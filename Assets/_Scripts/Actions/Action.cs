using System;
using HeartOfIron.Enums;

/// <summary>
/// Represents the most general form of an action that can be performed by a mech.
/// </summary>
public abstract class Action
{
    private Guid _id;
    public Guid Id => _id;
    /// <summary>
    /// The type of action being performed.
    /// </summary>
    private ActionType _actionType;
    public ActionType ActionType => _actionType;
    /// <summary>
    /// The mech that is performing the action.
    /// </summary>
    private MechController _sourceMech;
    public MechController SourceMech => _sourceMech;
    /// <summary>
    /// The mech that is the target of the action.
    /// Can be null if the action does not target a specific mech.
    /// </summary>
    private MechController _targetMech;
    public MechController TargetMech => _targetMech;

    public Action(ActionType actionType, MechController sourceMech, MechController targetMech = null)
    {
        _id = Guid.NewGuid();
        _actionType = actionType;
        _sourceMech = sourceMech;
        _targetMech = targetMech;
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <returns>An ActionResult indicating the outcome of the action execution. 
    /// Any result other than success should indicate why the action failed.</returns>
    public abstract ActionResult Execute();

    /// <summary>
    /// Checks if the action can be executed.
    /// </summary>
    /// <returns>An ActionResult indicating whether the action can be executed. 
    /// Any result other than success should indicate why the action cannot be executed.</returns>
    public abstract ActionResult CanExecute();
}
