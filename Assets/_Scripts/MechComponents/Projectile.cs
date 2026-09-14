using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _damage;

    public void Initialize(float damage, float lifetime)
    {
        _damage = damage;
        Invoke(nameof(Deactivate), lifetime);
        EventRouter.RegisterWithHub(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        HandleCollision(collision);
    }

    private void Deactivate()
    {
        CancelInvoke(nameof(Deactivate)); // Cancel any pending deactivation to avoid conflicts
        EventRouter.UnregisterFromHub(this);
        gameObject.SetActive(false);
    }

    private void HandleCollision(Collision collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name);
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
        } else if (collision.gameObject.GetComponent<Cover>() != null)
        {
            Cover cover = collision.gameObject.GetComponent<Cover>();
            cover.TakeDamage(_damage);
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
        Deactivate(); // Deactivate the projectile after handling the collision
    }
}
