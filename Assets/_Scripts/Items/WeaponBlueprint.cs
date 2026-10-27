using UnityEngine;
using System;
using HeartOfIron.Enums;

/// <summary>
/// Represents a blueprint for creating weapon items.
/// </summary>

[CreateAssetMenu(fileName = "WeaponBlueprint", menuName = "Scriptable Objects/WeaponBlueprint")]
[Serializable]
public class WeaponBlueprint : ItemBlueprint
{
    [Header("Weapon Properties")]
    /// <summary>
    /// The type of the weapon.
    /// </summary>
    [SerializeField] private WeaponName weaponName;
    public WeaponName WeaponName => weaponName;
    /// <summary>
    /// The number of slots required to equip the weapon.
    /// </summary>
    [SerializeField] private int slotsRequired;
    public int SlotsRequired => slotsRequired;
    [Header("Weapon Shooting Stats")]
    /// <summary>
    /// The base damage of the weapon.
    /// </summary>
    [SerializeField] private int damage;
    public int Damage => damage;
    /// <summary>
    /// The number of shots fired by the weapon.
    /// </summary>
    [SerializeField] private int numShots;
    public int NumShots => numShots;
    /// <summary>
    /// The maximum ammunition capacity of the weapon.
    /// </summary>
    [SerializeField] private int maxAmmo;
    public int MaxAmmo => maxAmmo;
    [Header("Weapon Shooting Accuracy Stats")]
    /// <summary>
    /// The accuracy of the weapon at point-blank range.
    /// </summary>
    [SerializeField] private float pointBlankAccuracy;
    public float PointBlankAccuracy => pointBlankAccuracy;
    /// <summary>
    /// The accuracy dropoff of the weapon over distance.
    /// </summary>
    [SerializeField] private float accuracyDropoff;
    public float AccuracyDropoff => accuracyDropoff;
    [Header("Weapon Melee Stats")]
    /// <summary>
    /// The base melee damage of the weapon.
    /// </summary>
    [SerializeField] private int meleeDamage;
    public int MeleeDamage => meleeDamage;
    /// <summary>
    /// The melee range of the weapon.
    /// </summary>
    [SerializeField] private int meleeRange;
    public int MeleeRange => meleeRange;
    [Header("Weapon Melee Accuracy Stat")]
    /// <summary>
    /// The accuracy of the weapon for melee attacks.
    /// </summary>
    [SerializeField] private float meleeAccuracy;
    public float MeleeAccuracy => meleeAccuracy;

    [Header("Weapon Timing Stats")]
    /// <summary>
    /// The time it takes to reload the weapon.
    /// </summary>
    [SerializeField] private float reloadTime;
    public float ReloadTime => reloadTime;
    /// <summary>
    /// The rate of fire of the weapon.
    /// </summary>
    [SerializeField] private float fireRate;
    public float FireRate => fireRate;
    /// <summary>
    /// The swing rate of the weapon for melee attacks.
    /// </summary>
    [SerializeField] private float swingRate;
    public float SwingRate => swingRate;

    /// <summary>
    /// A method to increase the stats of the weapon based on another item.
    /// </summary>
    /// <param name="item">The item to compare and potentially increase stats from.</param>
    /// <returns>True if any stats were increased, false otherwise.</returns>
    public override bool IncreaseStats(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Item is null. Cannot increase stats.");
            return false;
        }
        if (item is Weapon weapon)
        {
            return IncreaseWeaponStats(weapon);
        }
        else
        {
            Debug.LogWarning("Item is not a Weapon. Cannot increase stats.");
            return false;
        }
    }

    /// <summary>
    /// A helper method to increase the stats of the weapon based on another weapon.
    /// </summary>
    /// <param name="weapon">The weapon to compare and potentially increase stats from.</param>
    /// <returns>True if any stats were increased, false otherwise.</returns>
    private bool IncreaseWeaponStats(Weapon weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("Weapon is null. Cannot increase stats.");
            return false;
        }
        if (weapon.Damage > this.damage)
        {
            this.damage = weapon.Damage;
        }
        if (weapon.MaxAmmo > this.maxAmmo)
        {
            this.maxAmmo = weapon.MaxAmmo;
        }
        if (weapon.PointBlankAccuracy > this.pointBlankAccuracy)
        {
            this.pointBlankAccuracy = weapon.PointBlankAccuracy;
        }
        if (weapon.AccuracyDropoff < this.accuracyDropoff)
        {
            this.accuracyDropoff = weapon.AccuracyDropoff;
        }
        if (weapon.MeleeDamage > this.meleeDamage)
        {
            this.meleeDamage = weapon.MeleeDamage;
        }
        if (weapon.MeleeRange > this.meleeRange && this.PointBlankAccuracy == 0f) // Only increase melee range if zero range accuracy is 0
        {
            this.meleeRange = weapon.MeleeRange;
        }
        if (weapon.MeleeAccuracy > this.meleeAccuracy)
        {
            this.meleeAccuracy = weapon.MeleeAccuracy;
        }
        if (weapon.ReloadTime < this.reloadTime)
        {
            this.reloadTime = weapon.ReloadTime;
        }
        if (weapon.FireRate < this.fireRate)
        {
            this.fireRate = weapon.FireRate;
        }
        if (weapon.SwingRate < this.swingRate)
        {
            this.swingRate = weapon.SwingRate;
        }
        return true;
    }

    /// <summary>
    /// Gets the information of the weapon blueprint as a formatted string.
    /// </summary>
    /// <returns>A string containing the weapon blueprint information.</returns>
    public override string GetBlueprintInfo()
    {
        return $"Name: {ItemName}\nType: {ItemType}\nWeight: {Weight}\nCost: {Cost}\n" +
               $"Slots Required: {slotsRequired}\nDamage: {damage}\nNum Shots: {numShots}\nMax Ammo: {maxAmmo}\n" +
               $"Zero Range Accuracy: {pointBlankAccuracy}\nAccuracy Dropoff: {accuracyDropoff}\n" +
               $"Melee Damage: {meleeDamage}\nMelee Range: {meleeRange}\nMelee Accuracy: {meleeAccuracy}\n" +
               $"Reload Time: {reloadTime}\nFire Rate: {fireRate}\nSwing Rate: {swingRate}";
    }
}
