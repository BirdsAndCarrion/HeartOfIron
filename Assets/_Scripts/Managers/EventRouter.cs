using UnityEngine;
using UnityEngine.Events;
using HeartOfIron.Enums;
using System.Collections.Generic;

public static class EventRouter
{

    public static UnityEvent<MechController> OnMechDeclaredActive = new UnityEvent<MechController>();
    public static readonly Queue<UnityEvent<MechController>> MechDeclaredActiveEventQueue = new Queue<UnityEvent<MechController>>();
    private static bool isProcessingMechDeclaredActiveQueue = false;
    public static UnityEvent<MechController> OnMechSelected = new UnityEvent<MechController>();
    public static readonly Queue<UnityEvent<MechController>> MechSelectedEventQueue = new Queue<UnityEvent<MechController>>();
    private static bool isProcessingMechSelectedQueue = false;
    public static UnityEvent<MechController> OnMechDestroyed = new UnityEvent<MechController>();
    public static readonly Queue<UnityEvent<MechController>> MechDestroyedEventQueue = new Queue<UnityEvent<MechController>>();
    private static bool isProcessingMechDestroyedQueue = false;

    public static UnityEvent<MechController> ROnMechDeclaredActive = new UnityEvent<MechController>();
    public static UnityEvent<MechController> ROnMechSelected = new UnityEvent<MechController>();
    public static UnityEvent<MechController> ROnMechDestroyed = new UnityEvent<MechController>();

    private static int mechRegisteredCount = 0;

    public static void RegisterWithHub(MechController mechController)
    {
        if (mechController == null)
        {
            Debug.LogError("Attempted to register a null MechController with the EventRouter.");
            return;
        }

        // Remove first so repeated registrations (e.g. multiple instances calling Awake) don't stack duplicate listeners on these static events.
        OnMechDeclaredActive.RemoveListener(OnMechDeclaredActiveHandler);
        OnMechSelected.RemoveListener(OnMechSelectedHandler);
        OnMechDestroyed.RemoveListener(OnMechDestroyedHandler);

        OnMechDeclaredActive.AddListener(OnMechDeclaredActiveHandler);
        OnMechSelected.AddListener(OnMechSelectedHandler);
        OnMechDestroyed.AddListener(OnMechDestroyedHandler);

        mechRegisteredCount++;
    }

    public static void UnregisterFromHub(MechController mechController)
    {
        if (mechController == null)
        {
            Debug.LogError("Attempted to unregister a null MechController from the EventRouter.");
            return;
        }

        mechRegisteredCount = Mathf.Max(0, mechRegisteredCount - 1);
        if (mechRegisteredCount == 0)
        {
            OnMechDeclaredActive.RemoveListener(OnMechDeclaredActiveHandler);
            OnMechSelected.RemoveListener(OnMechSelectedHandler);
            OnMechDestroyed.RemoveListener(OnMechDestroyedHandler);
        }
    }

    private static void OnMechDeclaredActiveHandler(MechController mechController)
    {
        Debug.Log($"EventRouter: Mech declared active: {mechController.Mech.Name}");
        MechDeclaredActiveEventQueue.Enqueue(ROnMechDeclaredActive);
        if (!isProcessingMechDeclaredActiveQueue)
        {
            isProcessingMechDeclaredActiveQueue = true;
            while (MechDeclaredActiveEventQueue.Count > 0)
            {
                var evt = MechDeclaredActiveEventQueue.Dequeue();
                evt?.Invoke(mechController);
            }
            isProcessingMechDeclaredActiveQueue = false;
        }
    }

    private static void OnMechSelectedHandler(MechController mechController)
    {
        Debug.Log($"EventRouter: Mech selected: {mechController.Mech.Name}");
        MechSelectedEventQueue.Enqueue(ROnMechSelected);
        if (!isProcessingMechSelectedQueue)
        {
            isProcessingMechSelectedQueue = true;
            while (MechSelectedEventQueue.Count > 0)
            {
                var evt = MechSelectedEventQueue.Dequeue();
                evt?.Invoke(mechController);
            }
            isProcessingMechSelectedQueue = false;
        }
    }

