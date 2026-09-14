using UnityEngine;
using HeartOfIron.Enums;
using System.Collections.Generic;

public class ReloadAction : AttackAction
{
    public ReloadAction(MechController sourceMech, List<Weapon> weapons) : base(ActionType.Reload, sourceMech, null, weapons)
    {
    }

    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.LogWarning($"ReloadAction cannot be executed because CanExecute() returned: {canExecuteResult}");
            return canExecuteResult;
        }
        List<ActionResult> results = new List<ActionResult>();
        foreach (Weapon w in Weapons)
        {
            if (!w.CanReload())
            {
                Debug.LogWarning($"{SourceMech.Mech.Name} cannot reload {w.Name} because it is on cooldown.");
                results.Add(ActionResult.Retry);
                continue;
            }
            Debug.Log($"{SourceMech.Mech.Name} is reloading {w.Name}.");
            w.Reload();
            results.Add(ActionResult.Success);
        }
        if (results.Contains(ActionResult.Success))
        {
            return ActionResult.Success;
        }
        return ActionResult.Retry;
    }

    public override ActionResult CanExecute()
    {
        if (SourceMech == null || Weapons == null || Weapons.Count == 0)
        {
            Debug.LogWarning("ReloadAction cannot be executed because SourceMech or Weapons is null or empty.");
            return ActionResult.NullInput;
        }
        foreach (Weapon w in Weapons)
        {
            if (w == null)
            {
                return ActionResult.NullInput;
            }
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if reload action can be executed.");
        return ActionResult.Success;
    }
}
