using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.AI;
using Unity.AI.Navigation;
using HeartOfIron.Enums;
using HeartOfIron.Structs;
using System.Collections;

public class TestActionsPlayMode
{
    private GameObject planeObject;
    private GameObject sourceMechObject;
    private MechController sourceMechController;
    private Mech sourceMech;
    private GameObject targetMechObject;
    private MechController targetMechController;
    private Mech targetMech;
    private GameObject actionMarkerObject;
    private ActionMarker actionMarker;

    private class PseudoActionMarkerAssembler : ActionMarkerAssembler
    {
        public override ActionMarker AssembleActionMarker(Vector3 position, MechController sourceMech, bool addToMech = true)
        {
            GameObject actionMarkerObject = new GameObject("PseudoActionMarker");
            ActionMarker actionMarker = actionMarkerObject.AddComponent<ActionMarker>();
            actionMarker.Initialize(position, sourceMech);
            if (addToMech)
            {
                sourceMech.InsertActionMarkerAt(actionMarker, 0);
            }
            return actionMarker;
        }
    }

        

    [SetUp]
    public void Setup()
    {
        // Create a plane for the NavMesh
        planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeObject.transform.position = Vector3.zero;
        planeObject.transform.localScale = new Vector3(10, 1, 10); // Scale the plane to make it larger

        // Add a NavMeshSurface component to the plane and bake the NavMesh
        var navMeshSurface = planeObject.AddComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();

        // Create source Mech
        sourceMechObject = new GameObject("SourceMech");
        sourceMech = new Mech("Test Mech", MechType.Light, Faction.Player);
        GameObject overwatchObject = new GameObject("Overwatch");
        overwatchObject.transform.parent = sourceMechObject.transform;
        overwatchObject.AddComponent<OverwatchScript>();

        GameObject mechVisionObject = new GameObject("MechVision");
        mechVisionObject.transform.parent = sourceMechObject.transform;
        mechVisionObject.AddComponent<MechVision>();
        mechVisionObject.GetComponent<MechVision>().SetVisionParameters(10f, 90f);
        sourceMechObject.AddComponent<NavMeshAgent>();
        sourceMechController = sourceMechObject.AddComponent<MechController>();
        sourceMechController.Initialize(sourceMech);
        overwatchObject.GetComponent<OverwatchScript>().Initialize(sourceMechController);

        Weapon weapon = new Weapon("Test weapon", Weight.Light, 100, 10, 5, 20, 1.5f, 0.5f, 1.0f, 5, 1, 1);
        sourceMech.EquipWeapon(weapon);
        Weapon weapon2 = new Weapon("Test melee weapon", Weight.Light, 100, 0, 15, 0, 0f, 0f, 0f, 5, 1, 1);
        sourceMech.EquipWeapon(weapon2);

        // Create target Mech
        targetMechObject = new GameObject("TargetMech");
        targetMech = new Mech("Target Mech", MechType.Light, Faction.Enemy);
        GameObject targetOverwatchObject = new GameObject("TargetOverwatch");
        targetOverwatchObject.transform.parent = targetMechObject.transform;
        targetOverwatchObject.AddComponent<OverwatchScript>();

        GameObject targetMechVisionObject = new GameObject("TargetMechVision");
        targetMechVisionObject.transform.parent = targetMechObject.transform;
        targetMechVisionObject.AddComponent<MechVision>();
        targetMechVisionObject.GetComponent<MechVision>().SetVisionParameters(10f, 90f);
        targetMechObject.AddComponent<NavMeshAgent>();
        targetMechController = targetMechObject.AddComponent<MechController>();
        targetMechController.Initialize(targetMech);
        targetOverwatchObject.GetComponent<OverwatchScript>().Initialize(targetMechController);

        // Create ActionMarker
        actionMarkerObject = new GameObject("ActionMarker");
        actionMarker = actionMarkerObject.AddComponent<ActionMarker>();
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(planeObject);
        GameObject.DestroyImmediate(sourceMechObject);
        GameObject.DestroyImmediate(targetMechObject);
        GameObject.DestroyImmediate(actionMarkerObject);
    }

