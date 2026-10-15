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

    public override string GetDescription()
    {
        return $"{Name}:\n Damage: {Damage}\n Ammo: {CurrentAmmo}/{MaxAmmo}\n Reload Time: {ReloadTime}s\n Fire Rate: {FireRate}s\n Shots: {NumShots}\n Slots Required: {SlotsRequired}";
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (cooldownTime > 0)
        {
            cooldownTime -= deltaTime;
            if (cooldownTime < 0)
            {
                cooldownTime = 0; // Ensure cooldown time doesn't go below zero
            }
        }
    }

    public void ConsumeAmmo(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Amount to consume must be greater than zero.");
            return;
        }

        _currentAmmo -= amount;
        if (_currentAmmo < 0)
        {
            _currentAmmo = 0; // Ensure current ammo doesn't go below zero
        }
        cooldownTime = FireRate; // Set cooldown time to fire rate after consuming ammo
        Debug.Log($"{Name} consumed {amount} ammo. Current ammo: {_currentAmmo}/{_maxAmmo}");
    }

    public void SwingCooldown()
    {
        cooldownTime = SwingRate; // Set cooldown time to swing rate after a melee attack
        Debug.Log($"{Name} is on swing cooldown for {SwingRate} seconds.");
    }

    public float CalculateSpread(float distance)
    {
        if (distance <= 0)
        {
            Debug.LogWarning("Distance must be greater than zero to calculate accuracy.");
            return 0f;
        }

        float spread = 1f - PointBlankAccuracy + (AccuracyDropoff * distance);
        return Mathf.Clamp(spread, 0f, 1f);
    }

    public void Reload()
    {
        _currentAmmo = _maxAmmo;
        cooldownTime = ReloadTime; // Set cooldown time to reload time after reloading
        Debug.Log($"{Name} is reloading for {ReloadTime} seconds.");
        Debug.Log($"{Name} reloaded. Current ammo: {_currentAmmo}/{_maxAmmo}");
    }

    public bool CanAttack(float distance)
    {
        if (cooldownTime > 0)
        {
            Debug.LogWarning($"{Name} cannot attack because it is on cooldown for {cooldownTime} more seconds.");
            return false;
        }
        if (distance > MeleeRange && _currentAmmo <= 0)
        {
            Debug.LogWarning($"{Name} cannot attack because the target is out of range and there are no shots available.");
            return false;
        }

        return true;
    }

    public bool CanReload()
    {
        if (cooldownTime > 0)
        {
            Debug.LogWarning($"{Name} cannot reload because it is on cooldown for {cooldownTime} more seconds.");
            return false;
        }
        return true;
    }

    public float GetAverageDamage(float distance)
    {
        if (distance < MeleeRange)
        {
            return MeleeDamage * MeleeAccuracy; // Average damage for melee attacks
        }
        float spread = CalculateSpread(distance);
        float hitChance = 1f - spread; // Assuming hit chance is inversely proportional to spread
        float averageDamage = Damage * hitChance;
        return averageDamage;
    }

    public override Item Clone()
    {
        return new Weapon(WeaponName, Name, Weight, Cost, _damage, _meleeDamage, _maxAmmo, _reloadTime, _fireRate, _swingRate, _meleeRange, _numShots, _slotsRequired, _pointBlankAccuracy, _accuracyDropoff, _meleeAccuracy);
    }
}
