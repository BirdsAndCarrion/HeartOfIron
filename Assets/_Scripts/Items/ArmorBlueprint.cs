using UnityEngine;
using System;
using HeartOfIron.Enums;

[CreateAssetMenu(fileName = "ArmorBlueprint", menuName = "Scriptable Objects/ArmorBlueprint")]
[Serializable]
public class ArmorBlueprint : ItemBlueprint
{
    [Header("Armor Properties")]
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

    public override bool IncreaseStats(Item item)
    {
        if (item is Armor armor)
        {
            return IncreaseArmorStats(armor);
        }
        else
        {
            Debug.LogWarning("Item is not an Armor. Cannot increase stats.");
            return false;
        }
    }

    private bool IncreaseArmorStats(Armor armor)
    {
        if (armor == null)
        {
            Debug.LogWarning("Armor is null. Cannot increase stats.");
            return false;
        }
        if (armor.Resistance > this.resistance)
        {
            this.resistance = armor.Resistance;
        }
        if (armor.Coverage > this.coverage)
        {
            this.coverage = armor.Coverage;
        }
        if (armor.DefenseRating > this.defenseRating)
        {
            this.defenseRating = armor.DefenseRating;
        }
        if (armor.MaxDurability > this.maxDurability)
        {
            this.maxDurability = armor.MaxDurability;
        }
        return true;
    }

    public override string GetBlueprintInfo()
    {
        return $"Name: {ItemName}\nType: {ItemType}\nWeight: {Weight}\nCost: {Cost}\n" +
               $"Armor Type: {armorType}\nResistance: {resistance}\nCoverage: {coverage}\n" +
               $"Defense Rating: {defenseRating}\nMax Durability: {maxDurability}";
    }
}
