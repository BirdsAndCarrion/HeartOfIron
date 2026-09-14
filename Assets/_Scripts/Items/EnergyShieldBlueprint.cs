using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnergyShieldBlueprint", menuName = "Scriptable Objects/EnergyShieldBlueprint")]
[Serializable]
public class EnergyShieldBlueprint : ItemBlueprint
{
    [Header("Energy Shield Properties")]
    [SerializeField] private float maxCapacity;
    public float MaxCapacity => maxCapacity;
    [SerializeField] private float rechargeRate;
    public float RechargeRate => rechargeRate;
    [SerializeField] private float rechargeDelay;
    public float RechargeDelay => rechargeDelay;

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

    public override string GetBlueprintInfo()
    {
        return $"Name: {ItemName}\nType: {ItemType}\nWeight: {Weight}\nCost: {Cost}\n" +
               $"Max Capacity: {maxCapacity}\nRecharge Rate: {rechargeRate}\nRecharge Delay: {rechargeDelay}";
    }
}
