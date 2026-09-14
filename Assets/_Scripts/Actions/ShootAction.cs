using UnityEngine;
using HeartOfIron.Enums;
using System.Collections.Generic;

public class ShootAction : AttackAction
{
    
    private readonly float SPREAD_MULTIPLIER = 0.1f; // Adjust this value to control the spread of the shot
    private readonly float MAX_FIRING_ANGLE = 60f; // Maximum firing angle for the weapon's arc
    private readonly float MAX_WEAPON_RANGE = 1000f; // Maximum range for the weapon's projectile

    public ShootAction(MechController sourceMech, MechController targetMech, List<Weapon> weapons) : base(ActionType.Shoot, sourceMech, targetMech, weapons)
    {
    }

    public override ActionResult Execute()
    {
        ActionResult canExecuteResult = CanExecute();
        if (canExecuteResult != ActionResult.Success)
        {
            Debug.LogWarning($"ShootAction cannot be executed because CanExecute() returned: {canExecuteResult}");
            if (canExecuteResult == ActionResult.NullInput)
                return ActionResult.Failure;
            return ActionResult.Retry; // Outside of firing cone, retry after rotating mech
        }
        List<ActionResult> results = new List<ActionResult>();
        foreach (Weapon w in Weapons)
        {
            Debug.Log($"{SourceMech.Mech.Name} is shooting at {TargetMech.Mech.Name} with {w.Name}.");
            Transform target = GetTargetLimb();
            if (target == null)
            {
                Debug.LogWarning("No viable target limb found.");
                return ActionResult.Failure;
            }
            SourceMech.Target.transform.position = target.position;
            Vector3 direction = (target.position - SourceMech.transform.position).normalized;
            // Calculate the distance between the source and target mechs
            float distance = Vector3.Distance(SourceMech.transform.position, target.position);
            // Check that the weapon can attack at the calculated distance
            if (w.CanAttack(distance))
            {
                ShootProjectile shootProjectile = GetShootProjectile(w);
                if (shootProjectile != null)
                {
                    for (int i = 0; i < w.NumShots; i++)
                    {
                        Vector3 spreadDirection = GetSpreadDirection(direction, distance, w);
                        shootProjectile.Shoot(spreadDirection, w.Damage, MAX_WEAPON_RANGE);
                        Debug.Log($"{SourceMech.Mech.Name} shot at {TargetMech.Mech.Name} with {w.Name}. Direction: {spreadDirection}");
                    }
                    w.ConsumeAmmo(1); // Consume 1 ammo for the shot
                    return ActionResult.Success;
                }
                else
                {
                    Debug.LogError($"ShootProjectile component not found for weapon {w.Name} on {SourceMech.Mech.Name}.");
                    return ActionResult.Failure;
                }
            }
            else
            {
                Debug.LogWarning($"{SourceMech.Mech.Name} cannot shoot because the weapon cannot attack or the ShootProjectile component is missing.");
                return ActionResult.Failure;
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

    private ShootProjectile GetShootProjectile(Weapon weapon)
    {
        ShootProjectile[] shootProjectiles = SourceMech.ShootProjectiles;
        foreach (var shootProjectile in shootProjectiles)
        {
            if (shootProjectile != null && shootProjectile.gameObject.name == weapon.Name)
            {
                return shootProjectile;
            }
        }
        return null;
    }

    private Vector3 GetSpreadDirection(Vector3 direction, float distance, Weapon weapon)
    {
        float spread = weapon.CalculateSpread(distance) * SPREAD_MULTIPLIER;
        spread = Mathf.Max(spread - SourceMech.GetBonus(ActionType.Shoot), 0f);
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 spreadDirection = direction + perpendicular * UnityEngine.Random.Range(-spread, spread);
        spreadDirection.Normalize();
        return spreadDirection;
    }

    private Transform GetTargetLimb()
    {
        List<Transform> targetLimbs = TargetMech.Limbs;
        Transform viableTargetLimb = null;
        do
        {
            RaycastHit hit;
            Transform targetLimb = targetLimbs[0];
            if (Physics.Raycast(SourceMech.transform.position, (targetLimb.position - SourceMech.transform.position).normalized, out hit))
            {
                if (hit.transform == targetLimb)
                {
                    viableTargetLimb = targetLimb;
                }
            }
            targetLimbs.RemoveAt(0);
        } while (viableTargetLimb == null && targetLimbs.Count > 0);
        return viableTargetLimb;
    }

    public override ActionResult CanExecute()
    {
        if (SourceMech == null || TargetMech == null || Weapons == null || Weapons.Count == 0)
        {
            Debug.LogWarning("ShootAction cannot be executed because SourceMech, TargetMech, or Weapons is null.");
            return ActionResult.NullInput;
        }
        foreach (Weapon w in Weapons)
        {
            if (w == null)
            {
                Debug.LogWarning("ShootAction cannot be executed because one of the weapons is null.");
                return ActionResult.NullInput;
            }
        }
        VisibilityQuery visibilityQuery = new VisibilityQuery(TargetMech);
        EventRouter.RegisterWithHub(visibilityQuery);
        EventRouter.OnVisibilityChecked.Invoke(visibilityQuery);
        EventRouter.UnregisterFromHub(visibilityQuery);
        if (!visibilityQuery.isVisible)
        {
            Debug.LogWarning($"{SourceMech.Mech.Name} cannot shoot at {TargetMech.Mech.Name} because the target is not visible.");
            return ActionResult.Failure;
        }
        // Check to make sure angle is within the weapon's firing arc
        Vector3 directionToTarget = (TargetMech.transform.position - SourceMech.transform.position).normalized;
        float angleToTarget = Vector3.Angle(SourceMech.transform.forward, directionToTarget);
        if (angleToTarget > MAX_FIRING_ANGLE)
        {
            Debug.LogWarning($"{SourceMech.Mech.Name} cannot shoot at {TargetMech.Mech.Name} because the target is outside the weapon's firing arc.");
            return ActionResult.Retry;
        }
        Debug.Log($"{SourceMech.Mech.Name}: Checking if shoot action can be executed.");
        return ActionResult.Success;
    }
}
