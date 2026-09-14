using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    [SerializeField] private List<Mech> _playerMechs = new List<Mech>();
    public List<Mech> PlayerMechs => _playerMechs;
    [SerializeField] private List<Mech> _activeMechs = new List<Mech>();
    public List<Mech> ActiveMechs => _activeMechs;

    [SerializeField] private List<Pilot> _playerPilots = new List<Pilot>();
    public List<Pilot> PlayerPilots => _playerPilots;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (_playerMechs == null)
            {
                _playerMechs = new List<Mech>();
            }
            if (_activeMechs == null)
            {
                _activeMechs = new List<Mech>();
            }
            if (_playerPilots == null)
            {
                _playerPilots = new List<Pilot>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public void TimePasses(int timeUnits)
    {
        RepairMechs(timeUnits);
    }

    public void AddMech(Mech mech)
    {
        if (!_playerMechs.Contains(mech))
        {
            _playerMechs.Add(mech);
        }
        else
        {
            Debug.LogWarning($"Mech {mech.Name} is already in the player's collection.");
        }
    }

    public void AddPilot(Pilot pilot)
    {
        if (!_playerPilots.Contains(pilot))
        {
            _playerPilots.Add(pilot);
        }
        else
        {
            Debug.LogWarning($"Pilot {pilot.Name} is already in the player's collection.");
        }
    }

    public void RemoveMech(Mech mech)
    {
        if (_playerMechs.Contains(mech))
        {
            _playerMechs.Remove(mech);
        }
        else
        {
            Debug.LogWarning($"Mech {mech.Name} is not in the player's collection.");
        }
    }

    public void RemovePilot(Pilot pilot)
    {
        if (_playerPilots.Contains(pilot))
        {
            _playerPilots.Remove(pilot);
        }
        else
        {
            Debug.LogWarning($"Pilot {pilot.Name} is not in the player's collection.");
        }
    }

    public void DeployMech(Mech mech)
    {
        if (!_activeMechs.Contains(mech))
        {
            _activeMechs.Add(mech);
        }
        else
        {
            Debug.LogWarning($"Mech {mech.Name} is already deployed.");
        }
    }

    public void RecallMech(Mech mech)
    {
        if (_activeMechs.Contains(mech))
        {
            mech.GainExperience(100); // Example experience gain for recalling a mech
            _activeMechs.Remove(mech);
        }
        else
        {
            Debug.LogWarning($"Mech {mech.Name} is not currently deployed.");
        }
    }

    public void RepairMechs(float repairTime)
    {
        foreach (Mech mech in _playerMechs)
        {
            if (!_activeMechs.Contains(mech))
            {
                mech.RepairAll(repairTime);
            }
        }
    }
}