    private static void OnMechDestroyedHandler(MechController mechController)
    {
        Debug.Log($"EventRouter: Mech destroyed: {mechController.Mech.Name}");
        MechDestroyedEventQueue.Enqueue(ROnMechDestroyed);
        if (!isProcessingMechDestroyedQueue)
        {
            isProcessingMechDestroyedQueue = true;
            while (MechDestroyedEventQueue.Count > 0)
            {
                var evt = MechDestroyedEventQueue.Dequeue();
                evt?.Invoke(mechController);
            }
            isProcessingMechDestroyedQueue = false;
        }
    }

    public static UnityEvent OnCombatStarted = new UnityEvent();
    public static UnityEvent<EndCondition, int> OnCombatEnded = new UnityEvent<EndCondition, int>();
    public static UnityEvent OnDeploymentStarted = new UnityEvent();
    public static UnityEvent<CombatState> OnCombatStateChanged = new UnityEvent<CombatState>();

    public static UnityEvent ROnCombatStarted = new UnityEvent();
    public static UnityEvent<EndCondition, int> ROnCombatEnded = new UnityEvent<EndCondition, int>();
    public static UnityEvent ROnDeploymentStarted = new UnityEvent();
    public static UnityEvent<CombatState> ROnCombatStateChanged = new UnityEvent<CombatState>();

    public static UnityEvent OnExtractionZoneEnabled = new UnityEvent();
    public static UnityEvent OnExtractionZoneDisabled = new UnityEvent();

    public static UnityEvent ROnExtractionZoneEnabled = new UnityEvent();
    public static UnityEvent ROnExtractionZoneDisabled = new UnityEvent();

    public static void RegisterWithHub(CombatManager combatManager)
    {
        if (combatManager == null)
        {
            Debug.LogError("Attempted to register a null CombatManager with the EventRouter.");
            return;
        }

        // Remove first so repeated registrations don't stack duplicate listeners on these static events.
        OnCombatStarted.RemoveListener(OnCombatStartedHandler);
        OnCombatEnded.RemoveListener(OnCombatEndedHandler);
        OnDeploymentStarted.RemoveListener(OnDeploymentStartedHandler);
        OnCombatStateChanged.RemoveListener(OnCombatStateChangedHandler);
        OnExtractionZoneEnabled.RemoveListener(OnExtractionZoneEnabledHandler);
        OnExtractionZoneDisabled.RemoveListener(OnExtractionZoneDisabledHandler);

        OnCombatStarted.AddListener(OnCombatStartedHandler);
        OnCombatEnded.AddListener(OnCombatEndedHandler);
        OnDeploymentStarted.AddListener(OnDeploymentStartedHandler);
        OnCombatStateChanged.AddListener(OnCombatStateChangedHandler);
        OnExtractionZoneEnabled.AddListener(OnExtractionZoneEnabledHandler);
        OnExtractionZoneDisabled.AddListener(OnExtractionZoneDisabledHandler);
    }

    public static void UnregisterFromHub(CombatManager combatManager)
    {
        if (combatManager == null)
        {
            Debug.LogError("Attempted to unregister a null CombatManager from the EventRouter.");
            return;
        }

        OnCombatStarted.RemoveListener(OnCombatStartedHandler);
        OnCombatEnded.RemoveListener(OnCombatEndedHandler);
        OnDeploymentStarted.RemoveListener(OnDeploymentStartedHandler);
        OnCombatStateChanged.RemoveListener(OnCombatStateChangedHandler);
        OnExtractionZoneEnabled.RemoveListener(OnExtractionZoneEnabledHandler);
        OnExtractionZoneDisabled.RemoveListener(OnExtractionZoneDisabledHandler);
    }

