using UnityEngine;
using System;
using HeartOfIron.Enums;

/// <summary>
/// Represents a weapon item, including its stats, attacks, and description.
/// </summary>
[Serializable]
public class Weapon : Item
{
    [Header("Basic Weapon Stats")]
    /// <summary>
    /// The name of the type of weapon (e.g., Sword, Gun, Axe).
    /// </summary>
    [SerializeField] private WeaponName _weaponName;
    public WeaponName WeaponName => _weaponName;
    /// <summary>
    /// The damage dealt by the weapon.
    /// </summary>
    [SerializeField] private int _damage;
    public int Damage => _damage;
    /// <summary>
    /// The melee damage dealt by the weapon.
    /// </summary>
    [SerializeField] private int _meleeDamage;
    public int MeleeDamage => _meleeDamage;
    /// <summary>
    /// The maximum ammo capacity of the weapon.
    /// </summary>
    [SerializeField] private int _maxAmmo;
    public int MaxAmmo => _maxAmmo;
    /// <summary>
    /// The current ammo count of the weapon.
    /// </summary>
    [SerializeField] private int _currentAmmo;
    public int CurrentAmmo => _currentAmmo;
    /// <summary>
    /// The time it takes to reload the weapon.
    /// </summary>
    [SerializeField] private float _reloadTime;
    public float ReloadTime => _reloadTime;
    /// <summary>
    /// The rate of fire of the weapon.
    /// </summary>
    [SerializeField] private float _fireRate;
    public float FireRate => _fireRate;
    /// <summary>
    /// The swing rate of the weapon (for melee attacks).
    /// </summary>
    [SerializeField] private float _swingRate;
    public float SwingRate => _swingRate;
    /// <summary>
    /// The melee range of the weapon.
    /// </summary>
    [SerializeField] private int _meleeRange;
    public int MeleeRange => _meleeRange;
    /// <summary>
    /// The number of shots fired by the weapon.
    /// </summary>
    [SerializeField] private int _numShots;
    public int NumShots => _numShots;
    /// <summary>
    /// The number of slots required to equip the weapon.
    /// </summary>
    [SerializeField] private int _slotsRequired;
    public int SlotsRequired => _slotsRequired;
    [Header("Accuracy and Spread Stats")]
    /// <summary>
    /// The accuracy of the weapon at point-blank range.
    /// </summary>
    [SerializeField] private float _pointBlankAccuracy;
    public float PointBlankAccuracy => _pointBlankAccuracy;
    /// <summary>
    /// Indicates whether the weapon is a melee weapon.
    /// </summary>
    public bool IsMelee => _pointBlankAccuracy == 0f; // If zero range accuracy is 0, it's a melee weapon
    /// <summary>
    /// The accuracy dropoff of the weapon over distance.
    /// </summary>
    [SerializeField] private float _accuracyDropoff;
    public float AccuracyDropoff => _accuracyDropoff;
    /// <summary>
    /// The accuracy of the weapon for melee attacks.
    /// </summary>
    [SerializeField] private float _meleeAccuracy;
    public float MeleeAccuracy => _meleeAccuracy;
    /// <summary>
    /// The cooldown time of the weapon between attacks.
    /// </summary>
    [SerializeField] private float cooldownTime;
    public float CooldownTime => cooldownTime;

    public Weapon(WeaponName weaponName, string itemName, Weight weight, int cost, int damage, int meleeDamage, int maxAmmo, float reloadTime, float fireRate, float swingRate, int meleeRange, int numShots, int slotsRequired, float pointBlankAccuracy = 1.0f, float accuracyDropoff = 0.01f, float meleeAccuracy = 0.9f)
        : base(itemName, weight, ItemType.Weapon, cost)
    {
        _weaponName = weaponName;
        _damage = damage;
        _meleeDamage = meleeDamage;
        _maxAmmo = maxAmmo;
        _currentAmmo = maxAmmo; // Initialize current ammo to max ammo
        _reloadTime = reloadTime;
        _fireRate = fireRate;
        _swingRate = swingRate;
        _meleeRange = meleeRange;
        _numShots = numShots;
        _slotsRequired = slotsRequired;
        _pointBlankAccuracy = pointBlankAccuracy;
        _accuracyDropoff = accuracyDropoff;
        _meleeAccuracy = meleeAccuracy;
    }

    /// <summary>
    /// Gets the ranged attack associated with the weapon.
    /// </summary>
    /// <returns>The ranged attack if applicable; otherwise, null.</returns>
    public RangedAttack GetRangedAttack()
    {
        if (IsMelee)
        {
            return null; // Melee weapons do not have a ranged attack
        } 
        return new RangedAttack(this, _damage, _fireRate, _meleeRange, _pointBlankAccuracy, _accuracyDropoff, _numShots, _maxAmmo);
    }

    /// <summary>
    /// Gets the melee attack associated with the weapon.
    /// </summary>
    /// <returns>The melee attack.</returns>
    public MeleeAttack GetMeleeAttack()
    {
        return new MeleeAttack(this, _meleeDamage, _swingRate, _meleeRange, _meleeAccuracy, _slotsRequired);
    }

    /// <summary>
    /// Gets a textual description of the weapon, including its stats.
    /// </summary>
    /// <returns>A string describing the weapon.</returns>
    public override string GetDescription()
    {
        if (IsMelee)
        {
            return $"{Name}:\n Weight: {Weight}\n Slots: {SlotsRequired}\n Type: {WeaponName}\n Melee Damage: {MeleeDamage}\n Swing Rate: {MathF.Round(SwingRate, 2)}s\n Melee Range: {MathF.Round(MeleeRange, 2)}\n Melee Accuracy: {MathF.Round(MeleeAccuracy, 2)}";
        }
        return $"{Name}:\n Weight: {Weight}\n Slots: {SlotsRequired}\n Type: {WeaponName}\n Damage: {Damage}\n Fire Rate: {MathF.Round(FireRate, 2)}s\n Shots: {NumShots}\n Point Blank Accuracy: {PointBlankAccuracy}\n Accuracy Dropoff: {AccuracyDropoff}\n Ammo: {MaxAmmo}\n\n Melee Damage: {MeleeDamage}\n Swing Rate: {MathF.Round(SwingRate, 2)}s\n Melee Range: {MathF.Round(MeleeRange, 2)}\n Melee Accuracy: {MathF.Round(MeleeAccuracy, 2)}";
    }
    
    /// <summary>
    /// Creates a copy of the weapon.
    /// </summary>
    /// <returns>A new instance of the weapon with the same properties.</returns>
    public override Item Clone()
    {
        return new Weapon(WeaponName, Name, Weight, Cost, _damage, _meleeDamage, _maxAmmo, _reloadTime, _fireRate, _swingRate, _meleeRange, _numShots, _slotsRequired, _pointBlankAccuracy, _accuracyDropoff, _meleeAccuracy);
    }
}
