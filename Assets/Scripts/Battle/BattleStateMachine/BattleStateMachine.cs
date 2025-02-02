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
    public ActionType playerSelectedAction { get; private set; }
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

        States = new Dictionary<StateKey, GenericState>(){
            {StateKey.LOAD_STATE, new BattleLoadState(this)},
            {StateKey.PLAYER_TURN_STATE, new BattlePlayerTurnState(this)},
            {StateKey.ACTION_SEQUENCE_STATE, new BattleActionSequenceState(this)}
        };

        currentState = States[StateKey.LOAD_STATE];
        base.Start();
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
        switch (playerSelectedAction) {
            case ActionType.BASIC_ATTACK:
                if (currentState == States[StateKey.PLAYER_TURN_STATE]) {
                    AddBattleAction(new BasicAttack(targetEnemy, playerBattleUnit));
                
                    ChangeState(States[StateKey.ACTION_SEQUENCE_STATE]);
                }
                break;
            default:
                break;
        }
    }

    public void AddBattleAction(BattleAction action) {
        Debug.Log($"[BattleStateMachine] Battle Action Added: {action}" );
        actionSequence.Add(action);
    }

    public void SetPlayerAction(ActionType action) {
        playerSelectedAction = action;


        switch (playerSelectedAction) {
            case ActionType.HEAL:
                AddBattleAction(new Heal(playerBattleUnit));
            
                ChangeState(States[StateKey.ACTION_SEQUENCE_STATE]);
                break;
            default:
                break;
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