    private static void OnCombatStartedHandler()
    {
        Debug.Log("EventRouter: Combat started");
        ROnCombatStarted?.Invoke();
    }

    private static void OnExtractionZoneEnabledHandler()
    {
        Debug.Log("EventRouter: Extraction zone enabled");
        ROnExtractionZoneEnabled?.Invoke();
    }

    private static void OnExtractionZoneDisabledHandler()
    {
        Debug.Log("EventRouter: Extraction zone disabled");
        ROnExtractionZoneDisabled?.Invoke();
    }

    private static void OnCombatEndedHandler(EndCondition endCondition, int rewards)
    {
        Debug.Log($"EventRouter: Combat ended. EndCondition: {endCondition}");
        ROnCombatEnded?.Invoke(endCondition, rewards);
    }

    private static void OnDeploymentStartedHandler()
    {
        Debug.Log("EventRouter: Deployment started");
        // await WaitForCombatUIManagerStarted();
        ROnDeploymentStarted?.Invoke();
    }

    private static void OnCombatStateChangedHandler(CombatState combatState)
    {
        Debug.Log($"EventRouter: Combat state changed: {combatState}");
        // await WaitForCombatUIManagerStarted();
        ROnCombatStateChanged?.Invoke(combatState);
    }

    /* private static async Task WaitForCombatUIManagerStarted()
    {
        while (CombatUIManager.Instance == null || !CombatUIManager.Instance.IsInitialized)
        {
            await Task.Delay(100);
        }
    } */

    public static UnityEvent<MechController, MechController> OnMechSpotted = new UnityEvent<MechController, MechController>();
    public static readonly Queue<UnityEvent<MechController, MechController>> MechSpottedEventQueue = new Queue<UnityEvent<MechController, MechController>>();
    private static bool isProcessingMechSpottedQueue = false;

    public static UnityEvent<MechController, MechController> OnMechLost = new UnityEvent<MechController, MechController>();
    public static readonly Queue<UnityEvent<MechController, MechController>> MechLostEventQueue = new Queue<UnityEvent<MechController, MechController>>();
    private static bool isProcessingMechLostQueue = false;

    public static UnityEvent<MechController, MechController> ROnMechSpotted = new UnityEvent<MechController, MechController>();
    public static UnityEvent<MechController, MechController> ROnMechLost = new UnityEvent<MechController, MechController>();

    public static void RegisterWithHub(MechVision mechVision)
    {
        if (mechVision == null)
        {
            Debug.LogError("Attempted to register a null MechVision with the EventRouter.");
            return;
        }

        // Remove first so repeated registrations don't stack duplicate listeners on these static events.
        OnMechSpotted.RemoveListener(OnMechSpottedHandler);
        OnMechLost.RemoveListener(OnMechLostHandler);

        OnMechSpotted.AddListener(OnMechSpottedHandler);
        OnMechLost.AddListener(OnMechLostHandler);
    }

    public static void UnregisterFromHub(MechVision mechVision)
    {
        if (mechVision == null)
        {
            Debug.LogError("Attempted to unregister a null MechVision from the EventRouter.");
            return;
        }

        OnMechSpotted.RemoveListener(OnMechSpottedHandler);
        OnMechLost.RemoveListener(OnMechLostHandler);
    }

    private static void OnMechSpottedHandler(MechController spotter, MechController spotted)
    {
        Debug.Log($"EventRouter: Mech spotted. Spotter: {spotter.Mech.Name}, Spotted: {spotted.Mech.Name}");
        MechSpottedEventQueue.Enqueue(ROnMechSpotted);
        if (!isProcessingMechSpottedQueue)
        {
            isProcessingMechSpottedQueue = true;
            while (MechSpottedEventQueue.Count > 0)
            {
                UnityEvent<MechController, MechController> evt = MechSpottedEventQueue.Dequeue();
                evt?.Invoke(spotter, spotted);
            }
            isProcessingMechSpottedQueue = false;
        }
    }   

