using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic State Machine to be inherited from. Provides functionality for Storing & Automatically Updating States.
/// </summary>
/// <typeparam name="StateKey">StateKey is a nested Enum. One must exist for every state in the machine</typeparam>
public class FiniteStateMachine<StateKey> : MonoBehaviour where StateKey : Enum {
    protected Dictionary<StateKey, GenericState<StateKey>> States = new Dictionary<StateKey, GenericState<StateKey>>();
    protected GenericState<StateKey> currentState;

    protected virtual void Start() {
        currentState.EnterState();
    }

    private void Update() {
        if (currentState.Key.Equals(currentState.GetNextState())) {
            currentState.UpdateState();
        } else {
            ChangeState(currentState.GetNextState());
        }
    }

    /// <summary>
    /// Called in order to change update the BattleState
    /// ? Idea: turn this into coroutine?
    /// </summary>
    private void ChangeState(StateKey newState) {
        Debug.Log($"[{name}] [Exit State: {currentState.ToString().ToUpper()}]");
        currentState.ExitState();
        
        currentState = States[newState];
        Debug.Log($"[{name}] [Enter State: {currentState.ToString().ToUpper()}]");
        currentState.EnterState();
    }
}
