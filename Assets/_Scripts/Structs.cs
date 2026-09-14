using System;
using HeartOfIron.Enums;

namespace HeartOfIron.Structs
{

    [Serializable]
    public struct PilotSkill
    {
        public SkillType SkillType;
        public int level;
        public float bonus;

        public PilotSkill(SkillType skillType, int level, float bonus)
        {
            SkillType = skillType;
            this.level = level;
            this.bonus = bonus;
        }

        public PilotSkill(SkillType skillType)
        {
            SkillType = skillType;
            level = 1;
            bonus = 0f;
        }

        public void LevelUp()
        {
            level++;
            bonus += 0.1f; // Example: Increase bonus by 0.1 for each level up
        }

        public void IncreaseBonus(float amount)
        {
            bonus += amount;
        }
    }
}