    private static void OnMechLostHandler(MechController spotter, MechController lost)
    {
        Debug.Log($"EventRouter: Mech lost. Spotter: {spotter.Mech.Name}, Lost: {lost.Mech.Name}");
        MechLostEventQueue.Enqueue(ROnMechLost);
        if (!isProcessingMechLostQueue)
        {
            isProcessingMechLostQueue = true;
            while (MechLostEventQueue.Count > 0)
            {
                UnityEvent<MechController, MechController> evt = MechLostEventQueue.Dequeue();
                evt?.Invoke(spotter, lost);
            }
            isProcessingMechLostQueue = false;
        }
    }

    public static UnityEvent<ActionMarker> OnActionMarkerSelected = new UnityEvent<ActionMarker>();
    public static UnityEvent<ActionMarker> ROnActionMarkerSelected = new UnityEvent<ActionMarker>();
    public static readonly Queue<UnityEvent<ActionMarker>> ActionMarkerSelectedEventQueue = new Queue<UnityEvent<ActionMarker>>();
    private static bool isProcessingActionMarkerSelectedQueue = false;

    public static void RegisterWithHub(ActionMarker actionMarker)
    {
        if (actionMarker == null)
        {
            Debug.LogError("Attempted to register a null ActionMarker with the EventRouter.");
            return;
        }

        // Remove first so repeated registrations (once per ActionMarker instance) don't stack duplicate listeners on this static event.
        OnActionMarkerSelected.RemoveListener(OnActionMarkerSelectedHandler);
        OnActionMarkerSelected.AddListener(OnActionMarkerSelectedHandler);
    }

    public static void UnregisterFromHub(ActionMarker actionMarker)
    {
        if (actionMarker == null)
        {
            Debug.LogError("Attempted to unregister a null ActionMarker from the EventRouter.");
            return;
        }

        OnActionMarkerSelected.RemoveListener(OnActionMarkerSelectedHandler);
    }

    private static void OnActionMarkerSelectedHandler(ActionMarker actionMarker)
    {
        Debug.Log($"EventRouter: ActionMarker selected. ActionMarker: {actionMarker.name}");
        ActionMarkerSelectedEventQueue.Enqueue(ROnActionMarkerSelected);
        
        if (!isProcessingActionMarkerSelectedQueue)
        {
            isProcessingActionMarkerSelectedQueue = true;
            while (ActionMarkerSelectedEventQueue.Count > 0)
            {
                UnityEvent<ActionMarker> evt = ActionMarkerSelectedEventQueue.Dequeue();
                evt?.Invoke(actionMarker);
            }
            isProcessingActionMarkerSelectedQueue = false;
        }
    }

    public static UnityEvent<VisibilityQuery> OnVisibilityChecked = new UnityEvent<VisibilityQuery>();
    public static UnityEvent<VisibilityQuery> ROnVisibilityChecked = new UnityEvent<VisibilityQuery>();
    private static readonly Queue<UnityEvent<VisibilityQuery>> VisibilityCheckedEventQueue = new Queue<UnityEvent<VisibilityQuery>>();
    private static bool isProcessingVisibilityCheckedQueue = false;

    public static void RegisterWithHub(VisibilityQuery visibilityQuery)
    {
        if (visibilityQuery == null)
        {
            Debug.LogError("Attempted to register a null VisibilityQuery with the EventRouter.");
            return;
        }

        OnVisibilityChecked.RemoveListener(OnVisibilityCheckedHandler);
        OnVisibilityChecked.AddListener(OnVisibilityCheckedHandler);
    }

    public static void UnregisterFromHub(VisibilityQuery visibilityQuery)
    {
        if (visibilityQuery == null)
        {
            Debug.LogError("Attempted to unregister a null VisibilityQuery from the EventRouter.");
            return;
        }

        OnVisibilityChecked.RemoveListener(OnVisibilityCheckedHandler);
    }

