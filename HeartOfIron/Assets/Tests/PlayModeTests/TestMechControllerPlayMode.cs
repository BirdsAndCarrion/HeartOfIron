using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.AI;
using HeartOfIron.Enums;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System;
using HeartOfIron.Structs;

public class TestMechControllerPlayMode
{
    private Camera mainCamera;
    private GameObject mechControllerObject;
    private GameObject plane;
    private GameObject overwatchObject;
    private MechController mechController;
    private Mech mech;

    [SetUp]
    public void SetUp()
    {
        if (Camera.main == null)
        {
            mainCamera = new GameObject("Main Camera").AddComponent<Camera>();
            mainCamera.transform.position = new Vector3(0, 10, -10);
            mainCamera.transform.LookAt(Vector3.zero);
            mainCamera.tag = "MainCamera";
        }

        mech = new Mech("TestMech", MechType.Light, Faction.Player);

        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Plane";
        plane.transform.position = Vector3.zero;
        plane.transform.localScale = new Vector3(10, 1, 10); // Scale the plane to make it larger
        plane.AddComponent<NavMeshSurface>(); // Add NavMeshSurface component to the plane
        plane.GetComponent<NavMeshSurface>().BuildNavMesh(); // Build the NavMesh

        // Create a new GameObject and add the MechController component
        mechControllerObject = new GameObject("MechController");
        mechControllerObject.AddComponent<NavMeshAgent>();

        overwatchObject = new GameObject("Overwatch");
        overwatchObject.transform.parent = mechControllerObject.transform;
        overwatchObject.transform.localPosition = Vector3.zero; // Position the OverwatchScript at the origin of the MechController
        OverwatchScript oScript = overwatchObject.AddComponent<OverwatchScript>();

        mechController = mechControllerObject.AddComponent<MechController>(); /////// First OScript found of MC
        mechController.Initialize(mech); // Initialize the MechController with the Mech
        Assert.IsNotNull(mechController.NavMeshAgent); // Ensure the NavMeshAgent is correctly assigned to the MechController
        Assert.AreEqual(mechController.transform.position, mechController.NavMeshAgent.destination); // Ensure the NavMeshAgent has no destination set initially

        Assert.AreEqual(oScript, mechController.OverwatchScript); // Ensure the OverwatchScript is correctly assigned to the MechController
        
        mechController.OverwatchScript.Initialize(mechController); // Initialize the OverwatchScript instance selected by the MechController

    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after each test
        GameObject.DestroyImmediate(plane);
        GameObject.DestroyImmediate(mechControllerObject);
        GameObject.DestroyImmediate(overwatchObject);
        GameObject.DestroyImmediate(mainCamera.gameObject);
        GameObject.DestroyImmediate(mechController);

    }

    [UnityTest]
    public IEnumerator TestInitializeWithNullMech()
    {
        
        // Initialize the MechController with a null Mech
        mechController.Initialize(null);

        LogAssert.Expect(LogType.Error, "Cannot initialize MechController with a null Mech.");
        
        yield return null;

        // Assert that the mech is not initialized and the state is set to Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(10f, mechController.CurrentSpeed);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
    }

    [UnityTest]
    public IEnumerator TestSetShootProjectiles()
    {   
        ShootProjectile[] projectiles = new ShootProjectile[2];
        GameObject projectile1 = new GameObject("Projectile1");
        GameObject projectile2 = new GameObject("Projectile2");
        projectiles[0] = projectile1.AddComponent<ShootProjectile>();
        projectiles[1] = projectile2.AddComponent<ShootProjectile>();

        mechController.SetShootProjectiles(projectiles);
        
        yield return null;

        // Assert that the shoot projectiles are set correctly
        Assert.AreEqual(projectile1.GetComponent<ShootProjectile>(), mechController.ShootProjectiles[0]);
        Assert.AreEqual(projectile2.GetComponent<ShootProjectile>(), mechController.ShootProjectiles[1]);
    }

    [UnityTest]
    public IEnumerator TestSetShootProjectilesWithNullArray()
    {   
        LogAssert.Expect(LogType.Warning, "Attempted to set shoot projectiles to null.");
        mechController.SetShootProjectiles(null);
        
        yield return null;

        // Assert that the shoot projectiles are set to null
        Assert.IsNull(mechController.ShootProjectiles[0]);
        Assert.IsNull(mechController.ShootProjectiles[1]);
    }

