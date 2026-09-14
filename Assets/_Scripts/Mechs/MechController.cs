using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using HeartOfIron.Enums;

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
    public Transform Target;
    public Transform Head;
    public Transform Torso;
    public Transform LeftArm;
    public Transform RightArm;
    public Transform LeftLeg;
    public Transform RightLeg;
    public List<Transform> Limbs => new List<Transform> { Torso, Head, LeftLeg, RightLeg, LeftArm, RightArm };
    public Animator Animator;

    void Awake()
    {
        Debug.Log($"MechController Awake: {gameObject.name}");
        EventRouter.RegisterWithHub(this);
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
                            Action secondaryAction = firstActionMarker.ActionPair.SecondaryAction;
                            ActionResult actionExecuted = secondaryAction.Execute();
                            switch (secondaryAction.ActionType)
                            {
                                case ActionType.Shoot:
                                case ActionType.Melee:
                                    if (actionExecuted == ActionResult.Success && _actionMarkers.Count > 1)
                                    {
                                        _actionMarkers.Remove(firstActionMarker);
                                        Destroy(firstActionMarker.gameObject);
                                    } else if (actionExecuted != ActionResult.Retry)
                                    {
                                        _actionMarkers.Remove(firstActionMarker);
                                        Destroy(firstActionMarker.gameObject);
                                    } else if (actionExecuted == ActionResult.Retry)
                                    {
                                        FaceTarget(secondaryAction.TargetMech.transform.position);
                                    }
                                    break;
                                case ActionType.Reload:
                                    if (actionExecuted != ActionResult.Retry)
                                    {
                                        _actionMarkers.Remove(firstActionMarker);
                                        Destroy(firstActionMarker.gameObject);
                                    }
                                    break;
                                case ActionType.Move:
                                case ActionType.Charge:
                                case ActionType.Hunker:
                                case ActionType.Overwatch:
                                    _actionMarkers.Remove(firstActionMarker);
                                    Destroy(firstActionMarker.gameObject);
                                    break;
                                default:
                                    Debug.LogWarning($"Secondary action of type {secondaryAction.ActionType} is not handled for ActionMarker: {firstActionMarker.Id}");
                                    break;
                            }
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

    private void FaceTarget(Vector3 targetPosition)
    {
        if (_navMeshAgent != null)
        {
            _navMeshAgent.updateRotation = false; // Disable automatic rotation so we can manually control it
            Vector3 targetDirection = targetPosition - transform.position;
            targetDirection.y = 0; // Keep only the horizontal direction
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _navMeshAgent.angularSpeed);
            }
        }
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
        GameObject overwatchChild = new GameObject("Overwatch");
        overwatchChild.transform.parent = gameObject.transform;
        _overwatchScript = overwatchChild.AddComponent<OverwatchScript>();
        _overwatchScript.Initialize(this);
        GameObject visionChild = new GameObject("MechVision");
        visionChild.transform.parent = gameObject.transform;
        MechVision mechVision = visionChild.AddComponent<MechVision>();
        mechVision.SetVisionParameters(_mech.VisionRange, _mech.VisionAngle);
        if (Animator != null)
        {
            Animator.SetBool("IsMoving", false);
            // Set if arms are active based on health values > 0
            if (_mech.Health[(int)BodyPart.LeftArm] > 0)
            {
                Animator.SetBool("LeftArmActive", true);
                Animator.SetFloat("LeftArmBlend", 0f);
            }
            if (_mech.Health[(int)BodyPart.RightArm] > 0)
            {
                Animator.SetBool("RightArmActive", true);
                Animator.SetFloat("RightArmBlend", 0f);
            }
            // Set arm blend values based on equipped items and health (no item or 0 health = 0 blend, one handed = 0.5 blend, two handed = 1 blend)
            // If weapon is melee, toggle melee for each arm accordingly
            Weapon rightWeapon = _mech.Inventory.Weapons[(int)Hand.Right];
            Weapon leftWeapon = _mech.Inventory.Weapons[(int)Hand.Left];
            if (rightWeapon == null)
            {
                Animator.SetFloat("RightArmBlend", 0f);
            } else if (rightWeapon.SlotsRequired == 1)
            {
                Animator.SetFloat("RightArmBlend", 0.5f);
                if (rightWeapon.IsMelee)
                {
                    Animator.SetBool("MeleeR", true);
                }
            } else if (rightWeapon.SlotsRequired == 2)
            {
                Animator.SetFloat("RightArmBlend", 1f);
                Animator.SetFloat("LeftArmBlend", 1f);
                if (rightWeapon.IsMelee)
                {
                    Animator.SetBool("MeleeR", true);
                    Animator.SetBool("MeleeL", true);
                }
            }
            if (leftWeapon == null)
            {
                Animator.SetFloat("LeftArmBlend", 0f);
            } else if (leftWeapon.SlotsRequired == 1)
            {
                Animator.SetFloat("LeftArmBlend", 0.5f);
                if (leftWeapon.IsMelee)
                {
                    Animator.SetBool("MeleeL", true);
                }
            }
        }
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
                    Animator.SetBool("IsMoving", false); // Ensure the moving animation is turned off when leaving the moving state
                    break;
                case MechState.Brawling:
                    _currentSpeed = _currentSpeed * 2;
                    break;
                case MechState.Charging:
                    Animator.SetBool("IsMoving", false); // Ensure the moving animation is turned off when leaving the charging state
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
                    Animator.SetBool("IsMoving", true);
                    break;
                case MechState.Brawling:
                    _currentSpeed *= 0.5f;
                    break;
                case MechState.Charging:
                    _moveTimer = MOVE_TIMER_RESET_VALUE; // Reset the move timer when entering the charging state
                    Animator.SetBool("IsMoving", true); // Ensure the moving animation is turned on when charging
                    break;
                case MechState.Overwatch:
                    _overwatchScript?.ActivateOverwatch(); // Activate overwatch when entering the overwatch state
                    break;
                case MechState.Hunkering:
                    break;
                case MechState.Dead:
                    _navMeshAgent.ResetPath(); // Stop movement when the mech is dead
                    Animator.SetBool("IsMoving", false); // Ensure the moving animation is turned off when dead
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
        Animator.SetBool("IsMoving", false); // Ensure the moving animation is turned off when stopping movement
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
        // TODO: Implement logic for applying shader effects or visual feedback for damage taken
        if (_mech.Health[location] <= 0 && (location == (int)BodyPart.Head || location == (int)BodyPart.Torso))
        {
            UpdateMechState(MechState.Dead);
            EventRouter.OnMechDestroyed?.Invoke(this);
        }
        else if (_mech.Health[location] <= 0 && (location == (int)BodyPart.LeftArm || location == (int)BodyPart.RightArm))
        {
            Debug.LogWarning($"{_mech.Name} has lost its weapon at location {location}.");
            // TODO: Update animator parameter for weapon loss
            if (location == (int)BodyPart.LeftArm)
            {
                // TODO: Handle left arm weapon loss
                Animator.SetBool("LeftArmActive", false);
            }
            else if (location == (int)BodyPart.RightArm)
            {
                // TODO: Handle right arm weapon loss
                Animator.SetBool("RightArmActive", false);
            }
        }
        else if (_mech.Health[location] <= 0 && (location == (int)BodyPart.LeftLeg || location == (int)BodyPart.RightLeg))
        {
            Debug.LogWarning($"{_mech.Name} has lost its mobility at location {location}.");
            _currentSpeed *= 0.51f;
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
                // TODO: Adjust for disabled arms affecting shooting accuracy
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
                // TODO: Adjust for disabled arms affecting melee attacks
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
