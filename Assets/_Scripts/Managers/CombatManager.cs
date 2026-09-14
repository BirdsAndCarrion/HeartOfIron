using UnityEngine;
using System.Collections.Generic;
using HeartOfIron.Enums;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    [SerializeField] private int PLAYER_MECH_LIMIT = 3;
    [SerializeField] private int TERRAIN_WIDTH = 1000;
    [SerializeField] private int TERRAIN_LENGTH = 1000;

    [SerializeField] private int REWARD_PER_DESTROYED_ENEMY = 100;
    [SerializeField] private int REWARD_FOR_SURVIVAL = 350;
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
        EventRouter.ROnMechSelected.AddListener(SetActiveMech);
        EventRouter.ROnMechDestroyed.AddListener(HandleMechDestroyed);
        UpdateCombatState(CombatState.Loading);
        Camera.main.transform.position = new Vector3(TERRAIN_WIDTH / 2f, 50, 10);
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        EventRouter.OnDeploymentStarted?.Invoke();
    }

    void OnDestroy()
    {
        EventRouter.ROnMechDeclaredActive.RemoveListener(SetActiveMech);
        EventRouter.ROnMechSelected.RemoveListener(SetActiveMech);
        EventRouter.ROnMechDestroyed.RemoveListener(HandleMechDestroyed);
        EventRouter.UnregisterFromHub(this);
    }

    void StartCombat()
    {
        CreateTestEnemyMechs();
        SpawnExtractionZone(); // Assuming there's a method to spawn the extraction zone
        EnemyLogicManager.Instance.AddMechsToGraph(_enemyMechs);
        EventRouter.OnCombatStarted?.Invoke();
        UpdateCombatState(CombatState.Paused);
    }

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

    void Update()
    {
        if (_combatState == CombatState.Playing)
        {
            _combatTimer += Time.deltaTime;
        }
    }

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

    public void UpdateCombatState(CombatState newState)
    {
        _combatState = newState;
        Debug.Log($"Combat state updated to: {_combatState}");
        EventRouter.OnCombatStateChanged?.Invoke(_combatState);
    }

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

    public bool IsValidPlacementPosition(Vector3 position)
    {
        if (position.x < TERRAIN_WIDTH / 4 || position.x > 3 * TERRAIN_WIDTH / 4 || position.z > TERRAIN_LENGTH / 4)
        {
            Debug.LogWarning($"Position {position} is outside the valid deployment area.");
            return false;
        }
        return true;
    }

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

    void AddEnemyMech(MechController mechController)
    {
        if (!_enemyMechs.Contains(mechController))
        {
            _enemyMechs.Add(mechController);
            Debug.Log($"Enemy Mech added: {mechController.Mech.Name}");
        }
    }

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

    public List<MechController> GetVisibleEnemyMechs()
    {
        return EnemyLogicManager.Instance.GetVisibleEnemyMechs();
    }
}
