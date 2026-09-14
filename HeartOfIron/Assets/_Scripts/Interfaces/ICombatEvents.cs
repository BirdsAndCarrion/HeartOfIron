using UnityEngine;
using System;
using HeartOfIron.Enums;

public interface ICombatEvents
{
    event EventHandler<CombatStateChangedEventArgs> OnCombatStateChanged;
    event EventHandler OnDeploymentStarted;
    event EventHandler OnCombatStarted;
    event EventHandler<CombatEndedEventArgs> OnCombatEnded;
}

public class CombatEndedEventArgs : EventArgs
{
    public bool PlayerWon { get; private set; }
    public int RewardValue { get; private set; }

    public CombatEndedEventArgs(bool playerWon, int rewardValue)
    {
        PlayerWon = playerWon;
        RewardValue = rewardValue;
    }
}

public class CombatStateChangedEventArgs : EventArgs
{
    public CombatState NewState { get; private set; }

    public CombatStateChangedEventArgs(CombatState newState)
    {
        NewState = newState;
    }
}