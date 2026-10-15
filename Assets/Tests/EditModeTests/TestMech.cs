using UnityEngine;
using System;
using NUnit.Framework;
using UnityEngine.TestTools;
using HeartOfIron.Enums;

[TestFixture]
public class TestMech
{
    private Weapon Create1HRangedWeapon()
    {
        return new Weapon(WeaponName.LightPistol, "Test1HRangedWeapon", Weight.Light, 100, 25, 15, 30, 0.8f, 0.1f, 0.5f, 5, 1, 1);
    }
    
    private Armor CreateArmor()
    {
        return new Armor("TestArmor", Weight.Light, ArmorType.Torso, 100, 0.2f, 1f, 5f, 100f);
    }

    private EnergyShield CreateEnergyShield()
    {
        return new EnergyShield("TestEnergyShield", Weight.Medium, 150, 100f, 2f, 10f);
    }

    [Test]
    public void TestMechCreation()
    {
        // Arrange
        string mechName = "TestMech";
        MechType mechType = MechType.Light;
        Faction faction = Faction.Player;
        

        // Act
        Mech mech = new Mech(mechName, mechType, faction);

        // Assert
        Assert.AreEqual(mechName, mech.Name);
        Assert.AreEqual(mechType, mech.Type);
        Assert.AreEqual(1, mech.Level);
        Assert.AreEqual(0, mech.Experience);
        Assert.AreEqual(new float[6] {100f, 100f, 100f, 100f, 100f, 100f}, mech.Health);
        Assert.AreEqual(new float[6] {100f, 100f, 100f, 100f, 100f, 100f}, mech.MaxHealth);
        Assert.AreEqual(10f, mech.MaxSpeed);
        Assert.AreEqual(10f, mech.VisionRange);
        Assert.AreEqual(90f, mech.VisionAngle);
        Assert.IsNotNull(mech.Inventory);
        Assert.AreEqual(faction, mech.Faction);
        Assert.AreEqual(Color.gray, mech.Color);
        Assert.IsNull(mech.Pilot);
    }

    [Test]
    public void TestAdvancedMechCreation()
    {
        // Arrange
        string mechName = "AdvancedMech";
        MechType mechType = MechType.Heavy;
        int level = 5;
        int experience = 200;
        float[] health = new float[6] { 80f, 90f, 70f, 60f, 50f, 40f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 8f;
        float visionRange = 15f;
        float visionAngle = 120f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Enemy;
        Color color = Color.red;
        Pilot pilot = new Pilot("TestPilot", faction);

        // Act
        Mech mech = new Mech(mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        // Assert
        Assert.AreEqual(mechName, mech.Name);
        Assert.AreEqual(mechType, mech.Type);
        Assert.AreEqual(level, mech.Level);
        Assert.AreEqual(experience, mech.Experience);
        Assert.AreEqual(health, mech.Health);
        Assert.AreEqual(maxHealth, mech.MaxHealth);
        Assert.AreEqual(maxSpeed, mech.MaxSpeed);
        Assert.AreEqual(visionRange, mech.VisionRange);
        Assert.AreEqual(visionAngle, mech.VisionAngle);
        Assert.AreEqual(inventory, mech.Inventory);
        Assert.AreEqual(faction, mech.Faction);
        Assert.AreEqual(color, mech.Color);
        Assert.AreEqual(pilot, mech.Pilot);
    }

    [Test]
    public void TestReturningMechCreation()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 90f, 80f, 70f, 60f, 50f, 40f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        // Assert
        Assert.AreEqual(mechId, mech.Id);
        Assert.AreEqual(mechName, mech.Name);
        Assert.AreEqual(mechType, mech.Type);
        Assert.AreEqual(level, mech.Level);
        Assert.AreEqual(experience, mech.Experience);
        Assert.AreEqual(health, mech.Health);
        Assert.AreEqual(maxHealth, mech.MaxHealth);
        Assert.AreEqual(maxSpeed, mech.MaxSpeed);
        Assert.AreEqual(visionRange, mech.VisionRange);
        Assert.AreEqual(visionAngle, mech.VisionAngle);
        Assert.AreEqual(inventory, mech.Inventory);
        Assert.AreEqual(faction, mech.Faction);
        Assert.AreEqual(color, mech.Color);
        Assert.AreEqual(pilot, mech.Pilot);
    }

    [Test]
    public void TestEquipWeapon()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        Weapon weapon = Create1HRangedWeapon();

        // Act
        bool equipped = mech.EquipWeapon(weapon);

        // Assert
        Assert.IsTrue(equipped);
        Assert.AreEqual(weapon, mech.Inventory.Weapons[(int)Hand.Right]);
    }

    [Test]
    public void TestEquipArmor()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        Armor armor = CreateArmor();

        // Act
        bool equipped = mech.EquipArmor(armor);

