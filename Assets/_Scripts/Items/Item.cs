using UnityEngine;
using System;
using HeartOfIron.Enums;

[Serializable]
public abstract class Item
{
    [SerializeField] private Guid _id;
    public Guid Id => _id;
    [SerializeField] private string _itemName;
    public string Name => _itemName;
    [SerializeField] private ItemType _itemType;
    public ItemType Type => _itemType;
    [SerializeField] private Weight _weight;
    public Weight Weight => _weight;
    [NonSerialized] private Mech _owner;
    public Mech Owner => _owner;
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