    private static void OnVisibilityCheckedHandler(VisibilityQuery visibilityQuery)
    {
        Debug.Log($"EventRouter: Visibility checked. Mech: {visibilityQuery.MechController.Mech.Name}, IsVisible: {visibilityQuery.isVisible}");
        VisibilityCheckedEventQueue.Enqueue(ROnVisibilityChecked);
        
        if (!isProcessingVisibilityCheckedQueue)
        {
            isProcessingVisibilityCheckedQueue = true;
            while (VisibilityCheckedEventQueue.Count > 0)
            {
                UnityEvent<VisibilityQuery> evt = VisibilityCheckedEventQueue.Dequeue();
                evt?.Invoke(visibilityQuery);
            }
            isProcessingVisibilityCheckedQueue = false;
        }
    }

    public static UnityEvent<Projectile, Transform> OnProjectileCollision = new UnityEvent<Projectile, Transform>();
    public static UnityEvent<Projectile, Transform> ROnProjectileCollision = new UnityEvent<Projectile, Transform>();

    public static UnityEvent<GameObject> OnTracerDeactivation = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> ROnTracerDeactivation = new UnityEvent<GameObject>();

    private static readonly Queue<UnityEvent<Projectile, Transform>> ProjectileCollisionEventQueue = new Queue<UnityEvent<Projectile, Transform>>();
    private static bool isProcessingProjectileCollisionQueue = false;
    private static readonly Queue<UnityEvent<GameObject>> TracerDeactivationEventQueue = new Queue<UnityEvent<GameObject>>();
    private static bool isProcessingTracerDeactivationQueue = false;

    public static void RegisterWithHub(Projectile projectile)
    {
        if (projectile == null)
        {
            Debug.LogError("Attempted to register a null Projectile with the EventRouter.");
            return;
        }

        OnProjectileCollision.RemoveListener(OnProjectileCollisionHandler);
        OnProjectileCollision.AddListener(OnProjectileCollisionHandler);
    }

    public static void RegisterWithHub(GameObject tracer)
    {
        if (tracer == null)
        {
            Debug.LogError("Attempted to register a null Tracer with the EventRouter.");
            return;
        }

        OnTracerDeactivation.RemoveListener(OnTracerDeactivationHandler);
        OnTracerDeactivation.AddListener(OnTracerDeactivationHandler);
    }

    public static void UnregisterFromHub(Projectile projectile)
    {
        if (projectile == null)
        {
            Debug.LogError("Attempted to unregister a null Projectile from the EventRouter.");
            return;
        }

        OnProjectileCollision.RemoveListener(OnProjectileCollisionHandler);
    }

    public static void UnregisterFromHub(GameObject tracer)
    {
        if (tracer == null)
        {
            Debug.LogError("Attempted to unregister a null Tracer from the EventRouter.");
            return;
        }

        OnTracerDeactivation.RemoveListener(OnTracerDeactivationHandler);
    }

    private static void OnProjectileCollisionHandler(Projectile projectile, Transform hitTransform)
    {
        Debug.Log($"EventRouter: Projectile collision detected. Projectile: {projectile.name}, Hit Transform: {hitTransform.name}");
        ProjectileCollisionEventQueue.Enqueue(ROnProjectileCollision);

        if (!isProcessingProjectileCollisionQueue)
        {
            isProcessingProjectileCollisionQueue = true;
            while (ProjectileCollisionEventQueue.Count > 0)
            {
                UnityEvent<Projectile, Transform> evt = ProjectileCollisionEventQueue.Dequeue();
                evt?.Invoke(projectile, hitTransform);
            }
            isProcessingProjectileCollisionQueue = false;
        }
    }
    
