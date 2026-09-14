
public interface IDamageable
{
    float TakeDamage(float damageAmount, int location = 1);

    void Repair(float repairAmount, int location = 1);
}
