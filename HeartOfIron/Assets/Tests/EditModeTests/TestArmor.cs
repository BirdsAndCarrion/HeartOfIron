using UnityEngine;
using NUnit.Framework;
using System;
using HeartOfIron.Enums;

[TestFixture]
public class TestArmor
{
    [Test]
    public void TestArmorCreation()
    {
        // Arrange
        string armorName = "TestArmor";
        Weight armorWeight = Weight.Medium;
        ArmorType armorType = ArmorType.Torso;
        int armorCost = 100;
        float armorResistance = 0.2f;
        float armorCoverage = 0.8f;
        float armorDefenseRating = 5f;
        float armorMaxDurability = 100f;

        // Act
        Armor armor = new Armor(armorName, armorWeight, armorType, armorCost,
            armorResistance, armorCoverage, armorDefenseRating, armorMaxDurability);

        // Assert
        Assert.AreEqual(armorName, armor.Name);
        Assert.AreEqual(armorWeight, armor.Weight);
        Assert.AreEqual(armorType, armor.ArmorType);
        Assert.AreEqual(armorCost, armor.Cost);
        Assert.AreEqual(armorResistance, armor.Resistance);
        Assert.AreEqual(armorCoverage, armor.Coverage);
        Assert.AreEqual(armorDefenseRating, armor.DefenseRating);
        Assert.AreEqual(armorMaxDurability, armor.MaxDurability);
        Assert.AreEqual(armorMaxDurability, armor.CurrentDurabilityR);
        Assert.AreEqual(armorMaxDurability, armor.CurrentDurabilityL);
    }

    [Test]
    public void TestGetDurability()
    {
        // Arrange
        Armor armor = new Armor("TestArmor", Weight.Medium, ArmorType.Torso, 100,
            0.2f, 0.8f, 5f, 100f);

        // Act & Assert
        Assert.AreEqual(100f, armor.GetDurability(0)); // Right side
        Assert.AreEqual(100f, armor.GetDurability(1)); // Left side
    }

    [Test]
    public void TestTakeDamage_FullCoverage()
    {
        // Arrange
        Armor armor = new Armor("TestArmor", Weight.Medium, ArmorType.Torso, 100,
            0.2f, 1.0f, 5f, 100f); // Full coverage for testing

        // Act
        float damageTakenR = armor.TakeDamage(50f, 0); // Right side
        float damageTakenL = armor.TakeDamage(30f, 1); // Left side

        // Assert
        Assert.AreEqual(0f, damageTakenR, 0.01f); // Effective damage after resistance and defense rating
        Assert.AreEqual(0f, damageTakenL, 0.01f); // Effective damage after resistance and defense rating
        Assert.AreEqual(65f, armor.CurrentDurabilityR, 0.01f); // Remaining durability on right side
        Assert.AreEqual(81f, armor.CurrentDurabilityL, 0.01f); // Remaining durability on left side
    }

    [Test]
    public void TestTakeDamage_NoCoverage()
    {
        // Arrange
        Armor armor = new Armor("TestArmor", Weight.Medium, ArmorType.Torso, 100,
            0.2f, 0.0f, 5f, 100f); // No coverage for testing

        // Act
        float damageTakenR = armor.TakeDamage(50f, 0); // Right side
        float damageTakenL = armor.TakeDamage(30f, 1); // Left side

        // Assert
        Assert.AreEqual(0f, damageTakenR, 0.01f); // Effective damage after defense rating only
        Assert.AreEqual(0f, damageTakenL, 0.01f); // Effective damage after defense rating only
        Assert.AreEqual(55f, armor.CurrentDurabilityR, 0.01f); // Durability should remain unchanged on right side
        Assert.AreEqual(75f, armor.CurrentDurabilityL, 0.01f); // Durability should remain unchanged on left side
    }

    [Test]
    public void TestTakeDamage_DurabilityDepletion()
    {
        // Arrange
        Armor armor = new Armor("TestArmor", Weight.Medium, ArmorType.Torso, 100,
            0.0f, 0.0f, 0f, 100f); // No resistance, no coverage, no defense rating for testing durability depletion

        // Act
        float damageTakenR = armor.TakeDamage(120f, 0); // Right side
        float damageTakenL = armor.TakeDamage(120f, 1); // Left side

        // Assert
        Assert.AreEqual(20f, damageTakenR, 0.01f); // Effective damage after resistance and defense rating
        Assert.AreEqual(20f, damageTakenL, 0.01f); // Effective damage after resistance and defense rating
        Assert.AreEqual(0f, armor.CurrentDurabilityR, 0.01f); // Durability should not go below zero on right side
        Assert.AreEqual(0f, armor.CurrentDurabilityL, 0.01f); // Durability should not go below zero on left side
    }

    [Test]
    public void TestRepair()
    {
        // Arrange
        Armor armor = new Armor("TestArmor", Weight.Medium, ArmorType.Torso, 100,
            0.2f, 0.0f, 5f, 100f);

        // Act
        armor.TakeDamage(50f, 0); // Right side
        armor.TakeDamage(30f, 1); // Left side
        armor.Repair(20f, 0); // Repair right side
        armor.Repair(15f, 1); // Repair left side

        // Assert
        Assert.AreEqual(75f, armor.CurrentDurabilityR, 0.01f); // Remaining durability on right side after repair
        Assert.AreEqual(90f, armor.CurrentDurabilityL, 0.01f); // Remaining durability on left side after repair
    }

    [Test]
    public void TestRepair_ExceedMaxDurability()
    {
        // Arrange
        Armor armor = new Armor("TestArmor", Weight.Medium, ArmorType.Torso, 100,
            0.2f, 0.8f, 5f, 100f);

        // Act
        armor.TakeDamage(50f, 0); // Right side
        armor.Repair(60f, 0); // Attempt to repair beyond max durability

        // Assert
        Assert.AreEqual(100f, armor.CurrentDurabilityR, 0.01f); // Durability should not exceed max durability on right side
    }

    [Test]
    public void TestGetDescription()
    {
        // Arrange
        Armor armor = new Armor("TestArmor", Weight.Medium, ArmorType.Torso, 100,
            0.2f, 0.8f, 5f, 100f);

        // Act
        string description = armor.GetDescription();

        // Assert
        string expectedDescription = "TestArmor: Type: Torso\n Resistance: 0.2\n Coverage: 0.8\n Defense Rating: 5\n Durability: 100/100 (Right)\n 100/100 (Left)";
        Assert.AreEqual(expectedDescription, description);
    }
}