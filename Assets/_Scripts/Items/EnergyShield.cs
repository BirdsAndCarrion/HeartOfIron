using UnityEngine;
using System;
using HeartOfIron.Enums;
using HeartOfIron.Structs;

/// <summary>
/// Represents an energy shield item that provides a protective energy barrier for the mech.
/// Contains functions for managing the shield's capacity, recharge, and damage absorption.
/// Provides methods for taking damage and repairing the shield over time.
/// </summary>
[Serializable]
public class EnergyShield : Item
{
    /// <summary>
    /// The maximum capacity of the energy shield.
    /// </summary>
    [SerializeField] private float maxCapacity;
    public float MaxCapacity => maxCapacity;
    /// <summary>
    /// The current capacity of the energy shield.
    /// </summary>
    [SerializeField] private float currentCapacity;
    public float CurrentCapacity => currentCapacity;
    /// <summary>
    /// The rate at which the energy shield recharges per second.
    /// </summary>
    [SerializeField] private float rechargeRate;
    public float RechargeRate => rechargeRate;
    /// <summary>
    /// The delay before the energy shield starts recharging after taking damage.
    /// </summary>
    [SerializeField] private float rechargeDelay;
    public float RechargeDelay => rechargeDelay;
    /// <summary>
    /// The current timer tracking the delay before the energy shield starts recharging.
    /// </summary>
    [SerializeField] private float rechargeTimer;
    public float RechargeTimer => rechargeTimer;

    public EnergyShield(string name, Weight weight, int cost, float maxCapacity, float rechargeRate, float rechargeDelay)
        : base(name, weight, ItemType.EnergyShield, cost)
    {
        this.maxCapacity = maxCapacity;
        this.currentCapacity = maxCapacity; // Initialize current capacity to max capacity
        this.rechargeRate = rechargeRate;
        this.rechargeDelay = rechargeDelay;
        this.rechargeTimer = 0f; // Initialize recharge timer to 0
    }

    /// <summary>
    /// Updates the recharge state of the energy shield based on the elapsed time.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
    public void UpdateRecharge(float deltaTime)
    {
        if (currentCapacity < maxCapacity)
        {
            while (deltaTime > 0f)
            {
                if (rechargeTimer > 0f)
                {
                    rechargeTimer -= Mathf.Min(deltaTime, 1f);
                    if (rechargeTimer < 0f)
                    {
                        rechargeTimer = 0f; // Ensure recharge timer does not go below 0
                    }
                }
                else
                {
                    currentCapacity += rechargeRate * Mathf.Min(deltaTime, 1f);
                    if (currentCapacity > maxCapacity)
                    {
                        currentCapacity = maxCapacity; // Ensure current capacity does not exceed max capacity
                    }
                }
                // Debug.Log($"Energy Shield Recharge Update: Current Capacity = {currentCapacity}, Recharge Timer = {rechargeTimer}, Delta Time = {deltaTime}");
                deltaTime -= Mathf.Min(deltaTime, 1f);
            }
        }
    }

    /// <summary>
    /// Applies damage to the energy shield, reducing its current capacity.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply to the shield.</param>
    /// <param name="location">The location of the damage (optional, default is 0).</param>
    /// <returns>The remaining damage that goes through the shield after absorption.</returns>
    public float TakeDamage(float damageAmount, int location = 0)
    {
        if (currentCapacity > 0)
        {
            float effectiveDamage = Mathf.Max(damageAmount - currentCapacity, 0); // Calculate remaining damage after shield capacity
            currentCapacity -= damageAmount;
            if (currentCapacity < 0)
            {
                currentCapacity = 0; // Ensure current capacity doesn't go below zero
            }
            rechargeTimer = rechargeDelay; // Reset the recharge timer
            return effectiveDamage; // Return the remaining damage that goes through the shield
        }
        else
        {
            return damageAmount; // If shield is depleted, all damage goes through
        }
    }

    /// <summary>
    /// Repairs the energy shield, increasing its current capacity.
    /// </summary>
    /// <param name="repairAmount">The amount of repair to apply to the shield.</param>
    /// <param name="location">The location of the repair (optional, default is 0).</param>
    public void Repair(float repairAmount, int location = 0)
    {
        currentCapacity += repairAmount;
        if (currentCapacity > maxCapacity)
        {
            currentCapacity = maxCapacity; // Ensure current capacity doesn't exceed max capacity
        }
    }

    /// <summary>
    /// Gets a textual description of the energy shield, including its name, weight, current capacity, max capacity, recharge rate, and recharge delay.
    /// </summary>
    /// <returns>A string containing the energy shield's details.</returns>
    public override string GetDescription()
    {
        return $"{Name}\n Weight: {Weight}\n Capacity: {MathF.Round(CurrentCapacity, 2)}/{MathF.Round(MaxCapacity, 2)}\n Recharge Rate: {MathF.Round(RechargeRate, 2)}\n Recharge Delay: {MathF.Round(RechargeDelay, 2)}";
    }

    /// <summary>
    /// Creates a new instance of the energy shield with the same properties as the current one.
    /// </summary>
    /// <returns>A new instance of the energy shield with the same properties as the current one.</returns>
    public override Item Clone()
    {
        return new EnergyShield(Name, Weight, Cost, MaxCapacity, RechargeRate, RechargeDelay);
    }
}
