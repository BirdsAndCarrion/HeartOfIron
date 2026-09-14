using UnityEngine;

public class Cover : MonoBehaviour, IDamageable
{
    private float health = 100f;
    public float Health => health;
    private float maxHealth = 100f;
    public float MaxHealth => maxHealth;

    public void Initialize(float initialHealth)
    {
        health = initialHealth;
        maxHealth = initialHealth;
    }

    void Update()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public float TakeDamage(float damageAmount, int location = 1)
    {
        health -= damageAmount;
        if (health < 0f) health = 0f;
        // TODO: Implement any additional effects or logic when the cover takes damage
        return damageAmount;
    }

    public void Repair(float repairAmount, int location = 1)
    {
        health += repairAmount;
        // TODO: Implement any additional effects or logic when the cover is repaired
        if (health > maxHealth) health = maxHealth;
    }
}
