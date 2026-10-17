using UnityEngine;
using System;
using HeartOfIron.Enums;
using HeartOfIron.Structs;

[Serializable]
public class Armor : Item
{
    [SerializeField] private ArmorType armorType;
    public ArmorType ArmorType => armorType;
    [SerializeField] private float resistance;
    public float Resistance => resistance;
    [SerializeField] private float coverage;
    public float Coverage => coverage;
    [SerializeField] private float defenseRating;
    public float DefenseRating => defenseRating;
    [SerializeField] private float maxDurability;
    public float MaxDurability => maxDurability;
    [SerializeField] private float currentDurabilityR;
    public float CurrentDurabilityR => currentDurabilityR;
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

    public override string GetDescription()
    {
        if (armorType == ArmorType.Head || armorType == ArmorType.Torso)
        {
            return $"{Name}\n Weight: {Weight}\n Type: {ArmorType}\n Resistance: {MathF.Round(Resistance, 2)}\n Coverage: {MathF.Round(Coverage, 2)}\n Defense Rating: {MathF.Round(DefenseRating, 2)}\n Durability: {MathF.Round(CurrentDurabilityR, 2)}/{MathF.Round(MaxDurability, 2)}";
        }
        return $"{Name}\n Weight: {Weight}\n Type: {ArmorType}\n Resistance: {MathF.Round(Resistance, 2)}\n Coverage: {MathF.Round(Coverage, 2)}\n Defense Rating: {MathF.Round(DefenseRating, 2)}\n Durability: {MathF.Round(CurrentDurabilityR, 2)}/{MathF.Round(MaxDurability, 2)} (Right)\n {MathF.Round(CurrentDurabilityL, 2)}/{MathF.Round(MaxDurability, 2)} (Left)";
    }

    public override Item Clone()
    {
        return new Armor(Name, Weight, ArmorType, Cost, Resistance, Coverage, DefenseRating, MaxDurability);
    }

}
