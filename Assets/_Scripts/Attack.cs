using UnityEngine;

public abstract class Attack
{
    private Weapon weapon;
    public Weapon Weapon => weapon;
    private int damage;
    public int Damage => damage;
    private float cooldown;
    public float Cooldown => cooldown;
    private float timer;
    public float Timer => timer;

    public Attack(Weapon weapon, int damage, float cooldown)
    {
        this.weapon = weapon;
        this.damage = damage;
        this.cooldown = cooldown;
        this.timer = 0f; // Initialize the timer to 0
    }

    public abstract float GetAverageDamage(float distance);

    public void UpdateTimer(float deltaTime)
    {
        if (timer > 0)
        {
            timer -= deltaTime;
            if (timer < 0)
            {
                timer = 0; // Ensure timer doesn't go below zero
            }
        }
    }

    public bool IsReady()
    {
        return timer <= 0;
    }
    
    public abstract bool CanAttack(float distance);

    public void SetTimer(float time)
    {
        timer = time; // Set the timer to the specified time value
    }

    public void ResetTimer()
    {
        timer = 0f; // Reset the timer to 0
    }
}
