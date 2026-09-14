using UnityEngine;
using System;
using HeartOfIron.Enums;

public class ReloadAction : AttackAction
{
    public ReloadAction(MechController sourceMech, Weapon weapon) : base(ActionType.Reload, sourceMech, null, weapon)
    {
    }

    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.LogWarning("ReloadAction cannot be executed because CanExecute() returned false.");
            return false;
        }

        Debug.Log($"{SourceMech.Mech.Name} is reloading {Weapon.Name}.");
        Weapon.Reload();
        return true;
    }

    public override bool CanExecute()
    {
        if (SourceMech == null || Weapon == null)
        {
            Debug.LogWarning("ReloadAction cannot be executed because SourceMech or Weapon is null.");
            return false;
        }
        if (!Weapon.CanReload())
        {
            Debug.LogWarning($"{SourceMech.Mech.Name} cannot reload {Weapon.Name} because it is on cooldown.");
            return false;
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if reload action can be executed.");
        return true;
    }
}
