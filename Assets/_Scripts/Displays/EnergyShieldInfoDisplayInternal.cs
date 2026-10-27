using UnityEngine;
using TMPro;
using HeartOfIron.Enums;
using System;

/// <summary>
/// Displays detailed information about a specific energy shield in the UI.
/// Updates the UI elements with the shield's name, weight, max capacity, recharge rate, and recharge delay.
/// This class is intended to interface with a specific element in the UI to display energy shield information.
/// </summary>
public class EnergyShieldInfoDisplayInternal : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _energyShieldNameText;
    [SerializeField] private TextMeshProUGUI _energyShieldWeightText;
    [SerializeField] private TextMeshProUGUI _energyShieldMaxCapacityText;
    [SerializeField] private TextMeshProUGUI _energyShieldRechargeRateText;
    [SerializeField] private TextMeshProUGUI _energyShieldRechargeDelayText;

    public void DisplayEnergyShieldInfo(EnergyShield shield)
    {
        _energyShieldNameText.text = shield.Name;
        if (shield.Weight == Weight.Light)
        {
            _energyShieldWeightText.text = "L";   
        } else if (shield.Weight == Weight.Medium)
        {
            _energyShieldWeightText.text = "M";
        } else if (shield.Weight == Weight.Heavy)
        {
            _energyShieldWeightText.text = "H";
        }
        _energyShieldMaxCapacityText.text = "Max Cap: " + Math.Round(shield.MaxCapacity, 2).ToString();
        _energyShieldRechargeRateText.text = "Rec Rate: " + Math.Round(shield.RechargeRate, 2).ToString();
        _energyShieldRechargeDelayText.text = "Rec Delay: " + Math.Round(shield.RechargeDelay, 2).ToString();
    }
}
