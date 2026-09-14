using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SaveObject
{
    [SerializeField] private float _gameTime;
    public float GameTime => _gameTime;
    [SerializeField] private float _playerBolts;
    public float PlayerBolts => _playerBolts;


    [SerializeField] private SerializableList<Item> _playerInventory;
    public List<Item> PlayerInventory => _playerInventory.Items;
    [SerializeField] private SerializableList<ItemBlueprint> _availableResearch;
    public List<ItemBlueprint> AvailableResearch => _availableResearch.Items;
    
    
    [SerializeField] private SerializableList<Mech> _playerMechs;
    public List<Mech> PlayerMechs => _playerMechs.Items;
    [SerializeField] private SerializableList<Pilot> _playerPilots;
    public List<Pilot> PlayerPilots => _playerPilots.Items;

    public SaveObject(float gameTime, float playerBolts, List<Item> playerInventory, List<ItemBlueprint> availableResearch, List<Mech> playerMechs, List<Pilot> playerPilots)
    {
        _gameTime = gameTime;
        _playerBolts = playerBolts;
        _playerInventory = new SerializableList<Item>(playerInventory);
        _availableResearch = new SerializableList<ItemBlueprint>(availableResearch);
        _playerMechs = new SerializableList<Mech>(playerMechs);
        _playerPilots = new SerializableList<Pilot>(playerPilots);
    }

    public string Serialize()
    {
        if (_playerInventory == null)
        {
            Debug.LogError("Player inventory is null. Cannot serialize.");
        }
        Debug.Log("Player Inventory Count: " + _playerInventory.Items.Count);
        Debug.Log("_playerInventory as JSON: " + JsonUtility.ToJson(_playerInventory));
        if (_playerInventory.Items.Count > 0)
        {
            Debug.Log("_playerInventory[0] as JSON: " + JsonUtility.ToJson(_playerInventory.Items[0]));
        }
        if (_availableResearch == null)
        {
            Debug.LogError("Available research is null. Cannot serialize.");
        }
        Debug.Log("_availableResearch Count: " + _availableResearch.Items.Count);
        Debug.Log("_availableResearch as JSON: " + JsonUtility.ToJson(_availableResearch));
        if (_availableResearch.Items.Count > 0)
        {
            Debug.Log("_availableResearch[0] as JSON: " + JsonUtility.ToJson(_availableResearch.Items[0]));
        }
        if(_playerMechs == null)
        {
            Debug.LogError("Player mechs is null. Cannot serialize.");
        }
        Debug.Log("_playerMechs Count: " + _playerMechs.Items.Count);
        Debug.Log("_playerMechs as JSON: " + JsonUtility.ToJson(_playerMechs));
        if (_playerMechs.Items.Count > 0)
        {
            Debug.Log("_playerMechs[0] as JSON: " + JsonUtility.ToJson(_playerMechs.Items[0]));
        }
        if(_playerPilots == null)
        {
            Debug.LogError("Player pilots is null. Cannot serialize.");
        }
        Debug.Log("_playerPilots Count: " + _playerPilots.Items.Count);
        Debug.Log("_playerPilots as JSON: " + JsonUtility.ToJson(_playerPilots));
        if (_playerPilots.Items.Count > 0)
        {
            Debug.Log("_playerPilots[0] as JSON: " + JsonUtility.ToJson(_playerPilots.Items[0]));
        }
        return JsonUtility.ToJson(this);
    }

    [Serializable]
    public class SerializableList<T>
    {
        [SerializeReference] public List<T> Items;

        public SerializableList(List<T> items)
        {
            Items = items;
        }
    }
}
