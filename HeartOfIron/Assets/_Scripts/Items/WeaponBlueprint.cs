using UnityEngine;
using System;
using HeartOfIron.Enums;

[CreateAssetMenu(fileName = "WeaponBlueprint", menuName = "Scriptable Objects/WeaponBlueprint")]
[Serializable]
public class WeaponBlueprint : ItemBlueprint
{
    [Header("Weapon Properties")]
    [SerializeField] private int slotsRequired;
    public int SlotsRequired => slotsRequired;
    [Header("Weapon Shooting Stats")]
    [SerializeField] private int damage;
    public int Damage => damage;
    [SerializeField] private int numShots;
    public int NumShots => numShots;
    [SerializeField] private int maxAmmo;
    public int MaxAmmo => maxAmmo;
    [Header("Weapon Shooting Accuracy Stats")]
    [SerializeField] private float pointBlankAccuracy;
    public float PointBlankAccuracy => pointBlankAccuracy;
    [SerializeField] private float accuracyDropoff;
    public float AccuracyDropoff => accuracyDropoff;
    [Header("Weapon Melee Stats")]
    [SerializeField] private int meleeDamage;
    public int MeleeDamage => meleeDamage;
    [SerializeField] private int meleeRange;
    public int MeleeRange => meleeRange;
    [Header("Weapon Melee Accuracy Stat")]

    [SerializeField] private float meleeAccuracy;
    public float MeleeAccuracy => meleeAccuracy;

    [Header("Weapon Timing Stats")]
    [SerializeField] private float reloadTime;
    public float ReloadTime => reloadTime;
    [SerializeField] private float fireRate;
    public float FireRate => fireRate;
    [SerializeField] private float swingRate;
    public float SwingRate => swingRate;

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

    public override string GetBlueprintInfo()
    {
        return $"Name: {ItemName}\nType: {ItemType}\nWeight: {Weight}\nCost: {Cost}\n" +
               $"Slots Required: {slotsRequired}\nDamage: {damage}\nNum Shots: {numShots}\nMax Ammo: {maxAmmo}\n" +
               $"Zero Range Accuracy: {pointBlankAccuracy}\nAccuracy Dropoff: {accuracyDropoff}\n" +
               $"Melee Damage: {meleeDamage}\nMelee Range: {meleeRange}\nMelee Accuracy: {meleeAccuracy}\n" +
               $"Reload Time: {reloadTime}\nFire Rate: {fireRate}\nSwing Rate: {swingRate}";
    }
}
