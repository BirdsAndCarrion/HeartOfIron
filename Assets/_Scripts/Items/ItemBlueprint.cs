using UnityEngine;
using System;
using HeartOfIron.Enums;

/// <summary>
/// Represents a blueprint for creating items, including their general properties and methods for modifying stats.
/// </summary>
[CreateAssetMenu(fileName = "ItemBlueprint", menuName = "Scriptable Objects/ItemBlueprint")]
[Serializable]
public class ItemBlueprint : ScriptableObject
{
    [Header("General Properties")]
    /// <summary>
    /// The name of the item blueprint.
    /// </summary>
    [SerializeField] private string itemName;
    public string ItemName => itemName;
    /// <summary>
    /// The type of the item blueprint.
    /// </summary>
    [SerializeField] private ItemType itemType;
    public ItemType ItemType => itemType;
    /// <summary>
    /// The weight of the item blueprint.
    /// </summary>
    [SerializeField] private Weight weight;
    public Weight Weight => weight;
    /// <summary>
    /// The cost of the item blueprint.
    /// </summary>
    [SerializeField] private int cost;
    public int Cost => cost;
    /// <summary>
    /// Indicates whether the item blueprint has been modified.
    /// </summary>
    [SerializeField] private bool modified;
    public bool Modified => modified;

    /// <summary>
    /// Increases the stats of the specified item based on the blueprint.
    /// </summary>
    /// <param name="item">The item to modify.</param>
    /// <returns>True if the stats were successfully increased; otherwise, false.</returns>
    public virtual bool IncreaseStats(Item item)
    {
        // Default implementation (can be overridden in derived classes)
        return false;
    }

    /// <summary>
    /// Sets the modified status of the blueprint.
    /// </summary>
    /// <param name="isModified">True if the blueprint is modified; otherwise, false.</param>
    public void SetModified(bool isModified)
    {
        modified = isModified;
    }

    /// <summary>
    /// Gets a string representation of the blueprint information.
    /// </summary>
    /// <returns>A string containing the blueprint information.</returns>
    public virtual string GetBlueprintInfo()
    {
        return $"Name: {itemName}\nType: {itemType}\nWeight: {weight}\nCost: {cost}";
    }
}
