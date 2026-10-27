using UnityEngine;
using HeartOfIron.Enums;

/// <summary>
/// Represents a pair of actions for a mech, consisting of a mandatory movement action and an optional secondary action.
/// Provides methods to manage and validate the secondary action.
/// Ensures that the mech moves to the intended position before executing any secondary action.
/// </summary>
public class ActionPair
{
    /// <summary>
    /// The mandatory movement action for the mech.
    /// </summary>
    private MovementAction _movementAction;
    public MovementAction MovementAction => _movementAction;
    /// <summary>
    /// The optional secondary action for the mech.
    /// </summary>
    private Action _secondaryAction;
    public Action SecondaryAction => _secondaryAction;

    /// <summary>
    /// Initializes a new instance of the ActionPair class with a movement action and a secondary action.
    /// </summary>
    /// <param name="movementAction">The mandatory movement action.</param>
    /// <param name="secondaryAction">The optional secondary action.</param>
    public ActionPair(MovementAction movementAction, Action secondaryAction)
    {
        this._movementAction = movementAction;
        this._secondaryAction = secondaryAction;
    }

    /// <summary>
    /// Initializes a new instance of the ActionPair class with only a movement action.
    /// </summary>
    /// <param name="movementAction">The mandatory movement action.</param>
    public ActionPair(MovementAction movementAction)
    {
        this._movementAction = movementAction;
        this._secondaryAction = null;
    }

    /// <summary>
    /// Sets the secondary action for the action pair.
    /// </summary>
    /// <param name="secondaryAction">The secondary action to be set.</param>
    /// <remarks>
    /// The secondary action cannot be null.
    /// If the secondary action is of type Move, the secondary action will be cleared and set to null.
    /// </remarks>
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
