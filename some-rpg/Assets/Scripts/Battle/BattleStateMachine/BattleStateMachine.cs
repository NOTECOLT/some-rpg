using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class to handle all states in a turn based battle
public class BattleStateMachine : MonoBehaviour {
    public BattleLoadState BattleLoadState = new BattleLoadState();
    public BattlePlayerTurnState BattlePlayerTurnState = new BattlePlayerTurnState();
    public BattleActionSequenceState BattleActionSequenceState = new BattleActionSequenceState();

    public BattleBaseState CurrentState;

    /// <summary>
    /// List of Battle Actions (attacks, heals, etc.) to be executed in order by the battle manager
    /// </summary>
    public List<BattleAction> ActionSequence;

    public TMP_Text mainTextbox;
    public GameObject QTEButton;

    void Start() {
        QTEButton.SetActive(false);

        CurrentState = BattleLoadState;
        CurrentState.EnterState(this);
    }

    void Update() {
        CurrentState.UpdateState(this);
    }

    /// <summary>
    /// Called in order to change update the BattleState
    /// ? Idea: turn this into coroutine?
    /// </summary>
    public void ChangeState(BattleBaseState newState) {
        CurrentState.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    public void AddBattleAction(BattleAction action) {
        Debug.Log($"[BattleStateMachine] Battle Action Added: {action.ActionType} on {action.tempRef.EnemyType.EnemyName}" );
        ActionSequence.Add(action);
    }
}
