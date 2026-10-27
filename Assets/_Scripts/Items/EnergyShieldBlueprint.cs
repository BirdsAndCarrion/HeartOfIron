using UnityEngine;
using System;

/// <summary>
/// Represents a blueprint for creating energy shield items, defining their properties and stats.
/// Contains functions for increasing stats and retrieving blueprint information.
/// </summary>
[CreateAssetMenu(fileName = "EnergyShieldBlueprint", menuName = "Scriptable Objects/EnergyShieldBlueprint")]
[Serializable]
public class EnergyShieldBlueprint : ItemBlueprint
{
    [Header("Energy Shield Properties")]
    /// <summary>
    /// The maximum capacity of the energy shield.
    /// </summary>
    [SerializeField] private float maxCapacity;
    public float MaxCapacity => maxCapacity;
    [SerializeField] private float rechargeRate;
    /// <summary>
    /// The recharge rate of the energy shield.
    /// </summary>
    public float RechargeRate => rechargeRate;
    [SerializeField] private float rechargeDelay;
    /// <summary>
    /// The recharge delay of the energy shield.
    /// </summary>
    public float RechargeDelay => rechargeDelay;

    /// <summary>
    /// Increases the stats of the given energy shield item based on this blueprint's properties.
    /// </summary>
    /// <param name="item">The energy shield item whose stats are to be increased.</param>
    /// <returns>True if the stats were successfully increased, false otherwise.</returns>
    public override bool IncreaseStats(Item item)
    {
        if (item is EnergyShield energyShield)
        {
            return IncreaseEnergyShieldStats(energyShield);
        }
        else
        {
            Debug.LogWarning("Item is not an EnergyShield. Cannot increase stats.");
            return false;
        }
    }

    /// <summary>
    /// Increases the blueprint's stats based on the given energy shield's properties.
    /// </summary>
    /// <param name="energyShield">The energy shield whose properties are used to increase the blueprint's stats.</param>
    /// <returns>True if the stats were successfully increased, false otherwise.</returns>
    private bool IncreaseEnergyShieldStats(EnergyShield energyShield)
    {
        if (energyShield == null)
        {
            Debug.LogWarning("EnergyShield is null. Cannot increase stats.");
            return false;
        }
        if (energyShield.MaxCapacity > this.maxCapacity)
        {
            this.maxCapacity = energyShield.MaxCapacity;
        }
        if (energyShield.RechargeRate > this.rechargeRate)
        {
            this.rechargeRate = energyShield.RechargeRate;
        }
        if (energyShield.RechargeDelay < this.rechargeDelay)
        {
            this.rechargeDelay = energyShield.RechargeDelay;
        }
        return true;
    }

    /// <summary>
    /// Gets a textual description of the energy shield blueprint, including its name, type, weight, cost, max capacity, recharge rate, and recharge delay.
    /// </summary>
    /// <returns>A string containing the blueprint's details.</returns>
    public override string GetBlueprintInfo()
    {
        return $"Name: {ItemName}\nType: {ItemType}\nWeight: {Weight}\nCost: {Cost}\n" +
               $"Max Capacity: {maxCapacity}\nRecharge Rate: {rechargeRate}\nRecharge Delay: {rechargeDelay}";
    }
}