    [UnityTest]
    public IEnumerator TestSetShootProjectilesWithInvalidLength()
    {   
        ShootProjectile[] projectiles = new ShootProjectile[3]; // Invalid length
        GameObject projectile1 = new GameObject("Projectile1");
        GameObject projectile2 = new GameObject("Projectile2");
        GameObject projectile3 = new GameObject("Projectile3");
        projectiles[0] = projectile1.AddComponent<ShootProjectile>();
        projectiles[1] = projectile2.AddComponent<ShootProjectile>();
        projectiles[2] = projectile3.AddComponent<ShootProjectile>();

        mechController.SetShootProjectiles(projectiles);
        
        yield return null;

        // Assert that the shoot projectiles are not set due to invalid length
        Assert.IsNull(mechController.ShootProjectiles[0]);
        Assert.IsNull(mechController.ShootProjectiles[1]);
    }

    [UnityTest]
    public IEnumerator TestSetShootProjectilesWithNullElements()
    {
        ShootProjectile[] projectiles = new ShootProjectile[2];
        projectiles[0] = null; // First element is null
        GameObject projectile2 = new GameObject("Projectile2");
        projectiles[1] = projectile2.AddComponent<ShootProjectile>();

        mechController.SetShootProjectiles(projectiles);
        
        yield return null;

        Assert.IsNull(mechController.ShootProjectiles[0]);
        Assert.AreEqual(projectile2.GetComponent<ShootProjectile>(), mechController.ShootProjectiles[1]);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateIdleToIdle()
    {

        Assert.AreEqual(overwatchObject.GetComponent<OverwatchScript>(), mechController.OverwatchScript);

        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);

        // Update the mech state to Idle
        mechController.UpdateMechState(MechState.Idle);
        yield return null;

        // Assert that the mech state is still Idle
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateIdleToMoving()
    {
        // Update the mech state to Moving
        mechController.UpdateMechState(MechState.Moving);

        yield return null;

        // Assert that the mech state is now Moving
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateIdleToBrawling()
    {
        // Update the mech state to Brawling
        mechController.UpdateMechState(MechState.Brawling);

        yield return null;

        // Assert that the mech state is now Brawling
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Brawling, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed * 0.5f, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateIdleToCharging()
    {
        // Update the mech state to Charging
        mechController.UpdateMechState(MechState.Charging);

        yield return null;

        // Assert that the mech state is now Charging
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Charging, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateIdleToOverwatch()
    {
        // Update the mech state to Overwatch
        mechController.UpdateMechState(MechState.Overwatch);

        yield return null;

        // Assert that the mech state is now Overwatch
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Overwatch, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsTrue(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateIdleToHunkering()
    {
        // Update the mech state to Hunkering
        mechController.UpdateMechState(MechState.Hunkering);

        yield return null;

        // Assert that the mech state is now Hunkering
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Hunkering, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateIdleToDead()
    {
        // Update the mech state to Dead
        mechController.UpdateMechState(MechState.Dead);

        yield return null;

        // Assert that the mech state is now Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateMovingToIdle()
    {
        // Update the mech state to Moving first
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Now update the mech state to Idle
        mechController.UpdateMechState(MechState.Idle);
        yield return null;

        // Assert that the mech state is now Idle
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f); // Allowing a small delta for floating point comparison
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateMovingToMoving()
    {
        // Update the mech state to Moving first
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Now update the mech state to Moving again
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Assert that the mech state is still Moving
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f); // Allowing a small delta for floating point comparison
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateMovingToBrawling()
    {
        // Update the mech state to Moving first
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Now update the mech state to Brawling
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Assert that the mech state is now Brawling
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Brawling, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f); // Allowing a small delta for floating point comparison
        Assert.AreEqual(mech.MaxSpeed * 0.5f, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateMovingToCharging()
    {
        // Update the mech state to Moving first
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Now update the mech state to Charging
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Assert that the mech state is now Charging
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Charging, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f); // Allowing a small delta for floating point comparison
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateMovingToOverwatch()
    {
        // Update the mech state to Moving first
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Now update the mech state to Overwatch
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Assert that the mech state is now Overwatch
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Overwatch, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f); // Allowing a small delta for floating point comparison
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsTrue(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateMovingToHunkering()
    {
        // Update the mech state to Moving first
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Now update the mech state to Hunkering
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Assert that the mech state is now Hunkering
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Hunkering, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f); // Allowing a small delta for floating point comparison
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateMovingToDead()
    {
        // Update the mech state to Moving first
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Now update the mech state to Dead
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Assert that the mech state is now Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f); // Allowing a small delta for floating point comparison
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateBrawlingToIdle()
    {
        // Update the mech state to Brawling first
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Now update the mech state to Idle
        mechController.UpdateMechState(MechState.Idle);
        yield return null;

        // Assert that the mech state is now Idle
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateBrawlingToMoving()
    {
        // Update the mech state to Brawling first
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Now update the mech state to Moving
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Assert that the mech state is now Moving
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateBrawlingToBrawling()
    {
        // Update the mech state to Brawling first
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Now update the mech state to Brawling again
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Assert that the mech state is still Brawling
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Brawling, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed * 0.5f, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateBrawlingToCharging()
    {
        // Update the mech state to Brawling first
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Now update the mech state to Charging
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Assert that the mech state is now Charging
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Charging, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateBrawlingToOverwatch()
    {
        // Update the mech state to Brawling first
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Now update the mech state to Overwatch
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Assert that the mech state is now Overwatch
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Overwatch, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsTrue(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateBrawlingToHunkering()
    {
        // Update the mech state to Brawling first
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Now update the mech state to Hunkering
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Assert that the mech state is now Hunkering
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Hunkering, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateBrawlingToDead()
    {
        // Update the mech state to Brawling first
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Now update the mech state to Dead
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Assert that the mech state is now Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateChargingToIdle()
    {
        // Update the mech state to Charging first
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Now update the mech state to Idle
        mechController.UpdateMechState(MechState.Idle);
        yield return null;

        // Assert that the mech state is now Idle
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateChargingToMoving()
    {
        // Update the mech state to Charging first
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Now update the mech state to Moving
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Assert that the mech state is now Moving
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateChargingToBrawling()
    {
        // Update the mech state to Charging first
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Now update the mech state to Brawling
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Assert that the mech state is now Brawling
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Brawling, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed * 0.5f, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateChargingToCharging()
    {
        // Update the mech state to Charging first
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Now update the mech state to Charging again
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Assert that the mech state is still Charging
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Charging, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateChargingToOverwatch()
    {
        // Update the mech state to Charging first
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Now update the mech state to Overwatch
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Assert that the mech state is now Overwatch
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Overwatch, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsTrue(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateChargingToHunkering()
    {
        // Update the mech state to Charging first
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Now update the mech state to Hunkering
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Assert that the mech state is now Hunkering
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Hunkering, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateChargingToDead()
    {
        // Update the mech state to Charging first
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Now update the mech state to Dead
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Assert that the mech state is now Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateOverwatchToIdle()
    {

        // Update the mech state to Overwatch first
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Now update the mech state to Idle
        mechController.UpdateMechState(MechState.Idle);
        yield return null;

        // Assert that the mech state is now Idle
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateOverwatchToMoving()
    {
        // Update the mech state to Overwatch first
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Now update the mech state to Moving
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Assert that the mech state is now Moving
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateOverwatchToBrawling()
    {
        // Update the mech state to Overwatch first
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Now update the mech state to Brawling
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Assert that the mech state is now Brawling
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Brawling, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed * 0.5f, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateOverwatchToCharging()
    {
        // Update the mech state to Overwatch first
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Now update the mech state to Charging
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Assert that the mech state is now Charging
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Charging, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateOverwatchToOverwatch()
    {
        // Update the mech state to Overwatch first
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Now update the mech state to Overwatch again
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Assert that the mech state is still Overwatch
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Overwatch, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsTrue(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateOverwatchToHunkering()
    {
        // Update the mech state to Overwatch first
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Now update the mech state to Hunkering
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Assert that the mech state is now Hunkering
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Hunkering, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateOverwatchToDead()
    {
        // Update the mech state to Overwatch first
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Now update the mech state to Dead
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Assert that the mech state is now Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateHunkeringToIdle()
    {
        // Update the mech state to Hunkering first
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Now update the mech state to Idle
        mechController.UpdateMechState(MechState.Idle);
        yield return null;

        // Assert that the mech state is now Idle
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateHunkeringToMoving()
    {
        // Update the mech state to Hunkering first
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Now update the mech state to Moving
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Assert that the mech state is now Moving
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateHunkeringToBrawling()
    {
        // Update the mech state to Hunkering first
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Now update the mech state to Brawling
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Assert that the mech state is now Brawling
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Brawling, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed * 0.5f, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateHunkeringToCharging()
    {
        // Update the mech state to Hunkering first
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Now update the mech state to Charging
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Assert that the mech state is now Charging
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Charging, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateHunkeringToOverwatch()
    {
        // Update the mech state to Hunkering first
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Now update the mech state to Overwatch
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Assert that the mech state is now Overwatch
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Overwatch, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsTrue(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateHunkeringToHunkering()
    {
        // Update the mech state to Hunkering first
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Now update the mech state to Hunkering again
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Assert that the mech state is still Hunkering
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Hunkering, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateHunkeringToDead()
    {
        // Update the mech state to Hunkering first
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Now update the mech state to Dead
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Assert that the mech state is now Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateDeadToIdle()
    {
        // Update the mech state to Dead first
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Now update the mech state to Idle
        mechController.UpdateMechState(MechState.Idle);
        yield return null;

        // Assert that the mech state is still Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateDeadToMoving()
    {
        // Update the mech state to Dead first
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Now update the mech state to Moving
        mechController.UpdateMechState(MechState.Moving);
        yield return null;

        // Assert that the mech state is still Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateDeadToBrawling()
    {
        // Update the mech state to Dead first
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Now update the mech state to Brawling
        mechController.UpdateMechState(MechState.Brawling);
        yield return null;

        // Assert that the mech state is still Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateDeadToCharging()
    {
        // Update the mech state to Dead first
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Now update the mech state to Charging
        mechController.UpdateMechState(MechState.Charging);
        yield return null;

        // Assert that the mech state is still Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateDeadToOverwatch()
    {
        // Update the mech state to Dead first
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Now update the mech state to Overwatch
        mechController.UpdateMechState(MechState.Overwatch);
        yield return null;

        // Assert that the mech state is still Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateDeadToHunkering()
    {
        // Update the mech state to Dead first
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Now update the mech state to Hunkering
        mechController.UpdateMechState(MechState.Hunkering);
        yield return null;

        // Assert that the mech state is still Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestUpdateMechStateDeadToDead()
    {
        // Update the mech state to Dead first
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Now update the mech state to Dead again
        mechController.UpdateMechState(MechState.Dead);
        yield return null;

        // Assert that the mech state is still Dead
        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestMoveTo()
    {

        GameObject CombatManagerObject = new GameObject("CombatManager");
        CombatManagerObject.AddComponent<CombatManager>();
        yield return null;
        CombatManager.Instance.UpdateCombatState(CombatState.Playing);

        Camera.main.transform.position = new Vector3(0, 10, -10);
        Camera.main.transform.LookAt(Vector3.zero);

        yield return null;

        float waitTime = 2f;
        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            mechController.MoveTo(new Vector3(10, 0, 10));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreNotEqual(mechController.transform.position, mechController.NavMeshAgent.destination);
        Assert.AreNotEqual(Vector3.zero, mechController.gameObject.transform.position);

        GameObject.DestroyImmediate(CombatManager.Instance.gameObject);
    }

    [UnityTest]
    public IEnumerator TestMoveToWithInvalidDestination()
    {
        Assert.AreEqual(mechController.transform.position, mechController.NavMeshAgent.destination);
        LogAssert.Expect(LogType.Error, "Destination is not on the NavMesh. Cannot move mech.");
        mechController.MoveTo(new Vector3(1000, 0, 1000));
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(mechController.transform.position, mechController.NavMeshAgent.destination);
        Assert.LessOrEqual(Vector3.Distance(Vector3.zero, mechController.gameObject.transform.position), 0.1f); // NavMesh placement only snaps the agent near the spawn point, not exactly to it
    }

    [UnityTest]
    public IEnumerator TestStopMovement()
    {
        mechController.MoveTo(new Vector3(10, 0, 10));
        yield return null;

        mechController.StopMovement();
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer, 0.01f);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(mechController.transform.position, mechController.NavMeshAgent.destination);
        Assert.AreNotEqual(Vector3.zero, mechController.gameObject.transform.position);
    }

    [UnityTest]
    public IEnumerator TestStopMovementWhenAlreadyStopped()
    {
        mechController.StopMovement();
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(mechController.transform.position, mechController.NavMeshAgent.destination);
        Assert.LessOrEqual(Vector3.Distance(Vector3.zero, mechController.gameObject.transform.position), 0.1f); // NavMesh placement only snaps the agent near the spawn point, not exactly to it
    }

    [UnityTest]
    public IEnumerator TestAddActionMarker()
    {
        GameObject markerObject = new GameObject("ActionMarker");
        ActionMarker marker = markerObject.AddComponent<ActionMarker>();
        marker.Initialize(new Vector3(5, 0, 5), mechController);

        mechController.AddActionMarker(marker);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(1, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(marker, mechController.ActionMarkers[0]);
        Assert.AreEqual(new Vector3(0, 0, 0), marker.transform.position);
        Assert.AreEqual(mechController, marker.SourceMech);
        Assert.LessOrEqual(Vector3.Distance(Vector3.zero, mechController.transform.position), 0.1f);
    }

    [UnityTest]
    public IEnumerator TestAddActionMarkerWithNullMarker()
    {
        GameObject markerObject = new GameObject("ActionMarker");
        ActionMarker marker = markerObject.AddComponent<ActionMarker>();

        mechController.AddActionMarker(marker);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.LessOrEqual(Vector3.Distance(Vector3.zero, mechController.transform.position), 0.1f);
    }

    [UnityTest]
    public IEnumerator TestInsertActionMarkerAt()
    {
        GameObject markerObject1 = new GameObject("ActionMarker1");
        ActionMarker marker1 = markerObject1.AddComponent<ActionMarker>();
        marker1.Initialize(new Vector3(5, 0, 5), mechController);

        GameObject markerObject2 = new GameObject("ActionMarker2");
        ActionMarker marker2 = markerObject2.AddComponent<ActionMarker>();
        marker2.Initialize(new Vector3(10, 0, 10), mechController);

        GameObject markerObject3 = new GameObject("ActionMarker3");
        ActionMarker marker3 = markerObject3.AddComponent<ActionMarker>();
        marker3.Initialize(new Vector3(5, 0, 0), mechController);

        mechController.AddActionMarker(marker1);
        yield return null;

        mechController.InsertActionMarkerAt(marker2, 0);
        yield return null;

        mechController.InsertActionMarkerAt(marker3, 1);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(3, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(marker2, mechController.ActionMarkers[0]);
        Assert.AreEqual(marker3, mechController.ActionMarkers[1]);
        Assert.AreEqual(marker1, mechController.ActionMarkers[2]);
    }

    [UnityTest]
    public IEnumerator TestInsertActionMarkerAtWithNullMarker()
    {
        GameObject markerObject = new GameObject("ActionMarker");
        ActionMarker marker = markerObject.AddComponent<ActionMarker>();

        mechController.InsertActionMarkerAt(marker, 0);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.LessOrEqual(Vector3.Distance(Vector3.zero, mechController.transform.position), 0.1f);
    }

    [UnityTest]
    public IEnumerator TestInsertActionMarkerAtWithInvalidIndex()
    {
        GameObject markerObject1 = new GameObject("ActionMarker1");
        ActionMarker marker1 = markerObject1.AddComponent<ActionMarker>();
        marker1.Initialize(new Vector3(5, 0, 5), mechController);

        GameObject markerObject2 = new GameObject("ActionMarker2");
        ActionMarker marker2 = markerObject2.AddComponent<ActionMarker>();
        marker2.Initialize(new Vector3(10, 0, 10), mechController);

        GameObject markerObject3 = new GameObject("ActionMarker3");
        ActionMarker marker3 = markerObject3.AddComponent<ActionMarker>();
        marker3.Initialize(new Vector3(5, 0, 0), mechController);

        mechController.AddActionMarker(marker1);
        yield return null;

        mechController.InsertActionMarkerAt(marker2, 0);
        yield return null;

        LogAssert.Expect(LogType.Warning, "Index 7 is out of bounds for inserting ActionMarker.");

        mechController.InsertActionMarkerAt(marker3, 7);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(2, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(marker2, mechController.ActionMarkers[0]);
        Assert.AreEqual(marker1, mechController.ActionMarkers[1]);
    }

    [UnityTest]
    public IEnumerator TestRemoveActionMarker()
    {
        GameObject markerObject = new GameObject("ActionMarker");
        ActionMarker marker = markerObject.AddComponent<ActionMarker>();
        marker.Initialize(new Vector3(5, 0, 5), mechController);

        mechController.AddActionMarker(marker);
        yield return null;

        mechController.RemoveActionMarker(marker);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    [UnityTest]
    public IEnumerator TestRemoveActionMarkerWithNullMarker()
    {
        GameObject markerObject = new GameObject("ActionMarker");
        ActionMarker marker = markerObject.AddComponent<ActionMarker>();
        marker.Initialize(new Vector3(5, 0, 5), mechController);

        GameObject markerObject2 = new GameObject("ActionMarker2");
        ActionMarker marker2 = markerObject2.AddComponent<ActionMarker>();

        mechController.AddActionMarker(marker);
        yield return null;

        mechController.RemoveActionMarker(marker2);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(1, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(marker, mechController.ActionMarkers[0]);
    }

    [UnityTest]
    public IEnumerator TestTakeDamage()
    {
        mechController.TakeDamage(10, (int)BodyPart.Head);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(90, mechController.Mech.Health[(int)BodyPart.Head]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.LeftArm]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.RightArm]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.LeftLeg]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.RightLeg]);
    }

    [UnityTest]
    public IEnumerator TestTakeDamageWithDestroyedHead()
    {
        mechController.TakeDamage(150, (int)BodyPart.Head);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.Head]);
    }

    [UnityTest]
    public IEnumerator TestTakeDamageWithDestroyedTorso()
    {
        mechController.TakeDamage(150, (int)BodyPart.Torso);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.Torso]);
    }

    [UnityTest]
    public IEnumerator TestTakeDamageWithDestroyedArm()
    {
        mechController.TakeDamage(150, (int)BodyPart.LeftArm);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.LeftArm]);
    }

    [UnityTest]
    public IEnumerator TestTakeDamageWithDestroyedLeg()
    {
        mechController.TakeDamage(150, (int)BodyPart.LeftLeg);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.LeftLeg]);
    }

    [UnityTest]
    public IEnumerator TestTakeDamageWithInvalidLocation()
    {
        LogAssert.Expect(LogType.Error, "Invalid location index 10 for mech TestMech. Damage not applied.");
        mechController.TakeDamage(10, 10); // Invalid body part index
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.Head]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.LeftArm]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.RightArm]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.LeftLeg]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.RightLeg]);
    }

    [UnityTest]
    public IEnumerator TestRepair()
    {
        mechController.TakeDamage(50, (int)BodyPart.Head);
        mechController.TakeDamage(30, (int)BodyPart.Torso);
        yield return null;

        Assert.AreEqual(50, mechController.Mech.Health[(int)BodyPart.Head]);
        mechController.Repair(30, (int)BodyPart.Head);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(80, mechController.Mech.Health[(int)BodyPart.Head]);
        Assert.AreEqual(70, mechController.Mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.LeftArm]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.RightArm]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.LeftLeg]);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.RightLeg]);
    }

    [UnityTest]
    public IEnumerator TestRepairOnDestroyedMech()
    {
        mechController.TakeDamage(150, (int)BodyPart.Torso);
        yield return null;

        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.Torso]);
        mechController.Repair(50, (int)BodyPart.Torso);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Dead, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.Torso]);
    }

    [UnityTest]
    public IEnumerator TestRepairOnPartDestroyedMech()
    {
        mechController.TakeDamage(150, (int)BodyPart.LeftArm);
        mechController.TakeDamage(40, (int)BodyPart.Torso);
        yield return null;

        Assert.AreEqual(60, mechController.Mech.Health[(int)BodyPart.Torso]);
        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.LeftArm]);
        mechController.Repair(50, (int)BodyPart.LeftArm);
        mechController.Repair(30, (int)BodyPart.Torso);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(0, mechController.Mech.Health[(int)BodyPart.LeftArm]);
        Assert.AreEqual(90, mechController.Mech.Health[(int)BodyPart.Torso]);
    }

    [UnityTest]
    public IEnumerator TestRepairOverheal()
    {
        mechController.TakeDamage(50, (int)BodyPart.Head);
        yield return null;

        Assert.AreEqual(50, mechController.Mech.Health[(int)BodyPart.Head]);
        mechController.Repair(100, (int)BodyPart.Head);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(100, mechController.Mech.Health[(int)BodyPart.Head]);
    }

    [UnityTest]
    public IEnumerator TestRepairWithInvalidLocation()
    {
        mechController.TakeDamage(50, (int)BodyPart.Head);
        yield return null;

        Assert.AreEqual(50, mechController.Mech.Health[(int)BodyPart.Head]);
        LogAssert.Expect(LogType.Error, "Invalid location index 10 for mech TestMech. Repair not applied.");
        mechController.Repair(30, 10); // Invalid body part index
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
        Assert.AreEqual(50, mechController.Mech.Health[(int)BodyPart.Head]);
    }

    [UnityTest]
    public IEnumerator TestGetBonusForShoot()
    {
        Pilot pilot = new Pilot(Guid.NewGuid(), "TestPilot", 1, 0, 0, Faction.Player, 
        new PilotSkill[] {
            new PilotSkill(SkillType.Gunnery, 2, 0.5f),
            new PilotSkill(SkillType.Piloting, 1, 0.2f),
            new PilotSkill(SkillType.Perception, 3, 0.7f),
            new PilotSkill(SkillType.Navigation, 1, 0.3f),
        }, Color.blue);

        mech.AddPilot(pilot);

        float bonus = mechController.GetBonus(ActionType.Shoot);

        yield return null;

        Assert.AreEqual(1f, bonus, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestGetBonusForShootWhileMoving()
    {
        Pilot pilot = new Pilot(Guid.NewGuid(), "TestPilot", 1, 0, 0, Faction.Player, 
        new PilotSkill[] {
            new PilotSkill(SkillType.Gunnery, 2, 0.5f),
            new PilotSkill(SkillType.Piloting, 1, 0.2f),
            new PilotSkill(SkillType.Perception, 3, 0.7f),
            new PilotSkill(SkillType.Navigation, 1, 0.3f),
        }, Color.blue);

        mech.AddPilot(pilot);

        mechController.UpdateMechState(MechState.Moving);
        float bonus = mechController.GetBonus(ActionType.Shoot);
        yield return null;


        Assert.AreEqual(1f, bonus, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestGetBonusForMelee()
    {
        Pilot pilot = new Pilot(Guid.NewGuid(), "TestPilot", 1, 0, 0, Faction.Player,
        new PilotSkill[] {
            new PilotSkill(SkillType.Gunnery, 2, 0.5f),
            new PilotSkill(SkillType.Piloting, 1, 0.2f),
            new PilotSkill(SkillType.Perception, 3, 0.7f),
            new PilotSkill(SkillType.Navigation, 1, 0.3f),
        }, Color.blue);

        mech.AddPilot(pilot);
        float bonus = mechController.GetBonus(ActionType.Melee);
        yield return null;

        Assert.AreEqual(0.2f, bonus, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestGetBonusForMove()
    {
        Pilot pilot = new Pilot(Guid.NewGuid(), "TestPilot", 1, 0, 0, Faction.Player,
        new PilotSkill[] {
            new PilotSkill(SkillType.Gunnery, 2, 0.5f),
            new PilotSkill(SkillType.Piloting, 1, 0.2f),
            new PilotSkill(SkillType.Perception, 3, 0.7f),
            new PilotSkill(SkillType.Navigation, 1, 0.3f),
        }, Color.blue);

        mech.AddPilot(pilot);

        float bonus = mechController.GetBonus(ActionType.Move);
        yield return null;
        Assert.AreEqual(0.3f, bonus, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestGetBonusWithoutPilot()
    {
        float shootBonus = mechController.GetBonus(ActionType.Shoot);
        float meleeBonus = mechController.GetBonus(ActionType.Melee);
        float moveBonus = mechController.GetBonus(ActionType.Move);
        yield return null;
        Assert.AreEqual(0f, shootBonus, 0.01f);
        Assert.AreEqual(0f, meleeBonus, 0.01f);
        Assert.AreEqual(0f, moveBonus, 0.01f);
    }

    [UnityTest]
    public IEnumerator TestOnSelected()
    {
        LogAssert.Expect(LogType.Log, "EventRouter: Mech selected: " + mechController.Mech.Name);
        LogAssert.Expect(LogType.Log, "EventRouter: Mech declared active: " + mechController.Mech.Name);
        string output = mechController.OnSelected();
        yield return null;

        Assert.AreEqual("MechController: " + mechController.Mech.Name, output);
    }

    [UnityTest]
    public IEnumerator TestIsLowHealth()
    {
        mechController.TakeDamage(80, (int)BodyPart.Torso);
        yield return null;

        bool isLowHealth = mechController.IsLowHealth();
        yield return null;

        Assert.IsFalse(isLowHealth);

        mechController.TakeDamage(80, (int)BodyPart.Head);
        mechController.TakeDamage(80, (int)BodyPart.LeftArm);
        mechController.TakeDamage(80, (int)BodyPart.RightArm);
        mechController.TakeDamage(80, (int)BodyPart.LeftLeg);
        mechController.TakeDamage(80, (int)BodyPart.RightLeg);
        yield return null;

        isLowHealth = mechController.IsLowHealth();
        yield return null;

        Assert.IsTrue(isLowHealth);
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenCombatStateIsNotPlaying()
    {
        GameObject combatManagerObject = new GameObject("CombatManager");
        CombatManager combatManager = combatManagerObject.AddComponent<CombatManager>();
        yield return null;

        combatManager.UpdateCombatState(CombatState.Paused);
        yield return null;

        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);

        GameObject.DestroyImmediate(combatManagerObject);
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenactionMarkersIsEmpty()
    {
        GameObject combatManagerObject = new GameObject("CombatManager");
        CombatManager combatManager = combatManagerObject.AddComponent<CombatManager>();
        yield return null;

        combatManager.UpdateCombatState(CombatState.Playing);
        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    /*[UnityTest]
    public IEnumerator TestUpdateWhenAwayFromActionMarker()
    {
        GameObject combatManagerObject = new GameObject("CombatManager");
        CombatManager combatManager = combatManagerObject.AddComponent<CombatManager>();
        yield return null;

        combatManager.UpdateCombatState(CombatState.Playing);
        yield return null;

        GameObject markerObject = new GameObject("ActionMarker");
        ActionMarker marker = markerObject.AddComponent<ActionMarker>();
        marker.Initialize(new Vector3(10, 0, 10), mechController);

        mechController.AddActionMarker(marker);

        yield return null;

        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(1, mechController.ActionMarkers.Count);
        Assert.AreEqual(marker, mechController.ActionMarkers[0]);
        Assert.AreEqual(MechState.Moving, mechController.MechState);
        Assert.AreEqual(0.5f, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }*/

    [UnityTest]
    public IEnumerator TestUpdateWhenNoSecondaryAction()
    {
        GameObject combatManagerObject = new GameObject("CombatManager");
        CombatManager combatManager = combatManagerObject.AddComponent<CombatManager>();
        yield return null;

        combatManager.UpdateCombatState(CombatState.Playing);
        yield return null;

        GameObject markerObject = new GameObject("ActionMarker");
        ActionMarker marker = markerObject.AddComponent<ActionMarker>();
        marker.Initialize(new Vector3(0, 0, 0), mechController);

        mechController.AddActionMarker(marker);

        Assert.AreEqual(1, mechController.ActionMarkers.Count);

        yield return null;

        Assert.AreEqual(mech, mechController.Mech);
        Assert.AreEqual(MechState.Idle, mechController.MechState);
        Assert.AreEqual(0, mechController.MoveTimer);
        Assert.AreEqual(mech.MaxSpeed, mechController.CurrentSpeed);
        Assert.AreEqual(0, mechController.ActionMarkers.Count);
        Assert.IsTrue(mechController.OverwatchScript.HasOverwatch);
        Assert.IsFalse(mechController.OverwatchScript.IsOverwatchActive);
    }

    /*[UnityTest]
    public IEnumerator TestUpdateWhenSecondaryActionIsShoot()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenSecondaryActionIsMelee()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenSecondaryActionIsReload()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenSecondaryActionIsOverwatch()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenSecondaryActionIsHunker()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenSecondaryActionIsChargeCompleting()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUpdateWhenSecondaryActionIsCharge()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }*/
}