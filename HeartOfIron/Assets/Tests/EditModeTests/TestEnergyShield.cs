using UnityEngine;
using NUnit.Framework;
using HeartOfIron.Enums;

[TestFixture]
public class TestEnergyShield
{
    [Test]
    public void TestEnergyShieldCreation()
    {
        // Arrange
        string shieldName = "TestShield";
        Weight shieldWeight = Weight.Medium;
        int shieldCost = 150;
        float shieldCapacity = 200f;
        float shieldRechargeRate = 5f;
        float shieldRechargeDelay = 2f;

        // Act
        EnergyShield energyShield = new EnergyShield(shieldName, shieldWeight, shieldCost, shieldCapacity, shieldRechargeRate, shieldRechargeDelay);

        // Assert
        Assert.AreEqual(shieldName, energyShield.Name);
        Assert.AreEqual(shieldWeight, energyShield.Weight);
        Assert.AreEqual(shieldCost, energyShield.Cost);
        Assert.AreEqual(shieldCapacity, energyShield.CurrentCapacity);
        Assert.AreEqual(shieldRechargeRate, energyShield.RechargeRate);
        Assert.AreEqual(shieldRechargeDelay, energyShield.RechargeDelay);
    }

    [Test]
    public void TestEnergyShieldTakeDamage()
    {
        EnergyShield energyShield = new EnergyShield("TestShield", Weight.Medium, 150, 200f, 5f, 2f);
        float damageAmount = 50f;

        float remainingDamage = energyShield.TakeDamage(damageAmount);
        Assert.AreEqual(0f, remainingDamage); // Assuming the shield absorbs all damage
        Assert.AreEqual(150f, energyShield.CurrentCapacity); // Current capacity should be reduced by damage amount
        Assert.AreEqual(energyShield.RechargeDelay, energyShield.RechargeTimer); // Recharge timer should be set to recharge delay
    }

    [Test]
    public void TestEnergyShieldUpdateRecharge()
    {
        EnergyShield energyShield = new EnergyShield("TestShield", Weight.Medium, 150, 200f, 5f, 2f);
        energyShield.TakeDamage(50f); // Reduce capacity to 150
        float deltaTime = 1f;

        // Simulate time passing for recharge delay
        energyShield.UpdateRecharge(deltaTime);
        Assert.AreEqual(1f, energyShield.RechargeTimer); // Recharge timer should decrease

        // Simulate time passing for recharge
        energyShield.UpdateRecharge(deltaTime);
        Assert.AreEqual(0f, energyShield.RechargeTimer); // Recharge timer should be zero
        Assert.AreEqual(150f, energyShield.CurrentCapacity);

        // Simulate recharge over time
        energyShield.UpdateRecharge(deltaTime);
        Assert.AreEqual(155f, energyShield.CurrentCapacity); // Current capacity should increase by recharge rate
    }

    [Test]
    public void TestEnergyShieldUpdateRechargeDoesNotExceedMaxCapacity()
    {
        EnergyShield energyShield = new EnergyShield("TestShield", Weight.Medium, 150, 200f, 10f, 2f);
        energyShield.TakeDamage(50f); // Reduce capacity to 150
        float deltaTime = 10f; // Simulate a long time passing

        // Simulate recharge over time
        energyShield.UpdateRecharge(deltaTime);
        Assert.AreEqual(200f, energyShield.CurrentCapacity); // Current capacity should not exceed max capacity
    }

    [Test]
    public void TestEnergyShieldRepair()
    {
        EnergyShield energyShield = new EnergyShield("TestShield", Weight.Medium, 150, 200f, 5f, 2f);
        energyShield.TakeDamage(50f); // Reduce capacity to 150

        energyShield.Repair(30f);
        Assert.AreEqual(180f, energyShield.CurrentCapacity); // Current capacity should increase by repair amount

        energyShield.Repair(50f); // Attempt to repair beyond max capacity
        Assert.AreEqual(200f, energyShield.CurrentCapacity); // Current capacity should not exceed max capacity
    }

    [Test]
    public void TestEnergyShieldGetDescription()
    {
        EnergyShield energyShield = new EnergyShield("TestShield", Weight.Medium, 150, 200f, 5f, 2f);
        string description = energyShield.GetDescription();

        string expectedDescription = "TestShield: Max Capacity: 200\n Current Capacity: 200\n Recharge Rate: 5\n Recharge Delay: 2";
        Assert.AreEqual(expectedDescription, description);
    }
}