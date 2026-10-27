using UnityEngine;
using NUnit.Framework;
using HeartOfIron.Enums;

[TestFixture]
public class TestInventory
{

    private Mech CreateTestMech()
    {
        return new Mech("TestMech", MechType.Light, Faction.Player);
    }

    private Weapon Create1HRangedWeapon()
    {
        return new Weapon(WeaponName.LightPistol, "Test1HRangedWeapon", Weight.Light, 100, 25, 15, 30, 0.8f, 0.1f, 0.5f, 5, 1, 1);
    }

    private Weapon Create2HRangedWeapon()
    {
        return new Weapon(WeaponName.MediumRifle, "Test2HRangedWeapon", Weight.Medium, 150, 35, 20, 40, 0.7f, 0.15f, 0.6f, 5, 1, 2);
    }

    private Weapon Create1HMeleeWeapon()
    {
        return new Weapon(WeaponName.LightSword, "Test1HMeleeWeapon", Weight.Light, 80, 0, 40, 0, 0f, 0f, 0.4f, 5, 1, 1, 0f, 1f, 0.9f);
    }

    private Weapon Create2HMeleeWeapon()
    {
        return new Weapon(WeaponName.MediumSword, "Test2HMeleeWeapon", Weight.Medium, 120, 0, 50, 0, 0f, 0f, 0.5f, 5, 1, 2, 0f, 1f, 0.85f);
    }

    private Armor CreateArmor()
    {
        return new Armor("TestArmor", Weight.Light, ArmorType.Torso, 100, 0.2f, 0.8f, 5f, 100f);
    }

    private EnergyShield CreateEnergyShield()
    {
        return new EnergyShield("TestEnergyShield", Weight.Medium, 150, 100f, 2f, 10f);
    }


    [Test]
    public void TestEmptyInventoryCreation()
    {
        // Act
        Inventory inventory = new Inventory();

        // Assert
        Assert.IsNotNull(inventory.Weapons);
        Assert.AreEqual(2, inventory.Weapons.Length);
        Assert.IsNull(inventory.Weapons[(int)Hand.Right]);
        Assert.IsNull(inventory.Weapons[(int)Hand.Left]);

        Assert.IsNotNull(inventory.Armors);
        Assert.AreEqual(4, inventory.Armors.Length);
        Assert.IsNull(inventory.Armors[(int)ArmorType.Head]);
        Assert.IsNull(inventory.Armors[(int)ArmorType.Torso]);
        Assert.IsNull(inventory.Armors[(int)ArmorType.Arms]);
        Assert.IsNull(inventory.Armors[(int)ArmorType.Legs]);

        Assert.IsNull(inventory.EnergyShield);
    }

    [Test]
    public void TestInventoryCreation()
    {
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = Create1HMeleeWeapon();
        Armor armor = CreateArmor();
        EnergyShield energyShield = CreateEnergyShield();

        // Act
        Inventory inventory = new Inventory(CreateTestMech(), weapon1, weapon2, null, armor, null, null, energyShield);

        // Assert
        Assert.AreEqual(weapon1, inventory.Weapons[(int)Hand.Right]);
        Assert.AreEqual(weapon2, inventory.Weapons[(int)Hand.Left]);
        Assert.IsNotNull(inventory.Armors);
        Assert.IsNull(inventory.Armors[(int)ArmorType.Head]);
        Assert.AreEqual(armor, inventory.Armors[(int)ArmorType.Torso]);
        Assert.IsNull(inventory.Armors[(int)ArmorType.Arms]);
        Assert.IsNull(inventory.Armors[(int)ArmorType.Legs]);
        Assert.AreEqual(energyShield, inventory.EnergyShield);
    }

    [Test]
    public void TestHasItem()
    {
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = Create1HMeleeWeapon();
        Armor armor = CreateArmor();
        EnergyShield energyShield = CreateEnergyShield();

        Inventory inventory = new Inventory(CreateTestMech(), weapon1, weapon2, null, armor, null, null, energyShield);

        // Assert
        Assert.IsTrue(inventory.HasItem(weapon1));
        Assert.IsTrue(inventory.HasItem(weapon2));
        Assert.IsTrue(inventory.HasItem(armor));
        Assert.IsTrue(inventory.HasItem(energyShield));

        // Test for an item not in the inventory
        Weapon weaponNotInInventory = Create2HRangedWeapon();
        Assert.IsFalse(inventory.HasItem(weaponNotInInventory));
    }

