using UnityEngine;

public static class ItemAssembler
{
    public static Item AssembleItem(ItemBlueprint blueprint)
    {
        if (blueprint is ArmorBlueprint aB)
        {
            float adjustedResistance = AdjustResistance(aB.Resistance);
            float adjustedCoverage = AdjustCoverage(aB.Coverage);
            float adjustedDefenseRating = AdjustDefenseRating(aB.DefenseRating);
            float adjustedMaxDurability = AdjustMaxDurability(aB.MaxDurability);
            return new Armor(aB.ItemName, aB.Weight, aB.ArmorType, aB.Cost, adjustedResistance, adjustedCoverage, adjustedDefenseRating, adjustedMaxDurability);
        }
        else if (blueprint is EnergyShieldBlueprint eB)
        {
            float adjustedMaxCapacity = AdjustMaxCapacity(eB.MaxCapacity);
            float adjustedRechargeRate = AdjustRechargeRate(eB.RechargeRate);
            float adjustedRechargeDelay = AdjustRechargeDelay(eB.RechargeDelay);
            return new EnergyShield(eB.ItemName, eB.Weight, eB.Cost, adjustedMaxCapacity, adjustedRechargeRate, adjustedRechargeDelay);
        }
        else if (blueprint is WeaponBlueprint wB)
        {
            int adjustedDamage = AdjustDamage(wB.Damage);
            int adjustedMaxAmmo = AdjustMaxAmmo(wB.MaxAmmo);
            float adjustedPointBlankAccuracy = AdjustPointBlankAccuracy(wB.PointBlankAccuracy);
            float adjustedAccuracyDropoff = AdjustAccuracyDropoff(wB.AccuracyDropoff);
            int adjustedMeleeDamage = AdjustMeleeDamage(wB.MeleeDamage);
            float adjustedMeleeAccuracy = AdjustMeleeAccuracy(wB.MeleeAccuracy);
            float adjustedReloadTime = AdjustReloadTime(wB.ReloadTime);
            float adjustedFireRate = AdjustFireRate(wB.FireRate);
            float adjustedSwingRate = AdjustSwingRate(wB.SwingRate);

            return new Weapon(wB.ItemName, wB.Weight, wB.Cost, adjustedDamage, adjustedMeleeDamage, adjustedMaxAmmo, adjustedReloadTime, adjustedFireRate, adjustedSwingRate, wB.MeleeRange, wB.NumShots, wB.SlotsRequired, adjustedPointBlankAccuracy, adjustedAccuracyDropoff, adjustedMeleeAccuracy);
        } else if (blueprint is MechBlueprint mB)
        {
            UnitManager.Instance.AddMech(MechAssembler.AssembleNewMech(mB));
            return null; // Mech assembly does not return an Item, so we return null here.
        }
        else
        {
            Debug.LogError("Unknown blueprint type: " + blueprint.GetType());
            return null;
        }
    }

    /// <summary>
    /// Stat changers for Armor. These methods adjust the resistance, coverage, defense rating, and max durability of an armor based on random chance.
    /// </summary>

