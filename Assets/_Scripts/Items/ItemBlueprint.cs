using UnityEngine;
using System;
using HeartOfIron.Enums;

[CreateAssetMenu(fileName = "ItemBlueprint", menuName = "Scriptable Objects/ItemBlueprint")]
[Serializable]
public class ItemBlueprint : ScriptableObject
{
    [Header("General Properties")]
    [SerializeField] private string itemName;
    public string ItemName => itemName;
    [SerializeField] private ItemType itemType;
    public ItemType ItemType => itemType;
    [SerializeField] private Weight weight;
    public Weight Weight => weight;
    [SerializeField] private int cost;
    public int Cost => cost;
    [SerializeField] private bool modified;
    public bool Modified => modified;

    public virtual bool IncreaseStats(Item item)
    {
        // Default implementation (can be overridden in derived classes)
        return false;
    }

    public void SetModified(bool isModified)
    {
        modified = isModified;
    }

    public virtual string GetBlueprintInfo()
    {
        return $"Name: {itemName}\nType: {itemType}\nWeight: {weight}\nCost: {cost}";
    }
}
