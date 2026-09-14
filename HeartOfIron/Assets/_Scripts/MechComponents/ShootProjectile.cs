using UnityEngine;
using System;

public class ShootProjectile : MonoBehaviour
{

    [SerializeField] private GameObject projectilePrefab; // Assign this in the Unity Inspector

    [SerializeField] private float projectileSpeed = 20f; // Speed of the projectile

    [SerializeField] private Transform firePoint; // The point from which the projectile will be fired

    [SerializeField] private ParticleSystem muzzleFlash; // Optional: Particle system for muzzle flash effect

    public void Shoot(Vector3 direction, float damage, float distance)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Projectile prefab or fire point is not assigned.");
            return;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play(); // Play the muzzle flash effect
        }
        // TODO: Add logic for playing shooting sound effects here if needed

        // Instantiate the projectile at the fire point's position and rotation
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        projectile.GetComponent<Projectile>().Initialize(damage, distance / projectileSpeed); // Set the damage and lifetime based on distance and speed

        // Set the projectile's velocity
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Rigidbody component.");
        }
    }
}
