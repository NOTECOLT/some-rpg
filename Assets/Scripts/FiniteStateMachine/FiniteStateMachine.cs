using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine<StateKey> : MonoBehaviour where StateKey : Enum {
    public Dictionary<StateKey, GenericState> States = new Dictionary<StateKey, GenericState>();
    public GenericState currentState;

    protected virtual void Start() {
        currentState.EnterState();
    }

    protected virtual void Update() {
        currentState.UpdateState();
    }

    /// <summary>
    /// Called in order to change update the BattleState
    /// ? Idea: turn this into coroutine?
    /// </summary>
    public void ChangeState(GenericState newState) {
        currentState = newState;
        currentState.EnterState();
    }
}
