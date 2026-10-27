using UnityEngine;
using HeartOfIron.Enums;
using System.Collections.Generic;

/// <summary>
/// Represents a reload action performed by a mech, which involves reloading ranged attacks.
/// This class handles the execution and validation of reload actions, ensuring that the ranged attacks can be reloaded and applying the reload logic.
/// </summary>
public class ReloadAction : AttackAction
{
    public ReloadAction(MechController sourceMech, List<RangedAttack> attacks) : base(ActionType.Reload, sourceMech, null, attacks)
    {
    }

    /// <inheritdoc/>
    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.LogWarning($"ReloadAction cannot be executed because CanExecute() returned: {canExecuteResult}");
            return canExecuteResult;
        }
        List<RangedAttack> rangedAttacks = new List<RangedAttack>();
        foreach (Attack a in Attacks)
        {
            if (a is RangedAttack rangedAttack)
            {
                rangedAttacks.Add(rangedAttack);
            }
        }
        List<ActionResult> results = new List<ActionResult>();
        foreach (RangedAttack r in rangedAttacks)
        {
            if (!r.IsReady())
            {
                Debug.LogWarning($"{SourceMech.Mech.Name} cannot reload {r.Weapon.Name} because it is on cooldown.");
                results.Add(ActionResult.Retry);
                continue;
            }
            Debug.Log($"{SourceMech.Mech.Name} is reloading {r.Weapon.Name}.");
            r.Reload();
            results.Add(ActionResult.Success);
        }
        if (results.Contains(ActionResult.Success))
        {
            return ActionResult.Success;
        }
        return ActionResult.Retry;
    }

    /// <inheritdoc/>
    public override ActionResult CanExecute()
    {
        if (SourceMech == null || Attacks == null || Attacks.Count == 0)
        {
            Debug.LogWarning("ReloadAction cannot be executed because SourceMech or Attacks is null or empty.");
            return ActionResult.NullInput;
        }
        foreach (Attack a in Attacks)
        {
            if (a == null)
            {
                return ActionResult.NullInput;
            }
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if reload action can be executed.");
        return ActionResult.Success;
    }
}
