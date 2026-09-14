using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using HeartOfIron.Enums;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class MechController : MonoBehaviour, ISelectable, IDamageable
{
    [SerializeField] private float ACTION_MARKER_REACHED_THRESHOLD = 1.5f;
    [SerializeField] private float MOVE_TIMER_RESET_VALUE = 0.5f;
    [SerializeField] private float MOVING_AIM_PENALTY = 0.1f;
    private Mech _mech;
    public Mech Mech => _mech;
    private float _currentSpeed;
    public float CurrentSpeed => _currentSpeed;
    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    private List<ActionMarker> _actionMarkers;
    public List<ActionMarker> ActionMarkers => _actionMarkers;
    private MechState _mechState;
    public MechState MechState => _mechState;
    private float _moveTimer;
    public float MoveTimer => _moveTimer;
    private OverwatchScript _overwatchScript;
    public OverwatchScript OverwatchScript => _overwatchScript;
    private ShootProjectile[] _shootProjectile;
    public ShootProjectile[] ShootProjectiles => _shootProjectile;

   

    void Awake()
    {
        Debug.Log($"MechController Awake: {gameObject.name}");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing from the MechController.");
        } else
        {
            Debug.Log($"NavMeshAgent found: {_navMeshAgent.name}");
            // Clear any path/destination the agent may have inherited from a reused native slot (e.g. a just-destroyed agent in tests).
            if (_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.ResetPath();
            }
        }
        _actionMarkers = new List<ActionMarker>();
        if (_overwatchScript == null)
        {
            Debug.Log("OverwatchScript component is missing from the MechController. Attempting to find it in children.");
        }
        foreach (OverwatchScript overwatchScript in GetComponentsInChildren<OverwatchScript>())
        {
            if (overwatchScript.gameObject != gameObject)
            {
                _overwatchScript = overwatchScript;
                break;
            } else {
                Debug.LogWarning("Found OverwatchScript on the same GameObject as MechController, which is unexpected.");
            }
        }
        if (_overwatchScript == null)
        {
            _overwatchScript = GetComponent<OverwatchScript>();
        }
        Debug.Log($"OverwatchScript found: {_overwatchScript != null} at {(_overwatchScript != null ? _overwatchScript.gameObject.name : "null")}");
        if (_overwatchScript == null)
        {
            Debug.LogError("OverwatchScript component is missing from the children of the MechController.");
        }
    }

    void Update()
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.speed = _currentSpeed + GetBonus(ActionType.Move); // Update the NavMeshAgent speed based on the mech's current speed and any movement bonuses
        }
        // Debug.Log($"{Mech.Name}: Mech Speed: {_mech.MaxSpeed} Current Speed: {_currentSpeed}, NavMeshAgent Speed: {_navMeshAgent.speed}");
        if (CombatManager.Instance != null && CombatManager.Instance.CombatState == CombatState.Playing)
        {
            foreach (Weapon weapon in _mech.Inventory.Weapons)
            {
                if (weapon != null)
                    weapon.UpdateCooldown(Time.deltaTime);
            }
            _moveTimer -= Time.deltaTime;
            if (_moveTimer <= 0f)
            {
                _moveTimer = 0f;
                _mechState = MechState.Idle;
            }
            if (_actionMarkers != null && _actionMarkers.Count > 0)
            {
                ActionMarker firstActionMarker = _actionMarkers[0];
                if (firstActionMarker != null)
                {
                    // Check if the mech has reached the destination of the first action marker
                    if (Vector3.Distance(transform.position, firstActionMarker.transform.position) <= ACTION_MARKER_REACHED_THRESHOLD)
                    {
                        // Check if the action pair has a secondary action
                        if (firstActionMarker.ActionPair != null && firstActionMarker.ActionPair.SecondaryAction != null)
                        {
                            // Execute the secondary action
                            bool actionExecuted = firstActionMarker.ActionPair.SecondaryAction.Execute();
                            // Remove the action marker from the list and destroy it
                            _actionMarkers.Remove(firstActionMarker);
                            Destroy(firstActionMarker.gameObject);
                        }
                        else
                        {
                            // If there's no secondary action, just remove the action marker
                            Debug.Log($"Mech {_mech.Name} has no secondary action for ActionMarker: {firstActionMarker.Id}");
                            _actionMarkers.Remove(firstActionMarker);
                            Destroy(firstActionMarker.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log($"Mech {_mech.Name} moving towards ActionMarker: {firstActionMarker.Id} at position {firstActionMarker.transform.position}");
                        firstActionMarker.ActionPair.MovementAction.Execute();
                        _moveTimer = MOVE_TIMER_RESET_VALUE; // Reset the move timer to a small value to allow for continuous movement
                    }
                }
                else
                {
                    Debug.LogWarning("First action marker is null. Removing it from the list.");
                    _actionMarkers.RemoveAt(0);
                }
            } else if (_actionMarkers == null)
            {
                Debug.LogWarning("Action markers list is null. Initializing a new list.");
                _actionMarkers = new List<ActionMarker>();
            } else {
                Debug.Log("No action markers to process.");
            }
        }
        else
        {
            // If the combat state is not playing, stop the mech's movement
            // Debug.LogWarning("Combat state is not playing. Stopping mech movement.");
            _navMeshAgent.ResetPath();
        }

    }

    void Start()
    {
        EventRouter.RegisterWithHub(this);
    }

    void OnDestroy()
    {
        EventRouter.UnregisterFromHub(this);
    }

    public void Initialize(Mech mech)
    {
        if (mech == null)
        {
            Debug.LogError("Cannot initialize MechController with a null Mech.");
            return;
        }
        _mech = mech;
        _currentSpeed = _mech.MaxSpeed;
        if (_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is missing from current MechController.");
        }
        _navMeshAgent.speed = _currentSpeed;
        _shootProjectile = new ShootProjectile[2]; // Assuming a mech can have up to 2 shooting projectiles, adjust as needed
    }

    public void SetShootProjectiles(ShootProjectile[] shootProjectiles)
    {
        if (shootProjectiles == null)
        {
            Debug.LogWarning("Attempted to set shoot projectiles to null.");
            return;
        }
        if (shootProjectiles.Length > 2)
        {
            Debug.LogWarning("Attempted to set more than 2 shoot projectiles.");
            return;
        }
        _shootProjectile = shootProjectiles;
    }

    public bool UpdateMechState(MechState newState)
    {
        if (_mechState != newState && _mechState != MechState.Dead)
        {
            // Undo any effects of the current state before changing to the new state
            switch (_mechState)
            {
                case MechState.Idle:
                    break;
                case MechState.Moving:
                    break;
                case MechState.Brawling:
                    _currentSpeed = _mech.MaxSpeed; // Reset speed when leaving brawling state
                    break;
                case MechState.Charging:
                    break;
                case MechState.Overwatch:
                    _overwatchScript?.DeactivateOverwatch(); // Deactivate overwatch when leaving the overwatch state
                    break;
                case MechState.Hunkering:
                    break;
                case MechState.Dead:
                    Debug.LogWarning("Attempted to change state from Dead. This should not happen.");
                    return false; // Cannot change state from Dead
                default:
                    Debug.LogWarning($"Unhandled current MechState: {_mechState}");
                    break;
            }
            // Apply any effects of the new state
            switch (newState)
            {
                case MechState.Idle:
                    break;
                case MechState.Moving:
                    _moveTimer = MOVE_TIMER_RESET_VALUE; // Reset the move timer when entering the moving state
                    break;
                case MechState.Brawling:
                    _currentSpeed = _mech.MaxSpeed * 0.5f;
                    break;
                case MechState.Charging:
                    _moveTimer = MOVE_TIMER_RESET_VALUE; // Reset the move timer when entering the charging state
                    break;
                case MechState.Overwatch:
                    _overwatchScript?.ActivateOverwatch(); // Activate overwatch when entering the overwatch state
                    break;
                case MechState.Hunkering:
                    break;
                case MechState.Dead:
                    _navMeshAgent.ResetPath(); // Stop movement when the mech is dead
                    break;
                default:
                    Debug.LogWarning($"Unhandled MechState: {newState}");
                    break;
            }
            _mechState = newState;
            return true;
        }
        return false;
    }

    public void MoveTo(Vector3 destination)
    {
        if (_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is missing from current MechController.");
            return;
        } else if (_mechState == MechState.Dead)
        {
            Debug.LogWarning("Cannot move a dead mech.");
            return;
        } else if (destination == null)
        {
            Debug.LogError("Destination is null. Cannot move mech.");
            return;
        } else if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.LogError($"Destination is not on the NavMesh. Cannot move mech.");
            return;
        } else if (_navMeshAgent.isOnNavMesh == false)
        {
            Debug.LogError("NavMeshAgent is not on the NavMesh. Cannot move mech.");
            return;
        }
        Debug.Log($"Mech {_mech.Name} moving to destination: {destination}");
        _navMeshAgent.SetDestination(destination);
        _mechState = MechState.Moving;
        _moveTimer = MOVE_TIMER_RESET_VALUE; // Initialize the move timer when starting movement
    }

    public void StopMovement()
    {
        _navMeshAgent.ResetPath();
    }

    public void AddActionMarker(ActionMarker actionMarker)
    {
        if (!_actionMarkers.Contains(actionMarker) && actionMarker != null && actionMarker.Id != Guid.Empty && actionMarker.ActionPair != null && actionMarker.ActionPair.MovementAction != null)
        {
            Debug.Log($"Adding ActionMarker: {actionMarker.Id} to Mech: {_mech.Name}");
            _actionMarkers.Add(actionMarker);
        } else {
            Debug.LogWarning($"ActionMarker {actionMarker?.Id} is already in the list or is invalid. Cannot add to Mech: {_mech.Name}.");
        }
    }

    public void InsertActionMarkerAt(ActionMarker actionMarker, int index)
    {
        if (index < 0 || index > _actionMarkers.Count)
        {
            Debug.LogWarning($"Index {index} is out of bounds for inserting ActionMarker.");
            return;
        }
        if (!_actionMarkers.Contains(actionMarker) && actionMarker != null && actionMarker.Id != Guid.Empty && actionMarker.ActionPair != null && actionMarker.ActionPair.MovementAction != null)
        {
            _actionMarkers.Insert(index, actionMarker);
        }
        else
        {
            Debug.LogWarning($"ActionMarker {actionMarker?.Id} is already in the list or is invalid. Cannot insert at index {index}.");
        }
    }

    public void RemoveActionMarker(ActionMarker actionMarker)
    {
        if (_actionMarkers.Contains(actionMarker))
        {
            _actionMarkers.Remove(actionMarker);
            Destroy(actionMarker.gameObject);
        }
    }

    public float TakeDamage(float damageAmount, int location)
    {
        if (location < 0 || location >= _mech.Health.Length)
        {
            Debug.LogError($"Invalid location index {location} for mech {_mech.Name}. Damage not applied.");
            return 0f;
        }
        float overflowDamage = _mech.TakeDamage(damageAmount, location);
        // TODO: Implement logic for handling mech destruction or disabling specific parts based on the location of the damage
        // TODO: Implement logic for applying shader effects or visual feedback for damage taken
        if (_mech.Health[location] <= 0 && (location == (int)BodyPart.Head || location == (int)BodyPart.Torso))
        {
            UpdateMechState(MechState.Dead);
            EventRouter.OnMechDestroyed?.Invoke(this);
        }
        else if (_mech.Health[location] <= 0 && (location == (int)BodyPart.LeftArm || location == (int)BodyPart.RightArm))
        {
            Debug.LogWarning($"{_mech.Name} has lost its weapon at location {location}.");
        }
        else if (_mech.Health[location] <= 0 && (location == (int)BodyPart.LeftLeg || location == (int)BodyPart.RightLeg))
        {
            Debug.LogWarning($"{_mech.Name} has lost its mobility at location {location}.");
        }
        Debug.Log($"{_mech.Name} took {damageAmount} damage at location {location}. Remaining health: {_mech.Health[location]}");
        return _mech.Health[location]; // Return the remaining health at the specified location
    }

    public void Repair(float repairAmount, int location)
    {
        if (location < 0 || location >= _mech.Health.Length)
        {
            Debug.LogError($"Invalid location index {location} for mech {_mech.Name}. Repair not applied.");
            return;
        } else if (_mech.Health[location] <= 0)
        {
            Debug.LogWarning($"Mech {_mech.Name} is destroyed at location {location}. Cannot repair.");
            return;
        } else if (_mechState == MechState.Dead)
        {
            Debug.LogWarning($"Cannot repair a dead mech: {_mech.Name}.");
            return;
        }
        _mech.Repair(repairAmount, location);
        Debug.Log($"{_mech.Name} repaired {repairAmount} at location {location}. Current health: {_mech.Health[location]}");
    }

    public float GetBonus(ActionType actionType/*, bool isOffensive*/)
    {
        if (_mech == null || _mech.Pilot == null)
        {
            return 0f;
        }

        switch (actionType)
        {
            case ActionType.Shoot:
                // if (isOffensive)
                // {
                    float offensiveShootingBonus = 0f;
                    if (_mechState == MechState.Moving)
                    {
                        offensiveShootingBonus -= Mathf.Min(0, MOVING_AIM_PENALTY + _mech.Pilot.GetSkillValue(SkillType.Piloting)); // Example penalty for shooting while moving
                    }
                    offensiveShootingBonus += _mech.Pilot.GetSkillValue(SkillType.Gunnery);
                    return offensiveShootingBonus;
                // }
            case ActionType.Melee:
                // if (isOffensive)
                // {
                    return _mech.Pilot.GetSkillValue(SkillType.Piloting); // Example bonus for melee attacks
                // }
            case ActionType.Move:
                return _mech.Pilot.GetSkillValue(SkillType.Navigation); // Example bonus for movement
            default:
                return 0f;
        }
    }

    public string OnSelected()
    {
        EventRouter.OnMechSelected?.Invoke(this);
        if (_mech.Faction == Faction.Player)
        {
            EventRouter.OnMechDeclaredActive?.Invoke(this);
        }
        return $"MechController: {_mech.Name}";
    }

    public bool IsLowHealth()
    {
        return _mech.IsLowHealth();
    }
}
