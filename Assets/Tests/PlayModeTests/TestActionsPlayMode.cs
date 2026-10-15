using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using HeartOfIron.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TestActionsPlayMode
{
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

        

    [UnitySetUp]
    public IEnumerator Setup()
    {

        SceneManager.LoadScene("TestScene");

        yield return null; // Wait for the scene to load

        // Create source Mech
        sourceMechObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sourceMechObject.name = "SourceMech";
        sourceMechObject.transform.position = Vector3.zero;
        sourceMech = new Mech("Test Mech", MechType.Light, Faction.Player);
        sourceMechController = sourceMechObject.AddComponent<MechController>();
        sourceMechController.Animator = sourceMechObject.AddComponent<Animator>();
        sourceMechController.Initialize(sourceMech);

        Weapon weapon = new Weapon(WeaponName.LightPistol, "Test weapon", Weight.Light, 100, 10, 5, 20, 1.5f, 0.5f, 1.0f, 5, 1, 1);
        sourceMech.EquipWeapon(weapon);
        Weapon weapon2 = new Weapon(WeaponName.LightSword, "Test melee weapon", Weight.Light, 100, 0, 15, 0, 0f, 0f, 0f, 5, 1, 1);
        sourceMech.EquipWeapon(weapon2);

        // Create target Mech
        targetMechObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetMechObject.name = "TargetMech";
        targetMechObject.transform.position = new Vector3(10, 0, 10);
        targetMech = new Mech("Target Mech", MechType.Light, Faction.Enemy);
        targetMechController = targetMechObject.AddComponent<MechController>();
        targetMechController.Animator = targetMechObject.AddComponent<Animator>();
        targetMechController.Initialize(targetMech);

        // Create ActionMarker
        actionMarkerObject = new GameObject("ActionMarker");
        actionMarkerObject.transform.position = new Vector3(5, 0, 5);
        actionMarker = actionMarkerObject.AddComponent<ActionMarker>();

        Assume.That(actionMarker, Is.Not.Null);
        Assume.That(actionMarkerObject, Is.Not.Null);
        Assume.That(sourceMechController, Is.Not.Null);
        Assume.That(sourceMech, Is.Not.Null);
        Assume.That(targetMechController, Is.Not.Null);
        Assume.That(targetMech, Is.Not.Null);
    }

    [TearDown]
    public void Teardown()
    {
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
        actionMarker.Initialize(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));

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

        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
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

        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
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
        MeleeAction meleeAction1 = new MeleeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionPair actionPair = new ActionPair(moveAction, meleeAction1);

        MeleeAction meleeAction2 = new MeleeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
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

        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
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
        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
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
        MeleeAction meleeAction = new MeleeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
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
        ReloadAction reloadAction = new ReloadAction(sourceMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult canExecute = reloadAction.CanExecute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, canExecute);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_CanExecute_FailingConditions()
    {
        ReloadAction reloadAction = new ReloadAction(null, new List<Weapon> { sourceMech.Inventory.Weapons[0] });
        ActionResult canExecute = reloadAction.CanExecute();
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute);

        reloadAction = new ReloadAction(sourceMechController, null);
        canExecute = reloadAction.CanExecute();
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_Execute()
    {
        ReloadAction reloadAction = new ReloadAction(sourceMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
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
        ReloadAction reloadAction = new ReloadAction(null, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult executed = reloadAction.Execute();
        //LogAssert.Expect(LogType.Warning, "ReloadAction cannot be executed because CanExecute() returned: NullInput.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed);

        reloadAction = new ReloadAction(sourceMechController, null);
        executed = reloadAction.Execute();
        //LogAssert.Expect(LogType.Warning, "ReloadAction cannot be executed because CanExecute() returned: NullInput.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_Execute_AlreadyFullAmmo()
    {
        ReloadAction reloadAction = new ReloadAction(sourceMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult executed = reloadAction.Execute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, executed);
        Assert.AreEqual(sourceMech.Inventory.Weapons[0].CurrentAmmo, sourceMech.Inventory.Weapons[0].MaxAmmo);
    }

    [UnityTest]
    public IEnumerator TestReloadAction_Execute_WeaponOnCooldown()
    {
        ReloadAction reloadAction = new ReloadAction(sourceMechController, new List<Weapon>{sourceMech.Inventory.Weapons[0]});
        sourceMech.Inventory.Weapons[0].ConsumeAmmo(5); // Consume some ammo to test reloading
        ActionResult executed = reloadAction.Execute();
        //LogAssert.Expect(LogType.Warning, $"{sourceMech.Name} cannot reload {sourceMech.Inventory.Weapons[0].Name} because it is on cooldown.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_CanExecute()
    {
        HunkerAction hunkerAction = new HunkerAction(sourceMechController);
        ActionResult canExecute = hunkerAction.CanExecute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, canExecute);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_CanExecute_FailingConditions()
    {
        HunkerAction hunkerAction = new HunkerAction(null);
        ActionResult canExecute = hunkerAction.CanExecute();
        LogAssert.Expect(LogType.Warning, "HunkerAction cannot be executed because SourceMech is null.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute);

        sourceMechController.UpdateMechState(MechState.Hunkering);
        hunkerAction = new HunkerAction(sourceMechController);
        ActionResult canExecute2 = hunkerAction.CanExecute();
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute2);

        sourceMechController.UpdateMechState(MechState.Dead);
        hunkerAction = new HunkerAction(sourceMechController);
        ActionResult canExecute3 = hunkerAction.CanExecute();
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute3);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_Execute()
    {
        HunkerAction hunkerAction = new HunkerAction(sourceMechController);
        ActionResult executed = hunkerAction.Execute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, executed);
        Assert.AreEqual(MechState.Hunkering, sourceMechController.MechState);
    }

    [UnityTest]
    public IEnumerator TestHunkerAction_Execute_FailingConditions()
    {
        HunkerAction hunkerAction = new HunkerAction(null);
        ActionResult executed = hunkerAction.Execute();
        LogAssert.Expect(LogType.Warning, "HunkerAction cannot be executed because SourceMech is null.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed);
        Assert.AreNotEqual(MechState.Hunkering, sourceMechController.MechState);

        sourceMechController.UpdateMechState(MechState.Hunkering);
        hunkerAction = new HunkerAction(sourceMechController);
        ActionResult executed2 = hunkerAction.Execute();
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed2);
        Assert.AreEqual(MechState.Hunkering, sourceMechController.MechState);

        sourceMechController.UpdateMechState(MechState.Dead);
        hunkerAction = new HunkerAction(sourceMechController);
        ActionResult executed3 = hunkerAction.Execute();
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed3);
        Assert.AreNotEqual(MechState.Hunkering, sourceMechController.MechState);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_CanExecute()
    {
        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult canExecute = chargeAction.CanExecute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, canExecute);
    
    
    }

    [UnityTest]
    public IEnumerator TestChargeAction_CanExecute_FailingConditions()
    {
        ChargeAction chargeAction = new ChargeAction(null, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult canExecute = chargeAction.CanExecute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute);

        ChargeAction chargeAction2 = new ChargeAction(sourceMechController, null, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult canExecute2 = chargeAction2.CanExecute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the target mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute2);

        ChargeAction chargeAction3 = new ChargeAction(sourceMechController, targetMechController, null);
        ActionResult canExecute3 = chargeAction3.CanExecute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the weapon is null.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute3);

        ChargeAction chargeActionDeadSource = new ChargeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        sourceMechController.UpdateMechState(MechState.Dead);
        ActionResult canExecute4 = chargeActionDeadSource.CanExecute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute4);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_Execute()
    {
        GameObject actionMarkerAssembler = new GameObject("ActionMarkerAssembler");
        actionMarkerAssembler.AddComponent<PseudoActionMarkerAssembler>();

        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult executed = chargeAction.Execute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, executed);
        Assert.AreEqual(MechState.Charging, sourceMechController.MechState);
        Assert.IsNotNull(sourceMechController.ActionMarkers);
        Assert.AreEqual(1, sourceMechController.ActionMarkers.Count);

        GameObject.DestroyImmediate(actionMarkerAssembler);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_Execute_FailingConditions()
    {
        ChargeAction chargeAction = new ChargeAction(null, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult executed = chargeAction.Execute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed);

        ChargeAction chargeAction2 = new ChargeAction(sourceMechController, null, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult executed2 = chargeAction2.Execute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the target mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed2);

        ChargeAction chargeAction3 = new ChargeAction(sourceMechController, targetMechController, null);
        ActionResult executed3 = chargeAction3.Execute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute ChargeAction because the weapon is null.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed3);
    }

    [UnityTest]
    public IEnumerator TestChargeAction_Execute_AlreadyCharging()
    {
        sourceMechController.UpdateMechState(MechState.Charging);
        ChargeAction chargeAction = new ChargeAction(sourceMechController, targetMechController, new List<Weapon>(sourceMech.Inventory.Weapons));
        ActionResult executed = chargeAction.Execute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, executed);
        Assert.AreEqual(MechState.Charging, sourceMechController.MechState);
        Assert.IsNotNull(sourceMechController.ActionMarkers);
        Assert.AreEqual(0, sourceMechController.ActionMarkers.Count);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_CanExecute()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionResult canExecute = moveAction.CanExecute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, canExecute);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_CanExecute_FailingConditions()
    {
        MoveAction moveAction = new MoveAction(null, new Vector3(5, 0, 5));
        ActionResult canExecute = moveAction.CanExecute();
        //LogAssert.Expect(LogType.Warning, "MoveAction cannot be executed because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute);

        MoveAction moveAction2 = new MoveAction(sourceMechController, new Vector3(1000, 0, 1000)); // Assuming this position is unreachable
        ActionResult canExecute2 = moveAction2.CanExecute();
        //LogAssert.Expect(LogType.Log, $"Cannot execute MoveAction. No valid path to destination {new Vector3(1000, 0, 1000)}.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute2);

        sourceMechController.UpdateMechState(MechState.Dead);
        moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionResult canExecute3 = moveAction.CanExecute();
        //LogAssert.Expect(LogType.Warning, "MoveAction cannot be executed because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute3);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_Execute()
    {
        MoveAction moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionResult executed = moveAction.Execute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, executed);
    }

    [UnityTest]
    public IEnumerator TestMoveAction_Execute_FailingConditions()
    {
        MoveAction moveAction = new MoveAction(null, new Vector3(5, 0, 5));
        ActionResult executed = moveAction.Execute();
        //LogAssert.Expect(LogType.Warning, "Cannot execute MoveAction. Conditions not met.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed);

        MoveAction moveAction2 = new MoveAction(sourceMechController, new Vector3(1000, 0, 1000)); // Assuming this position is unreachable
        ActionResult executed2 = moveAction2.Execute();
        //LogAssert.Expect(LogType.Log, $"Cannot execute MoveAction. No valid path to destination {new Vector3(1000, 0, 1000)}.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed2);

        sourceMechController.UpdateMechState(MechState.Dead);
        moveAction = new MoveAction(sourceMechController, new Vector3(5, 0, 5));
        ActionResult executed3 = moveAction.Execute();
        //LogAssert.Expect(LogType.Warning, "MoveAction cannot be executed because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed3);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_CanExecute()
    {
        OverwatchAction overwatchAction = new OverwatchAction(sourceMechController);
        ActionResult canExecute = overwatchAction.CanExecute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, canExecute);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_CanExecute_FailingConditions()
    {
        OverwatchAction overwatchAction = new OverwatchAction(null);
        ActionResult canExecute = overwatchAction.CanExecute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute);

        sourceMechController.UpdateMechState(MechState.Overwatch);
        overwatchAction = new OverwatchAction(sourceMechController);
        ActionResult canExecute2 = overwatchAction.CanExecute();
        //LogAssert.Expect(LogType.Warning, $"{sourceMechController.Mech.Name}: Cannot execute OverwatchAction because the mech is already in overwatch.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute2);

        sourceMechController.UpdateMechState(MechState.Dead);
        overwatchAction = new OverwatchAction(sourceMechController);
        ActionResult canExecute3 = overwatchAction.CanExecute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction because the mech is null or dead.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, canExecute3);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_Execute()
    {
        OverwatchAction overwatchAction = new OverwatchAction(sourceMechController);
        ActionResult executed = overwatchAction.Execute();
        yield return null;

        Assert.AreEqual(ActionResult.Success, executed);
        Assert.AreEqual(MechState.Overwatch, sourceMechController.MechState);
    }

    [UnityTest]
    public IEnumerator TestOverwatchAction_Execute_FailingConditions()
    {
        OverwatchAction overwatchAction = new OverwatchAction(null);
        ActionResult executed = overwatchAction.Execute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction due to failed CanExecute check.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed);

        sourceMechController.UpdateMechState(MechState.Overwatch);
        overwatchAction = new OverwatchAction(sourceMechController);
        ActionResult executed2 = overwatchAction.Execute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction due to failed CanExecute check.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed2);

        sourceMechController.UpdateMechState(MechState.Dead);
        overwatchAction = new OverwatchAction(sourceMechController);
        ActionResult executed3 = overwatchAction.Execute();
        //LogAssert.Expect(LogType.Warning, $"Cannot execute OverwatchAction due to failed CanExecute check.");
        yield return null;

        Assert.AreNotEqual(ActionResult.Success, executed3);
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