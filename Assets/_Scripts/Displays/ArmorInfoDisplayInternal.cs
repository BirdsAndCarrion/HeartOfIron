using UnityEngine;
using TMPro;
using HeartOfIron.Enums;
using System;

/// <summary>
/// Displays detailed information about a specific armor piece in the UI.
/// Updates the UI elements with the armor's name, weight, type, coverage, resistance, defense rating, and durability.
/// This class is intended to interface with a specific element in the UI to display armor information.
/// </summary>
public class ArmorInfoDisplayInternal : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _armorNameText;
    [SerializeField] private TextMeshProUGUI _armorWeightText;
    [SerializeField] private TextMeshProUGUI _armorTypeText;
    [SerializeField] private TextMeshProUGUI _armorCoverageText;
    [SerializeField] private TextMeshProUGUI _armorResistanceText;
    [SerializeField] private TextMeshProUGUI _armorDefenseRatingText;
    [SerializeField] private TextMeshProUGUI _armorDurabilityText;

    public void DisplayArmorInfo(Armor armor)
    {
        _armorNameText.text = armor.Name;
        if (armor.Weight == Weight.Light)
        {
            _armorWeightText.text = "L";   
        } else if (armor.Weight == Weight.Medium)
        {
            _armorWeightText.text = "M";
        } else if (armor.Weight == Weight.Heavy)
        {
            _armorWeightText.text = "H";
        }
        _armorTypeText.text = armor.ArmorType.ToString();
        _armorCoverageText.text = "Cov: " + Math.Round(armor.Coverage, 2).ToString();
        _armorResistanceText.text = "Res: " + Math.Round(armor.Resistance, 2).ToString();
        _armorDefenseRatingText.text = "Def Rat: " + Math.Round(armor.DefenseRating, 2).ToString();
        _armorDurabilityText.text = "Dur: " + Math.Round(armor.MaxDurability, 2).ToString();
    }
}
