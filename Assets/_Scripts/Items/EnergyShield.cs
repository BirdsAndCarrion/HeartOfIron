using UnityEngine;
using System;
using HeartOfIron.Enums;
using HeartOfIron.Structs;

[Serializable]
public class EnergyShield : Item
{
    [SerializeField] private float maxCapacity;
    public float MaxCapacity => maxCapacity;
    [SerializeField] private float currentCapacity;
    public float CurrentCapacity => currentCapacity;
    [SerializeField] private float rechargeRate;
    public float RechargeRate => rechargeRate;
    [SerializeField] private float rechargeDelay;
    public float RechargeDelay => rechargeDelay;
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

    public void Repair(float repairAmount, int location = 0)
    {
        currentCapacity += repairAmount;
        if (currentCapacity > maxCapacity)
        {
            currentCapacity = maxCapacity; // Ensure current capacity doesn't exceed max capacity
        }
    }

    public override string GetDescription()
    {
        return $"{Name}: Max Capacity: {MaxCapacity}\n Current Capacity: {CurrentCapacity}\n Recharge Rate: {RechargeRate}\n Recharge Delay: {RechargeDelay}";
    }

    public override Item Clone()
    {
        return new EnergyShield(Name, Weight, Cost, MaxCapacity, RechargeRate, RechargeDelay);
    }
}
