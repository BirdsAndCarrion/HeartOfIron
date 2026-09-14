using UnityEngine;
using HeartOfIron.Enums;
using System.Collections.Generic;

public class MeleeAction : AttackAction
{
    public MeleeAction(MechController sourceMech, MechController targetMech, List<Weapon> weapons) : base(ActionType.Melee, sourceMech, targetMech, weapons)
    {
    }

    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.LogWarning($"MeleeAction cannot be executed because CanExecute() returned: {canExecuteResult}");
            if (canExecuteResult == ActionResult.NullInput)
                return ActionResult.Failure;
            return ActionResult.Retry; // Retry if melee action cannot be executed for other reasons
        }
        List<ActionResult> results = new List<ActionResult>();
        foreach (Weapon w in Weapons)
        {
            Debug.Log($"{SourceMech.Mech.Name} is performing a melee attack on {TargetMech.Mech.Name} with {w.Name}.");
            // Implement the melee attack logic here (e.g., apply damage to the target mech)
            if (Vector3.Distance(SourceMech.transform.position, TargetMech.transform.position) <= w.MeleeRange && w.CanAttack(w.MeleeRange) 
                && UnityEngine.Random.value <= w.MeleeAccuracy + SourceMech.GetBonus(ActionType.Melee))
            {
                if (w == SourceMech.Mech.Inventory.Weapons[(int)Hand.Right] && w.SlotsRequired == 2)
                {
                    SourceMech.Animator.SetTrigger("MeleeTwo");
                } else if (w == SourceMech.Mech.Inventory.Weapons[(int)Hand.Right])
                {
                    SourceMech.Animator.SetTrigger("MeleeRight");
                } else
                {
                    SourceMech.Animator.SetTrigger("MeleeLeft");
                }
                w.SwingCooldown(); // Set the cooldown time for the melee attack
                Debug.Log($"{SourceMech.Mech.Name} hit {TargetMech.Mech.Name} with a melee attack.");
                TargetMech.TakeDamage(w.MeleeDamage, 0); // TODO: Placeholder until location based damage is figured out for melee combat
                results.Add(ActionResult.Success);
            }
            else
            {
                if (Vector3.Distance(SourceMech.transform.position, TargetMech.transform.position) > w.MeleeRange)
                {
                    Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} because the target is out of melee range.");
                }
                else if (!w.CanAttack(w.MeleeRange))
                {
                    Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} because the weapon is on cooldown or out of ammo.");
                }
                else {
                    w.SwingCooldown(); // Set the cooldown time for the melee attack
                    Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} due to unknown reasons.");
                }
                results.Add(ActionResult.Failure);
            }
        }
        // Return the overall result based on individual weapon results
        if (results.Contains(ActionResult.NullInput))
        {
            return ActionResult.Failure;
        } else if (results.Contains(ActionResult.Success))
        {
            return ActionResult.Success;
        } else if (results.Contains(ActionResult.Retry))
        {
            return ActionResult.Retry;
        }
        return ActionResult.Failure;
    }

    public override ActionResult CanExecute()
    {
        if (SourceMech == null || TargetMech == null || Weapons == null || Weapons.Count == 0)
        {
            Debug.LogWarning("MeleeAction cannot be executed because SourceMech, TargetMech, or Weapon is null.");
            return ActionResult.NullInput;
        }
        foreach (Weapon weapon in Weapons)
        {
            if (weapon == null)
            {
                Debug.LogWarning("MeleeAction cannot be executed because one of the weapons is null.");
                return ActionResult.NullInput;
            }
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if melee action can be executed.");
        return ActionResult.Success;
    }
}
