using UnityEngine;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using HeartOfIron.Enums;
using HeartOfIron.Structs;

[TestFixture]
public class TestPilot
{
    [Test]
    public void TestPilotCreation()
    {
        // Arrange
        string pilotName = "TestPilot";
        Faction faction = Faction.Player;

        // Act
        Pilot pilot = new Pilot(pilotName, faction);

        // Assert
        Assert.AreEqual(pilotName, pilot.Name);
        Assert.AreEqual(faction, pilot.Faction);
        Assert.AreEqual(1, pilot.Level);
        Assert.AreEqual(0, pilot.Experience);
        Assert.AreEqual(0, pilot.UnspentSkillPoints);
        Assert.IsNotNull(pilot.Skills);
        Assert.AreEqual(4, pilot.Skills.Length);
        Assert.IsNull(pilot.AssignedMech);
        Assert.AreEqual(Color.gray, pilot.Color);
    }

    [Test]
    public void TestAdvancedPilotCreation()
    {
        string pilotName = "AdvancedPilot";
        int level = 5;
        int experience = 1000;
        Faction faction = Faction.Player;
        Color color = Color.red;

        Pilot pilot = new Pilot(pilotName, level, experience, faction, color);

        Assert.AreEqual(pilotName, pilot.Name);
        Assert.AreEqual(faction, pilot.Faction);
        Assert.AreEqual(level, pilot.Level);
        Assert.AreEqual(experience, pilot.Experience);
        Assert.AreEqual(0, pilot.UnspentSkillPoints);
        Assert.IsNotNull(pilot.Skills);
        Assert.AreEqual(4, pilot.Skills.Length);
        Assert.IsNull(pilot.AssignedMech);
        Assert.AreEqual(color, pilot.Color);
    }

