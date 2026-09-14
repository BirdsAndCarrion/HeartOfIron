using UnityEngine;
using System;
using HeartOfIron.Enums;

public class MeleeAction : AttackAction
{
    public MeleeAction(MechController sourceMech, MechController targetMech, Weapon weapon) : base(ActionType.Melee, sourceMech, targetMech, weapon)
    {
    }

    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.LogWarning("MeleeAction cannot be executed because CanExecute() returned false.");
            return false;
        }

        Debug.Log($"{SourceMech.Mech.Name} is performing a melee attack on {TargetMech.Mech.Name} with {Weapon.Name}.");
        // Implement the melee attack logic here (e.g., apply damage to the target mech)
        if (Vector3.Distance(SourceMech.transform.position, TargetMech.transform.position) <= Weapon.MeleeRange &&Weapon.CanAttack(Weapon.MeleeRange) 
            && UnityEngine.Random.value <= Weapon.MeleeAccuracy + SourceMech.GetBonus(ActionType.Melee))
        {
            Weapon.SwingCooldown(); // Set the cooldown time for the melee attack
            Debug.Log($"{SourceMech.Mech.Name} hit {TargetMech.Mech.Name} with a melee attack.");
            TargetMech.TakeDamage(Weapon.MeleeDamage, 0); // Placeholder until location based damage is figured out for melee combat
            return true;
        }
        else
        {
            if (Vector3.Distance(SourceMech.transform.position, TargetMech.transform.position) > Weapon.MeleeRange)
            {
                Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} because the target is out of melee range.");
            }
            else if (!Weapon.CanAttack(Weapon.MeleeRange))
            {
                Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} because the weapon is on cooldown or out of ammo.");
            }
            else {
                Weapon.SwingCooldown(); // Set the cooldown time for the melee attack
                Debug.LogWarning($"{SourceMech.Mech.Name} cannot perform melee attack on {TargetMech.Mech.Name} due to unknown reasons.");
            }
            return false;
        }
    }

    public override bool CanExecute()
    {
        if (SourceMech == null || TargetMech == null || Weapon == null)
        {
            Debug.LogWarning("MeleeAction cannot be executed because SourceMech, TargetMech, or Weapon is null.");
            return false;
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if melee action can be executed.");
        return true;
    }
}
