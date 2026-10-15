using UnityEngine;
using System;
using HeartOfIron.Enums;

[Serializable]
public class Weapon : Item
{
    [Header("Basic Weapon Stats")]
    [SerializeField] private WeaponName _weaponName;
    public WeaponName WeaponName => _weaponName;
    [SerializeField] private int _damage;
    public int Damage => _damage;
    [SerializeField] private int _meleeDamage;
    public int MeleeDamage => _meleeDamage;
    [SerializeField] private int _maxAmmo;
    public int MaxAmmo => _maxAmmo;
    [SerializeField] private int _currentAmmo;
    public int CurrentAmmo => _currentAmmo;
    [SerializeField] private float _reloadTime;
    public float ReloadTime => _reloadTime;
    [SerializeField] private float _fireRate;
    public float FireRate => _fireRate;
    [SerializeField] private float _swingRate;
    public float SwingRate => _swingRate;
    [SerializeField] private int _meleeRange;
    public int MeleeRange => _meleeRange;
    [SerializeField] private int _numShots;
    public int NumShots => _numShots;
    [SerializeField] private int _slotsRequired;
    public int SlotsRequired => _slotsRequired;
    [Header("Accuracy and Spread Stats")]
    [SerializeField] private float _pointBlankAccuracy;
    public float PointBlankAccuracy => _pointBlankAccuracy;
    public bool IsMelee => _pointBlankAccuracy == 0f; // If zero range accuracy is 0, it's a melee weapon
    [SerializeField] private float _accuracyDropoff;
    public float AccuracyDropoff => _accuracyDropoff;
    [SerializeField] private float _meleeAccuracy;
    public float MeleeAccuracy => _meleeAccuracy;
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

    public RangedAttack GetRangedAttack()
    {
        if (IsMelee)
        {
            return null; // Melee weapons do not have a ranged attack
        } 
        return new RangedAttack(this, _damage, _fireRate, _meleeRange, _pointBlankAccuracy, _accuracyDropoff, _numShots, _maxAmmo);
    }

    public MeleeAttack GetMeleeAttack()
    {
        return new MeleeAttack(this, _meleeDamage, _swingRate, _meleeRange, _meleeAccuracy, _slotsRequired);
    }

    public override string GetDescription()
    {
        return $"{Name}:\n Damage: {Damage}\n Ammo: {CurrentAmmo}/{MaxAmmo}\n Reload Time: {ReloadTime}s\n Fire Rate: {FireRate}s\n Shots: {NumShots}\n Slots Required: {SlotsRequired}";
    }

    public override Item Clone()
    {
        return new Weapon(WeaponName, Name, Weight, Cost, _damage, _meleeDamage, _maxAmmo, _reloadTime, _fireRate, _swingRate, _meleeRange, _numShots, _slotsRequired, _pointBlankAccuracy, _accuracyDropoff, _meleeAccuracy);
    }
}
