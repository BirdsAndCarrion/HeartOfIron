using UnityEngine;
using NUnit.Framework;
using System;
using HeartOfIron.Enums;

public class TestWeapon
{
    [Test]
    public void TestWeaponCreation()
    {
        // Arrange
        string weaponName = "TestWeapon";
        Weight weaponWeight = Weight.Medium;
        int weaponCost = 100;
        int weaponDamage = 25;
        int meleeDamage = 10;
        int maxAmmo = 30;
        float reloadTime = 2.0f;
        float fireRate = 0.5f;
        float swingRate = 1.0f;
        int meleeRange = 2;
        int numShots = 1;
        int slotsRequired = 1;
        float pointBlankAccuracy = 1.0f;
        float accuracyDropoff = 0.01f;
        float meleeAccuracy = 0.9f;
        // Act
        Weapon weapon = new Weapon(WeaponName.LightPistol, weaponName, weaponWeight, weaponCost, weaponDamage, meleeDamage, maxAmmo, reloadTime, fireRate, swingRate, meleeRange, numShots, slotsRequired, pointBlankAccuracy, accuracyDropoff, meleeAccuracy);

        // Assert
        Assert.AreEqual(weaponName, weapon.Name);
        Assert.AreEqual(weaponWeight, weapon.Weight);
        Assert.AreEqual(ItemType.Weapon, weapon.Type);
        Assert.AreEqual(weaponCost, weapon.Cost);
        Assert.AreEqual(weaponDamage, weapon.Damage);
        Assert.AreEqual(meleeDamage, weapon.MeleeDamage);
        Assert.AreEqual(maxAmmo, weapon.MaxAmmo);
        Assert.AreEqual(reloadTime, weapon.ReloadTime);
        Assert.AreEqual(fireRate, weapon.FireRate);
        Assert.AreEqual(swingRate, weapon.SwingRate);
        Assert.AreEqual(meleeRange, weapon.MeleeRange);
        Assert.AreEqual(numShots, weapon.NumShots);
        Assert.AreEqual(slotsRequired, weapon.SlotsRequired);
        Assert.AreEqual(pointBlankAccuracy, weapon.PointBlankAccuracy);
        Assert.AreEqual(accuracyDropoff, weapon.AccuracyDropoff);
        Assert.AreEqual(meleeAccuracy, weapon.MeleeAccuracy);
    }

    [Test]
    public void TestWeaponDescription()
    {
        // Arrange
        string weaponName = "TestWeapon";
        Weight weaponWeight = Weight.Medium;
        int weaponCost = 100;
        int weaponDamage = 25;
        int meleeDamage = 10;
        int maxAmmo = 30;
        float reloadTime = 2.0f;
        float fireRate = 0.5f;
        float swingRate = 1.0f;
        int meleeRange = 2;
        int numShots = 1;
        int slotsRequired = 1;

        Weapon weapon = new Weapon(WeaponName.LightPistol, weaponName, weaponWeight, weaponCost, weaponDamage, meleeDamage, maxAmmo, reloadTime, fireRate, swingRate, meleeRange, numShots, slotsRequired);

        // Act
        string description = weapon.GetDescription();

        // Assert
        string expectedDescription = $"{weaponName}:\n Damage: {weaponDamage}\n Ammo: {maxAmmo}/{maxAmmo}\n Reload Time: {reloadTime}s\n Fire Rate: {fireRate}s\n Shots: {numShots}\n Slots Required: {slotsRequired}";
        Assert.AreEqual(expectedDescription, description);
    }

    [Test]
    public void TestConsumeAmmo()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        int initialAmmo = attack.CurrentAmmo;
        Assert.AreEqual(30, initialAmmo);
        Assert.AreEqual(0f, attack.Timer); // Ensure initial cooldown time is 0

        // Act
        attack.ConsumeAmmo(5);
        Assert.AreEqual(weapon.FireRate, attack.Timer); // Cooldown time should be set to fire rate after consuming ammo

        // Assert
        Assert.AreEqual(initialAmmo - 5, attack.CurrentAmmo);

        // Test consuming more ammo than available
        attack.ConsumeAmmo(50);
        Assert.AreEqual(0, attack.CurrentAmmo); // Ammo should not go below zero

