using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    private float _damage;

    public void Initialize(float damage, float lifetime)
    {
        _damage = damage;
        Destroy(gameObject, lifetime); // Destroy the projectile after its lifetime expires
    }

    void OnCollisionEnter(Collision collision)
    {
        // Handle collision logic here, e.g., apply damage to the target
        if (collision.gameObject.GetComponent<Hitbox>() != null)
        {
            Hitbox hitbox = collision.gameObject.GetComponent<Hitbox>();
            IDamageable damageable = hitbox.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                int hitLocation = hitbox.GetLocationIndex(); // Get the hit location index from the Hitbox
                damageable.TakeDamage(_damage, hitLocation);
            }
        }
        // Get the BulletImpact child object
        Transform bulletImpact = transform.Find("BulletImpact");
        if (bulletImpact != null)
        {
            // Play the particle system on the ExplosionParticles and DustParticles child objects
            ParticleSystem explosionParticles = bulletImpact.Find("ExplosionParticles")?.GetComponent<ParticleSystem>();
            ParticleSystem dustParticles = bulletImpact.Find("DustParticles")?.GetComponent<ParticleSystem>();
            if (explosionParticles != null)
            {
                explosionParticles.Play();
            }
            if (dustParticles != null)
            {
                dustParticles.Play();
            }
        }
        Destroy(gameObject); // Destroy the projectile on impact
    }
}
