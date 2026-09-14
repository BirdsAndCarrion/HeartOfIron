using UnityEngine;
using HeartOfIron.Enums;
using UnityEngine.AI;

public class MoveAction : MovementAction
{
    public MoveAction(MechController sourceMech, Vector3 destination) : base(ActionType.Move, sourceMech, destination)
    {
    }

    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.LogWarning($"Cannot execute MoveAction. Conditions not met: {canExecuteResult}");
            return canExecuteResult;
        }

        SourceMech.MoveTo(Destination);
        Debug.Log($"{SourceMech.Mech.Name}: Executing MoveAction to {Destination}.");
        return ActionResult.Success;
    }

    public override ActionResult CanExecute()
    {
        if (SourceMech == null)
        {
            Debug.LogWarning("MoveAction cannot be executed because the mech is null.");
            return ActionResult.NullInput;
        } else if (SourceMech.MechState == MechState.Dead)
        {
            Debug.LogWarning("MoveAction cannot be executed because the mech is dead.");
            return ActionResult.Failure;
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if MoveAction can be executed.");
        NavMeshPath path = new NavMeshPath();
        bool pathExists = NavMesh.CalculatePath(SourceMech.transform.position, Destination, NavMesh.AllAreas, path);
        if (!pathExists || path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log($"Cannot execute MoveAction. No valid path to destination {Destination}.");
            return ActionResult.Failure;
        }

        return ActionResult.Success;
    }
}