    private static float AdjustResistance(float baseResistance)
    {
        // 10% chance to increase resistance by 10% - 25%
        // 15% chance to increase resistance by 0% - 10%
        // 50% chance to keep resistance the same
        // 20% chance to decrease resistance by 0% - 10%
        // 5% chance to decrease resistance by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseResistance * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseResistance * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseResistance; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseResistance * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseResistance * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustCoverage(float baseCoverage)
    {
        // 10% chance to increase coverage by 10% - 25%
        // 15% chance to increase coverage by 0% - 10%
        // 50% chance to keep coverage the same
        // 20% chance to decrease coverage by 0% - 10%
        // 5% chance to decrease coverage by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseCoverage * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseCoverage * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseCoverage; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseCoverage * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseCoverage * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustDefenseRating(float baseDefenseRating)
    {
        // 10% chance to increase defense rating by 10% - 25%
        // 15% chance to increase defense rating by 0% - 10%
        // 50% chance to keep defense rating the same
        // 20% chance to decrease defense rating by 0% - 10%
        // 5% chance to decrease defense rating by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseDefenseRating * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseDefenseRating * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseDefenseRating; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseDefenseRating * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseDefenseRating * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustMaxDurability(float baseMaxDurability)
    {
        // 10% chance to increase max durability by 10% - 25%
        // 15% chance to increase max durability by 0% - 10%
        // 50% chance to keep max durability the same
        // 20% chance to decrease max durability by 0% - 10%
        // 5% chance to decrease max durability by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseMaxDurability * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseMaxDurability * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseMaxDurability; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseMaxDurability * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseMaxDurability * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    /// <summary>
    /// Stat changers for Energy Shields. These methods adjust the max capacity, recharge rate, and recharge delay of an energy shield based on random chance.
    /// </summary>
    
    private static float AdjustMaxCapacity(float baseMaxCapacity)
    {
        // 10% chance to increase max capacity by 10% - 25%
        // 15% chance to increase max capacity by 0% - 10%
        // 50% chance to keep max capacity the same
        // 20% chance to decrease max capacity by 0% - 10%
        // 5% chance to decrease max capacity by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseMaxCapacity * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseMaxCapacity * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseMaxCapacity; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseMaxCapacity * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseMaxCapacity * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustRechargeRate(float baseRechargeRate)
    {
        // 10% chance to increase recharge rate by 10% - 25%
        // 15% chance to increase recharge rate by 0% - 10%
        // 50% chance to keep recharge rate the same
        // 20% chance to decrease recharge rate by 0% - 10%
        // 5% chance to decrease recharge rate by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseRechargeRate * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseRechargeRate * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseRechargeRate; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseRechargeRate * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseRechargeRate * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustRechargeDelay(float baseRechargeDelay)
    {
        // 10% chance to decrease recharge delay by 10% - 25%
        // 15% chance to decrease recharge delay by 0% - 10%
        // 50% chance to keep recharge delay the same
        // 20% chance to increase recharge delay by 0% - 10%
        // 5% chance to increase recharge delay by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseRechargeDelay * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseRechargeDelay * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseRechargeDelay; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseRechargeDelay * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseRechargeDelay * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    /// <summary>
    /// Stat changers for Weapons. These methods adjust the damage, ammo capacity, reload time, fire rate, and accuracy of a weapon based on random chance.
    /// </summary>
    
    private static int AdjustDamage(int baseDamage)
    {
        // 10% chance to increase damage by 10% - 25%
        // 15% chance to increase damage by 0% - 10%
        // 50% chance to keep damage the same
        // 20% chance to decrease damage by 0% - 10%
        // 5% chance to decrease damage by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return Mathf.RoundToInt(baseDamage * (1 + UnityEngine.Random.Range(0.10f, 0.25f)));
        }
        else if (randomValue < 0.25f)
        {
            return Mathf.RoundToInt(baseDamage * (1 + UnityEngine.Random.Range(0f, 0.10f)));
        }
        else if (randomValue < 0.75f)
        {
            return baseDamage; // No change
        }
        else if (randomValue < 0.95f)
        {
            return Mathf.RoundToInt(baseDamage * (1 - UnityEngine.Random.Range(0f, 0.10f)));
        }
        else
        {
            return Mathf.RoundToInt(baseDamage * (1 - UnityEngine.Random.Range(0.10f, 0.25f)));
        }
    }

    private static int AdjustMaxAmmo(int baseMaxAmmo)
    {
        // 10% chance to increase max ammo by 10% - 25%
        // 15% chance to increase max ammo by 0% - 10%
        // 50% chance to keep max ammo the same
        // 20% chance to decrease max ammo by 0% - 10%
        // 5% chance to decrease max ammo by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return Mathf.RoundToInt(baseMaxAmmo * (1 + UnityEngine.Random.Range(0.10f, 0.25f)));
        }
        else if (randomValue < 0.25f)
        {
            return Mathf.RoundToInt(baseMaxAmmo * (1 + UnityEngine.Random.Range(0f, 0.10f)));
        }
        else if (randomValue < 0.75f)
        {
            return baseMaxAmmo; // No change
        }
        else if (randomValue < 0.95f)
        {
            return Mathf.RoundToInt(baseMaxAmmo * (1 - UnityEngine.Random.Range(0f, 0.10f)));
        }
        else
        {
            return Mathf.RoundToInt(baseMaxAmmo * (1 - UnityEngine.Random.Range(0.10f, 0.25f)));
        }
    }

    private static float AdjustPointBlankAccuracy(float basePointBlankAccuracy)
    {
        // 10% chance to increase zero range accuracy by 10% - 25%
        // 15% chance to increase zero range accuracy by 0% - 10%
        // 50% chance to keep zero range accuracy the same
        // 20% chance to decrease zero range accuracy by 0% - 10%
        // 5% chance to decrease zero range accuracy by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return basePointBlankAccuracy * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return basePointBlankAccuracy * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return basePointBlankAccuracy; // No change
        }
        else if (randomValue < 0.95f)
        {
            return basePointBlankAccuracy * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return basePointBlankAccuracy * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustAccuracyDropoff(float baseAccuracyDropoff)
    {
        // 10% chance to decrease accuracy decay by 10% - 25%
        // 15% chance to decrease accuracy decay by 0% - 10%
        // 50% chance to keep accuracy decay the same
        // 20% chance to increase accuracy decay by 0% - 10%
        // 5% chance to increase accuracy decay by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseAccuracyDropoff * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseAccuracyDropoff * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseAccuracyDropoff; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseAccuracyDropoff * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseAccuracyDropoff * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static int AdjustMeleeDamage(int baseMeleeDamage)
    {
        // 10% chance to increase melee damage by 10% - 25%
        // 15% chance to increase melee damage by 0% - 10%
        // 50% chance to keep melee damage the same
        // 20% chance to decrease melee damage by 0% - 10%
        // 5% chance to decrease melee damage by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return Mathf.RoundToInt(baseMeleeDamage * (1 + UnityEngine.Random.Range(0.10f, 0.25f)));
        }
        else if (randomValue < 0.25f)
        {
            return Mathf.RoundToInt(baseMeleeDamage * (1 + UnityEngine.Random.Range(0f, 0.10f)));
        }
        else if (randomValue < 0.75f)
        {
            return baseMeleeDamage; // No change
        }
        else if (randomValue < 0.95f)
        {
            return Mathf.RoundToInt(baseMeleeDamage * (1 - UnityEngine.Random.Range(0f, 0.10f)));
        }
        else
        {
            return Mathf.RoundToInt(baseMeleeDamage * (1 - UnityEngine.Random.Range(0.10f, 0.25f)));
        }
    }

    private static float AdjustMeleeAccuracy(float baseMeleeAccuracy)
    {
        // 10% chance to increase melee accuracy by 10% - 25%
        // 15% chance to increase melee accuracy by 0% - 10%
        // 50% chance to keep melee accuracy the same
        // 20% chance to decrease melee accuracy by 0% - 10%
        // 5% chance to decrease melee accuracy by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseMeleeAccuracy * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseMeleeAccuracy * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseMeleeAccuracy; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseMeleeAccuracy * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseMeleeAccuracy * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustReloadTime(float baseReloadTime)
    {
        // 10% chance to decrease reload time by 10% - 25%
        // 15% chance to decrease reload time by 0% - 10%
        // 50% chance to keep reload time the same
        // 20% chance to increase reload time by 0% - 10%
        // 5% chance to increase reload time by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseReloadTime * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseReloadTime * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseReloadTime; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseReloadTime * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseReloadTime * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustFireRate(float baseFireRate)
    {
        // 10% chance to increase fire rate by 10% - 25%
        // 15% chance to increase fire rate by 0% - 10%
        // 50% chance to keep fire rate the same
        // 20% chance to decrease fire rate by 0% - 10%
        // 5% chance to decrease fire rate by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseFireRate * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseFireRate * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseFireRate; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseFireRate * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseFireRate * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }

    private static float AdjustSwingRate(float baseSwingRate)
    {
        // 10% chance to increase swing rate by 10% - 25%
        // 15% chance to increase swing rate by 0% - 10%
        // 50% chance to keep swing rate the same
        // 20% chance to decrease swing rate by 0% - 10%
        // 5% chance to decrease swing rate by 10% - 25%
        float randomValue = UnityEngine.Random.value;
        if (randomValue < 0.10f)
        {
            return baseSwingRate * (1 + UnityEngine.Random.Range(0.10f, 0.25f));
        }
        else if (randomValue < 0.25f)
        {
            return baseSwingRate * (1 + UnityEngine.Random.Range(0f, 0.10f));
        }
        else if (randomValue < 0.75f)
        {
            return baseSwingRate; // No change
        }
        else if (randomValue < 0.95f)
        {
            return baseSwingRate * (1 - UnityEngine.Random.Range(0f, 0.10f));
        }
        else
        {
            return baseSwingRate * (1 - UnityEngine.Random.Range(0.10f, 0.25f));
        }
    }
}
