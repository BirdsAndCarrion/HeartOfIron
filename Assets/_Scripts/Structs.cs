using System;
using HeartOfIron.Enums;
using UnityEngine;

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

    [Serializable]
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    [Serializable]
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    public struct DamageReport
    {
        public float remainingDamage;
        public float flagIndicator;

        public DamageReport(float remainingDamage, float flagIndicator)
        {
            this.remainingDamage = remainingDamage;
            this.flagIndicator = flagIndicator;
        }
    }
}
