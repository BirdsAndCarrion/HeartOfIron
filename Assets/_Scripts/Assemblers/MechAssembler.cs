using System;
using HeartOfIron.Enums;

/// <summary>
/// Assembles mechs based on their blueprints.
/// Provides functionality to create test enemy mechs and new player mechs from their respective blueprints.
/// </summary>
public static class MechAssembler
{
    /// <summary>
    /// Assembles a test enemy mech with a random name and type.
    /// </summary>
    /// <returns>The assembled test enemy mech.</returns>
    public static Mech AssembleTestEnemyMech()
    {
        string randomName = "Enemy Mech " + UnityEngine.Random.Range(1, 1000);
        MechType randomType = (MechType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(MechType)).Length);
        Faction faction = Faction.Enemy;
        Mech testMech = new Mech(randomName, randomType, faction);
        return testMech;
    }

    /// <summary>
    /// Assembles a new mech based on the provided blueprint.
    /// </summary>
    /// <param name="blueprint">The blueprint of the mech to assemble.</param>
    /// <returns>The assembled mech.</returns>
    public static Mech AssembleNewMech(MechBlueprint blueprint)
    {
        string name = blueprint.ItemName;
        MechType type = blueprint.MechType;
        Faction faction = Faction.Player; // Assuming new mechs are for the player
        Mech newMech = new Mech(name, type, faction);
        return newMech;
    }
}
