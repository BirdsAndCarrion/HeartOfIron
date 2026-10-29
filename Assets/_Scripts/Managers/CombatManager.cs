using UnityEngine;
using System.Collections.Generic;
using HeartOfIron.Enums;

/// <summary>
/// Manages combat logic, including player and enemy mechs, combat state, and rewards.
/// Handles the flow of combat, including starting and ending combat, calculating rewards, and updating the combat timer.
/// Also manages the active mech, enemy mech, and tracks destroyed enemies.
/// Provides methods to start and end combat, and calculate rewards based on combat outcomes.
/// </summary>
/// <remarks>
/// This manager assumes the presence of an EventRouter for handling combat-related events.
/// It works in conjunction with the UnitManager and EnemyLogicManager to manage the state and behavior of mechs during combat.
/// It relies on CombatUIManager to handle the user interface aspects of combat, such as displaying the combat timer and combat state.
/// </remarks>
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    /// <summary>
    /// The maximum number of player mechs allowed in combat.
    /// </summary>
    [SerializeField] private int PLAYER_MECH_LIMIT = 3;
    /// <summary>
    /// The width of the terrain for combat.
    /// </summary>
    [SerializeField] private int TERRAIN_WIDTH = 1000;
    /// <summary>
    /// The length of the terrain for combat.
    /// </summary>
    [SerializeField] private int TERRAIN_LENGTH = 1000;

    /// <summary>
    /// The reward given for each destroyed enemy.
    /// </summary>
    [SerializeField] private int REWARD_PER_DESTROYED_ENEMY = 100;
    /// <summary>
    /// The reward given for surviving combat.
    /// </summary>
    [SerializeField] private int REWARD_FOR_SURVIVAL = 350;
    /// <summary>
    /// The multiplier applied if all enemies are destroyed to complete the mission.
    /// </summary>
    [SerializeField] private float MULTIPLIER_FOR_DESTROYING_ALL_ENEMIES = 1.5f;

    private MechController _activeMech;
    public MechController ActiveMech => _activeMech;
    private MechController _enemyMech;
    public MechController EnemyMech => _enemyMech;
    private List<MechController> _playerMechs;
    public List<MechController> PlayerMechs => _playerMechs;
    private List<MechController> _enemyMechs;
    public List<MechController> EnemyMechs => _enemyMechs;
    private List<MechController> destroyedEnemies;
    public List<MechController> DestroyedEnemies => destroyedEnemies;
    private CombatState _combatState;
    public CombatState CombatState => _combatState;
    private float _combatTimer;
    public float CombatTimer => _combatTimer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        _playerMechs = new List<MechController>();
        _enemyMechs = new List<MechController>();
        destroyedEnemies = new List<MechController>();
        
    }

    void Start()
    {
        EventRouter.RegisterWithHub(this);
        EventRouter.ROnMechDeclaredActive.AddListener(SetActiveMech);
        // EventRouter.ROnMechSelected.AddListener(SetActiveMech);
        EventRouter.ROnMechDestroyed.AddListener(HandleMechDestroyed);
        UpdateCombatState(CombatState.Loading);
        Camera.main.transform.position = new Vector3(TERRAIN_WIDTH / 2f, 50, 10);
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        EventRouter.OnDeploymentStarted?.Invoke();
    }

    void OnDestroy()
    {
        EventRouter.ROnMechDeclaredActive.RemoveListener(SetActiveMech);
        // EventRouter.ROnMechSelected.RemoveListener(SetActiveMech);
        EventRouter.ROnMechDestroyed.RemoveListener(HandleMechDestroyed);
        EventRouter.UnregisterFromHub(this);
    }

    /// <summary>
    /// Starts the combat by creating enemy mechs, spawning the extraction zone, and initializing the enemy logic.
    /// </summary>
    /// <remarks>
    /// This method assumes that the enemy mechs are created for testing purposes and that the extraction zone is properly spawned.
    /// It also adds the enemy mechs to the EnemyLogicManager's graph for AI behavior.
    /// </remarks>
    void StartCombat()
    {
        CreateTestEnemyMechs();
        SpawnExtractionZone(); // Assuming there's a method to spawn the extraction zone
        EnemyLogicManager.Instance.AddMechsToGraph(_enemyMechs);
        EventRouter.OnCombatStarted?.Invoke();
        UpdateCombatState(CombatState.Paused);
    }

    /// <summary>
    /// Ends the combat based on the specified end condition, calculates rewards, and handles cleanup of player and enemy mechs.
    /// </summary>
    /// <param name="endCondition">The condition that determines how the combat ended (e.g., Victory, Defeat, Survived).</param>
    public void EndCombat(EndCondition endCondition)
    {
        int rewards = CalculateRewards(endCondition);
        EventRouter.OnCombatEnded?.Invoke(endCondition, rewards);
        foreach (MechController mechcontroller in _playerMechs)
        {
            Mech mech = mechcontroller.Mech;
            if (mech != null)
            {
                UnitManager.Instance.RecallMech(mech);
            }
        }
        foreach (MechController mechcontroller in _enemyMechs)
        {
            Destroy(mechcontroller.gameObject);
        }
        UpdateCombatState(CombatState.GameOver);
    }

    /// <summary>
    /// Calculates the rewards based on the end condition and destroyed enemies.
    /// </summary>
    /// <param name="endCondition">The condition that determines how the combat ended.</param>
    /// <returns>The total rewards calculated.</returns>
    int CalculateRewards(EndCondition endCondition)
    {
        int rewards = destroyedEnemies.Count * REWARD_PER_DESTROYED_ENEMY;
        if (endCondition == EndCondition.Survived)
        {
            rewards += REWARD_FOR_SURVIVAL;
        } else if (endCondition == EndCondition.Victory)
        {
            rewards = Mathf.RoundToInt(rewards * MULTIPLIER_FOR_DESTROYING_ALL_ENEMIES) + REWARD_FOR_SURVIVAL;
        }
        GameManager.Instance.AddBolts(rewards);
        Debug.Log($"Rewards calculated: {rewards}");
        return rewards;
    }

    /// <summary>
    /// Updates the combat timer if the combat is in the playing state.
    /// </summary>
    void Update()
    {
        if (_combatState == CombatState.Playing)
        {
            _combatTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Sets the specified mech as the active mech.
    /// </summary>
    /// <param name="mechController">The mech controller to set as active.</param>
    public void SetActiveMech(MechController mechController)
    {
        if (mechController == null)
        {
            Debug.LogWarning("Attempted to set active mech to null.");
            return;
        }
        _activeMech = mechController;
        Debug.Log($"Active Mech set to: {_activeMech.Mech.Name}");
    }

    /// <summary>
    /// Toggles the active mech to the next available player mech.
    /// </summary>
    public void ToggleActiveMech()
    {
        if (_activeMech != null)
        {
            int currentIndex = _playerMechs.IndexOf(_activeMech);
            int nextIndex = (currentIndex + 1) % _playerMechs.Count;
            _activeMech = _playerMechs[nextIndex];
            Debug.Log($"Active Mech toggled to: {_activeMech.Mech.Name}");
        }
    }

    /// <summary>
    /// Updates the combat state to the specified new state and triggers the corresponding event.
    /// </summary>
    /// <param name="newState">The new combat state to set.</param>
    public void UpdateCombatState(CombatState newState)
    {
        _combatState = newState;
        Debug.Log($"Combat state updated to: {_combatState}");
        EventRouter.OnCombatStateChanged?.Invoke(_combatState);
    }

    /// <summary>
    /// Handles the destruction of a mech, updating player and enemy lists, and ending combat if necessary.
    /// </summary>
    /// <param name="mechController">The mech controller that was destroyed.</param>
    public void HandleMechDestroyed(MechController mechController)
    {
        if (mechController == null)
        {
            Debug.LogWarning("Attempted to handle destruction of a null mech.");
            return;
        }
        
        if (_playerMechs.Contains(mechController))
        {
            RemovePlayerMech(mechController);
            Debug.Log($"Player Mech destroyed: {mechController.Mech.Name}");
            if (_playerMechs.Count == 0)
            {
                _activeMech = null;
                EndCombat(EndCondition.Defeat);
                Debug.Log("All player mechs destroyed. Game Over.");
            }
            else if (_activeMech == mechController)
            {
                ToggleActiveMech();
            }
        }
        else if (_enemyMechs.Contains(mechController))
        {
            _enemyMechs.Remove(mechController);
            destroyedEnemies.Add(mechController);
            Destroy(mechController.gameObject);
            Debug.Log($"Enemy Mech destroyed: {mechController.Mech.Name}");
            if (_enemyMechs.Count == 0)
            {
                EndCombat(EndCondition.Victory);
                Debug.Log("All enemy mechs destroyed. You win!");
            }
        }
    }

    /// <summary>
    /// Checks if the specified position is valid for unit placement within the deployment area.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position is valid, false otherwise.</returns>
    public bool IsValidPlacementPosition(Vector3 position)
    {
        if (position.x < TERRAIN_WIDTH / 4 || position.x > 3 * TERRAIN_WIDTH / 4 || position.z > TERRAIN_LENGTH / 4)
        {
            Debug.LogWarning($"Position {position} is outside the valid deployment area.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Attempts to place a unit at the specified position if it is valid.
    /// </summary>
    /// <param name="mechData">The mech data for the unit to place.</param>
    /// <param name="position">The position to place the unit at.</param>
    /// <returns>True if the unit was successfully placed, false otherwise.</returns>
    public bool TryPlaceUnitAtPosition(Mech mechData, Vector3 position)
    {
        // Implement logic to check if the position is valid for unit placement
        MechController newMechController = MechObjectAssembler.Instance.AssembleMechObject(mechData, position, Quaternion.identity);
        if (mechData.Faction == Faction.Player)
        {
            AddPlayerMech(newMechController);
            UnitManager.Instance.DeployMech(mechData);
            if (_playerMechs.Count > PLAYER_MECH_LIMIT)
            {
                Debug.LogWarning($"Player Mech limit exceeded. Limit: {PLAYER_MECH_LIMIT}, Current: {_playerMechs.Count}");
                RemovePlayerMech(newMechController);
                return false;
            } else if (_playerMechs.Count == PLAYER_MECH_LIMIT || _playerMechs.Count >= UnitManager.Instance.PlayerMechs.Count)
            {
                Debug.Log($"Player Mech limit reached. Limit: {PLAYER_MECH_LIMIT}");
                StartCombat();
            }
        }
        // For now, let's assume all positions are valid
        Debug.Log($"Attempting to place unit at position: {position}");
        return true;
    }

    /// <summary>
    /// Adds a player mech to the list of player mechs, ensuring it does not exceed the limit.
    /// </summary>
    /// <param name="mechController">The mech controller to add.</param>
    void AddPlayerMech(MechController mechController)
    {
        if (mechController == null)
        {
            Debug.LogError("Attempted to add a null mech controller to player mechs.");
            return;
        } else if (mechController.Mech == null)
        {
            Debug.LogError("Attempted to add a mech controller with null Mech data to player mechs.");
            return;
        } else if (_playerMechs == null)
        {
            Debug.LogError("Player mechs list is null.");
            return;
        } else if (_playerMechs.Count >= PLAYER_MECH_LIMIT)
        {
            Debug.LogWarning($"Cannot add mech {mechController.Mech.Name}. Player Mech limit of {PLAYER_MECH_LIMIT} reached.");
            return;
        }
        if (!_playerMechs.Contains(mechController) && _playerMechs.Count < PLAYER_MECH_LIMIT)
        {
            _playerMechs.Add(mechController);
            if (_activeMech == null)
            {
                _activeMech = mechController;
            }
            Debug.Log($"Player Mech added: {mechController.Mech.Name}");
        }
    }

    /// <summary>
    /// Removes a player mech from the list of player mechs and destroys its game object.
    /// </summary>
    /// <param name="mechController">The mech controller to remove.</param>
    void RemovePlayerMech(MechController mechController)
    {
        if (_playerMechs.Contains(mechController))
        {
            _playerMechs.Remove(mechController);
            Destroy(mechController.gameObject);
            Debug.Log($"Player Mech removed: {mechController.Mech.Name}");
            if (_activeMech == mechController)
            {
                ToggleActiveMech();
            }
        }
    }

    /// <summary>
    /// Adds an enemy mech to the list of enemy mechs.
    /// </summary>
    /// <param name="mechController">The mech controller to add.</param>
    void AddEnemyMech(MechController mechController)
    {
        if (!_enemyMechs.Contains(mechController))
        {
            _enemyMechs.Add(mechController);
            Debug.Log($"Enemy Mech added: {mechController.Mech.Name}");
        }
    }

    /// <summary>
    /// Creates and adds test enemy mechs to the battlefield.
    /// </summary>
    void CreateTestEnemyMechs()
    {
        for (int i = 0; i < 3; i++)
        {
            Mech enemyMechData = MechAssembler.AssembleTestEnemyMech();
            bool isValidPosition = false;
            Vector3 spawnPosition = Vector3.zero;
            while (!isValidPosition)
            {
                float x = UnityEngine.Random.Range(TERRAIN_WIDTH / 4f, 3 * TERRAIN_WIDTH / 4f);
                float z = UnityEngine.Random.Range(TERRAIN_LENGTH / 4f, TERRAIN_LENGTH);
                spawnPosition = new Vector3(x, TerrainGenerator.Instance.GetHeightAtPosition(x, z), z);
                isValidPosition = !IsValidPlacementPosition(spawnPosition);
            }
            MechController newEnemyMechController = MechObjectAssembler.Instance.AssembleMechObject(enemyMechData, spawnPosition, Quaternion.identity);
            AddEnemyMech(newEnemyMechController);
        }
    }

    /// <summary>
    /// Spawns the extraction zone at a valid position on the battlefield.
    /// </summary>
    void SpawnExtractionZone()
    {
        do
        {
            float x = UnityEngine.Random.Range(TERRAIN_WIDTH / 4f, 3 * TERRAIN_WIDTH / 4f);
            float z = UnityEngine.Random.Range(TERRAIN_LENGTH / 4f, TERRAIN_LENGTH);
            Vector3 spawnPosition = new Vector3(x, TerrainGenerator.Instance.GetHeightAtPosition(x, z), z);
            if (!IsValidPlacementPosition(spawnPosition))
            {
                ExtractionZone.Instance.SetPosition(spawnPosition);
                Debug.Log("Extraction zone spawned.");
                break;
            }
        } while (true);
    }

    /// <summary>
    /// Retrieves the list of enemy mechs that are currently visible.
    /// </summary>
    /// <returns>A list of visible enemy mech controllers.</returns>
    public List<MechController> GetVisibleEnemyMechs()
    {
        return EnemyLogicManager.Instance.GetVisibleEnemyMechs();
    }
}
