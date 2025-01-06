using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Class to handle all states in a turn based battle
public class BattleStateMachine : MonoBehaviour {
    // Game Object References -----------------
    public GameObject playerObject;
    public GameObject enemyObjectParent;
    public GameObject enemyObjectPrefab;
    public TMP_Text mainTextbox;
    public GameObject qteButton;
    // ----------------------------------------

    /// <summary> List of enemy objects in the battle </summary>
    public List<GameObject> enemyObjectList = new List<GameObject>();
    public BattleAction playerSelectedAction { get; private set; }
    public BattleUnit playerBattleUnit;

    // Battle States --------------------------
    public BattleLoadState battleLoadState = new BattleLoadState();
    public BattlePlayerTurnState battlePlayerTurnState = new BattlePlayerTurnState();
    public BattleActionSequenceState battleActionSequenceState = new BattleActionSequenceState();
    // ----------------------------------------

    public UnityEvent OnEnterPlayerTurnState;
    public UnityEvent OnEnterActionSequenceState;

    public BattleBaseState currentState;

    /// <summary>
    /// List of Battle Actions (attacks, heals, etc.) to be executed in order by the battle manager
    /// </summary>
    public List<BattleAction> actionSequence;

    void Start() {
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component! Battle failed to load.");
            return;
        }

        currentState = battleLoadState;
        currentState.EnterState(this);
    }

    void OnDestroy() {
        OnEnterPlayerTurnState.RemoveAllListeners();
        OnEnterActionSequenceState.RemoveAllListeners();
    }

    // Listener Function to be added to every enemy target.
    /// <summary>
    /// Triggers when an enemy is clicked on during enemy target selection
    /// </summary>
    /// <param name="targetEnemy">Passed Target Id of the clicked enemy</param>
    public void OnEnemyClicked(Enemy targetEnemy) {
        if (currentState == battlePlayerTurnState && playerSelectedAction != null) {
            AddBattleAction(new BasicAttack(targetEnemy, playerBattleUnit));
        
            ChangeState(battleActionSequenceState);
        }
    }

    void Update() {
        currentState.UpdateState(this);
    }

    /// <summary>
    /// Called in order to change update the BattleState
    /// ? Idea: turn this into coroutine?
    /// </summary>
    public void ChangeState(BattleBaseState newState) {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void AddBattleAction(BattleAction action) {
        Debug.Log($"[BattleStateMachine] Battle Action Added: {action.ActionName} on {action.TargetUnit.Name}" );
        actionSequence.Add(action);
    }

    public void SetPlayerAction(BattleAction action) {
        playerSelectedAction = action;
    }

    public void SetPlayerActionNull() {
        playerSelectedAction = null;
    }
}
