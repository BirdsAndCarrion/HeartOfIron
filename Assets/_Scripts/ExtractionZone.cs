using UnityEngine;
using System.Collections.Generic;
using HeartOfIron.Enums;

public class ExtractionZone : MonoBehaviour
{
    public static ExtractionZone Instance { get; private set; }

    private bool activated = false;

    private List<MechController> _mechsInZone = new List<MechController>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        activated = false;
        EventRouter.ROnCombatStarted.AddListener(EnableExtractionZone);
    }

    void OnDestroy()
    {
        EventRouter.ROnCombatStarted.RemoveListener(EnableExtractionZone);
    }

    void Update()
    {
        if (!activated)
        {
            return;
        }
        if (_mechsInZone.Count != CombatManager.Instance.PlayerMechs.Count)
        {
            return;
        }
        bool allMechsInZone = true;
        foreach (MechController mech in CombatManager.Instance.PlayerMechs)
        {
            if (mech != null && !_mechsInZone.Contains(mech))
            {
                allMechsInZone = false;
                break;
            }
        }
        if (allMechsInZone)
        {
            Debug.Log("All mechs are in the extraction zone.");
            CombatManager.Instance.EndCombat(EndCondition.Survived);
        }
    }

    void EnableExtractionZone()
    {
        activated = true;
    }

    void DisableExtractionZone()
    {
        activated = false;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    void OnTriggerEnter(Collider other)
    {
        MechController mech = other.GetComponent<MechController>();
        if (mech != null && !_mechsInZone.Contains(mech) && mech.Mech != null && mech.MechState != MechState.Dead && mech.Mech.Faction == Faction.Player)
        {
            _mechsInZone.Add(mech);
            Debug.Log($"Mech entered extraction zone: {mech.Mech.Name}");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        MechController mech = other.GetComponent<MechController>();
        if (mech != null && _mechsInZone.Contains(mech))
        {
            _mechsInZone.Remove(mech);
            Debug.Log($"Mech exited extraction zone: {mech.Mech.Name}");
        }
    }
}
