using UnityEngine;
using System.Collections.Generic;
using HeartOfIron.Enums;

public class AttackListMount : MonoBehaviour
{
    public ActionType actionType;
    public List<RangedAttack> attacks;
    public List<MeleeAttack> meleeAttacks;
    public MechController target;

    public void AddRangedAttack(RangedAttack attack)
    {
        if (attacks == null)
            attacks = new List<RangedAttack>();
        if (HasMeleeAttacks())
            meleeAttacks.Clear();
        attacks.Add(attack);
    }

    public void AddMeleeAttack(MeleeAttack attack)
    {
        if (meleeAttacks == null)
            meleeAttacks = new List<MeleeAttack>();
        if (HasRangedAttacks())
            attacks.Clear();
        meleeAttacks.Add(attack);
    }

    public void RemoveRangedAttack(RangedAttack attack)
    {
        if (attacks != null)
            attacks.Remove(attack);
    }

    public void RemoveMeleeAttack(MeleeAttack attack)
    {
        if (meleeAttacks != null)
            meleeAttacks.Remove(attack);
    }

    public void ClearRangedAttacks()
    {
        if (attacks != null)
            attacks.Clear();
    }

    public void ClearMeleeAttacks()
    {
        if (meleeAttacks != null)
            meleeAttacks.Clear();
    }

    public void ClearAllAttacks()
    {
        ClearRangedAttacks();
        ClearMeleeAttacks();
    }

    public bool HasRangedAttacks()
    {
        return attacks != null && attacks.Count > 0;
    }

    public bool HasMeleeAttacks()
    {
        return meleeAttacks != null && meleeAttacks.Count > 0;
    }

    public float GetMinimumMeleeRange()
    {
        if (meleeAttacks == null || meleeAttacks.Count == 0)
            return 0f;
        float minRange = float.MaxValue;
        foreach (MeleeAttack attack in meleeAttacks)
        {
            if (attack != null && attack.Range < minRange)
                minRange = attack.Range;
        }
        return minRange;
    }
}