    [Test]
    public void TestEquipWeapon()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon = Create1HRangedWeapon();

        // Act
        bool equipped = inventory.EquipWeapon(weapon, owner);

        // Assert
        Assert.IsTrue(equipped);
        Assert.AreEqual(weapon, inventory.Weapons[(int)Hand.Right]);
        Assert.IsNull(inventory.Weapons[(int)Hand.Left]);
    }

    [Test]
    public void TestEquipWeapon_TwoHandedWeapon()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon twoHandedWeapon = Create2HRangedWeapon();

        // Act
        bool equipped = inventory.EquipWeapon(twoHandedWeapon, owner);

        // Assert
        Assert.IsTrue(equipped);
        Assert.AreEqual(twoHandedWeapon, inventory.Weapons[(int)Hand.Right]);
        Assert.AreEqual(twoHandedWeapon, inventory.Weapons[(int)Hand.Left]); // Both slots should be occupied by the two-handed weapon
    }

    [Test]
    public void TestEquipWeapon_TwoIdenticalWeapons()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = Create1HRangedWeapon(); // Another instance of the same weapon

        // Act
        bool equippedFirst = inventory.EquipWeapon(weapon1, owner);
        bool equippedSecond = inventory.EquipWeapon(weapon2, owner);

        // Assert
        Assert.IsTrue(equippedFirst);
        Assert.IsTrue(equippedSecond);
        Assert.AreEqual(weapon1, inventory.Weapons[(int)Hand.Right]);
        Assert.AreEqual(weapon2, inventory.Weapons[(int)Hand.Left]);
    }

    [Test]
    public void TestEquipWeapon_SameWeaponTwice()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon = Create1HRangedWeapon();

        // Act
        bool equippedFirst = inventory.EquipWeapon(weapon, owner);
        bool equippedSecond = inventory.EquipWeapon(weapon, owner); // Attempt to equip the same weapon again

        // Assert
        Assert.IsTrue(equippedFirst);
        Assert.IsFalse(equippedSecond); // Should not be able to equip the same weapon twice
        Assert.AreEqual(weapon, inventory.Weapons[(int)Hand.Right]);
        Assert.IsNull(inventory.Weapons[(int)Hand.Left]);
    }

    [Test]
    public void TestEquipWeapon_FullInventory()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = Create1HMeleeWeapon();
        Weapon weapon3 = Create1HRangedWeapon(); // This weapon will not fit

        // Act
        bool equippedFirst = inventory.EquipWeapon(weapon1, owner);
        bool equippedSecond = inventory.EquipWeapon(weapon2, owner);
        bool equippedThird = inventory.EquipWeapon(weapon3, owner); // Attempt to equip a third weapon

        // Assert
        Assert.IsTrue(equippedFirst);
        Assert.IsTrue(equippedSecond);
        Assert.IsFalse(equippedThird); // Should not be able to equip the third weapon
        Assert.AreEqual(weapon1, inventory.Weapons[(int)Hand.Right]);
        Assert.AreEqual(weapon2, inventory.Weapons[(int)Hand.Left]);
    }

    [Test]
    public void TestEquipWeapon_FullInventoryWithTwoHandedWeapon()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon twoHandedWeapon = Create2HRangedWeapon();
        Weapon weapon1 = Create1HRangedWeapon(); // This weapon will not fit

        // Act
        bool equippedTwoHanded = inventory.EquipWeapon(twoHandedWeapon, owner);
        bool equippedFirst = inventory.EquipWeapon(weapon1, owner); // Attempt to equip a one-handed weapon after equipping a two-handed weapon

        // Assert
        Assert.IsTrue(equippedTwoHanded);
        Assert.IsFalse(equippedFirst); // Should not be able to equip the one-handed weapon
        Assert.AreEqual(twoHandedWeapon, inventory.Weapons[(int)Hand.Right]);
        Assert.AreEqual(twoHandedWeapon, inventory.Weapons[(int)Hand.Left]); // Both slots should be occupied by the two-handed weapon
    }

    [Test]
    public void TestEquipArmor()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Armor armor = CreateArmor();

        // Act
        bool equipped = inventory.EquipArmor(armor, owner);

        // Assert
        Assert.IsTrue(equipped);
        Assert.AreEqual(armor, inventory.Armors[(int)armor.ArmorType]);
    }

    [Test]
    public void TestEquipArmor_FullInventory()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Armor armor1 = CreateArmor();
        Armor armor2 = new Armor("TestArmor2", Weight.Medium, ArmorType.Torso, 120, 0.3f, 0.7f, 6f, 120f); // Another armor for the same slot

        // Act
        bool equippedFirst = inventory.EquipArmor(armor1, owner);
        bool equippedSecond = inventory.EquipArmor(armor2, owner); // Attempt to equip another armor in the same slot

        // Assert
        Assert.IsTrue(equippedFirst);
        Assert.IsFalse(equippedSecond); // Should not be able to equip the second armor in the same slot
        Assert.AreEqual(armor1, inventory.Armors[(int)armor1.ArmorType]);
    }

    [Test]
    public void TestEquipEnergyShield()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        EnergyShield energyShield = CreateEnergyShield();

        // Act
        bool equipped = inventory.EquipEnergyShield(energyShield, owner);

        // Assert
        Assert.IsTrue(equipped);
        Assert.AreEqual(energyShield, inventory.EnergyShield);
    }

    [Test]
    public void TestEquipEnergyShield_InventoryFull()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        EnergyShield energyShield1 = CreateEnergyShield();
        EnergyShield energyShield2 = new EnergyShield("TestEnergyShield2", Weight.Medium, 200, 120f, 3f, 15f); // Another energy shield

        // Act
        bool equippedFirst = inventory.EquipEnergyShield(energyShield1, owner);
        bool equippedSecond = inventory.EquipEnergyShield(energyShield2, owner); // Attempt to equip another energy shield

        // Assert
        Assert.IsTrue(equippedFirst);
        Assert.IsFalse(equippedSecond); // Should not be able to equip the second energy shield
        Assert.AreEqual(energyShield1, inventory.EnergyShield);
    }

    [Test]
    public void TestUnequipWeapon()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon = Create1HRangedWeapon();

        // Equip the weapon first
        bool equipped = inventory.EquipWeapon(weapon, owner);
        Assert.IsTrue(equipped);

        // Act
        inventory.UnequipWeapon(weapon);

        // Assert
        Assert.IsNull(inventory.Weapons[(int)Hand.Right]);
        Assert.IsNull(inventory.Weapons[(int)Hand.Left]);
    }

    [Test]
    public void TestUnequipWeapon_NotEquipped_EmptyInventory()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon = Create1HRangedWeapon();

        // Act
        inventory.UnequipWeapon(weapon); // Attempt to unequip a weapon that is not equipped

        // Assert
        Assert.IsNull(inventory.Weapons[(int)Hand.Right]);
        Assert.IsNull(inventory.Weapons[(int)Hand.Left]);
    }

    [Test]
    public void TestUnequipWeapon_NotEquipped_DifferentWeapon()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon equippedWeapon = Create1HRangedWeapon();
        Weapon differentWeapon = Create2HRangedWeapon();

        // Equip the first weapon
        bool equipped = inventory.EquipWeapon(equippedWeapon, owner);
        Assert.IsTrue(equipped);

        // Act
        inventory.UnequipWeapon(differentWeapon); // Attempt to unequip a different weapon that is not equipped

        // Assert
        Assert.AreEqual(equippedWeapon, inventory.Weapons[(int)Hand.Right]);
        Assert.IsNull(inventory.Weapons[(int)Hand.Left]);
    }

    [Test]
    public void TestUnequipArmor()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Armor armor = CreateArmor();

        // Equip the armor first
        bool equipped = inventory.EquipArmor(armor, owner);
        Assert.IsTrue(equipped);

        // Act
        inventory.UnequipArmor(armor);

        // Assert
        Assert.IsNull(inventory.Armors[(int)armor.ArmorType]);
    }

    [Test]
    public void TestUnequipArmor_NotEquipped_EmptyInventory()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Armor armor = CreateArmor();

        // Act
        inventory.UnequipArmor(armor); // Attempt to unequip armor that is not equipped

        // Assert
        Assert.IsNull(inventory.Armors[(int)armor.ArmorType]);
    }

    [Test]
    public void TestUnequipArmor_NotEquipped_DifferentArmor()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Armor equippedArmor = CreateArmor();
        Armor differentArmor = new Armor("TestArmor2", Weight.Medium, ArmorType.Torso, 120, 0.3f, 0.7f, 6f, 120f); // Another armor for the same slot

        // Equip the first armor
        bool equipped = inventory.EquipArmor(equippedArmor, owner);
        Assert.IsTrue(equipped);

        // Act
        inventory.UnequipArmor(differentArmor); // Attempt to unequip a different armor that is not equipped

        // Assert
        Assert.AreEqual(equippedArmor, inventory.Armors[(int)equippedArmor.ArmorType]);
    }

    [Test]
    public void TestUnequipEnergyShield()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        EnergyShield energyShield = CreateEnergyShield();

        // Equip the energy shield first
        bool equipped = inventory.EquipEnergyShield(energyShield, owner);
        Assert.IsTrue(equipped);

        // Act
        inventory.UnequipEnergyShield(energyShield);

        // Assert
        Assert.IsNull(inventory.EnergyShield);
    }

    [Test]
    public void TestUnequipEnergyShield_NotEquipped_EmptyInventory()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();

        // Act
        inventory.UnequipEnergyShield(CreateEnergyShield()); // Attempt to unequip energy shield when none is equipped

        // Assert
        Assert.IsNull(inventory.EnergyShield);
    }

    [Test]
    public void TestUnequipEnergyShield_NotEquipped_DifferentEnergyShield()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        EnergyShield equippedEnergyShield = CreateEnergyShield();
        EnergyShield differentEnergyShield = new EnergyShield("TestEnergyShield2", Weight.Medium, 200, 120f, 3f, 15f); // Another energy shield

        // Equip the first energy shield
        bool equipped = inventory.EquipEnergyShield(equippedEnergyShield, owner);
        Assert.IsTrue(equipped);
        Assert.AreEqual(equippedEnergyShield, inventory.EnergyShield);

        // Act
        inventory.UnequipEnergyShield(differentEnergyShield); // Attempt to unequip a different energy shield that is not equipped

        // Assert
        Assert.AreEqual(equippedEnergyShield, inventory.EnergyShield);
    }

    [Test]
    public void TestCanEquipItem()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon = Create1HRangedWeapon();
        Armor armor = CreateArmor();
        EnergyShield energyShield = CreateEnergyShield();

        // Act & Assert
        Assert.IsTrue(inventory.CanEquipItem(weapon));
        Assert.IsTrue(inventory.CanEquipItem(armor));
        Assert.IsTrue(inventory.CanEquipItem(energyShield));

        // Equip the items
        inventory.EquipWeapon(weapon, owner);
        inventory.EquipArmor(armor, owner);
        inventory.EquipEnergyShield(energyShield, owner);

        // Now these items should not be equippable again since they are already owned by the mech
        Assert.IsFalse(inventory.CanEquipItem(weapon));
        Assert.IsFalse(inventory.CanEquipItem(armor));
        Assert.IsFalse(inventory.CanEquipItem(energyShield));
    }

    [Test]
    public void TestGetIdealRangedWeapon()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = new Weapon(WeaponName.LightPistol, "OtherTest1HRangedWeapon", Weight.Light, 100, 20, 10, 25, 0.9f, 0.1f, 0.5f, 5, 1, 1, 0.75f, 0.1f, 0.5f); // Another ranged weapon
        inventory.EquipWeapon(weapon1, owner);
        inventory.EquipWeapon(weapon2, owner);

        float distance = 15f; // Test distance

        RangedAttack idealWeapon = inventory.GetIdealRangedAttack(distance);

        // Assert
        Assert.IsNotNull(idealWeapon);
        Assert.AreEqual(weapon1.GetRangedAttack(), idealWeapon);
    }

    [Test]
    public void TestGetIdealRangedWeapon_NoRangedWeapons()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon meleeWeapon = Create1HMeleeWeapon();
        inventory.EquipWeapon(meleeWeapon, owner);

        float distance = 15f; // Test distance

        RangedAttack idealWeapon = inventory.GetIdealRangedAttack(distance);

        // Assert
        Assert.IsNull(idealWeapon); // Should return null since there are no ranged weapons
    }

    [Test]
    public void TestGetIdealRangedWeapon_EqualWeapons()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = Create1HRangedWeapon(); // Another instance of the same weapon
        inventory.EquipWeapon(weapon1, owner);
        inventory.EquipWeapon(weapon2, owner);

        float distance = 15f; // Test distance

        RangedAttack idealWeapon = inventory.GetIdealRangedAttack(distance);

        // Assert
        Assert.IsNotNull(idealWeapon);
        Assert.AreEqual(weapon1.GetRangedAttack(), idealWeapon); // Should return the first weapon since both are equal
    }

    [Test]
    public void TestGetIdealRangedWeapon_OutOfAmmo()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = new Weapon(WeaponName.LightPistol, "OtherTest1HRangedWeapon", Weight.Light, 100, 20, 10, 25, 0.9f, 0.1f, 0.5f, 5, 1, 1, 0.75f, 0.1f, 0.5f); // Another ranged weapon
        inventory.EquipWeapon(weapon1, owner);
        inventory.EquipWeapon(weapon2, owner);

        float distance = 15f; // Test distance
        RangedAttack idealWeapon1 = inventory.GetIdealRangedAttack(distance);

        // Deplete ammo for both weapons
        idealWeapon1.ConsumeAmmo(idealWeapon1.CurrentAmmo); // Set current ammo to 0
        idealWeapon1.SetTimer(idealWeapon1.Cooldown); // Update cooldown to ensure it's ready to fire

        RangedAttack idealWeapon2 = inventory.GetIdealRangedAttack(distance);

        // Assert
        Assert.IsNotNull(idealWeapon2);
        Assert.AreEqual(weapon2.GetRangedAttack(), idealWeapon2); // Should return weapon2 since weapon1
    }

    [Test]
    public void TestGetIdealRangedWeapon_MeleeRange()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = new Weapon(WeaponName.LightPistol, "OtherTest1HRangedWeapon", Weight.Light, 100, 20, 10, 25, 0.9f, 0.1f, 0.5f, 5, 1, 1, 0.75f, 0.1f, 0.5f); // Another ranged weapon
        inventory.EquipWeapon(weapon1, owner);
        inventory.EquipWeapon(weapon2, owner);

        float distance = 0f; // Test distance that is within melee range

        RangedAttack idealWeapon = inventory.GetIdealRangedAttack(distance);

        // Assert
        Assert.IsNull(idealWeapon); // Should return null since both weapons are ranged and cannot attack at melee range
    }

    [Test]
    public void TestGetIdealMeleeWeapon()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon meleeWeapon1 = Create1HMeleeWeapon();
        Weapon meleeWeapon2 = Create1HMeleeWeapon(); // Another melee weapon
        inventory.EquipWeapon(meleeWeapon1, owner);
        inventory.EquipWeapon(meleeWeapon2, owner);

        float distance = 1f; // Test distance within melee range

        MeleeAttack idealMeleeWeapon = inventory.GetIdealMeleeAttack(distance);

        // Assert
        Assert.IsNotNull(idealMeleeWeapon);
        Assert.AreEqual(meleeWeapon1.GetMeleeAttack(), idealMeleeWeapon); // Should return the melee weapon with the highest average damage
    }

    [Test]
    public void TestGetInfo()
    {
        Mech owner = CreateTestMech();
        Inventory inventory = new Inventory();
        Weapon weapon1 = Create1HRangedWeapon();
        Weapon weapon2 = Create1HMeleeWeapon();
        Armor armor = CreateArmor();
        EnergyShield energyShield = CreateEnergyShield();

        inventory.EquipWeapon(weapon1, owner);
        inventory.EquipWeapon(weapon2, owner);
        inventory.EquipArmor(armor, owner);
        inventory.EquipEnergyShield(energyShield, owner);

        string info = inventory.GetInfo();

        string expectedInfo = "Inventory:\n" +
                              "Weapons:\n" +
                              $"- {weapon1.Name}\n" +
                              $"- {weapon2.Name}\n" +
                              "Armors:\n" +
                              "- Empty Slot\n" +
                              $"- {armor.Name} (Torso)\n" +
                              "- Empty Slot\n" +
                              "- Empty Slot\n" +
                              "Energy Shield:\n" +
                              $"- {energyShield.Name}\n";
        // Assert
        Assert.AreEqual(expectedInfo, info);
    }
}