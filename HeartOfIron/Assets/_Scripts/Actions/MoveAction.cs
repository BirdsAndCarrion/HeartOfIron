using UnityEngine;
using System;
using HeartOfIron.Enums;
using UnityEngine.AI;

public class MoveAction : MovementAction
{
    public MoveAction(MechController sourceMech, Vector3 destination) : base(ActionType.Move, sourceMech, destination)
    {
    }

    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.LogWarning($"Cannot execute MoveAction. Conditions not met.");
            return false;
        }

        SourceMech.MoveTo(Destination);
        Debug.Log($"{SourceMech.Mech.Name}: Executing MoveAction to {Destination}.");
        return true;
    }

    public override bool CanExecute()
    {
        if (SourceMech == null || SourceMech.MechState == MechState.Dead)
        {
            Debug.LogWarning("MoveAction cannot be executed because the mech is null or dead.");
            return false;
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if MoveAction can be executed.");
        NavMeshPath path = new NavMeshPath();
        bool pathExists = NavMesh.CalculatePath(SourceMech.transform.position, Destination, NavMesh.AllAreas, path);
        if (!pathExists || path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log($"Cannot execute MoveAction. No valid path to destination {Destination}.");
            return false;
        }

        return true;
    }
}
