using UnityEngine;
using System;
using HeartOfIron.Enums;

[RequireComponent(typeof(SphereCollider))]
public class OverwatchScript : MonoBehaviour
{
    private void ShootAtEnemy(MechController enemy)
    {
        if (_self != null && enemy != null)
        {
            Debug.Log($"{_self.Mech.Name} is shooting at {enemy.Mech.Name}.");
            // Implement shooting logic here
            if (_self.Mech.Inventory != null)
            {
                Weapon idealWeapon = _self.Mech.Inventory.GetIdealRangedWeapon(Vector3.Distance(_self.transform.position, enemy.transform.position));
                if (idealWeapon != null && idealWeapon.CanAttack(Vector3.Distance(_self.transform.position, enemy.transform.position)))
                {
                    // Implement the attack logic using the ideal weapon
                    Debug.Log($"{_self.Mech.Name} is attacking {enemy.Mech.Name} with {idealWeapon.Name}.");
                    ActionMarker actionMarker = ActionMarkerAssembler.Instance.AssembleActionMarker(_self.transform.position, _self, false);
                    ShootAction shootAction = new ShootAction(_self, enemy, idealWeapon);
                    actionMarker.SetSecondaryAction(shootAction);
                    _self.InsertActionMarkerAt(actionMarker, 0); // Insert the action marker at index 0 to ensure it is executed immediately
                }
                else
                {
                    Debug.LogWarning($"{_self.Mech.Name} has no suitable weapon to attack {enemy.Mech.Name}.");
                }
            }
        }
    }

    private MechController _self;
    private MechController _mechController;
    private SphereCollider _overwatchCollider;

    public bool HasOverwatch => _overwatchCollider != null;
    public bool IsOverwatchActive => _overwatchCollider != null && _overwatchCollider.enabled;

    public void Initialize(MechController self)
    {
        Debug.Log("Initializing OverwatchScript.");
        _self = self;
        _overwatchCollider = GetComponent<SphereCollider>();
        if (_overwatchCollider == null)
        {
            Debug.LogError("SphereCollider component is missing from the OverwatchScript.");
            return;
        } else if (_self == null)
        {
            Debug.LogError("MechController reference is null in OverwatchScript.");
            return;
        } else if (_self.Mech == null)
        {
            Debug.LogError("Mech reference is null in OverwatchScript.");
            return;
        }
        _overwatchCollider.isTrigger = true; // Ensure the collider is set as a trigger
        // Set the radius of the overwatch range (adjust as needed)
        _overwatchCollider.radius = _self.Mech.VisionRange; // Use the mech's vision range for the overwatch radius
        _overwatchCollider.enabled = false; // Initially disable the collider until overwatch is activated
        _mechController = null; // Initialize _mechController to null
        if (_overwatchCollider == null)
        {
            Debug.LogError("SphereCollider component is missing from the OverwatchScript.");
        } else
        {
            Debug.Log($"SphereCollider {_overwatchCollider.name} initialized with radius {_overwatchCollider.radius}.");
        }
    }

    public void ActivateOverwatch()
    {
        // Logic to activate overwatch
        Debug.Log("Overwatch activated.");
        if (_overwatchCollider == null)
        {
            Debug.LogError("SphereCollider component is missing from the OverwatchScript.");
            return;
        }
        _overwatchCollider.enabled = true; // Enable the collider when overwatch is activated
    }

    public void DeactivateOverwatch()
    {
        // Logic to deactivate overwatch
        Debug.Log("Overwatch deactivated.");
        if (_overwatchCollider == null)
        {
            Debug.LogError("SphereCollider component is missing from the OverwatchScript.");
            return;
        }
        _overwatchCollider.enabled = false; // Disable the collider when overwatch is deactivated
    }

    private void OnTriggerEnter(Collider other)
    {
        MechController otherMech = other.GetComponent<MechController>();
        if (otherMech != null && otherMech.Mech.Faction != _self.Mech.Faction && otherMech.MechState != MechState.Dead)
        {
            // Logic to handle enemy mech entering overwatch range
            Debug.Log("Enemy mech entered overwatch range.");
            if (_mechController == null || _mechController.MechState == MechState.Dead)
            {
                _mechController = otherMech;
            }
            ShootAtEnemy(otherMech);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        MechController otherMech = other.GetComponent<MechController>();
        if (otherMech != null && otherMech.Mech.Faction != _self.Mech.Faction && otherMech.MechState != MechState.Dead)
        {
            // Logic to handle enemy mech staying in overwatch range
            Debug.Log("Enemy mech is still in overwatch range.");
            if (_mechController == null || _mechController.MechState == MechState.Dead)
            {
                _mechController = otherMech;
            }
            ShootAtEnemy(otherMech);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MechController otherMech = other.GetComponent<MechController>();
        if (otherMech != null && otherMech.Mech.Faction != _self.Mech.Faction && otherMech.MechState != MechState.Dead)
        {
            // Logic to handle enemy mech exiting overwatch range
            Debug.Log("Enemy mech exited overwatch range.");
            if (_mechController == otherMech)
            {
                _mechController = null; // Reset the tracked mech when it exits the range
            }
        }
    }
}
