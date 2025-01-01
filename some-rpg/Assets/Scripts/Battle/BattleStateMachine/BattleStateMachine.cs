using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class to handle all states in a turn based battle
public class BattleStateMachine : MonoBehaviour {
    // Game Object References -----------------
    public GameObject playerObject;
    public GameObject enemyObjectParent;
    public GameObject enemyObjectPrefab;
    public TMP_Text mainTextbox;
    public GameObject QTEButton;
    // ----------------------------------------

    /// <summary> List of enemy objects in the battle </summary>
    public List<GameObject> enemyObjectList = new List<GameObject>();
    public ActionType playerSelectedAction;
    public BattleUnit playerBattleUnit;

    // Battle States --------------------------
    public BattleLoadState BattleLoadState = new BattleLoadState();
    public BattlePlayerTurnState BattlePlayerTurnState = new BattlePlayerTurnState();
    public BattleActionSequenceState BattleActionSequenceState = new BattleActionSequenceState();
    // ----------------------------------------

    public BattleBaseState CurrentState;

    /// <summary>
    /// List of Battle Actions (attacks, heals, etc.) to be executed in order by the battle manager
    /// </summary>
    public List<BattleAction> ActionSequence;

    void Start() {
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component! Battle failed to load.");
            return;
        }

        CurrentState = BattleLoadState;
        CurrentState.EnterState(this);
    }

    // BUTTON FUNCTIONS ---------------------------------------------
    // At the moment these are only called when certain buttons in the UI are clicked.
    public void SetStateToPlayerSelectAttack() {
        playerSelectedAction = ActionType.ATTACK;
    }

    public void ReturnToOverworldScene() {
        SceneLoader.Instance.LoadOverworld();
    }
    // --------------------------------------------------------------

    // Listener Function to be added to every enemy target.
    /// <summary>
    /// Triggers when an enemy is clicked on during enemy target selection
    /// </summary>
    /// <param name="targetEnemy">Passed Target Id of the clicked enemy</param>
    public void OnEnemyClicked(Enemy targetEnemy) {
        if (CurrentState == BattlePlayerTurnState) {
            AddBattleAction(new BattleAction(targetEnemy, playerSelectedAction, playerBattleUnit));
        }

        ChangeState(BattleActionSequenceState);
    }

    void Update() {
        CurrentState.UpdateState(this);
    }

    /// <summary>
    /// Called in order to change update the BattleState
    /// ? Idea: turn this into coroutine?
    /// </summary>
    public void ChangeState(BattleBaseState newState) {
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    public void AddBattleAction(BattleAction action) {
        Debug.Log($"[BattleStateMachine] Battle Action Added: {action.ActionType} on {action.TargetUnit.Name}" );
        ActionSequence.Add(action);
    }
}