        // Assert
        Assert.IsTrue(equipped);
        Assert.AreEqual(armor, mech.Inventory.Armors[(int)ArmorType.Torso]);
    }

    [Test]
    public void TestEquipEnergyShield()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        EnergyShield energyShield = CreateEnergyShield();

        // Act
        bool equipped = mech.EquipEnergyShield(energyShield);

        // Assert
        Assert.IsTrue(equipped);
        Assert.AreEqual(energyShield, mech.Inventory.EnergyShield);
    }

    [Test]
    public void TestUnequipWeapon()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        Weapon weapon = Create1HRangedWeapon();
        mech.EquipWeapon(weapon);

        // Act
        mech.UnequipWeapon(weapon);

        // Assert
        Assert.IsNull(mech.Inventory.Weapons[(int)Hand.Right]);
    }

    [Test]
    public void TestUnequipArmor()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        Armor armor = CreateArmor();
        mech.EquipArmor(armor);

        // Act
        mech.UnequipArmor(armor);

        // Assert
        Assert.IsNull(mech.Inventory.Armors[(int)ArmorType.Torso]);
    }

    [Test]
    public void TestUnequipEnergyShield()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        EnergyShield energyShield = CreateEnergyShield();
        mech.EquipEnergyShield(energyShield);

        // Act
        mech.UnequipEnergyShield(energyShield);

        // Assert
        Assert.IsNull(mech.Inventory.EnergyShield);
    }

    [Test]
    public void TestTakeDamage_ES()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipEnergyShield(CreateEnergyShield());
        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(50f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(100f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(100f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.AreEqual(50f, mech.Inventory.EnergyShield.CurrentCapacity);
        Assert.IsFalse(mech.IsDead());
    }

    [Test]
    public void TestTakeDamage_ES_Armor()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipEnergyShield(CreateEnergyShield());
        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(150f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(100f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(65f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.AreEqual(0f, mech.Inventory.EnergyShield.CurrentCapacity);
        Assert.IsFalse(mech.IsDead());
    }

    [Test]
    public void TestTakeDamage_ES_Armor_Health()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipEnergyShield(CreateEnergyShield());
        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(250f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(85f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(0f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.AreEqual(0f, mech.Inventory.EnergyShield.CurrentCapacity);
        Assert.IsFalse(mech.IsDead());
    }

    [Test]
    public void TestTakeDamage_ES_Armor_Health_Overkill()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipEnergyShield(CreateEnergyShield());
        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(400f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(0f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(0f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.AreEqual(0f, mech.Inventory.EnergyShield.CurrentCapacity);
        Assert.IsTrue(mech.IsDead());
    }

    [Test]
    public void TestTakeDamage_Armor()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(50f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(100f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(65f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.IsFalse(mech.IsDead());
        Assert.IsNull(mech.Inventory.EnergyShield);
    }

    [Test]
    public void TestTakeDamage_Armor_Health()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(150f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(85f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(0f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.IsFalse(mech.IsDead());
        Assert.IsNull(mech.Inventory.EnergyShield);
    }

    [Test]
    public void TestTakeDamage_Armor_Health_Overkill()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(300f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(0f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(0f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.IsTrue(mech.IsDead());
        Assert.IsNull(mech.Inventory.EnergyShield);
    }

    [Test]
    public void TestTakeDamage_Health()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.TakeDamage(50f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(50f, mech.Health[(int)BodyPart.Torso]);
        Assert.IsFalse(mech.IsDead());
        Assert.IsNull(mech.Inventory.EnergyShield);
        Assert.IsNull(mech.Inventory.Armors[(int)ArmorType.Torso]);
    }

    [Test]
    public void TestTakeDamage_Health_Overkill()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);


        mech.TakeDamage(150f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(0f, mech.Health[(int)BodyPart.Torso]);
        Assert.IsTrue(mech.IsDead());
        Assert.IsNull(mech.Inventory.EnergyShield);
        Assert.IsNull(mech.Inventory.Armors[(int)ArmorType.Torso]);
    }

    [Test]
    public void TestTakeDamage_InvalidLocation()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());

        LogAssert.Expect(LogType.Error, "Invalid location index 7 for mech ReturningMech. Damage not applied.");

        mech.TakeDamage(50f, 7); // Invalid location index

        // Assert
        Assert.AreEqual(100f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(100f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.IsFalse(mech.IsDead());
        Assert.IsNull(mech.Inventory.EnergyShield);
    }

    [Test]
    public void TestRepair_Health()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.TakeDamage(50f, (int)BodyPart.Torso);

        mech.Repair(30f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(80f, mech.Health[(int)BodyPart.Torso]);
    }

    [Test]
    public void TestRepair_Health_Overheal()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.TakeDamage(50f, (int)BodyPart.Torso);

        mech.Repair(70f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(100f, mech.Health[(int)BodyPart.Torso]);
    }

    [Test]
    public void TestRepair_Health_Armor()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(150f, (int)BodyPart.Torso);

        mech.Repair(5f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(90f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(5f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
    }

    [Test]
    public void TestRepair_Health_Armor_Overheal()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());

        mech.TakeDamage(150f, (int)BodyPart.Torso);

        mech.Repair(150f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(100f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(100f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
    }

    [Test]
    public void TestRepair_Health_Armor_ES()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());
        mech.EquipEnergyShield(CreateEnergyShield());

        mech.TakeDamage(250f, (int)BodyPart.Torso);

        mech.Repair(5f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(90f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(5f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.AreEqual(5f, mech.Inventory.EnergyShield.CurrentCapacity);
    }

    [Test]
    public void TestRepair_Health_Armor_ES_Overheal()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.EquipArmor(CreateArmor());
        mech.EquipEnergyShield(CreateEnergyShield());

        mech.TakeDamage(150f, (int)BodyPart.Torso);

        mech.Repair(150f, (int)BodyPart.Torso);

        // Assert
        Assert.AreEqual(100f, mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(100f, mech.Inventory.Armors[(int)ArmorType.Torso].GetDurability(0));
        Assert.AreEqual(100f, mech.Inventory.EnergyShield.CurrentCapacity);
    }

    [Test]
    public void TestRepair_InvalidLocation()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        mech.TakeDamage(50f, (int)BodyPart.Torso);

        LogAssert.Expect(LogType.Error, "Invalid location index 7 for mech ReturningMech. Repair not applied.");

        mech.Repair(50f, 7); // Invalid location index

        // Assert
        Assert.AreEqual(50f, mech.Health[(int)BodyPart.Torso]);
    }

    [Test]
    public void TestIsLowHealth()
    {
        Guid mechId = Guid.NewGuid();
        string mechName = "ReturningMech";
        MechType mechType = MechType.Medium;
        int level = 3;
        int experience = 100;
        float[] health = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float[] maxHealth = new float[6] { 100f, 100f, 100f, 100f, 100f, 100f };
        float maxSpeed = 9f;
        float visionRange = 12f;
        float visionAngle = 100f;
        Inventory inventory = new Inventory();
        Faction faction = Faction.Player;
        Color color = Color.blue;
        Pilot pilot = new Pilot("ReturningPilot", faction);

        // Act
        Mech mech = new Mech(mechId, mechName, mechType, level, experience, health, maxHealth, maxSpeed, visionRange, visionAngle, inventory, faction, color, pilot);

        Assert.IsFalse(mech.IsLowHealth());
        Assert.IsNull(mech.Inventory.Armors[(int)ArmorType.Torso]);
        Assert.IsNull(mech.Inventory.Armors[(int)ArmorType.Head]);
        Assert.IsNull(mech.Inventory.Armors[(int)ArmorType.Arms]);
        Assert.IsNull(mech.Inventory.Armors[(int)ArmorType.Legs]);
        Assert.IsNull(mech.Inventory.EnergyShield);

        mech.TakeDamage(80f, (int)BodyPart.Head);
        mech.TakeDamage(80f, (int)BodyPart.Torso);
        mech.TakeDamage(80f, (int)BodyPart.LeftArm);
        mech.TakeDamage(80f, (int)BodyPart.RightArm);
        mech.TakeDamage(80f, (int)BodyPart.LeftLeg);
        mech.TakeDamage(80f, (int)BodyPart.RightLeg);

        // Assert
        Assert.IsTrue(mech.IsLowHealth());
    }

    [Test]
    public void TestGainExperience()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        int initialLevel = mech.Level;
        int initialExperience = mech.Experience;

        // Act
        mech.GainExperience(150);

        // Assert
        Assert.AreEqual(initialExperience + 50, mech.Experience);
        Assert.AreEqual(initialLevel + 1, mech.Level); // Assuming level up occurs at 100 experience points
    }

    [Test]
    public void TestGainExperience_LevelUp()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        int initialLevel = mech.Level;
        int initialExperience = mech.Experience;

        // Act
        mech.GainExperience(400); // Assuming level up occurs at 100 experience points

        // Assert
        Assert.AreEqual(initialExperience + 100, mech.Experience);
        Assert.AreEqual(initialLevel + 2, mech.Level); // Should level up two times
    }

    [Test]
    public void TestAddPilot()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        Pilot pilot = new Pilot("TestPilot", Faction.Player);

        // Act
        mech.AddPilot(pilot);

        // Assert
        Assert.AreEqual(pilot, mech.Pilot);
    }

    [Test]
    public void TestRemovePilot()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        Pilot pilot = new Pilot("TestPilot", Faction.Player);
        mech.AddPilot(pilot);
        Assert.AreEqual(pilot, mech.Pilot); // Ensure pilot is added before removal

        // Act
        mech.RemovePilot();

        // Assert
        Assert.IsNull(mech.Pilot);
    }

    [Test]
    public void TestUpdateName()
    {
        // Arrange
        Mech mech = new Mech("OldName", MechType.Light, Faction.Player);
        string newName = "NewName";
        Assert.AreEqual("OldName", mech.Name); // Ensure initial name is correct

        // Act
        mech.UpdateName(newName);

        // Assert
        Assert.AreEqual(newName, mech.Name);
    }

    [Test]
    public void TestSetMechColor()
    {
        // Arrange
        Mech mech = new Mech("TestMech", MechType.Light, Faction.Player);
        Color newColor = Color.red;
        Assert.AreEqual(Color.gray, mech.Color); // Ensure initial color is correct

        // Act
        mech.SetMechColor(newColor);

        // Assert
        Assert.AreEqual(newColor, mech.Color);
    }

}
