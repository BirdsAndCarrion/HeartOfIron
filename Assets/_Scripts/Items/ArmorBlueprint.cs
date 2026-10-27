using UnityEngine;
using System;
using HeartOfIron.Enums;

/// <summary>
/// Represents a blueprint for creating armor items, defining their properties and stats.
/// Contains functions for increasing the stats of the blueprint based on armor items and retrieving blueprint information.
/// </summary>
[CreateAssetMenu(fileName = "ArmorBlueprint", menuName = "Scriptable Objects/ArmorBlueprint")]
[Serializable]
public class ArmorBlueprint : ItemBlueprint
{
    [Header("Armor Properties")]
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
    /// The percentage chance that the armor will successfully cover an attack.
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
    /// Increases the stats of the blueprint based on the provided armor item.
    /// </summary>
    /// <param name="item">The armor item used to increase the blueprint's stats.</param>
    /// <returns>True if the stats were successfully increased, false otherwise.</returns>
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

    /// <summary>
    /// Increases the stats of the blueprint based on the provided armor item.
    /// </summary>
    /// <param name="armor">The armor item used to increase the blueprint's stats.</param>
    /// <returns>True if the stats were successfully increased, false otherwise.</returns>
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

    /// <summary>
    /// Gets a textual representation of the blueprint's information.
    /// </summary>
    /// <returns>A string containing the blueprint's details.</returns>
    public override string GetBlueprintInfo()
    {
        return $"Name: {ItemName}\nType: {ItemType}\nWeight: {Weight}\nCost: {Cost}\n" +
               $"Armor Type: {armorType}\nResistance: {resistance}\nCoverage: {coverage}\n" +
               $"Defense Rating: {defenseRating}\nMax Durability: {maxDurability}";
    }
}
