using UnityEngine;
using HeartOfIron.Enums;
using System.Collections.Generic;

/// <summary>
/// Represents a melee attack action performed by a mech, which involves executing a series of melee attacks against a target mech.
/// This class handles the execution and validation of melee attacks, ensuring that the attacks are within range, can be performed, and applying damage to the target mech.
/// </summary>
/// <remarks>
/// The MeleeAction class extends the AttackAction class and specifically deals with melee combat.
/// It validates the melee attacks, checks for range and cooldowns, and applies damage to the target mech.
/// </remarks>
public class MeleeAction : AttackAction
{
    public MeleeAction(MechController sourceMech, MechController targetMech, List<MeleeAttack> attacks) : base(ActionType.Melee, sourceMech, targetMech, attacks)
    {
    }

    /// <inheritdoc/>
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
        List<MeleeAttack> meleeAttacks = new List<MeleeAttack>();
        foreach (Attack a in Attacks)
        {
            if (a is MeleeAttack meleeAttack)
            {
                meleeAttacks.Add(meleeAttack);
            }
        }
        List<ActionResult> results = new List<ActionResult>();
        foreach (MeleeAttack m in meleeAttacks)
        {
            Debug.Log($"{SourceMech.Mech.Name} is performing a melee attack on {TargetMech.Mech.Name} with {m.Weapon.Name}.");
            // Implement the melee attack logic here (e.g., apply damage to the target mech)
            if (Vector3.Distance(SourceMech.transform.position, TargetMech.transform.position) <= m.Range && m.CanAttack(m.Range) 
                && UnityEngine.Random.value <= m.Accuracy + SourceMech.GetBonus(ActionType.Melee))
            {
                if (m.Weapon.Name == SourceMech.Mech.Inventory.Weapons[(int)Hand.Right].Name && m.HandsRequired == 2)
                {
                    SourceMech.Animator.SetTrigger("MeleeTwo");
                } else if (m.Weapon.Name == SourceMech.Mech.Inventory.Weapons[(int)Hand.Right].Name)
                {
                    SourceMech.Animator.SetTrigger("MeleeRight");
                } else
                {
                    SourceMech.Animator.SetTrigger("MeleeLeft");
                }
                m.SetTimer(m.Cooldown); // Set the cooldown time for the melee attack
                Debug.Log($"{SourceMech.Mech.Name} hit {TargetMech.Mech.Name} with a melee attack.");
                TargetMech.TakeDamage(m.Damage, 0); // TODO: Placeholder until location based damage is figured out for melee combat
                results.Add(ActionResult.Success);
            }
            else
            {
                if (Vector3.Distance(SourceMech.transform.position, TargetMech.transform.position) > m.Range)
                {
                    Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} because the target is out of melee range of {m.Weapon.Name}.");
                }
                else if (!m.CanAttack(m.Range))
                {
                    Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} because the weapon {m.Weapon.Name} is on cooldown or out of ammo.");
                }
                else {
                    SourceMech.Mech.Inventory.SetTimer(m.Weapon, m.Cooldown); // Set the cooldown time for the melee attack
                    Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} with {m.Weapon.Name} due to unknown reasons.");
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

    /// <inheritdoc/>
    public override ActionResult CanExecute()
    {
        if (SourceMech == null || TargetMech == null || Attacks == null || Attacks.Count == 0)
        {
            Debug.LogWarning("MeleeAction cannot be executed because SourceMech, TargetMech, or Attacks is null.");
            return ActionResult.NullInput;
        }
        foreach (Attack attack in Attacks)
        {
            if (attack == null)
            {
                Debug.LogWarning("MeleeAction cannot be executed because one of the attacks is null.");
                return ActionResult.NullInput;
            }
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if melee action can be executed.");
        return ActionResult.Success;
    }
}
