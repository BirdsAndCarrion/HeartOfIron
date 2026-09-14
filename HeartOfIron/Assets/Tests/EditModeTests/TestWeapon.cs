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
        Weapon weapon = new Weapon(weaponName, weaponWeight, weaponCost, weaponDamage, meleeDamage, maxAmmo, reloadTime, fireRate, swingRate, meleeRange, numShots, slotsRequired, pointBlankAccuracy, accuracyDropoff, meleeAccuracy);

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

        Weapon weapon = new Weapon(weaponName, weaponWeight, weaponCost, weaponDamage, meleeDamage, maxAmmo, reloadTime, fireRate, swingRate, meleeRange, numShots, slotsRequired);

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
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        int initialAmmo = weapon.CurrentAmmo;
        Assert.AreEqual(30, initialAmmo);
        Assert.AreEqual(0f, weapon.CooldownTime); // Ensure initial cooldown time is 0

        // Act
        weapon.ConsumeAmmo(5);
        Assert.AreEqual(weapon.FireRate, weapon.CooldownTime); // Cooldown time should be set to fire rate after consuming ammo

        // Assert
        Assert.AreEqual(initialAmmo - 5, weapon.CurrentAmmo);

        // Test consuming more ammo than available
        weapon.ConsumeAmmo(50);
        Assert.AreEqual(0, weapon.CurrentAmmo); // Ammo should not go below zero

        Assert.AreEqual(weapon.FireRate, weapon.CooldownTime); // Cooldown time should be set to fire rate after consuming ammo
    }

    [Test]
    public void TestSwingCooldown()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        float initialCooldown = weapon.CooldownTime;
        Assert.AreEqual(0f, initialCooldown);

        weapon.SwingCooldown();

        Assert.AreEqual(weapon.SwingRate, weapon.CooldownTime); // Cooldown time should be set to swing rate after swinging
    }

    [Test]
    public void TestUpdateCooldown_ZeroTime()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        float initialCooldown = weapon.CooldownTime;
        Assert.AreEqual(0f, initialCooldown);

        // Act
        weapon.UpdateCooldown(0f);

        // Assert
        Assert.AreEqual(initialCooldown, weapon.CooldownTime);

        weapon.UpdateCooldown(1f);

        Assert.AreEqual(initialCooldown, weapon.CooldownTime);
    }

    [Test]
    public void TestUpdateCooldown_Decrease()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        weapon.ConsumeAmmo(5); // Set cooldown time to fire rate
        float initialCooldown = weapon.CooldownTime;
        Assert.AreEqual(weapon.FireRate, initialCooldown);

        // Act
        weapon.UpdateCooldown(0.2f);

        // Assert
        Assert.AreEqual(initialCooldown - 0.2f, weapon.CooldownTime);
    }

    [Test]
    public void TestUpdateCooldown_LargeNumber()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        weapon.ConsumeAmmo(5); // Set cooldown time to fire rate
        float initialCooldown = weapon.CooldownTime;
        Assert.AreEqual(weapon.FireRate, initialCooldown);

        // Act
        weapon.UpdateCooldown(100f); // Large number to ensure cooldown goes to zero

        // Assert
        Assert.AreEqual(0f, weapon.CooldownTime);
    }

    [Test]
    public void TestReload()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        weapon.ConsumeAmmo(10); // Reduce ammo to test reload
        Assert.AreEqual(20, weapon.CurrentAmmo); // Ensure ammo is reduced

        // Act
        weapon.Reload();

        // Assert
        Assert.AreEqual(30, weapon.CurrentAmmo); // Ammo should be restored to max
        Assert.AreEqual(weapon.ReloadTime, weapon.CooldownTime); // Cooldown time should be set to reload time after reloading
    }

    [Test]
    public void TestReload_AlreadyFull()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        Assert.AreEqual(30, weapon.CurrentAmmo); // Ensure ammo is full

        // Act
        weapon.Reload();

        // Assert
        Assert.AreEqual(30, weapon.CurrentAmmo); // Ammo should remain full
        Assert.AreEqual(weapon.ReloadTime, weapon.CooldownTime); // Cooldown time should be set to reload time after reloading
    }

    [Test]
    public void TestReload_ZeroAmmo()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        weapon.ConsumeAmmo(30); // Reduce ammo to zero
        Assert.AreEqual(0, weapon.CurrentAmmo); // Ensure ammo is zero

        // Act
        weapon.Reload();

        // Assert
        Assert.AreEqual(30, weapon.CurrentAmmo); // Ammo should be restored to max
        Assert.AreEqual(weapon.ReloadTime, weapon.CooldownTime); // Cooldown time should be set to reload time after reloading
    }

    [Test]
    public void TestCalculateSpread()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        float distance = 50f; // Example distance
        float expectedSpread = weapon.AccuracyDropoff * distance;

        // Act
        float actualSpread = weapon.CalculateSpread(distance);

        // Assert
        Assert.AreEqual(expectedSpread, actualSpread);
    }

    [Test]
    public void TestCalculateSpread_ZeroDistance()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        float distance = 0f; // Zero distance
        float expectedSpread = weapon.AccuracyDropoff * distance;

        // Act
        float actualSpread = weapon.CalculateSpread(distance);

        // Assert
        Assert.AreEqual(expectedSpread, actualSpread);
    }

    [Test]
    public void TestCanAttack_OnCooldown()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        weapon.ConsumeAmmo(5); // Set cooldown time to fire rate

        // Act
        bool canAttack = weapon.CanAttack(20f);

        // Assert
        Assert.IsFalse(canAttack); // Should not be able to attack while on cooldown
    }

    [Test]
    public void TestCanAttack_NoAmmo_Ranged()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        weapon.ConsumeAmmo(30); // Reduce ammo to zero
        weapon.UpdateCooldown(weapon.FireRate); // Ensure cooldown is reset to 0

        // Act
        bool canAttack = weapon.CanAttack(20f);

        // Assert
        Assert.IsFalse(canAttack); // Should not be able to attack with no ammo
    }

    [Test]
    public void TestCanAttack_NoAmmo_Melee()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);
        weapon.ConsumeAmmo(30); // Reduce ammo to zero
        weapon.UpdateCooldown(weapon.FireRate); // Ensure cooldown is reset to 0

        // Act
        bool canAttack = weapon.CanAttack(1f); // Melee range

        // Assert
        Assert.IsTrue(canAttack); // Should be able to attack in melee range even with no ammo
    }

    [Test]
    public void TestCanAttack_Valid()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);

        // Act
        bool canAttack = weapon.CanAttack(20f);

        // Assert
        Assert.IsTrue(canAttack); // Should be able to attack with ammo and not on cooldown
    }

    [Test]
    public void TestGetAverageDamage_Ranged()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);


        // Act
        float averageDamage = weapon.GetAverageDamage(20f); // Assuming a distance of 20 units

        // Assert
        Assert.AreEqual(20f, averageDamage); // Average damage should equal the weapon's damage
    }

    [Test]
    public void TestGetAverageDamage_Melee()
    {
        // Arrange
        Weapon weapon = new Weapon("TestWeapon", Weight.Medium, 100, 25, 10, 30, 2.0f, 0.5f, 1.0f, 2, 1, 1);

        // Act
        float averageDamage = weapon.GetAverageDamage(1f); // Assuming a distance of 1 unit (melee range)

        // Assert
        Assert.AreEqual(9f, averageDamage); // Average damage should equal the weapon's melee damage multiplied by melee accuracy
    }
    
}