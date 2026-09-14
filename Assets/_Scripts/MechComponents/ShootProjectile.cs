using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ShootProjectile : MonoBehaviour
{
    private readonly int INITIAL_POOL_SIZE = 10;
    private readonly int MAX_POOL_SIZE = 30;

    [SerializeField] private GameObject projectilePrefab; // Assign this in the Unity Inspector

    [SerializeField] private float projectileSpeed = 200f; // Speed of the projectile

    [SerializeField] private Transform firePoint; // The point from which the projectile will be fired

    [SerializeField] private ParticleSystem muzzleFlash; // Optional: Particle system for muzzle flash effect
    [SerializeField] private GameObject tracerPrefab;
    private ObjectPool<GameObject> _projectilePool;
    private ObjectPool<GameObject> _tracerPool;

    // TODO: Consider adding a shooting sound effect component here if needed

    void Awake()
    {
        // Pre-instantiate a pool of projectiles
        _projectilePool = new ObjectPool<GameObject>(
            () => Instantiate(projectilePrefab),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            obj => Destroy(obj),
            false,
            INITIAL_POOL_SIZE,
            MAX_POOL_SIZE
        );
        _tracerPool = new ObjectPool<GameObject>(
            () => Instantiate(tracerPrefab),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            obj => Destroy(obj),
            false,
            INITIAL_POOL_SIZE,
            MAX_POOL_SIZE
        );
        EventRouter.ROnProjectileCollision.AddListener(OnProjectileCollisionHandler);
        EventRouter.ROnTracerDeactivation.AddListener(OnTracerDeactivationHandler);
    }

    void OnDestroy()
    {
        EventRouter.ROnProjectileCollision.RemoveListener(OnProjectileCollisionHandler);
        EventRouter.ROnTracerDeactivation.RemoveListener(OnTracerDeactivationHandler);
    }

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
        Projectile projectile = null;
        GameObject projectileObj = _projectilePool.Get();
        projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {

            projectile.Initialize(damage, distance / projectileSpeed);
            projectile.transform.position = firePoint.position;
            projectile.transform.rotation = firePoint.rotation;
            // Set the projectile's velocity
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Reset the velocity before applying the new force
                rb.angularVelocity = Vector3.zero; // Reset the angular velocity before applying the new force

                rb.MovePosition(firePoint.position); // Reset the position to the fire point before applying the new force
                rb.MoveRotation(firePoint.rotation); // Reset the rotation to the fire point before applying the new force
                Physics.SyncTransforms(); // Ensure the physics engine is aware of the updated transform before applying the new force
                rb.AddForce(direction * projectileSpeed, ForceMode.VelocityChange);
            }
            else
            {
                Debug.LogError("Projectile prefab does not have a Rigidbody component.");
            }
        } 
        else
        {
            Debug.LogError("Failed to instantiate projectile.");
        }
    }

    private void OnProjectileCollisionHandler(Projectile projectile, Transform hitTransform)
    {
        // Spawn tracer at firepoint with size of distance to the hit point
        Vector3 tracerDirection = (hitTransform.position - firePoint.position).normalized;
        float tracerDistance = Vector3.Distance(firePoint.position, hitTransform.position);
        GameObject tracer = _tracerPool.Get();
        if (tracer != null)
        {
            tracer.transform.position = firePoint.position;
            tracer.transform.rotation = Quaternion.LookRotation(tracerDirection);
            tracer.transform.localScale = new Vector3(tracer.transform.localScale.x, tracer.transform.localScale.y, tracerDistance);
        }
        DeactivateProjectile(projectile);
    }
    
    private void OnTracerDeactivationHandler(GameObject tracer)
    {
        DeactivateTracer(tracer);
    }

    private void DeactivateProjectile(Projectile projectile)
    {
        if (projectile != null)
        {
            _projectilePool.Release(projectile.gameObject);
        }
    }

    private void DeactivateTracer(GameObject tracer)
    {
        if (tracer != null)
        {
            _tracerPool.Release(tracer);
        }
    }
}
