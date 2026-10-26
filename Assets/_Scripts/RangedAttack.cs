using UnityEngine;

public class RangedAttack : Attack
{
    float pointBlankRange;
    public float PointBlankRange => pointBlankRange;
    float pointBlankAccuracy;
    public float PointBlankAccuracy => pointBlankAccuracy;
    float accuracyDropoff;
    public float AccuracyDropoff => accuracyDropoff;
    int numShots;
    public int NumShots => numShots;
    int currentAmmo;
    public int CurrentAmmo => currentAmmo;
    int maxAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsOutOfAmmo => currentAmmo <= 0;
    

    public RangedAttack(Weapon weapon, int damage, float cooldown, float pointBlankRange, float pointBlankAccuracy, float accuracyDropoff, int numShots, int maxAmmo)
        : base(weapon, damage, cooldown)
    {
        this.pointBlankRange = pointBlankRange;
        this.pointBlankAccuracy = pointBlankAccuracy;
        this.accuracyDropoff = accuracyDropoff;
        this.numShots = numShots;
        this.currentAmmo = maxAmmo; // Initialize current ammo to max shots
        this.maxAmmo = maxAmmo; // Initialize max ammo to max shots
    }

    public float CalculateSpread(float distance)
    {
        if (distance <= 0)
        {
            Debug.LogWarning("Distance must be greater than zero to calculate accuracy.");
            return 0f;
        }

        float spread = 1f - PointBlankAccuracy + (AccuracyDropoff * (distance - PointBlankRange));
        return Mathf.Clamp(spread, 0f, 1f);
    }

    public override float GetAverageDamage(float distance)
    {
        float spread = CalculateSpread(distance);
        float hitChance = 1f - spread; // Assuming hit chance is inversely proportional to spread
        float averageDamage = Damage * hitChance * NumShots;
        return averageDamage;
    }

    public void ConsumeAmmo(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Amount to consume must be greater than zero.");
            return;
        }

        currentAmmo -= amount;
        if (currentAmmo < 0)
        {
            currentAmmo = 0; // Ensure current ammo doesn't go below zero
        }
        Debug.Log($"{Weapon.Name} consumed {amount} ammo. Current ammo: {currentAmmo}/{maxAmmo}");
    }
    
    public void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log($"{Weapon.Name} reloaded. Current ammo: {currentAmmo}/{maxAmmo}");
    }

    public override bool CanAttack(float distance)
    {
        if (IsOutOfAmmo)
        {
            Debug.LogWarning($"{Weapon.Name} cannot attack because it is out of ammo.");
            return false;
        } else if (distance < PointBlankRange)
        {
            Debug.LogWarning($"{Weapon.Name} cannot attack because the target is within the point-blank range of {PointBlankRange}.");
            return false;
        } else if (!IsReady())
        {
            Debug.LogWarning($"{Weapon.Name} cannot attack because it is on cooldown.");
            return false;
        }
        return true;
    }
}
