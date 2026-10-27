using UnityEngine;
using System;
using HeartOfIron.Enums;
using HeartOfIron.Structs;

/// <summary>
/// Represents an armor item that provides protection to a mech, with separate durability for the right and left sides.
/// Provides methods to get and modify the durability of the armor on each side.
/// </summary>
/// <remarks>
/// For armor locations that only cover one side, the right side durability is used and the left side durability is ignored.
/// </remarks>
[Serializable]
public class Armor : Item
{
    /// <summary>
    /// The area the armor covers on the mech.
    /// </summary>
    [SerializeField] private ArmorType armorType;
    public ArmorType ArmorType => armorType;
    /// <summary>
    /// The percentage of damage the armor reduces if it successfully covers the attack.
    /// </summary>
    [SerializeField] private float resistance;
    public float Resistance => resistance;
    /// <summary>
    /// The chance that the armor will successfully cover an incoming attack.
    /// </summary>
    [SerializeField] private float coverage;
    public float Coverage => coverage;
    /// <summary>
    /// The flat value by which the armor reduces incoming damage.
    /// </summary>
    [SerializeField] private float defenseRating;
    public float DefenseRating => defenseRating;
    /// <summary>
    /// The maximum durability of the armor.
    /// </summary>
    [SerializeField] private float maxDurability;
    public float MaxDurability => maxDurability;
    /// <summary>
    /// The current durability of the armor on the right side.
    /// </summary>
    [SerializeField] private float currentDurabilityR;
    public float CurrentDurabilityR => currentDurabilityR;
    /// <summary>
    /// The current durability of the armor on the left side.
    /// </summary>
    [SerializeField] private float currentDurabilityL;
    public float CurrentDurabilityL => currentDurabilityL;

    public Armor(string name, Weight weight, ArmorType armorType, int cost, float resistance, float coverage, float defenseRating, float maxDurability)
        : base(name, weight, ItemType.Armor, cost)
    {
        this.armorType = armorType;
        this.resistance = resistance;
        this.coverage = coverage;
        this.defenseRating = defenseRating;
        this.maxDurability = maxDurability;
        this.currentDurabilityR = maxDurability; // Initialize current durability to max durability
        this.currentDurabilityL = maxDurability; // Initialize current durability to max durability
    }

    /// <summary>
    /// Gets the current durability of the armor for the specified location.
    /// </summary>
    /// <param name="location">The location of the armor (0 for right side, 1 for left side).</param>
    /// <returns>The current durability of the armor at the specified location.</returns>
    public float GetDurability(int location = 0)
    {
        if (location == 0) // Right side
        {
            return currentDurabilityR;
        }
        else // Left side
        {
            return currentDurabilityL;
        }
    }

