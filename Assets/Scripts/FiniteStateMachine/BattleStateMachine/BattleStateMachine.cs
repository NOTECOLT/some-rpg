using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Class to handle all states in a turn based battle
public class BattleStateMachine : FiniteStateMachine<BattleStateMachine.StateKey> {
    public enum StateKey {
        LOAD_STATE,
        PLAYER_TURN_STATE,
        ACTION_SEQUENCE_STATE,
        END_BATTLE_STATE
    }

    #region GameObject References
    public GameObject enemyObjectParent;
    public GameObject enemyObjectPrefab;
    public TMP_Text mainTextbox;
    public GameObject qteButton;
    public GameObject endBattleScreen;
    public GameObject playerUnitPrefab;
    public GameObject playerSide;
    public UnitInfoMenuUI[] endBattlePlayerInfoPanels;
    #endregion

    /// <summary> List of enemy objects in the battle </summary>
    public List<GameObject> enemyObjectList = new List<GameObject>();
    public ActionType playerSelectedAction;
    public List<BattleUnit> playerBattleUnits = new List<BattleUnit>();
    public UnityEvent<string> OnResetBattleMenuUI;
    public UnityEvent OnEnterActionSequenceState;

    /// <summary>
    /// List of Battle Actions (attacks, heals, etc.) to be executed in order by the battle manager
    /// </summary>
    public List<BattleAction> actionSequence;

    public GameBattleState gameContext;

    protected override void Start() {
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component! Battle failed to load.");
            return;
        }

        States = new Dictionary<StateKey, GenericState<StateKey>>(){
            {StateKey.LOAD_STATE, new BattleLoadState(this, StateKey.LOAD_STATE)},
            {StateKey.PLAYER_TURN_STATE, new BattlePlayerTurnState(this, StateKey.PLAYER_TURN_STATE)},
            {StateKey.ACTION_SEQUENCE_STATE, new BattleActionSequenceState(this, StateKey.ACTION_SEQUENCE_STATE)},
            {StateKey.END_BATTLE_STATE, new BattleEndState(this, StateKey.END_BATTLE_STATE)}
        };

        currentState = States[StateKey.LOAD_STATE];
        base.Start();
    }

    void OnDestroy() {
        OnResetBattleMenuUI.RemoveAllListeners();
        OnEnterActionSequenceState.RemoveAllListeners();
    }

    public void PushBattleAction(BattleAction action) {
        Debug.Log($"[BattleStateMachine] Battle Action Pushed: {action}" );
        actionSequence.Add(action);
    }
    
    public void InsertBattleActionToTop(BattleAction action) {
        Debug.Log($"[BattleStateMachine] Battle Action Inserted to top: {action}");
        actionSequence.Insert(1, action);
    }

    /// <summary>
    /// Triggers when an enemy is clicked on during enemy target selection
    /// </summary>
    /// <param name="targetEnemy">Passed Target Id of the clicked enemy</param>
    public void OnEnemyClicked(Enemy targetEnemy) {
        if (currentState is BattlePlayerTurnState) {
            ((BattlePlayerTurnState)currentState).OnEnemyClicked(targetEnemy);
        }
    }

    /// <summary>
    /// Triggers when a player chooses an action.
    /// </summary>
    /// <param name="action">The Selected action (i.e. ATTACK, HEAL, etc.)</param> <summary>
    public void SetPlayerAction(ActionType action) {
        if (currentState is BattlePlayerTurnState) {
            ((BattlePlayerTurnState)currentState).OnPlayerSetAction(action);
        }
    }

    public void SetPlayerActionNull() {
        playerSelectedAction = ActionType.NULL;
    }

    public void CancelCurrentPlayerAction() {
        if (currentState is not BattlePlayerTurnState) return;
        ((BattlePlayerTurnState)currentState).SetFocusPreviousMember();
    }

    public void EndBattle() {
        gameContext.SavePartyData();
    }
}   