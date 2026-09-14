using UnityEngine;
using System;
using HeartOfIron.Enums;
using HeartOfIron.Structs;

[Serializable]
public class Pilot
{
    private readonly float BONUS_INCREMENT = 0.3f; // Example: Increase bonus by 0.3 for each level up
    [SerializeField] private Guid _id;
    public Guid Id => _id;
    [SerializeField] private string _name;
    public string Name => _name;
    [SerializeField] private int _level;
    public int Level => _level;
    [SerializeField] private int _experience;
    public int Experience => _experience;
    [SerializeField] private int _unspentSkillPoints;
    public int UnspentSkillPoints => _unspentSkillPoints;
    [SerializeField] private Faction _faction;
    public Faction Faction => _faction;
    [SerializeField] private PilotSkill[] _skills;
    [SerializeField] private Color _color;
    [NonSerialized] private Mech _assignedMech;
    public Mech AssignedMech
    {
        get => _assignedMech;
        set
        {
            // Ensure that pilot is removed from the previous mech before assigning to a new one
            if (_assignedMech != value)
            {
                if (_assignedMech != null && _assignedMech.Pilot == this)
                {
                    _assignedMech.RemovePilot(); // Remove the pilot from the previous mech
                }
                // Assign the new mech to the pilot
                _assignedMech = value;
                // Ensure that the new mech has this pilot assigned
                if (_assignedMech != null)
                {
                    if (_assignedMech.Pilot != null && _assignedMech.Pilot != this)
                    {
                        _assignedMech.Pilot.AssignedMech = null; // Unassign the previous pilot from the mech
                    }
                    _assignedMech.RemovePilot(); // Remove the pilot from the mech
                    _assignedMech.AddPilot(this); // Assign this pilot to the new mech
                    // Debug.Log($"Pilot {_name} assigned to mech {_assignedMech.Name}");
                } else if (_assignedMech == null)
                {
                    // Debug.Log($"Pilot {_name} is now unassigned from any mech.");
                }
            }
        }
    }
    public Color Color => _color;
    public PilotSkill[] Skills
    {
        get
        {
            EnsureSkillsInitialized();
            return _skills;
        }
    }

    private void EnsureSkillsInitialized()
    {
        if (_skills != null)
        {
            return;
        }
        Debug.Log($"Initializing skills for Pilot {_name}.");
        _skills = new PilotSkill[]
        {
            new PilotSkill(SkillType.Gunnery),
            new PilotSkill(SkillType.Piloting),
            new PilotSkill(SkillType.Perception),
            new PilotSkill(SkillType.Navigation)
        };
    }

    public Pilot(string name, Faction faction)
    {
        _id = Guid.NewGuid();
        _name = name;
        _level = 1;
        _experience = 0;
        _faction = faction;
        _color = Color.gray;
        EnsureSkillsInitialized();
    }

    public Pilot(string name, int level, int experience, Faction faction, Color color)
    {
        _id = Guid.NewGuid();
        _name = name;
        _level = level;
        _experience = experience;
        _faction = faction;
        _color = color;
        EnsureSkillsInitialized();
    }

    public Pilot(Guid id, string name, int level, int experience, int unspentSkillPoints, Faction faction, PilotSkill[] pilotSkills, Color color)
    {
        _id = id;
        _name = name;
        _level = level;
        _experience = experience;
        _faction = faction;
        _color = color;
        _unspentSkillPoints = unspentSkillPoints;
        _skills = pilotSkills;
        EnsureSkillsInitialized();
    }

    public float GetSkillValue(SkillType skillType)
    {
        EnsureSkillsInitialized();

        for (int i = 0; i < _skills.Length; i++)
        {
            if (_skills[i].SkillType == skillType)
            {
                return _skills[i].level * _skills[i].bonus;
            }
        }
        return 0f; // Return 0 if the skill type is not found
    }

    public string GetPilotInfo()
    {
        return $"Name: {Name}\nLevel: {Level}\nExperience: {Experience}\nFaction: {Faction}\nSkills:\n" +
               $"Gunnery: {GetSkillValue(SkillType.Gunnery)}\n" +
               $"Piloting: {GetSkillValue(SkillType.Piloting)}\n" +
               $"Perception: {GetSkillValue(SkillType.Perception)}\n" +
               $"Navigation: {GetSkillValue(SkillType.Navigation)}";
    }

    public void GainExperience(int amount)
    {
        _experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int experienceThreshold = _level * 100; // Example threshold calculation, you can adjust this formula as needed
        while (_experience >= experienceThreshold)
        {
            _experience -= experienceThreshold;
            LevelUp();
            experienceThreshold = _level * 100; // Update required experience for next level
        }
    }

    private void LevelUp()
    {
        _level++;
        Debug.Log($"Pilot {_name} leveled up to level {_level}!");
        _unspentSkillPoints += 2; // Example: Gain 2 skill points per level, adjust as needed
    }

    public void LevelUpSkill(SkillType skillType)
    {
        if (_unspentSkillPoints <= 0)
        {
            Debug.LogWarning($"Pilot {_name} has no unspent skill points.");
            return;
        }

        for (int i = 0; i < _skills.Length; i++)
        {
            if (_skills[i].SkillType == skillType)
            {
                _skills[i].LevelUp();
                _unspentSkillPoints--;
                Debug.Log($"Pilot {_name} leveled up {skillType} to level {_skills[i].level}. Remaining unspent skill points: {_unspentSkillPoints}");
                return;
            }
        }

        Debug.LogWarning($"Skill type {skillType} not found for pilot {_name}.");
    }

    public void IncreaseSkillBonus(SkillType skillType)
    {
        if (_unspentSkillPoints <= 0)
        {
            Debug.LogWarning($"Pilot {_name} has no unspent skill points.");
            return;
        }
        for (int i = 0; i < _skills.Length; i++)
        {
            if (_skills[i].SkillType == skillType)
            {
                _skills[i].IncreaseBonus(BONUS_INCREMENT);
                _unspentSkillPoints--;
                Debug.Log($"Pilot {_name} increased {skillType} bonus by {BONUS_INCREMENT}. New bonus: {_skills[i].bonus}");
                return;
            }
        }

        Debug.LogWarning($"Skill type {skillType} not found for pilot {_name}.");
    }

    public void UpdateName(string newName)
    {
        _name = newName;
        Debug.Log($"Pilot's name updated to: {_name}");
    }

    public void SetPilotColor(Color newColor)
    {
        _color = newColor;
        Debug.Log($"Pilot {_name}'s color updated to: {_color}");
    }
}