    [UnityTest]
    public IEnumerator TestActionMarker_Initialization_Move()
    {
        actionMarker.Initialize(new Vector3(5, 0, 5), sourceMechController);

        Assert.IsNotNull(actionMarker.ActionPair);
        Assert.IsNotNull(actionMarker.ActionPair.MovementAction);
        Assert.IsNull(actionMarker.ActionPair.SecondaryAction);
        Assert.AreEqual(sourceMechController, actionMarker.ActionPair.MovementAction.SourceMech);
        Assert.AreEqual(new Vector3(5, 0, 5), actionMarker.ActionPair.MovementAction.Destination);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestActionMarker_Initialization_Charge()
    {
        actionMarker.Initialize(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);

        Assert.IsNotNull(actionMarker.ActionPair);
        Assert.IsNotNull(actionMarker.ActionPair.MovementAction);
        Assert.IsNotNull(actionMarker.ActionPair.SecondaryAction);
        Assert.AreEqual(sourceMechController, actionMarker.ActionPair.MovementAction.SourceMech);
        Assert.AreEqual(targetMechController, actionMarker.ActionPair.SecondaryAction.TargetMech);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestActionMarker_SetSecondaryAction()
    {
        actionMarker.Initialize(new Vector3(5, 0, 5), sourceMechController);

        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        actionMarker.SetSecondaryAction(meleeAction);

        Assert.IsNotNull(actionMarker.ActionPair);
        Assert.IsNotNull(actionMarker.ActionPair.MovementAction);
        Assert.IsNotNull(actionMarker.ActionPair.SecondaryAction);
        Assert.AreEqual(meleeAction, actionMarker.ActionPair.SecondaryAction);
        Assert.AreEqual(sourceMechController, actionMarker.ActionPair.SecondaryAction.SourceMech);
        Assert.AreEqual(targetMechController, actionMarker.ActionPair.SecondaryAction.TargetMech);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestActionMarker_OnSelected()
    {
        actionMarker.Initialize(new Vector3(5, 0, 5), sourceMechController);

        string selectionInfo = actionMarker.OnSelected();
        string expectedInfo = $"ActionMarker ID: {actionMarker.Id}, Source Mech: {sourceMechController.Mech.Name}";
        Assert.AreEqual(expectedInfo, selectionInfo);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestActionPair_SetSecondaryAction()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionPair actionPair = new ActionPair(moveAction);

        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        actionPair.SetSecondaryAction(meleeAction);
        yield return null;

        Assert.IsNotNull(actionPair.SecondaryAction);
        Assert.AreEqual(meleeAction, actionPair.SecondaryAction);
        Assert.IsNotNull(actionPair.MovementAction);
        Assert.AreEqual(moveAction, actionPair.MovementAction);
    }

    [UnityTest]
    public IEnumerator TestActionPair_SetSecondaryAction_ExistingSecondaryAction()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        MeleeAction meleeAction1 = new MeleeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        ActionPair actionPair = new ActionPair(moveAction, meleeAction1);

        MeleeAction meleeAction2 = new MeleeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[0]);
        actionPair.SetSecondaryAction(meleeAction2);
        yield return null;

        Assert.IsNotNull(actionPair.SecondaryAction);
        Assert.AreEqual(meleeAction2, actionPair.SecondaryAction);
        Assert.IsNotNull(actionPair.MovementAction);
        Assert.AreEqual(moveAction, actionPair.MovementAction);
    }

    [UnityTest]
    public IEnumerator TestActionPair_SetSecondaryAction_ChargeAction()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionPair actionPair = new ActionPair(moveAction);

        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        actionPair.SetSecondaryAction(chargeAction);
        yield return null;

        Assert.IsNotNull(actionPair.SecondaryAction);
        Assert.AreEqual(chargeAction, actionPair.SecondaryAction);
        Assert.IsNotNull(actionPair.MovementAction);
        Assert.AreEqual(moveAction, actionPair.MovementAction);
    }

