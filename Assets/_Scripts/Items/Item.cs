using UnityEngine;
using System;
using HeartOfIron.Enums;

/// <summary>
/// Represents a generic item that can be owned by a mech, including its properties such as name, type, weight, cost, and owner.
/// </summary>
[Serializable]
public abstract class Item
{
    /// <summary>
    /// The unique identifier of the item.
    /// </summary>
    [SerializeField] private Guid _id;
    public Guid Id => _id;
    /// <summary>
    /// The name of the item.
    /// </summary>
    [SerializeField] private string _itemName;
    public string Name => _itemName;
    /// <summary>
    /// The type of the item. (e.g., Weapon, Armor, EnergyShield)
    /// </summary>
    [SerializeField] private ItemType _itemType;
    public ItemType Type => _itemType;
    /// <summary>
    /// The weight of the item.
    /// </summary>
    [SerializeField] private Weight _weight;
    public Weight Weight => _weight;
    /// <summary>
    /// The owner of the item (the mech that possesses it).
    /// </summary>
    [NonSerialized] private Mech _owner;
    public Mech Owner => _owner;
    /// <summary>
    /// The cost of the item.
    /// </summary>
    [SerializeField] private int _cost;
    public int Cost => _cost;

    public Item(string itemName, Weight weight, ItemType itemType)
    {
        _id = Guid.NewGuid();
        _itemName = itemName;
        _itemType = itemType;
        _weight = weight;
    }

    public Item(string itemName, Weight weight, ItemType itemType, int cost)
    {
        _id = Guid.NewGuid();
        _itemName = itemName;
        _itemType = itemType;
        _weight = weight;
        _cost = cost;
    }

    public Item(string itemName, Weight weight, ItemType itemType, Mech owner, int cost)
    {
        _id = Guid.NewGuid();
        _itemName = itemName;
        _itemType = itemType;
        _weight = weight;
        _owner = owner;
        _cost = cost;
    }

    public void SetOwner(Mech owner)
    {
        _owner = owner;
    }

    public void ClearOwner()
    {
        _owner = null;
    }

    public abstract string GetDescription();

    public abstract Item Clone();
}
