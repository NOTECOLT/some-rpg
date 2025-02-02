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
        ACTION_SEQUENCE_STATE
    }

    #region GameObject References
    public GameObject playerObject;
    public GameObject enemyObjectParent;
    public GameObject enemyObjectPrefab;
    public TMP_Text mainTextbox;
    public GameObject qteButton;
    #endregion

    /// <summary> List of enemy objects in the battle </summary>
    public List<GameObject> enemyObjectList = new List<GameObject>();
    public ActionType playerSelectedAction;
    public BattleUnit playerBattleUnit;
    public UnityEvent OnEnterPlayerTurnState;
    public UnityEvent OnEnterActionSequenceState;

    /// <summary>
    /// List of Battle Actions (attacks, heals, etc.) to be executed in order by the battle manager
    /// </summary>
    public List<BattleAction> actionSequence;

    protected override void Start() {
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component! Battle failed to load.");
            return;
        }

        States = new Dictionary<StateKey, GenericState<StateKey>>(){
            {StateKey.LOAD_STATE, new BattleLoadState(this, StateKey.LOAD_STATE)},
            {StateKey.PLAYER_TURN_STATE, new BattlePlayerTurnState(this, StateKey.PLAYER_TURN_STATE)},
            {StateKey.ACTION_SEQUENCE_STATE, new BattleActionSequenceState(this, StateKey.ACTION_SEQUENCE_STATE)}
        };

        currentState = States[StateKey.LOAD_STATE];
        base.Start();
    }

    void OnDestroy() {
        OnEnterPlayerTurnState.RemoveAllListeners();
        OnEnterActionSequenceState.RemoveAllListeners();
    }

    public void AddBattleAction(BattleAction action) {
        Debug.Log($"[BattleStateMachine] Battle Action Added: {action}" );
        actionSequence.Add(action);
    }
    
    /// <summary>
    /// Triggers when an enemy is clicked on during enemy target selection
    /// </summary>
    /// <param name="targetEnemy">Passed Target Id of the clicked enemy</param>
    public void OnEnemyClicked(Enemy targetEnemy) {
        if (currentState is IStateListener) {
            IStateListener listener = (IStateListener)currentState;
            listener.OnEnemyClicked(targetEnemy);
        }
    }

    /// <summary>
    /// Triggers when a player chooses an action.
    /// </summary>
    /// <param name="action">The Selected action (i.e. ATTACK, HEAL, etc.)</param> <summary>
    public void SetPlayerAction(ActionType action) {
        if (currentState is IStateListener) {
            IStateListener listener = (IStateListener)currentState;
            listener.OnPlayerSetAction(action);
        }
    }

    public void SetPlayerActionNull() {
        playerSelectedAction = ActionType.NULL;
    }

    public void EndBattle() {
        PlayerDataManager.Instance.Data.CurrentStats = (EntityStats)playerBattleUnit.CurrentStats.Clone();
        SceneLoader.Instance.LoadOverworld();
    }
}   