using UnityEngine;

public class MeleeAttack : Attack
{
    float range;
    public float Range => range;
    float accuracy;
    public float Accuracy => accuracy;
    int handsRequired;
    public int HandsRequired => handsRequired;
    

    public MeleeAttack(Weapon weapon, int damage, float cooldown, float range, float accuracy, int handsRequired)
        : base(weapon, damage, cooldown)
    {
        this.range = range;
        this.accuracy = accuracy;
        this.handsRequired = handsRequired;
    }

    public override float GetAverageDamage(float distance)
    {
        if (distance > Range)
        {
            return 0f; // Target is out of melee range
        }
        return Damage * Accuracy; // Average damage for melee attacks
    }

    public override bool CanAttack(float distance)
    {
        if (distance > Range)
        {
            Debug.LogWarning($"{Weapon.Name} cannot attack because the target is out of melee range of {Range}.");
            return false;
        } else if (!IsReady())
        {
            Debug.LogWarning($"{Weapon.Name} cannot attack because it is on cooldown.");
            return false;
        }
        return true;
    }
}
