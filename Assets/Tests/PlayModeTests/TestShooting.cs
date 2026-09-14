using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine.SceneManagement;

public class TestShooting
{

    private GameObject gO;
    private ShootProjectile shootProjectile;
    private GameObject Target;
    private Cover cover; 

    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        yield return null; // Wait for the scene to load
        // Code to set up test environment before each test
        gO = GameObject.Instantiate(Resources.Load<GameObject>("TestResources/TestShootProjectile"));
        gO.name = "ShootProjectile";
        gO.transform.position = Vector3.zero + Vector3.up * 5;
        shootProjectile = gO.GetComponent<ShootProjectile>();
        Target = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Target.name = "Target";
        Target.transform.position = Vector3.zero + (Vector3.right * 15) + (Vector3.up * 5);
        cover = Target.AddComponent<Cover>();
        yield return null; // Wait for the objects to be fully initialized
    }

    [TearDown]
    public void Teardown()
    {
        // Code to clean up test environment after each test
        Object.DestroyImmediate(gO);
        Object.DestroyImmediate(Target);
    }

    [UnityTest]
    public IEnumerator TestShoot()
    {
        int damage = 10;
        shootProjectile.Shoot(Vector3.right, damage, 100);
        // Wait on simulated time until the hit registers, since a fixed frame count
        // doesn't guarantee enough FixedUpdate physics steps have run to hit the target.
        float timeout = 5f;
        float elapsed = 0f;
        while (cover.Health == cover.MaxHealth && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }
        Assert.IsNotNull(shootProjectile);
        Assert.IsNotNull(cover);
        Assert.AreEqual(cover.MaxHealth, 100f);
        Assert.AreEqual(cover.MaxHealth - damage, cover.Health);
    }
}