        Assert.AreEqual(weapon.FireRate, attack.Timer); // Cooldown time should be set to fire rate after consuming ammo
    }

    [Test]
    public void TestSwingCooldown()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        MeleeAttack meleeAttack = weapon.GetMeleeAttack();
        Assert.IsNotNull(meleeAttack); // Ensure the melee attack is properly retrieved
        float initialCooldown = meleeAttack.Timer;
        Assert.AreEqual(0f, initialCooldown);

        meleeAttack.SetTimer(meleeAttack.Cooldown);

        Assert.AreEqual(weapon.SwingRate, meleeAttack.Timer); // Cooldown time should be set to swing rate after swinging
    }

    [Test]
    public void TestUpdateCooldown_ZeroTime()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        float initialCooldown = attack.Timer;
        Assert.AreEqual(0f, initialCooldown);

        // Act
        attack.SetTimer(attack.Cooldown);

        // Assert
        Assert.AreEqual(attack.Cooldown, attack.Timer);

        attack.UpdateTimer(1f);

        Assert.AreEqual(attack.Cooldown - 1f, attack.Timer);
    }

    [Test]
    public void TestUpdateCooldown_Decrease()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        attack.ConsumeAmmo(5); // Set cooldown time to fire rate
        float initialCooldown = attack.Timer;
        Assert.AreEqual(weapon.FireRate, initialCooldown);

        // Act
        attack.UpdateTimer(0.2f);

        // Assert
        Assert.AreEqual(initialCooldown - 0.2f, attack.Timer);
    }

    [Test]
    public void TestUpdateCooldown_LargeNumber()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        attack.ConsumeAmmo(5); // Set cooldown time to fire rate
        float initialCooldown = attack.Timer;
        Assert.AreEqual(weapon.FireRate, initialCooldown);

        // Act
        attack.UpdateTimer(100f); // Large number to ensure cooldown goes to zero

        // Assert
        Assert.AreEqual(0f, attack.Timer);
    }

    [Test]
    public void TestReload()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        attack.ConsumeAmmo(10); // Reduce ammo to test reload
        Assert.AreEqual(20, attack.CurrentAmmo); // Ensure ammo is reduced

        // Act
        attack.Reload();

        // Assert
        Assert.AreEqual(30, attack.CurrentAmmo); // Ammo should be restored to max
        Assert.AreEqual(weapon.ReloadTime, attack.Timer); // Cooldown time should be set to reload time after reloading
    }

    [Test]
    public void TestReload_AlreadyFull()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        Assert.AreEqual(30, attack.CurrentAmmo); // Ensure ammo is full

        // Act
        attack.Reload();

        // Assert
        Assert.AreEqual(30, attack.CurrentAmmo); // Ammo should remain full
        Assert.AreEqual(weapon.ReloadTime, attack.Timer); // Cooldown time should be set to reload time after reloading
    }

    [Test]
    public void TestReload_ZeroAmmo()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        attack.ConsumeAmmo(30); // Reduce ammo to zero
        Assert.AreEqual(0, attack.CurrentAmmo); // Ensure ammo is zero

        // Act
        attack.Reload();

        // Assert
        Assert.AreEqual(30, attack.CurrentAmmo); // Ammo should be restored to max
        Assert.AreEqual(weapon.ReloadTime, attack.Timer); // Cooldown time should be set to reload time after reloading
    }

    [Test]
    public void TestCalculateSpread()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        float distance = 50f; // Example distance
        float expectedSpread = weapon.AccuracyDropoff * distance;

        // Act
        float actualSpread = attack.CalculateSpread(distance);

        // Assert
        Assert.AreEqual(expectedSpread, actualSpread);
    }

    [Test]
    public void TestCalculateSpread_ZeroDistance()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        float distance = 0f; // Zero distance
        float expectedSpread = weapon.AccuracyDropoff * distance;

        // Act
        float actualSpread = attack.CalculateSpread(distance);

        // Assert
        Assert.AreEqual(expectedSpread, actualSpread);
    }

    [Test]
    public void TestCanAttack_OnCooldown()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        attack.ConsumeAmmo(5); // Set cooldown time to fire rate

        // Act
        bool canAttack = attack.CanAttack(20f);

        // Assert
        Assert.IsFalse(canAttack); // Should not be able to attack while on cooldown
    }

    [Test]
    public void TestCanAttack_NoAmmo_Ranged()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved
        attack.ConsumeAmmo(30); // Reduce ammo to zero
        attack.UpdateTimer(weapon.FireRate); // Ensure cooldown is reset to 0

        // Act
        bool canAttack = attack.CanAttack(20f);

        // Assert
        Assert.IsFalse(canAttack); // Should not be able to attack with no ammo
    }

    [Test]
    public void TestCanAttack_NoAmmo_Melee()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.LightSword, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the melee attack is properly retrieved

        attack.ConsumeAmmo(30); // Reduce ammo to zero
        attack.UpdateTimer(weapon.FireRate); // Ensure cooldown is reset to 0

        // Act
        bool canAttack = attack.CanAttack(1f); // Melee range

        // Assert
        Assert.IsTrue(canAttack); // Should be able to attack in melee range even with no ammo
    }

    [Test]
    public void TestCanAttack_Valid()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved

        // Act
        bool canAttack = attack.CanAttack(20f);

        // Assert
        Assert.IsTrue(canAttack); // Should be able to attack with ammo and not on cooldown
    }

    [Test]
    public void TestGetAverageDamage_Ranged()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.MediumRifle, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        RangedAttack attack = weapon.GetRangedAttack();
        Assert.IsNotNull(attack); // Ensure the ranged attack is properly retrieved


        // Act
        float averageDamage = attack.GetAverageDamage(20f); // Assuming a distance of 20 units

        // Assert
        Assert.AreEqual(20f, averageDamage); // Average damage should equal the weapon's damage
    }

    [Test]
    public void TestGetAverageDamage_Melee()
    {
        // Arrange
        Weapon weapon = new Weapon(WeaponName.LightSword, "TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        MeleeAttack attack = weapon.GetMeleeAttack();
        Assert.IsNotNull(attack); // Ensure the melee attack is properly retrieved

        // Act
        float averageDamage = attack.GetAverageDamage(1f); // Assuming a distance of 1 unit (melee range)

        // Assert
        Assert.AreEqual(9f, averageDamage); // Average damage should equal the weapon's melee damage multiplied by melee accuracy
    }
    
}