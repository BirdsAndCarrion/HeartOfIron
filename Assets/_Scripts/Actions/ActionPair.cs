using UnityEngine;
using HeartOfIron.Enums;

public class ActionPair
{
    private MovementAction _movementAction;
    public MovementAction MovementAction => _movementAction;
    private Action _secondaryAction;
    public Action SecondaryAction => _secondaryAction;

    public ActionPair(MovementAction movementAction, Action secondaryAction)
    {
        this._movementAction = movementAction;
        this._secondaryAction = secondaryAction;
    }

    public ActionPair(MovementAction movementAction)
    {
        this._movementAction = movementAction;
        this._secondaryAction = null;
    }

    public void SetSecondaryAction(Action secondaryAction)
    {
        if (secondaryAction == null)
        {
            Debug.LogWarning("Secondary action cannot be null. Please provide a valid action.");
            return;
        } else if (secondaryAction.ActionType == ActionType.Move)
        {
            Debug.LogWarning("Secondary action cannot be of type Move. Please provide a valid action.");
            _secondaryAction = null;
            return;
        }
        this._secondaryAction = secondaryAction;
    }
}
