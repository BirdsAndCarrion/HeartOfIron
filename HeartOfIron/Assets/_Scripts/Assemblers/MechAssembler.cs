using UnityEngine;
using System;
using HeartOfIron.Enums;

public static class MechAssembler
{
    public static Mech AssembleTestEnemyMech()
    {
        string randomName = "Enemy Mech " + UnityEngine.Random.Range(1, 1000);
        MechType randomType = (MechType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(MechType)).Length);
        Faction faction = Faction.Enemy;
        Mech testMech = new Mech(randomName, randomType, faction);
        return testMech;
    }

    public static Mech AssembleNewMech(MechBlueprint blueprint)
    {
        string name = blueprint.ItemName;
        MechType type = blueprint.MechType;
        Faction faction = Faction.Player; // Assuming new mechs are for the player
        Mech newMech = new Mech(name, type, faction);
        return newMech;
    }
}
