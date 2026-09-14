using UnityEngine;
using System;
using HeartOfIron.Enums;

public class ShootAction : AttackAction
{
    
    private float SPREAD_MULTIPLIER = 0.1f; // Adjust this value to control the spread of the shot

    public ShootAction(MechController sourceMech, MechController targetMech, Weapon weapon) : base(ActionType.Shoot, sourceMech, targetMech, weapon)
    {
    }

    public override bool Execute()
    {
        if (!CanExecute())
        {
            Debug.LogWarning("ShootAction cannot be executed because CanExecute() returned false.");
            return false;
        }

        Debug.Log($"{SourceMech.Mech.Name} is shooting at {TargetMech.Mech.Name} with {Weapon.Name}.");
        // Implement the shooting logic here (e.g., apply damage to the target mech)
        Vector3 direction = (TargetMech.transform.position - SourceMech.transform.position).normalized;
        // Calculate the distance between the source and target mechs
        float distance = Vector3.Distance(SourceMech.transform.position, TargetMech.transform.position);
        // Calculate the spread based on the distance and weapon's accuracy
        float spread = Weapon.CalculateSpread(distance) * SPREAD_MULTIPLIER;
        spread = Mathf.Max(spread - SourceMech.GetBonus(ActionType.Shoot), 0f); // Reduce spread based on the source mech's shooting bonus, ensuring it doesn't go below 0
        // Calculate the perpendicular vector to the direction of the shot
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
        // Apply the spread to the direction of the shot
        Vector3 spreadDirection = direction + perpendicular * UnityEngine.Random.Range(-spread, spread);
        spreadDirection.Normalize();
        // Perform the raycast to simulate the shot
        if (Weapon.CanAttack(distance))
        {
            ShootProjectile shootProjectile = GetShootProjectile();
            if (shootProjectile != null)
            {
                shootProjectile.Shoot(spreadDirection, Weapon.Damage, 1000f); // Assuming a max range of 1000 units for the projectile
                Weapon.ConsumeAmmo(1); // Consume 1 ammo for the shot
                Debug.Log($"{SourceMech.Mech.Name} shot at {TargetMech.Mech.Name} with {Weapon.Name}. Spread: {spread}, Direction: {spreadDirection}");
            }
            else
            {
                Debug.LogError($"ShootProjectile component not found for weapon {Weapon.Name} on {SourceMech.Mech.Name}.");
            }
        }
        else
        {
            Debug.LogWarning($"{SourceMech.Mech.Name} cannot shoot because the weapon cannot attack or the ShootProjectile component is missing.");
        }
        return true;
    }

    private ShootProjectile GetShootProjectile()
    {
        ShootProjectile[] shootProjectiles = SourceMech.ShootProjectiles;
        foreach (var shootProjectile in shootProjectiles)
        {
            if (shootProjectile != null && shootProjectile.gameObject.name == Weapon.Name)
            {
                return shootProjectile;
            }
        }
        return null;
    }

    public override bool CanExecute()
    {
        if (SourceMech == null || TargetMech == null || Weapon == null)
        {
            Debug.LogWarning("ShootAction cannot be executed because SourceMech, TargetMech, or Weapon is null.");
            return false;
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if shoot action can be executed.");
        return true;
    }
}
