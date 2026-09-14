using UnityEngine;
using System;
using System.Collections.Generic;
using HeartOfIron.Enums;
using HeartOfIron.Structs;
using UnityEngine.AI;

public class EnemyLogicManager : MonoBehaviour
{
    public static EnemyLogicManager Instance { get; private set; }

    private StateTreeManager _stateTreeManager;

    private Graph _enemyGraph;

    private float temperature = 1.0f; // Initial temperature for simulated annealing

    private float coolingRate = 0.99f; // Cooling rate for simulated annealing

    private float minTemperature = 0.01f; // Minimum temperature to stop the annealing process

    private float coolingInterval = 0.1f; // Time interval for cooling the temperature

    private float coolingTimer = 0f; // Timer to track the cooling interval

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _enemyGraph = new Graph();
            _stateTreeManager = new StateTreeManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        EventRouter.ROnMechSpotted.AddListener(HandleMechVisible);
        EventRouter.ROnMechLost.AddListener(HandleMechInvisible);
    }

    void OnDestroy()
    {
        EventRouter.ROnMechSpotted.RemoveListener(HandleMechVisible);
        EventRouter.ROnMechLost.RemoveListener(HandleMechInvisible);
    }

    void Update()
    {
        _enemyGraph.UpdateMechPositions();
        CoolDownTemperature();
        for (int i = 0; i < _enemyGraph.EnemyNodes.Count; i++)
        {
            Node enemyNode = _enemyGraph.EnemyNodes[i];
            if (enemyNode.OccupyingMech != null && enemyNode.OccupyingMech.ActionMarkers.Count < 1) // Limit the number of action markers per enemy mech
            {
                ActionMarker bestActionMarker = EvaluateBestActionForEnemy(enemyNode);
            }
        }
    }

    private void CoolDownTemperature()
    {
        coolingTimer += Time.deltaTime;
        if (coolingTimer >= coolingInterval)
        {
            temperature = Mathf.Max(minTemperature, temperature * coolingRate);
            coolingTimer = 0f;
        }
    }

    public void HandleMechVisible(MechController spottingMech, MechController spottedMech)
    {
        Node spottedNode = _enemyGraph.Nodes.Find(node => node.OccupyingMech == spottedMech);
        if (spottedNode != null)
        {
            Debug.Log($"Mech spotted: {spottedMech.Mech.Name} at position {spottedNode.Position}");
            // If the spotted mech is an enemy, we make it visible to the player by enabling its renderer
            if (spottedNode.OccupyingMech.Mech.Faction == Faction.Enemy)
            {
                Debug.LogWarning($"Spotted mech {spottedMech.Mech.Name} is an enemy. Making it visible to the player.");
                //spottedNode.OccupyingMech.transform.GetComponent<Renderer>().enabled = true;
            }
            // Toggle the edge from spottingMech to spottedMech to visible
            Node spottingNode = _enemyGraph.Nodes.Find(node => node.OccupyingMech == spottingMech);
            if (spottingNode != null)
            {
                Edge edgeToSpotted = spottingNode.Edges.Find(edge => edge.To == spottedNode);
                if (edgeToSpotted != null)
                {
                    edgeToSpotted.IsVisible = true;
                    Debug.Log($"Edge from {spottingMech.Mech.Name} to {spottedMech.Mech.Name} is now visible.");
                }
                else
                {
                    Debug.LogWarning($"No edge found from {spottingMech.Mech.Name} to {spottedMech.Mech.Name}.");
                }
            }
            else
            {
                Debug.LogWarning($"Spotting mech {spottingMech.Mech.Name} not found in the graph.");
            }
        }
        else
        {
            Debug.LogWarning($"Spotted mech {spottedMech.Mech.Name} not found in the graph.");
            AddMechToGraph(spottedMech); // Add the spotted mech to the graph if it's not already present
        }
    }

    public void HandleMechInvisible(MechController spottingMech, MechController spottedMech)
    {
        Node spottedNode = _enemyGraph.Nodes.Find(node => node.OccupyingMech == spottedMech);
        if (spottedNode != null)
        {
            Debug.Log($"Mech no longer visible: {spottedMech.Mech.Name} at position {spottedNode.Position}");
            // If the spotted mech is an enemy, we make it invisible to the player by disabling its renderer
            if (spottedNode.OccupyingMech.Mech.Faction == Faction.Enemy)
            {
                Debug.LogWarning($"Spotted mech {spottedMech.Mech.Name} is an enemy. Making it invisible to the player.");
                //spottedNode.OccupyingMech.transform.GetComponent<Renderer>().enabled = false;
            }
            // Toggle the edge from spottingMech to spottedMech to not visible
            Node spottingNode = _enemyGraph.Nodes.Find(node => node.OccupyingMech == spottingMech);
            if (spottingNode != null)
            {
                Edge edgeToSpotted = spottingNode.Edges.Find(edge => edge.To == spottedNode);
                if (edgeToSpotted != null)
                {
                    edgeToSpotted.IsVisible = false;
                    Debug.Log($"Edge from {spottingMech.Mech.Name} to {spottedMech.Mech.Name} is now not visible.");
                }
                else
                {
                    Debug.LogWarning($"No edge found from {spottingMech.Mech.Name} to {spottedMech.Mech.Name}.");
                }
            }
            else
            {
                Debug.LogWarning($"Spotting mech {spottingMech.Mech.Name} not found in the graph.");
            }
        }
        else
        {
            Debug.LogWarning($"Spotted mech {spottedMech.Mech.Name} not found in the graph.");
        }
    }

    public void AddMechsToGraph(List<MechController> mechControllers)
    {
        foreach (var mechController in mechControllers)
        {
            AddMechToGraph(mechController);
        }
    }

    public void AddMechToGraph(MechController mechController)
    {
        Node newNode = new Node(mechController);
        // TODO: Add logic to assign a role to the mech based on its type or other criteria
        _enemyGraph.AddNode(newNode);

    }

    public void RemoveMechFromGraph(MechController mechController)
    {
        Node nodeToRemove = _enemyGraph.Nodes.Find(node => node.OccupyingMech == mechController);
        if (nodeToRemove != null)
        {
            _enemyGraph.RemoveNode(nodeToRemove);
        }
        else
        {
            Debug.LogWarning($"MechController {mechController.name} not found in the graph.");
        }
    }

    private ActionMarker EvaluateBestActionForEnemy(Node enemyNode)
    {
        Debug.Log($"Evaluating best action for enemy mech: {enemyNode.OccupyingMech.Mech.Name} at position {enemyNode.Position}");
        List<Vector3> possiblePositions = new List<Vector3> { enemyNode.Position }; // Start with the current position
        for (int i = 0; i < 3; i++) // 3 rings spreading outwards
        {
            List<Vector3> compassDirections = new List<Vector3>
            {
                new Vector3(1, 0, 0),   // East
                new Vector3(1, 0, 1),   // Northeast
                new Vector3(1, 0, -1),  // Southeast
                new Vector3(-1, 0, 0),  // West
                new Vector3(-1, 0, 1),  // Northwest
                new Vector3(-1, 0, -1), // Southwest
                new Vector3(0, 0, 1),   // North
                new Vector3(0, 0, -1)   // South
            };
            float distance = (i + 1) * 5f; // Assuming each ring is 5 units apart
            foreach (var direction in compassDirections)
            {
                Vector3 newPosition = enemyNode.Position + direction.normalized * distance;
                newPosition.y = TerrainGenerator.Instance.GetHeightAtPosition(newPosition.x, newPosition.z); // Adjust Y based on terrain height
                if (IsPositionValid(newPosition))
                {
                    possiblePositions.Add(newPosition);
                }
            }
        }
        Vector3 bestPosition = enemyNode.Position; // Default to current position
        // TODO: Recalculate score for the current position based on the new scoring system
        // Make sure that the score it up to date with the information in the graph and the current state of the game
        float bestScore = enemyNode.value; // Start with the current node's value
        while (possiblePositions.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, possiblePositions.Count);
            Vector3 testPosition = possiblePositions[randomIndex];
            possiblePositions.RemoveAt(randomIndex);
            float score = EvaluatePositionScore(testPosition, enemyNode);
            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = testPosition;
                break; // Exit the loop early if a better position is found

            } else if (score <=  bestScore && UnityEngine.Random.value < Mathf.Exp(-Mathf.Abs(score - bestScore) / temperature)) // e^(-abs(score - bestScore) / temperature)
            {
                bestScore = score;
                bestPosition = testPosition;
                break; // Exit the loop early if a better position is found
            } else
            {
                // If the score is not better, continue to the next random position
                continue;
            }
        }
        ActionMarker bestActionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(bestPosition, enemyNode.OccupyingMech);
        EvaluateSecondaryActions(bestActionMarker, enemyNode);
        EnemyState newState = EvaluateCurrentState(enemyNode);
        StateTreeOption stateTreeOption = _stateTreeManager.GetStateTreeOption(newState);
        enemyNode.EnemyLogic.SetEnemyState(newState, _stateTreeManager);
        return bestActionMarker;
    }

    private float EvaluatePositionScore(Vector3 position, Node enemyNode)
    {
        float score = 0f;
        score += NearToEnemyScore(position, enemyNode) * GetNearToEnemyAdjust(enemyNode);
        score += NearToPlayerScore(position, enemyNode) * GetNearToPlayerAdjust(enemyNode);
        score += MaintainDistanceScore(position, enemyNode) * GetMaintainDistanceAdjust(enemyNode);
        score += CoverFromPlayerScore(position, enemyNode) * GetCoverFromPlayerAdjust(enemyNode);
        score += DamageToPlayerScore(position, enemyNode) * GetDamageToPlayerAdjust(enemyNode);
        score += DamageFromPlayerScore(position, enemyNode) * GetDamageFromPlayerAdjust(enemyNode);
        Debug.Log($"Evaluated position {position} for enemy mech {enemyNode.OccupyingMech.Mech.Name} with score: {score}");
        return score;
    }

    private float GetNearToEnemyAdjust(Node enemyNode)
    {
        return enemyNode.EnemyLogic.towardsEnemyModifier; // Use the modifier from the enemy logic
    }

    private float NearToEnemyScore(Vector3 position, Node enemyNode)
    {
        // Higher score for positions closer to the enemy
        float score = 0f;
        foreach (var eNode in _enemyGraph.EnemyNodes)
        {
            if (eNode.OccupyingMech.Mech.Faction == Faction.Enemy)
            {
                float distance = Vector3.Distance(position, eNode.Position);
                if (distance < 10f) // Assuming 10 units is the effective range for scoring
                {
                    score += 10f - distance; // Closer positions get higher scores
                }
            }
        }
        return score;
    }

    private float GetNearToPlayerAdjust(Node enemyNode)
    {
        return enemyNode.EnemyLogic.towardsPlayerModifier; // Use the modifier from the enemy logic
    }

    private float NearToPlayerScore(Vector3 position, Node enemyNode)
    {
        // Higher score for positions closer to the player
        float score = 0f;
        foreach (var playerNode in _enemyGraph.Nodes)
        {
            if (playerNode.OccupyingMech.Mech.Faction == Faction.Player)
            {
                float distance = Vector3.Distance(position, playerNode.Position);
                if (distance < 10f) // Assuming 10 units is the effective range for scoring
                {
                    score += 10f - distance; // Closer positions get higher scores
                }
            }
        }
        return score;
    }

    private float GetMaintainDistanceAdjust(Node enemyNode)
    {
        return enemyNode.EnemyLogic.maintainDistanceModifier; // Use the modifier from the enemy logic
    }

    private float MaintainDistanceScore(Vector3 position, Node enemyNode)
    {
        // Higher score for positions that keep approximately the same distance from the enemies
        return enemyNode.EnemyLogic.maintainDistanceModifier; // Use the modifier from the enemy logic
    }

    private float GetCoverFromPlayerAdjust(Node enemyNode)
    {
        return 1f; // Placeholder for cover from player adjustment logic
    }

    private float CoverFromPlayerScore(Vector3 position, Node enemyNode)
    {
        // Having cover from player mechs increases score
        float score = 0f;
        foreach (Edge edge in enemyNode.Edges)
        {
            if (edge.To.OccupyingMech != null && edge.To.OccupyingMech.Mech.Faction == Faction.Player)
            {
                // Raycast to check if there is cover between the enemy position and the player mech
                Vector3 direction = edge.To.Position - position;
                if (Physics.Raycast(position, direction, out RaycastHit hitInfo, direction.magnitude))
                {
                    if (hitInfo.collider.gameObject != edge.To.OccupyingMech.gameObject)
                    {
                        score += 5f; // Increase score for having cover
                    }
                }
            }
        }
        return score;
    }

    private float GetDamageToPlayerAdjust(Node enemyNode)
    {
        return 1f; // Placeholder for damage to player adjustment logic
    }

    private float DamageToPlayerScore(Vector3 position, Node enemyNode)
    {
        // Higher score for more damage potential to a player mech
        float score = 0f;
        foreach (Edge edge in enemyNode.Edges)
        {
            if (edge.To.OccupyingMech != null && edge.To.OccupyingMech.Mech.Faction == Faction.Player)
            {
                float distance = Vector3.Distance(position, edge.To.Position);
                for(int i = 0; i < enemyNode.OccupyingMech.Mech.Inventory.Weapons.Length; i++)
                {
                    Weapon weapon = enemyNode.OccupyingMech.Mech.Inventory.Weapons[i];
                    float averageDamage = weapon.GetAverageDamage(distance);
                    if (averageDamage > score)
                    {
                        score = averageDamage; // Update score to the highest average damage potential
                    }
                }
            }
        }
        return score;
    }

    private float GetDamageFromPlayerAdjust(Node enemyNode)
    {
        return -1f; // Placeholder for damage from player adjustment logic
    }

    private float DamageFromPlayerScore(Vector3 position, Node enemyNode)
    {
        float score = 0f;
        foreach (Edge edge in enemyNode.Edges)
        {
            if (edge.To.OccupyingMech != null && edge.To.OccupyingMech.Mech.Faction == Faction.Player)
            {
                float distance = Vector3.Distance(position, edge.To.Position);
                for(int i = 0; i < edge.To.OccupyingMech.Mech.Inventory.Weapons.Length; i++)
                {
                    Weapon weapon = edge.To.OccupyingMech.Mech.Inventory.Weapons[i];
                    float averageDamage = weapon.GetAverageDamage(distance);
                    score += averageDamage; // Accumulate the average damage potential from all player weapons
                }
            }
        }
        return score;
    }

    private bool IsPositionValid(Vector3 position)
    {
        // Must be within terrain bounds
        if (position.x < 0 || position.x > TerrainGenerator.Instance.terrainSize || position.z < 0 || position.z > TerrainGenerator.Instance.terrainSize)
        {
            return false;
        }
        // Must be on the navmesh
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            return false;
        }
        // Must not be occupied by another mech
        foreach (var node in _enemyGraph.Nodes)
        {
            if (Vector3.Distance(position, node.Position) < 1.0f) // Assuming 1 unit is the minimum distance to consider a position occupied
            {
                return false;
            }
        }
        // Must not be in an obstacle
        // TODO: Implement a method to check for obstacles in the game world. This could involve checking for colliders, terrain features, or other game-specific logic.
        if (Physics.CheckSphere(position, 0.5f, LayerMask.GetMask("Obstacle"))) // Assuming obstacles are on the "Obstacle" layer
        {
            return false;
        }
        return true;
    }

    private void EvaluateSecondaryActions(ActionMarker actionMarker, Node enemyNode)
    {
        // Check if mech is currently dead
        if (enemyNode.OccupyingMech.Mech.IsDead())
        {
            return; // No further actions if the mech is dead
        }
        // Check if mech is currently brawling
        if (enemyNode.OccupyingMech.MechState == MechState.Brawling)
        {
            return; // No further actions if the mech is brawling
        }
        // Get secondary action from InternalEnemyLogic and assign it to the actionMarker
        if (enemyNode.EnemyLogic != null && enemyNode.EnemyLogic.enemyState != EnemyState.Wander)
        {
            Action secondaryAction = _stateTreeManager.GetStateTreeOption(enemyNode.EnemyLogic.enemyState).secondaryActionFunction(enemyNode, _enemyGraph);
            actionMarker.SetSecondaryAction(secondaryAction);
        }
        
    }

    private EnemyState EvaluateCurrentState(Node enemyNode)
    {
        return _stateTreeManager.GetStateTreeOption(enemyNode.EnemyLogic.enemyState).nextStateFunction(enemyNode, _enemyGraph);
    }

    private class Graph
    {
        public List<Node> Nodes { get; private set; }
        public List<Node> EnemyNodes { get; private set; } // Track enemy nodes separately

        public Graph()
        {
            Nodes = new List<Node>();
            EnemyNodes = new List<Node>();
        }

        public void AddNode(Node node)
        {
            Nodes.Add(node);
            if (node.OccupyingMech != null && node.OccupyingMech.Mech.Faction == Faction.Enemy)
            {
                EnemyNodes.Add(node);
            }
            for (int i = 0; i < Nodes.Count - 1; i++)
            {
                Node existingNode = Nodes[i];
                float cost = Vector3.Distance(existingNode.Position, node.Position);
                AddEdge(existingNode, node, cost);
                AddEdge(node, existingNode, cost); // Assuming undirected graph
            }
        }

        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
            EnemyNodes.Remove(node); // Also remove from EnemyNodes if it's an enemy
            foreach (var n in Nodes)
            {
                n.Edges.RemoveAll(edge => edge.To == node);
            }
        }

        public void AddEdge(Node from, Node to, float cost)
        {
            Edge edge = new Edge(from, to, cost);
            from.Edges.Add(edge);
        }

        public void RemoveEdge(Node from, Node to)
        {
            from.Edges.RemoveAll(edge => edge.To == to);
        }

        public void UpdateMechPositions()
        {
            foreach (Node node in EnemyNodes)
            {
                if (node.OccupyingMech != null)
                {
                    node.Position = node.OccupyingMech.transform.position;
                    foreach (Edge edge in node.Edges)
                    {
                        edge.Cost = Vector3.Distance(edge.From.Position, edge.To.Position);
                        if (edge.To.OccupyingMech != null && edge.To.OccupyingMech.Mech.Faction == Faction.Player && edge.IsVisible)
                        {
                            edge.To.Position = edge.To.OccupyingMech.transform.position;
                        }
                    }
                }
            }
        }

        public Node FindNearestPlayerNode(Vector3 position)
        {
            Node nearestNode = null;
            float nearestDistance = float.MaxValue;
            foreach (var node in Nodes)
            {
                if (node.OccupyingMech.Mech.Faction == Faction.Player)
                {
                    float distance = Vector3.Distance(position, node.Position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestNode = node;
                    }
                }
            }
            return nearestNode;
        }
    }

    private class Edge
    {
        public Node From { get; private set; }
        public Node To { get; private set; }
        public bool IsVisible { get; set; } // Track visibility of the edge
        public float Cost { get; set; }

        public Edge(Node from, Node to, float cost)
        {
            From = from;
            To = to;
            Cost = cost;
            IsVisible = true; // Default to visible
        }
    }

    private class Node
    {
        public Vector3 Position { get; set; }
        public float value { get; set; }
        public List<Edge> Edges { get; private set; }
        public MechController OccupyingMech { get; set; } // Track which mech is occupying this node
        public EnemyRole role;
        public InternalEnemyLogic EnemyLogic { get; set; } // Track the enemy logic associated with this node

        public Node(MechController occupyingMech)
        {
            Position = occupyingMech.transform.position;
            Edges = new List<Edge>();
            OccupyingMech = occupyingMech;
            value = 0f; // Initialize value to 0
            EnemyLogic = new InternalEnemyLogic(EnemyState.Wander); // Default to Wander state
        }

        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
        }

        public List<Edge> GetPlayerEdges()
        {
            List<Edge> playerEdges = new List<Edge>();
            foreach (Edge edge in Edges)
            {
                if (edge.To.OccupyingMech != null && edge.To.OccupyingMech.Mech.Faction == Faction.Player)
                {
                    playerEdges.Add(edge);
                }
            }
            return playerEdges;
        }

        public List<Edge> CheckForFlag(EnemyState flag)
        {
            List<Edge> flaggedEdges = new List<Edge>();
            foreach (Edge edge in Edges)
            {
                if (edge.To.EnemyLogic.enemyState == flag && edge.To.OccupyingMech != null && edge.To.OccupyingMech.Mech.Faction != Faction.Player)
                {
                    flaggedEdges.Add(edge);
                }
            }
            return flaggedEdges;
        }
    }

    private class InternalEnemyLogic
    {
        private Func<Node, Graph, Action> getSecondaryAction; // Function to get secondary action based on node and graph

        public EnemyState enemyState;
        public int counter;

        public float towardsEnemyModifier;

        public float towardsPlayerModifier;

        public float maintainDistanceModifier;

        public float previousDistance;

        public InternalEnemyLogic(EnemyState enemyState, float tEM = 1, float tPM = 1, float mDM = 0, float pD = 1)
        {
            this.enemyState = enemyState;
            this.towardsEnemyModifier = tEM;
            this.towardsPlayerModifier = tPM;
            this.maintainDistanceModifier = mDM;
            this.previousDistance = pD;
            counter = 0;
        }

        public void SetEnemyState(EnemyState newState, StateTreeManager _stateTreeManager)
        {
            if (newState == enemyState)
            {
                counter++;
            } else
            {
                counter = 0;
            }
            enemyState = newState;
            StateTreeOption newOption = _stateTreeManager.GetStateTreeOption(newState);
            this.towardsEnemyModifier = newOption.towardsEnemyModifier;
            this.towardsPlayerModifier = newOption.towardsPlayerModifier;
            this.maintainDistanceModifier = newOption.maintainDistanceModifier;
            this.getSecondaryAction = newOption.secondaryActionFunction;
        }
    }

    private class StateTreeManager
    {

        private static bool IsMeleeRole(EnemyRole role)
        {
            return role == EnemyRole.Brawler || role == EnemyRole.Flanker;
        }

        private static bool IsLowHealth(Node node)
        {
            return node.OccupyingMech.IsLowHealth();
        }

        private static bool IsIsolated(Node node, Graph graph)
        {
            float isolationThreshold = 30f; // Define a threshold distance to consider a node isolated
            int nearbyCount = 0;
            foreach (Edge edge in node.Edges)
            {
                if (node.OccupyingMech.Mech != null && edge.To.OccupyingMech.Mech.Faction == node.OccupyingMech.Mech.Faction)
                {
                    float distance = Vector3.Distance(node.Position, edge.To.Position);
                    if (distance <= isolationThreshold)
                    {
                        nearbyCount++;
                    }
                }
            }
            if (nearbyCount <= 1) // Consider isolated if 1 or fewer nearby allies
            {
                return true;
            }
            return false;
        }
        
        private static bool IsClustered(Node node, Graph graph)
        {
            float clusterThreshold = 30f; // Define a threshold distance to consider a node clustered
            int nearbyCount = 0;
            foreach (Edge edge in node.Edges)
            {
                if (node.OccupyingMech.Mech != null && edge.To.OccupyingMech.Mech.Faction == node.OccupyingMech.Mech.Faction)
                {
                    float distance = Vector3.Distance(node.Position, edge.To.Position);
                    if (distance <= clusterThreshold)
                    {
                        nearbyCount++;
                    }
                }
            }
            if (nearbyCount >= 2) // Consider clustered if 2 or more nearby allies
            {
                return true;
            }
            return false;
        }

        private static bool DetectPriorityTarget(Node node, Graph graph)
        {
            return false; // Placeholder for priority target detection logic
        }

        public List<StateTreeOption> stateTreeOptions;

        static Func<Node, Graph, Action> WanderFunction = (node, graph) =>
        {
            return null; // Return null for now, as Wander may not have a secondary action
        };

        static Func<Node, Graph, EnemyState> WanderNextStateFunction = (node, graph) =>
        {
            if (node.GetPlayerEdges().Count > 0)
            {
                foreach (Edge edge in node.GetPlayerEdges())
                {
                    if (edge.IsVisible)
                    {
                        if (node.role == EnemyRole.Flanker)
                        {
                            return EnemyState.Prowl;
                        } else if (node.role == EnemyRole.Brawler)
                        {
                            return EnemyState.Rush;
                        } else
                        {
                            return EnemyState.StaticShoot;
                        }
                    }
                }
                return EnemyState.Search; // Transition to Search state if player is detected
            } else if (node.CheckForFlag(EnemyState.Search).Count > 0)
            {
                return EnemyState.Group; // Transition to Group state if any nearby nodes are in Search state   
            }
            float randomValue = UnityEngine.Random.value;
            if (randomValue < 0.25f && !IsMeleeRole(node.role))
            {
                return EnemyState.Overwatch; // 25% chance to transition to Overwatch state
            }
            else if (randomValue < 0.5f)
            {
                return EnemyState.Group;
            } else
            {
                return EnemyState.Wander; // 50% chance to stay in Wander state
            }
        };

        static Func<Node, Graph, Action> SearchFunction = (node, graph) =>
        {
            return null; // Return null for now, as Search may not have a secondary action
        };

        static Func<Node, Graph, EnemyState> SearchNextStateFunction = (node, graph) =>
        {
            if (node.GetPlayerEdges().Count > 0)
            {
                foreach(Edge edge in node.GetPlayerEdges())
                {
                    if (edge.IsVisible)
                    {
                        if (node.role == EnemyRole.Flanker)
                        {
                            return EnemyState.Prowl;
                        } else if (node.role == EnemyRole.Brawler)
                        {
                            return EnemyState.Rush;
                        } else
                        {
                            return EnemyState.StaticShoot;
                        }
                    }
                }
                // If we reach this point, no player mechs are visible, but we know at least one exists
                if (node.GetPlayerEdges().Count > 0)
                {
                    if (UnityEngine.Random.value < 1 - (node.EnemyLogic.counter * 0.1))
                    {
                        if (UnityEngine.Random.value < 0.75)
                        {
                            return EnemyState.Wander;
                        } else
                        {
                            return EnemyState.Group;
                        }
                    } else
                    {
                        return EnemyState.Search;
                    }
                }
            }
            if (UnityEngine.Random.value < 0.75)
            {
                return EnemyState.Wander;
            } else
            {
                return EnemyState.Group;
            }
        };

        static Func<Node, Graph, Action> OverwatchFunction = (node, graph) =>
        {
            return new OverwatchAction(node.OccupyingMech); 
        };

        static Func<Node, Graph, EnemyState> OverwatchNextStateFunction = (node, graph) =>
        {
            if (node.GetPlayerEdges().Count > 0 && node.GetPlayerEdges().Exists(edge => edge.IsVisible)) // If any player mechs are visible
            {
                if (!IsMeleeRole(node.role))
                {
                    return EnemyState.StaticShoot;
                } else if (node.role == EnemyRole.Flanker)
                {
                    return EnemyState.Prowl;
                } else if (node.role == EnemyRole.Brawler)
                {
                    return EnemyState.Rush;
                }
            } 
            if (node.GetPlayerEdges().Count > 0) // If player mechs are detected but not visible
            {
                if (UnityEngine.Random.value < 0.8) // 80% chance to transition to Search state
                {
                    return EnemyState.Search; // 50% chance to transition to Search state
                } else
                {
                    return EnemyState.Wander;
                }
            }
            if (UnityEngine.Random.value < 1 - (node.EnemyLogic.counter * 0.1)) // Decrease chance to stay in Overwatch as counter increases
            {
                return EnemyState.Wander;
            } else
            {
                return EnemyState.Overwatch; // 50% chance to stay in Overwatch state
            }
        };

        static Func<Node, Graph, Action> GroupFunction = (node, graph) =>
        {
            return null;
        };

        static Func<Node, Graph, EnemyState> GroupNextStateFunction = (node, graph) =>
        {
            if (node.GetPlayerEdges().Count > 0 && node.GetPlayerEdges().Exists(edge => edge.IsVisible)) // If any player mechs are visible
            {
                if (!IsMeleeRole(node.role))
                {
                    return EnemyState.StaticShoot;
                } else if (node.role == EnemyRole.Flanker)
                {
                    return EnemyState.Prowl;
                } else if (node.role == EnemyRole.Brawler)
                {
                    return EnemyState.Rush;
                }
            } 
            if (node.GetPlayerEdges().Count > 0) // If player mechs are detected but not visible
            {
                if (UnityEngine.Random.value < 0.8) // 80% chance to transition to Search state
                {
                    return EnemyState.Search; // 50% chance to transition to Search state
                } else
                {
                    return EnemyState.Wander;
                }
            }
            if (UnityEngine.Random.value < 1 - (node.EnemyLogic.counter * 0.1)) // Decrease chance to stay in Group as counter increases
            {
                return EnemyState.Group;
            } else if (!IsMeleeRole(node.role) && UnityEngine.Random.value < 0.5) // 50% chance to transition to Overwatch if not a melee role
            {
                return EnemyState.Overwatch;
            } else
            {
                return EnemyState.Wander; // 50% chance to transition to Wander state
            }
        };

        static Func<Node, Graph, Action> HunkerFunction = (node, graph) =>
        {
            // Placeholder for Hunker secondary action logic
            return new HunkerAction(node.OccupyingMech); // Return a default action for now
        };

        static Func<Node, Graph, Action> HoldPosFunction = (node, graph) =>
        {
            return new OverwatchAction(node.OccupyingMech);
        };

        Func<Node, Graph, Action> RetreatFunction = (node, graph) =>
        {
            // Target nearest targetable player mech
            Node nearestPlayerNode = graph.FindNearestPlayerNode(node.Position);
            Weapon bestWeapon = node.OccupyingMech.Mech.Inventory.GetIdealRangedWeapon(Vector3.Distance(node.Position, nearestPlayerNode.Position));
            if (bestWeapon == null)
            {
                return null; // No weapon to use, so no action
            } else if (bestWeapon.CanAttack(Vector3.Distance(node.Position, nearestPlayerNode.Position)))
            {
                return new ShootAction(node.OccupyingMech, nearestPlayerNode.OccupyingMech, bestWeapon);
            } else
            {
                return new ReloadAction(node.OccupyingMech, bestWeapon);
            }
        };

        Func<Node, Graph, Action> StaticShootFunction = (node, graph) =>
        {
            Node nearestPlayerNode = graph.FindNearestPlayerNode(node.Position);
            Weapon bestWeapon = node.OccupyingMech.Mech.Inventory.GetIdealRangedWeapon(Vector3.Distance(node.Position, nearestPlayerNode.Position));
            if (bestWeapon == null)
            {
                return null; // No weapon to use, so no action
            } else if (bestWeapon.CanAttack(Vector3.Distance(node.Position, nearestPlayerNode.Position)))
            {
                return new ShootAction(node.OccupyingMech, nearestPlayerNode.OccupyingMech, bestWeapon);
            } else
            {
                return new ReloadAction(node.OccupyingMech, bestWeapon);
            }
        };

        Func<Node, Graph, EnemyState> StaticShootNextStateFunction = (node, graph) =>
        {
            // If enemy detects a priority target, transition to TargetShoot state
            if (DetectPriorityTarget(node, graph))
            {
                return EnemyState.TargetShoot;
            }
            // If enemy detects a flag for TeamShoot, transition to TeamShoot state
            if (node.CheckForFlag(EnemyState.TeamShoot).Count > 0)
            {
                return EnemyState.TeamShoot;
            }
            // If enemy detects a player mech in range, return to StaticShoot state
            if (node.GetPlayerEdges().Count > 0 && node.GetPlayerEdges().Exists(edge => edge.IsVisible))
            {
                return EnemyState.StaticShoot;
            }
            // If enemy detects a player mech, but cannot see it, transition to Search state
            if (node.GetPlayerEdges().Count > 0)
            {
                return EnemyState.Search;
            }
            else
            {
                return EnemyState.Wander; // If no player mechs are detected, transition to Wander state
            }
        };

        Func<Node, Graph, Action> TargetShootFunction = (node, graph) =>
        {
            Node nearestPlayerNode = graph.FindNearestPlayerNode(node.Position);
            Weapon bestWeapon = node.OccupyingMech.Mech.Inventory.GetIdealRangedWeapon(Vector3.Distance(node.Position, nearestPlayerNode.Position));
            if (bestWeapon == null)
            {
                return null; // No weapon to use, so no action
            } else if (bestWeapon.CanAttack(Vector3.Distance(node.Position, nearestPlayerNode.Position)))
            {
                return new ShootAction(node.OccupyingMech, nearestPlayerNode.OccupyingMech, bestWeapon);
            } else
            {
                return new ReloadAction(node.OccupyingMech, bestWeapon);
            }
        };

        Func<Node, Graph, EnemyState> TargetShootNextStateFunction = (node, graph) =>
        {
            // If enemy detects a priority target, transition to TargetShoot state
            if (DetectPriorityTarget(node, graph))
            {
                return EnemyState.TargetShoot;
            }
            // If enemy detects a flag for TeamShoot, transition to TeamShoot state
            if (node.CheckForFlag(EnemyState.TeamShoot).Count > 0)
            {
                return EnemyState.TeamShoot;
            }
            // If enemy detects a player mech in range, return to StaticShoot state
            if (node.GetPlayerEdges().Count > 0 && node.GetPlayerEdges().Exists(edge => edge.IsVisible))
            {
                return EnemyState.StaticShoot;
            }
            // If enemy detects a player mech, but cannot see it, transition to Search state
            if (node.GetPlayerEdges().Count > 0)
            {
                return EnemyState.Search;
            }
            else
            {
                return EnemyState.Wander; // If no player mechs are detected, transition to Wander state
            }
        };

        Func<Node, Graph, Action> TeamShootFunction = (node, graph) =>
        {
            Node nearestPlayerNode = graph.FindNearestPlayerNode(node.Position);
            Weapon bestWeapon = node.OccupyingMech.Mech.Inventory.GetIdealRangedWeapon(Vector3.Distance(node.Position, nearestPlayerNode.Position));
            if (bestWeapon == null)
            {
                return null; // No weapon to use, so no action
            } else if (bestWeapon.CanAttack(Vector3.Distance(node.Position, nearestPlayerNode.Position)))
            {
                return new ShootAction(node.OccupyingMech, nearestPlayerNode.OccupyingMech, bestWeapon);
            } else
            {
                return new ReloadAction(node.OccupyingMech, bestWeapon);
            }
        };

        Func<Node, Graph, EnemyState> TeamShootNextStateFunction = (node, graph) =>
        {
            // If enemy detects a flag for TeamShoot, transition to TeamShoot state
            if (node.CheckForFlag(EnemyState.TeamShoot).Count > 0)
            {
                return EnemyState.TeamShoot;
            }
            // If enemy detects a player mech in range, return to StaticShoot state
            if (node.GetPlayerEdges().Count > 0 && node.GetPlayerEdges().Exists(edge => edge.IsVisible) || UnityEngine.Random.value < 1 - (node.EnemyLogic.counter * 0.1)) // Decrease chance to stay in TeamShoot as counter increases
            {
                return EnemyState.StaticShoot;
            }
            // If enemy detects a player mech, but cannot see it, transition to Search state
            if (node.GetPlayerEdges().Count > 0)
            {
                return EnemyState.Search;
            }
            else
            {
                return EnemyState.Wander; // If no player mechs are detected, transition to Wander state
            }
        };

        Func<Node, Graph, Action> CommitFunction = (node, graph) =>
        {
            return null;
        };

        Func<Node, Graph, EnemyState> CommitNextStateFunction = (node, graph) =>
        {
            if (node.OccupyingMech.MechState == MechState.Charging || node.OccupyingMech.MechState == MechState.Brawling)
            {
                return EnemyState.Commit; 
            }
            if (node.role == EnemyRole.Brawler)
            {
                return EnemyState.Rush;
            }
            else 
            {
                return EnemyState.Prowl;
            } 
        };

        Func<Node, Graph, Action> RushFunction = (node, graph) =>
        {
            Weapon bestWeapon = node.OccupyingMech.Mech.Inventory.GetIdealMeleeWeapon();
            if (Vector3.Distance(node.Position, graph.FindNearestPlayerNode(node.Position).Position) < 100f && bestWeapon != null)
            {
                return new ChargeAction(node.OccupyingMech, graph.FindNearestPlayerNode(node.Position).OccupyingMech, bestWeapon);
            }
            return null;
        };

        Func<Node, Graph, EnemyState> RushNextStateFunction = (node, graph) =>
        {
            if (node.OccupyingMech.MechState == MechState.Charging || node.OccupyingMech.MechState == MechState.Brawling)
            {
                return EnemyState.Commit; 
            }
            if (node.role == EnemyRole.Brawler)
            {
                return EnemyState.Rush;
            }
            else 
            {
                return EnemyState.Prowl;
            } 
        };

        Func<Node, Graph, Action> HitAndRunFunction = (node, graph) =>
        {
            Weapon bestWeapon = node.OccupyingMech.Mech.Inventory.GetIdealMeleeWeapon();
            if (Vector3.Distance(node.Position, graph.FindNearestPlayerNode(node.Position).Position) < 100f && bestWeapon != null)
            {
                return new ChargeAction(node.OccupyingMech, graph.FindNearestPlayerNode(node.Position).OccupyingMech, bestWeapon);
            }
            return null;
        };

        Func<Node, Graph, EnemyState> HitAndRunNextStateFunction = (node, graph) =>
        {
            if (node.OccupyingMech.MechState == MechState.Charging || node.OccupyingMech.MechState == MechState.Brawling)
            {
                return EnemyState.Commit; 
            }
            if (node.role == EnemyRole.Brawler)
            {
                return EnemyState.Rush;
            }
            else 
            {
                return EnemyState.Prowl;
            } 
        };

        Func<Node, Graph, Action> ProwlFunction = (node, graph) =>
        {
            return null;
        };

        Func<Node, Graph, EnemyState> ProwlNextStateFunction = (node, graph) =>
        {
            if (node.OccupyingMech.MechState == MechState.Charging || node.OccupyingMech.MechState == MechState.Brawling)
            {
                return EnemyState.Commit; 
            }
            if (node.role == EnemyRole.Brawler)
            {
                return EnemyState.Rush;
            }
            else 
            {
                if (UnityEngine.Random.value < 0.5)
                {
                    return EnemyState.HitAndRun; // 50% chance to transition to HitAndRun state
                }
                return EnemyState.Prowl;
            } 
        };

        Func<Node, Graph, Action> PeelFunction = (node, graph) =>
        {
            Weapon bestWeapon = node.OccupyingMech.Mech.Inventory.GetIdealMeleeWeapon();
            if (Vector3.Distance(node.Position, graph.FindNearestPlayerNode(node.Position).Position) < 100f && bestWeapon != null)
            {
                return new ChargeAction(node.OccupyingMech, graph.FindNearestPlayerNode(node.Position).OccupyingMech, bestWeapon);
            }
            return null;
        };

        Func<Node, Graph, EnemyState> PeelNextStateFunction = (node, graph) =>
        {
            if (node.OccupyingMech.MechState == MechState.Charging || node.OccupyingMech.MechState == MechState.Brawling)
            {
                return EnemyState.Commit; 
            }
            else
            {
                return EnemyState.Rush; // If not charging or brawling, transition to Rush state
            }
        };

        public StateTreeManager()
        {
            stateTreeOptions = new List<StateTreeOption>();
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Wander, WanderFunction, WanderNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Search, SearchFunction, SearchNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Overwatch, OverwatchFunction, OverwatchNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Group, GroupFunction, GroupNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Hunker, HunkerFunction, null));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.HoldPos, HoldPosFunction, null));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Retreat, RetreatFunction, null));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.StaticShoot, StaticShootFunction, StaticShootNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.TargetShoot, TargetShootFunction, TargetShootNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.TeamShoot, TeamShootFunction, TeamShootNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Commit, CommitFunction, CommitNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Rush, RushFunction, RushNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.HitAndRun, HitAndRunFunction, HitAndRunNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Prowl, ProwlFunction, ProwlNextStateFunction));
            stateTreeOptions.Add(new StateTreeOption(EnemyState.Peel, PeelFunction, PeelNextStateFunction));
        }

        public StateTreeOption GetStateTreeOption(EnemyState state)
        {
            foreach (var option in stateTreeOptions)
            {
                if (option.state == state)
                {
                    return option;
                }
            }
            return null;
        }
    }

    private class StateTreeOption
    {
        public EnemyState state;
        public float towardsEnemyModifier;

        public float towardsPlayerModifier;

        public float maintainDistanceModifier;
        public Func<Node, Graph, Action> secondaryActionFunction;
        public Func<Node, Graph, EnemyState> nextStateFunction;

        public StateTreeOption(EnemyState state, Func<Node, Graph, Action> secondaryActionFunction, Func<Node, Graph, EnemyState> nextStateFunction)
        {
            this.state = state;
            this.secondaryActionFunction = secondaryActionFunction;
            this.nextStateFunction = nextStateFunction;
        }
    }
}