    [UnityTest]
    public IEnumerator TestActionPair_SetSecondaryAction_MoveAction()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionPair actionPair = new ActionPair(moveAction);

        LogAssert.Expect(LogType.Warning, "Secondary action cannot be of type Move. Please provide a valid action.");

        MoveAction anotherMoveAction = new MoveAction(sourceMechController, new Vector3(10, 0, 10));
        actionPair.SetSecondaryAction(anotherMoveAction);
        yield return null;

        Assert.IsNull(actionPair.SecondaryAction);
        Assert.IsNotNull(actionPair.MovementAction);
        Assert.AreEqual(moveAction, actionPair.MovementAction);
    }

    [UnityTest]
    public IEnumerator TestActionPair_SetSecondaryAction_NullAction()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionPair actionPair = new ActionPair(moveAction);

        LogAssert.Expect(LogType.Warning, "Secondary action cannot be null. Please provide a valid action.");
        actionPair.SetSecondaryAction(null);
        yield return null;

        Assert.IsNull(actionPair.SecondaryAction);
        Assert.IsNotNull(actionPair.MovementAction);
        Assert.AreEqual(moveAction, actionPair.MovementAction);
    }

    [UnityTest]
    public IEnumerator TestActionPair_SetSecondaryAction_ExistingSecondaryAction_MoveAction()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        ActionPair actionPair = new ActionPair(moveAction, meleeAction);

        LogAssert.Expect(LogType.Warning, "Secondary action cannot be of type Move. Please provide a valid action.");

        MoveAction anotherMoveAction = new MoveAction(sourceMechController, new Vector3(10, 0, 10));
        actionPair.SetSecondaryAction(anotherMoveAction);
        yield return null;

        Assert.IsNull(actionPair.SecondaryAction);
        Assert.IsNotNull(actionPair.MovementAction);
        Assert.AreEqual(moveAction, actionPair.MovementAction);
    }

    [UnityTest]
    public IEnumerator TestActionPair_SetSecondaryAction_ExistingSecondaryAction_NullAction()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        ActionPair actionPair = new ActionPair(moveAction, meleeAction);

        LogAssert.Expect(LogType.Warning, "Secondary action cannot be null. Please provide a valid action.");
        actionPair.SetSecondaryAction(null);
        yield return null;

        Assert.IsNotNull(actionPair.SecondaryAction);
        Assert.AreEqual(meleeAction, actionPair.SecondaryAction);
        Assert.IsNotNull(actionPair.MovementAction);
        Assert.AreEqual(moveAction, actionPair.MovementAction);
    }

    [UnityTest]
    public IEnumerator TestMovementAction_SetDestination()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        moveAction.SetDestination(new Vector3(10, 0, 10));
        yield return null;

        Assert.AreEqual(new Vector3(10, 0, 10), moveAction.Destination);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_CanExecute()
    {
        ReloadAction reloadAction = new ReloadAction(sourceMechController, sourceMech.Inventory.Weapons[0]);
        bool canExecute = reloadAction.CanExecute();
        yield return null;

        Assert.IsTrue(canExecute);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_CanExecute_FailingConditions()
    {
        ReloadAction reloadAction = new ReloadAction(null, sourceMech.Inventory.Weapons[0]);
        bool canExecute = reloadAction.CanExecute();
        yield return null;

        Assert.IsFalse(canExecute);

        reloadAction = new ReloadAction(sourceMechController, null);
        canExecute = reloadAction.CanExecute();
        yield return null;

        Assert.IsFalse(canExecute);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_Execute()
    {
        ReloadAction reloadAction = new ReloadAction(sourceMechController, sourceMech.Inventory.Weapons[0]);
        sourceMech.Inventory.Weapons[0].ConsumeAmmo(5); // Consume some ammo to test reloading
        sourceMech.Inventory.Weapons[0].UpdateCooldown(5f);
        yield return null;

        Assert.AreNotEqual(sourceMech.Inventory.Weapons[0].CurrentAmmo, sourceMech.Inventory.Weapons[0].MaxAmmo);
        reloadAction.Execute();
        yield return null;

        Assert.AreEqual(sourceMech.Inventory.Weapons[0].CurrentAmmo, sourceMech.Inventory.Weapons[0].MaxAmmo);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_Execute_FailingConditions()
    {
        ReloadAction reloadAction = new ReloadAction(null, sourceMech.Inventory.Weapons[0]);
        bool executed = reloadAction.Execute();
        LogAssert.Expect(LogType.Warning, "ReloadAction cannot be executed because CanExecute() returned false.");
        yield return null;

        Assert.IsFalse(executed);

        reloadAction = new ReloadAction(sourceMechController, null);
        executed = reloadAction.Execute();
        LogAssert.Expect(LogType.Warning, "ReloadAction cannot be executed because CanExecute() returned false.");
        yield return null;

        Assert.IsFalse(executed);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_Execute_AlreadyFullAmmo()
    {
        ReloadAction reloadAction = new ReloadAction(sourceMechController, sourceMech.Inventory.Weapons[0]);
        bool executed = reloadAction.Execute();
        yield return null;

        Assert.IsTrue(executed);
        Assert.AreEqual(sourceMech.Inventory.Weapons[0].CurrentAmmo, sourceMech.Inventory.Weapons[0].MaxAmmo);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_Execute_WeaponOnCooldown()
    {
        ReloadAction reloadAction = new ReloadAction(sourceMechController, sourceMech.Inventory.Weapons[0]);
        sourceMech.Inventory.Weapons[0].ConsumeAmmo(5); // Consume some ammo to test reloading
        bool executed = reloadAction.Execute();
        LogAssert.Expect(LogType.Warning, $"{sourceMech.Name} cannot reload {sourceMech.Inventory.Weapons[0].Name} because it is on cooldown.");
        yield return null;

        Assert.IsFalse(executed);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_CanExecute()
    {
        HunkerAction hunkerAction = new HunkerAction(sourceMechController);
        bool canExecute = hunkerAction.CanExecute();
        yield return null;

        Assert.IsTrue(canExecute);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_CanExecute_FailingConditions()
    {
        HunkerAction hunkerAction = new HunkerAction(null);
        bool canExecute = hunkerAction.CanExecute();
        LogAssert.Expect(LogType.Warning, "HunkerAction cannot be executed because SourceMech is null.");
        yield return null;

        Assert.IsFalse(canExecute);

        sourceMechController.UpdateMechState(MechState.Hunkering);
        hunkerAction = new HunkerAction(sourceMechController);
        canExecute = hunkerAction.CanExecute();
        yield return null;

        Assert.IsFalse(canExecute);

        sourceMechController.UpdateMechState(MechState.Dead);
        hunkerAction = new HunkerAction(sourceMechController);
        canExecute = hunkerAction.CanExecute();
        yield return null;

        Assert.IsFalse(canExecute);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_Execute()
    {
        HunkerAction hunkerAction = new HunkerAction(sourceMechController);
        bool executed = hunkerAction.Execute();
        yield return null;

        Assert.IsTrue(executed);
        Assert.AreEqual(MechState.Hunkering, sourceMechController.MechState);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_Execute_FailingConditions()
    {
        HunkerAction hunkerAction = new HunkerAction(null);
        bool executed = hunkerAction.Execute();
        LogAssert.Expect(LogType.Warning, "HunkerAction cannot be executed because SourceMech is null.");
        yield return null;

        Assert.IsFalse(executed);
        Assert.AreNotEqual(MechState.Hunkering, sourceMechController.MechState);

        sourceMechController.UpdateMechState(MechState.Hunkering);
        hunkerAction = new HunkerAction(sourceMechController);
        executed = hunkerAction.Execute();
        yield return null;

        Assert.IsFalse(executed);
        Assert.AreEqual(MechState.Hunkering, sourceMechController.MechState);

        sourceMechController.UpdateMechState(MechState.Dead);
        hunkerAction = new HunkerAction(sourceMechController);
        executed = hunkerAction.Execute();
        yield return null;

        Assert.IsFalse(executed);
        Assert.AreNotEqual(MechState.Hunkering, sourceMechController.MechState);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_CanExecute()
    {
        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        bool canExecute = chargeAction.CanExecute();
        yield return null;

        Assert.IsTrue(canExecute);
    
    
    }

    [UnityTest]
    public IEnumerator TestChargeAction_CanExecute_FailingConditions()
    {
        ChargeAction chargeAction = new ChargeAction(null, targetMechController, sourceMech.Inventory.Weapons[1]);
        bool canExecute = chargeAction.CanExecute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(canExecute);

        ChargeAction chargeAction2 = new ChargeAction(sourceMechController, null, sourceMech.Inventory.Weapons[1]);
        canExecute = chargeAction2.CanExecute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the target mech is null or dead.");
        yield return null;

        Assert.IsFalse(canExecute);

        ChargeAction chargeAction3 = new ChargeAction(sourceMechController, targetMechController, null);
        canExecute = chargeAction3.CanExecute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the weapon is null.");
        yield return null;

        Assert.IsFalse(canExecute);

        ChargeAction chargeActionDeadSource = new ChargeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        sourceMechController.UpdateMechState(MechState.Dead);
        canExecute = chargeActionDeadSource.CanExecute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(canExecute);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_Execute()
    {
        GameObject actionMarkerAssembler = new GameObject("ActionMarkerAssembler");
        actionMarkerAssembler.AddComponent<PseudoActionMarkerAssembler>();

        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        bool executed = chargeAction.Execute();
        yield return null;

        Assert.IsTrue(executed);
        Assert.AreEqual(MechState.Charging, sourceMechController.MechState);
        Assert.IsNotNull(sourceMechController.ActionMarkers);
        Assert.AreEqual(1, sourceMechController.ActionMarkers.Count);

        GameObject.DestroyImmediate(actionMarkerAssembler);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_Execute_FailingConditions()
    {
        ChargeAction chargeAction = new ChargeAction(null, targetMechController, sourceMech.Inventory.Weapons[1]);
        bool executed = chargeAction.Execute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(executed);

        ChargeAction chargeAction2 = new ChargeAction(sourceMechController, null, sourceMech.Inventory.Weapons[1]);
        executed = chargeAction2.Execute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the target mech is null or dead.");
        yield return null;

        Assert.IsFalse(executed);

        ChargeAction chargeAction3 = new ChargeAction(sourceMechController, targetMechController, null);
        executed = chargeAction3.Execute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the weapon is null.");
        yield return null;

        Assert.IsFalse(executed);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_Execute_AlreadyCharging()
    {
        sourceMechController.UpdateMechState(MechState.Charging);
        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, sourceMech.Inventory.Weapons[1]);
        bool executed = chargeAction.Execute();
        yield return null;

        Assert.IsTrue(executed);
        Assert.AreEqual(MechState.Charging, sourceMechController.MechState);
        Assert.IsNotNull(sourceMechController.ActionMarkers);
        Assert.AreEqual(0, sourceMechController.ActionMarkers.Count);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_CanExecute()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        bool canExecute = moveAction.CanExecute();
        yield return null;

        Assert.IsTrue(canExecute);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_CanExecute_FailingConditions()
    {
        MoveAction moveAction = new MoveAction(null, new Vector3(5, 0, 5));
        bool canExecute = moveAction.CanExecute();
        LogAssert.Expect(LogType.Warning, "MoveAction cannot be executed because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(canExecute);

        MoveAction moveAction2 = new MoveAction(sourceMechController, new Vector3(1000, 0, 1000)); // Assuming this position is unreachable
        canExecute = moveAction2.CanExecute();
        LogAssert.Expect(LogType.Log, $"Cannot execute MoveAction. No valid path to destination {new Vector3(1000, 0, 1000)}.");
        yield return null;

        Assert.IsFalse(canExecute);

        sourceMechController.UpdateMechState(MechState.Dead);
        moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        canExecute = moveAction.CanExecute();
        LogAssert.Expect(LogType.Warning, "MoveAction cannot be executed because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(canExecute);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_Execute()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        bool executed = moveAction.Execute();
        yield return null;

        Assert.IsTrue(executed);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_Execute_FailingConditions()
    {
        MoveAction moveAction = new MoveAction(null, new Vector3(5, 0, 5));
        bool executed = moveAction.Execute();
        LogAssert.Expect(LogType.Warning, "Cannot execute MoveAction. Conditions not met.");
        yield return null;

        Assert.IsFalse(executed);

        MoveAction moveAction2 = new MoveAction(sourceMechController, new Vector3(1000, 0, 1000)); // Assuming this position is unreachable
        executed = moveAction2.Execute();
        LogAssert.Expect(LogType.Log, $"Cannot execute MoveAction. No valid path to destination {new Vector3(1000, 0, 1000)}.");
        yield return null;

        Assert.IsFalse(executed);

        sourceMechController.UpdateMechState(MechState.Dead);
        moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        executed = moveAction.Execute();
        LogAssert.Expect(LogType.Warning, "MoveAction cannot be executed because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(executed);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_CanExecute()
    {
        OverwatchAction overwatchAction = new OverwatchAction(sourceMechController);
        bool canExecute = overwatchAction.CanExecute();
        yield return null;

        Assert.IsTrue(canExecute);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_CanExecute_FailingConditions()
    {
        OverwatchAction overwatchAction = new OverwatchAction(null);
        bool canExecute = overwatchAction.CanExecute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(canExecute);

        sourceMechController.UpdateMechState(MechState.Overwatch);
        overwatchAction = new OverwatchAction(sourceMechController);
        canExecute = overwatchAction.CanExecute();
        LogAssert.Expect(LogType.Warning, $"{sourceMechController.Mech.Name}: Cannot execute OverwatchAction because the mech is already in overwatch.");
        yield return null;

        Assert.IsFalse(canExecute);

        sourceMechController.UpdateMechState(MechState.Dead);
        overwatchAction = new OverwatchAction(sourceMechController);
        canExecute = overwatchAction.CanExecute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction because the mech is null or dead.");
        yield return null;

        Assert.IsFalse(canExecute);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_Execute()
    {
        OverwatchAction overwatchAction = new OverwatchAction(sourceMechController);
        bool executed = overwatchAction.Execute();
        yield return null;

        Assert.IsTrue(executed);
        Assert.AreEqual(MechState.Overwatch, sourceMechController.MechState);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_Execute_FailingConditions()
    {
        OverwatchAction overwatchAction = new OverwatchAction(null);
        bool executed = overwatchAction.Execute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction due to failed CanExecute check.");
        yield return null;

        Assert.IsFalse(executed);

        sourceMechController.UpdateMechState(MechState.Overwatch);
        overwatchAction = new OverwatchAction(sourceMechController);
        executed = overwatchAction.Execute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction due to failed CanExecute check.");
        yield return null;

        Assert.IsFalse(executed);

        sourceMechController.UpdateMechState(MechState.Dead);
        overwatchAction = new OverwatchAction(sourceMechController);
        executed = overwatchAction.Execute();
        LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction due to failed CanExecute check.");
        yield return null;

        Assert.IsFalse(executed);
    }

    /*[UnityTest]
    public IEnumerator TestMeleeAction_CanExecute()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMeleeAction_CanExecute_FailingConditions()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMeleeAction_Execute()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMeleeAction_Execute_OutOfRange()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMeleeAction_Execute_WeaponCantAttack()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestShootAction_CanExecute()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestShootAction_CanExecute_FailingConditions()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestShootAction_Execute_CantExecute()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestShootAction_Execute_WeaponCantAttack()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestShootAction_Execute_CantFindShootProjectile()
    {
        Assert.Fail("Test not implemented yet.");
        yield return null;
    }
    }*/
}