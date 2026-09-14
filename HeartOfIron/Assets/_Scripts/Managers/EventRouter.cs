using UnityEngine;
using UnityEngine.Events;
using HeartOfIron.Enums;
using System.Threading.Tasks;
public static class EventRouter
{

    public static UnityEvent<MechController> OnMechDeclaredActive = new UnityEvent<MechController>();
    public static UnityEvent<MechController> OnMechSelected = new UnityEvent<MechController>();
    public static UnityEvent<MechController> OnMechDestroyed = new UnityEvent<MechController>();

    public static UnityEvent<MechController> ROnMechDeclaredActive = new UnityEvent<MechController>();
    public static UnityEvent<MechController> ROnMechSelected = new UnityEvent<MechController>();
    public static UnityEvent<MechController> ROnMechDestroyed = new UnityEvent<MechController>();

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
    }

    public static void UnregisterFromHub(MechController mechController)
    {
        if (mechController == null)
        {
            Debug.LogError("Attempted to unregister a null MechController from the EventRouter.");
            return;
        }

        OnMechDeclaredActive.RemoveListener(OnMechDeclaredActiveHandler);
        OnMechSelected.RemoveListener(OnMechSelectedHandler);
        OnMechDestroyed.RemoveListener(OnMechDestroyedHandler);
    }

    private static void OnMechDeclaredActiveHandler(MechController mechController)
    {
        Debug.Log($"EventRouter: Mech declared active: {mechController.Mech.Name}");
        ROnMechDeclaredActive?.Invoke(mechController);
    }

    private static void OnMechSelectedHandler(MechController mechController)
    {
        Debug.Log($"EventRouter: Mech selected: {mechController.Mech.Name}");
        ROnMechSelected?.Invoke(mechController);
    }

    private static void OnMechDestroyedHandler(MechController mechController)
    {
        Debug.Log($"EventRouter: Mech destroyed: {mechController.Mech.Name}");
        ROnMechDestroyed?.Invoke(mechController);
    }

    public static UnityEvent OnCombatStarted = new UnityEvent();
    public static UnityEvent<bool, int> OnCombatEnded = new UnityEvent<bool, int>();
    public static UnityEvent OnDeploymentStarted = new UnityEvent();
    public static UnityEvent<CombatState> OnCombatStateChanged = new UnityEvent<CombatState>();

    public static UnityEvent ROnCombatStarted = new UnityEvent();
    public static UnityEvent<bool, int> ROnCombatEnded = new UnityEvent<bool, int>();
    public static UnityEvent ROnDeploymentStarted = new UnityEvent();
    public static UnityEvent<CombatState> ROnCombatStateChanged = new UnityEvent<CombatState>();

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

        OnCombatStarted.AddListener(OnCombatStartedHandler);
        OnCombatEnded.AddListener(OnCombatEndedHandler);
        OnDeploymentStarted.AddListener(OnDeploymentStartedHandler);
        OnCombatStateChanged.AddListener(OnCombatStateChangedHandler);
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
    }

    private static void OnCombatStartedHandler()
    {
        Debug.Log("EventRouter: Combat started");
        ROnCombatStarted?.Invoke();
    }

    private static void OnCombatEndedHandler(bool isVictory, int rewards)
    {
        Debug.Log($"EventRouter: Combat ended. Victory: {isVictory}");
        ROnCombatEnded?.Invoke(isVictory, rewards);
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
    public static UnityEvent<MechController, MechController> OnMechLost = new UnityEvent<MechController, MechController>();

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
        ROnMechSpotted?.Invoke(spotter, spotted);
    }   

    private static void OnMechLostHandler(MechController spotter, MechController lost)
    {
        Debug.Log($"EventRouter: Mech lost. Spotter: {spotter.Mech.Name}, Lost: {lost.Mech.Name}");
        ROnMechLost?.Invoke(spotter, lost);
    }

    public static UnityEvent<ActionMarker> OnActionMarkerSelected = new UnityEvent<ActionMarker>();
    public static UnityEvent<ActionMarker> ROnActionMarkerSelected = new UnityEvent<ActionMarker>();

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
        ROnActionMarkerSelected?.Invoke(actionMarker);
    }
}
