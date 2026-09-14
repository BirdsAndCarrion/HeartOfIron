
namespace HeartOfIron.Enums
{
    public enum MechType
    {
        Light,
        Medium,
        Heavy
    }

    public enum Faction
    {
        Player,
        Enemy,
        Neutral
    }

    public enum ActionType
    {
        Shoot,
        Melee,
        Reload,
        Move,
        Charge,
        Hunker,
        Overwatch,
    }

    public enum CombatState
    {
        Loading,
        Paused,
        Playing,
        GameOver,
        OutOfCombat
    }

    public enum MechState
    {
        Idle,
        Moving,
        Brawling,
        Charging,
        Overwatch,
        Hunkering,
        Dead
    }

    public enum DisplayType
    {
        LongValue,
        ShortValue,
        Percentage,
        None
    }

    public enum ItemType
    {
        Weapon,
        Armor,
        EnergyShield,
        Mech,
    }

    public enum Weight
    {
        Light,
        Medium,
        Heavy
    }

    public enum ArmorType
    {
        Head,
        Torso,
        Arms,
        Legs
    }

    public enum Hand
    {
        Right,
        Left
    }

    public enum BodyPart
    {
        Head,
        Torso,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg
    }

    public enum SkillType
    {
        Gunnery,
        Piloting,
        Perception,
        Navigation,

    }

    public enum SceneName
    {
        MainMenu,
        CombatScene,
        MessHall,
        EngiBay,
        Garage,
        Lab,
        Loading
    }

    public enum TerrainType
    {
        Plains,
        Forest,
        Mountain,
    }

    public enum EnemyState
    {
        Wander,
        Search,
        Overwatch,
        Group,

        Hunker,
        HoldPos,
        Retreat,

        StaticShoot,
        TargetShoot,
        TeamShoot,
        AdvShoot,
        RetShoot,
        FlankShoot,

        Commit,
        Rush,
        HitAndRun,
        Prowl,
        Peel
    }

    public enum EnemyRole
    {
        Assault,
        Guerilla,
        Sniper,
        Brawler,
        Flanker,
        Hybrid
    }

    public enum ActionResult
    {
        Success,
        NullInput,
        Failure,
        Retry
    }

    public enum EndCondition
    {
        Victory,
        Survived,
        Defeat,
    }
}