    [Test]
    public void TestReturningPilotCreation()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 500;
        int unspentSkillPoints = 2;
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        Assert.AreEqual(pilotId, pilot.Id);
        Assert.AreEqual(pilotName, pilot.Name);
        Assert.AreEqual(level, pilot.Level);
        Assert.AreEqual(experience, pilot.Experience);
        Assert.AreEqual(unspentSkillPoints, pilot.UnspentSkillPoints);
        Assert.AreEqual(faction, pilot.Faction);
        Assert.AreEqual(skills, pilot.Skills);
        Assert.AreEqual(color, pilot.Color);
    }

    [Test]
    public void TestUpdateName()
    {
        string pilotName = "TestPilot";
        Faction faction = Faction.Player;
        Pilot pilot = new Pilot(pilotName, faction);

        string newName = "UpdatedPilot";
        pilot.UpdateName(newName);

        Assert.AreEqual(newName, pilot.Name);
    }

    [Test]
    public void TestSetPilotColor()
    {
        string pilotName = "TestPilot";
        Faction faction = Faction.Player;
        Pilot pilot = new Pilot(pilotName, faction);

        Color newColor = Color.blue;
        pilot.SetPilotColor(newColor);

        Assert.AreEqual(newColor, pilot.Color);
    }

    [Test]
    public void TestGetSkillValue()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 500;
        int unspentSkillPoints = 2;
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        Assert.AreEqual(0.9f, pilot.GetSkillValue(SkillType.Gunnery), 0.0001f);
        Assert.AreEqual(0.4f, pilot.GetSkillValue(SkillType.Piloting), 0.0001f);
        Assert.AreEqual(1.6f, pilot.GetSkillValue(SkillType.Perception), 0.0001f);
        Assert.AreEqual(0.1f, pilot.GetSkillValue(SkillType.Navigation), 0.0001f);
    }

    [Test]
    public void TestLevelUpSkill()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 500;
        int unspentSkillPoints = 2;
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        pilot.LevelUpSkill(SkillType.Gunnery);

        Assert.AreEqual(4, pilot.Skills[0].level); // Level goes up by 1
        Assert.AreEqual(0.4f, pilot.Skills[0].bonus, 0.0001f); // Bonus goes up by 0.1

        Assert.AreEqual(1, pilot.UnspentSkillPoints);

        Assert.AreEqual(1.6f, pilot.GetSkillValue(SkillType.Gunnery), 0.0001f);
        Assert.AreEqual(0.4f, pilot.GetSkillValue(SkillType.Piloting), 0.0001f);
        Assert.AreEqual(1.6f, pilot.GetSkillValue(SkillType.Perception), 0.0001f);
        Assert.AreEqual(0.1f, pilot.GetSkillValue(SkillType.Navigation), 0.0001f);
    }

    [Test]
    public void TestLevelUpSkillWithoutPoints()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 500;
        int unspentSkillPoints = 0; // No unspent skill points
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        pilot.LevelUpSkill(SkillType.Gunnery);

        // The skill level should not change since there are no unspent skill points
        Assert.AreEqual(3, pilot.Skills[0].level);
        Assert.AreEqual(0.3f, pilot.Skills[0].bonus, 0.0001f);

        Assert.AreEqual(0, pilot.UnspentSkillPoints);

        Assert.AreEqual(0.9f, pilot.GetSkillValue(SkillType.Gunnery), 0.0001f);
        Assert.AreEqual(0.4f, pilot.GetSkillValue(SkillType.Piloting), 0.0001f);
        Assert.AreEqual(1.6f, pilot.GetSkillValue(SkillType.Perception), 0.0001f);
        Assert.AreEqual(0.1f, pilot.GetSkillValue(SkillType.Navigation), 0.0001f);
    }

    [Test]
    public void TestIncreaseSkillBonus()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 500;
        int unspentSkillPoints = 2;
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        pilot.IncreaseSkillBonus(SkillType.Gunnery);
        Assert.AreEqual(0.6f, pilot.Skills[0].bonus, 0.0001f); // Bonus incremented by 0.3f
        Assert.AreEqual(1, pilot.UnspentSkillPoints);
        Assert.AreEqual(1.8f, pilot.GetSkillValue(SkillType.Gunnery), 0.0001f);
        Assert.AreEqual(0.4f, pilot.GetSkillValue(SkillType.Piloting), 0.0001f);
        Assert.AreEqual(1.6f, pilot.GetSkillValue(SkillType.Perception), 0.0001f);
        Assert.AreEqual(0.1f, pilot.GetSkillValue(SkillType.Navigation), 0.0001f);
    }

    [Test]
    public void TestGainExperience()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 200;
        int unspentSkillPoints = 2;
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        pilot.GainExperience(200);
        Assert.AreEqual(100, pilot.Experience); // Level up threshold is 300, so experience should roll over to 100
        Assert.AreEqual(4, pilot.Level);
        Assert.AreEqual(4, pilot.UnspentSkillPoints); // +2 skill points for leveling up
    }

    [Test]
    public void TestGainExperienceWithoutLevelUp()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 100;
        int unspentSkillPoints = 2;
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        pilot.GainExperience(100);
        Assert.AreEqual(200, pilot.Experience); // No level up yet
        Assert.AreEqual(3, pilot.Level);
        Assert.AreEqual(2, pilot.UnspentSkillPoints); // No change in skill points
    }

    [Test]
    public void TestGainExperienceMultipleLevelUps()
    {
        Guid pilotId = Guid.NewGuid();
        string pilotName = "ReturningPilot";
        int level = 3;
        int experience = 100;
        int unspentSkillPoints = 2;
        Faction faction = Faction.Player;
        PilotSkill[] skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery, 3, 0.3f),
            new PilotSkill(SkillType.Piloting, 2, 0.2f),
            new PilotSkill(SkillType.Perception, 4, 0.4f),
            new PilotSkill(SkillType.Navigation, 1, 0.1f)
        };
        Color color = Color.green;

        Pilot pilot = new Pilot(pilotId, pilotName, level, experience, unspentSkillPoints, faction, skills, color);

        pilot.GainExperience(700); // This should trigger multiple level-ups
        Assert.AreEqual(100, pilot.Experience); // After leveling up to level 5 (300 + 400), experience rolls over to 800 - 700 = 100
        Assert.AreEqual(5, pilot.Level);
        Assert.AreEqual(6, pilot.UnspentSkillPoints); // +4 skill points for leveling up twice
    }

    [Test]
    public void TestAssignAndRemoveMech()
    {
        // Arrange
        string pilotName = "TestPilot";
        Faction faction = Faction.Player;
        Pilot pilot = new Pilot(pilotName, faction);

        string mechName = "TestMech";
        MechType mechType = MechType.Light;
        Mech mech = new Mech(mechName, mechType, faction);

        // Act
        pilot.AssignedMech = mech;

        // Assert
        Assert.AreEqual(mech, pilot.AssignedMech);
        Assert.AreEqual(pilot, mech.Pilot);

        // Act - Remove the pilot from the mech
        pilot.AssignedMech = null;

        // Assert
        Assert.IsNull(pilot.AssignedMech);
        Assert.IsNull(mech.Pilot);
    }
}