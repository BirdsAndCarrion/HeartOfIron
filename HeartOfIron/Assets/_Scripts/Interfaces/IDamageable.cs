using UnityEngine;

public interface IDamageable
{
    float TakeDamage(float damageAmount, int location);

    void Repair(float repairAmount, int location = 0);
}