    private static void OnTracerDeactivationHandler(GameObject tracer)
    {
        Debug.Log($"EventRouter: Tracer deactivation detected. Tracer: {tracer.name}");
        TracerDeactivationEventQueue.Enqueue(ROnTracerDeactivation);

        if (!isProcessingTracerDeactivationQueue)
        {
            isProcessingTracerDeactivationQueue = true;
            while (TracerDeactivationEventQueue.Count > 0)
            {
                UnityEvent<GameObject> evt = TracerDeactivationEventQueue.Dequeue();
                evt?.Invoke(tracer);
            }
            isProcessingTracerDeactivationQueue = false;
        }
    }

    public static UnityEvent<GameObject, Item> OnHoverableItemTooltipEnter = new UnityEvent<GameObject, Item>();
    public static UnityEvent<GameObject, Item> ROnHoverableItemTooltipEnter = new UnityEvent<GameObject, Item>();
    private static readonly Queue<UnityEvent<GameObject, Item>> HoverableItemTooltipEnterEventQueue = new Queue<UnityEvent<GameObject, Item>>();
    private static bool isProcessingHoverableItemTooltipEnterQueue = false;
    public static UnityEvent<GameObject> OnHoverableItemTooltipExit = new UnityEvent<GameObject>();
    public static UnityEvent<GameObject> ROnHoverableItemTooltipExit = new UnityEvent<GameObject>();
    private static readonly Queue<UnityEvent<GameObject>> HoverableItemTooltipExitEventQueue = new Queue<UnityEvent<GameObject>>();
    private static bool isProcessingHoverableItemTooltipExitQueue = false;

    public static void RegisterWithHub(HoverableItemTooltip hoverableItemTooltip)
    {
        if (hoverableItemTooltip == null)
        {
            return;
        }
        OnHoverableItemTooltipEnter.RemoveListener(OnHoverableItemTooltipEnterHandler);
        OnHoverableItemTooltipExit.RemoveListener(OnHoverableItemTooltipExitHandler);
        OnHoverableItemTooltipEnter.AddListener(OnHoverableItemTooltipEnterHandler);
        OnHoverableItemTooltipExit.AddListener(OnHoverableItemTooltipExitHandler);
    }

    public static void UnregisterFromHub(HoverableItemTooltip hoverableItemTooltip)
    {
        if (hoverableItemTooltip == null)
        {
            return;
        }
        OnHoverableItemTooltipEnter.RemoveListener(OnHoverableItemTooltipEnterHandler);
        OnHoverableItemTooltipExit.RemoveListener(OnHoverableItemTooltipExitHandler);
    }

    private static void OnHoverableItemTooltipEnterHandler(GameObject go, Item item)
    {
        // Debug.Log($"OnHoverableItemTooltipEnterHandler invoked for GameObject: {go.name}, Item: {item?.Name}");
        HoverableItemTooltipEnterEventQueue.Enqueue(ROnHoverableItemTooltipEnter);

        if (!isProcessingHoverableItemTooltipEnterQueue)
        {
            isProcessingHoverableItemTooltipEnterQueue = true;
            while (HoverableItemTooltipEnterEventQueue.Count > 0)
            {
                UnityEvent<GameObject, Item> evt = HoverableItemTooltipEnterEventQueue.Dequeue();
                evt?.Invoke(go, item);
            }
            isProcessingHoverableItemTooltipEnterQueue = false;
        }
    }
    
    private static void OnHoverableItemTooltipExitHandler(GameObject go)
    {
        HoverableItemTooltipExitEventQueue.Enqueue(ROnHoverableItemTooltipExit);

        if (!isProcessingHoverableItemTooltipExitQueue)
        {
            isProcessingHoverableItemTooltipExitQueue = true;
            while (HoverableItemTooltipExitEventQueue.Count > 0)
            {
                UnityEvent<GameObject> evt = HoverableItemTooltipExitEventQueue.Dequeue();
                evt?.Invoke(go);
            }
            isProcessingHoverableItemTooltipExitQueue = false;
        }
    }
}