    /// <summary>
    /// Applies damage to the armor at the specified location and returns the remaining damage that goes through.
    /// </summary>
    /// <param name="damageAmount">The amount of incoming damage.</param>
    /// <param name="location">The location of the armor (0 for right side, 1 for left side).</param>
    /// <returns>The remaining damage that goes through the armor after mitigation.</returns>
    public float TakeDamage(float damageAmount, int location = 0)
    {
        if (location == 0) // Right side
        {
            if (GetDurability(location) <= 0)
            {
                return damageAmount; // If armor is broken, return full damage
            }
            if (UnityEngine.Random.value <= coverage)
            {
                float effectiveDamage = damageAmount * (1 - resistance);
                effectiveDamage = Mathf.Max(effectiveDamage - defenseRating, 0); // Ensure effective damage is not negative
                float remainingDamage = Mathf.Max(effectiveDamage - currentDurabilityR, 0); // Calculate remaining damage after armor durability
                currentDurabilityR -= effectiveDamage;
                if (currentDurabilityR < 0)
                {
                    currentDurabilityR = 0; // Ensure durability doesn't go below zero
                }
                return remainingDamage; // Return the remaining damage that goes through the armor
            }
            else
            {
                float effectiveDamage = Mathf.Max(damageAmount - defenseRating, 0); // Ensure effective damage is not negative
                float remainingDamage = Mathf.Max(effectiveDamage - currentDurabilityR, 0); // Calculate remaining damage after armor durability
                currentDurabilityR -= effectiveDamage;
                if (currentDurabilityR < 0)
                {
                    currentDurabilityR = 0; // Ensure durability doesn't go below zero
                }
                return remainingDamage; // Return the remaining damage that goes through the armor
            }
        }
        else
        {
            if (GetDurability(location) <= 0)
            {
                return damageAmount; // If armor is broken, return full damage
            }
            if (UnityEngine.Random.value <= coverage)
            {
                float effectiveDamage = damageAmount * (1 - resistance);
                effectiveDamage = Mathf.Max(effectiveDamage - defenseRating, 0); // Ensure effective damage is not negative
                float remainingDamage = Mathf.Max(effectiveDamage - currentDurabilityL, 0); // Calculate remaining damage after armor durability
                currentDurabilityL -= effectiveDamage;
                if (currentDurabilityL < 0)
                {
                    currentDurabilityL = 0; // Ensure durability doesn't go below zero
                }
                return remainingDamage; // Return the remaining damage that goes through the armor
            }
            else
            {
                float effectiveDamage = Mathf.Max(damageAmount - defenseRating, 0); // Ensure effective damage is not negative
                float remainingDamage = Mathf.Max(effectiveDamage - currentDurabilityL, 0); // Calculate remaining damage after armor durability
                currentDurabilityL -= effectiveDamage;
                if (currentDurabilityL < 0)
                {
                    currentDurabilityL = 0; // Ensure durability doesn't go below zero
                }
                return remainingDamage; // Return the remaining damage that goes through the armor
            }
        }
    }

    /// <summary>
    /// Repairs the armor at the specified location by the given amount.
    /// </summary>
    /// <param name="repairAmount">The amount to repair the armor.</param>
    /// <param name="location">The location of the armor (0 for right side, 1 for left side).</param>
    /// <remarks>
    /// If the repair amount exceeds the maximum durability, the durability will be capped at the maximum value.
    /// </remarks>
    public void Repair(float repairAmount, int location = 0)
    {
        if (location == 0) // Right side
        {
            currentDurabilityR += repairAmount;
            if (currentDurabilityR > maxDurability)
            {
                currentDurabilityR = maxDurability; // Ensure durability doesn't exceed max durability
            }
        }
        else // Left side
        {
            currentDurabilityL += repairAmount;
            if (currentDurabilityL > maxDurability)
            {
                currentDurabilityL = maxDurability; // Ensure durability doesn't exceed max durability
            }
        }
    }

    /// <summary>
    /// Gets a textual description of the armor, including its properties and current durability.
    /// </summary>
    /// <returns>A string describing the armor's properties and current durability.</returns>
    public override string GetDescription()
    {
        if (armorType == ArmorType.Head || armorType == ArmorType.Torso)
        {
            return $"{Name}\n Weight: {Weight}\n Type: {ArmorType}\n Resistance: {MathF.Round(Resistance, 2)}\n Coverage: {MathF.Round(Coverage, 2)}\n Defense Rating: {MathF.Round(DefenseRating, 2)}\n Durability: {MathF.Round(CurrentDurabilityR, 2)}/{MathF.Round(MaxDurability, 2)}";
        }
        return $"{Name}\n Weight: {Weight}\n Type: {ArmorType}\n Resistance: {MathF.Round(Resistance, 2)}\n Coverage: {MathF.Round(Coverage, 2)}\n Defense Rating: {MathF.Round(DefenseRating, 2)}\n Durability: {MathF.Round(CurrentDurabilityR, 2)}/{MathF.Round(MaxDurability, 2)} (Right)\n {MathF.Round(CurrentDurabilityL, 2)}/{MathF.Round(MaxDurability, 2)} (Left)";
    }

    /// <summary>
    /// Creates a copy of the armor item.
    /// </summary>
    /// <returns>A new instance of the armor with the same properties.</returns>
    public override Item Clone()
    {
        return new Armor(Name, Weight, ArmorType, Cost, Resistance, Coverage, DefenseRating, MaxDurability);
    }

}
