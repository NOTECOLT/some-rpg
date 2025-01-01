using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class to handle all states in a turn based battle
public class BattleStateMachine : MonoBehaviour {
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private GameObject _enemyObjectParent;
    [SerializeField] private GameObject _enemyObjectPrefab;

    // List of enemy targets in the battle
    [SerializeField] private List<GameObject> _enemyTargetList = new List<GameObject>();
    private ActionType _playerSelectedAction;
    private BattleUnit _playerBattleUnit;

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
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component!");
            return;
        }
        _playerSelectedAction = ActionType.NULL;

        foreach (EnemyType enemyType in SceneLoader.Instance.Encounters) {
            GameObject enemyObject = Instantiate(_enemyObjectPrefab, _enemyObjectParent.transform);
            Enemy enemy = new Enemy(enemyType, enemyObject);
            
            enemyObject.name = enemy.Name + " (Enemy Target)";
            enemyObject.GetComponent<SpriteRenderer>().sprite = enemy.EnemyType.Sprite;

            // UnityEvent listener used for selecting enemies
            enemyObject.GetComponent<EnemyObject>().Enemy = enemy;
            enemyObject.GetComponent<EnemyObject>().OnEnemyClicked.AddListener(OnEnemyClicked);

            _enemyTargetList.Add(enemyObject);
            Debug.Log("[BattleStateMachine] Instantiated EnemyTarget gameObject name=" + enemyObject.name + "; name=" + enemy.EnemyType.EnemyName + "; current HP=" + enemy.CurrentStats.HitPoints + ";");
        }

        _playerBattleUnit = new BattleUnit(PlayerData.Instance.BaseStats, _playerObject, "Player");


        QTEButton.SetActive(false);

        CurrentState = BattleLoadState;
        CurrentState.EnterState(this);
    }

    // BUTTON FUNCTIONS ---------------------------------------------
    // At the moment these are only called when certain buttons in the UI are clicked.
    public void SetStateToPlayerSelectAttack() {
        _playerSelectedAction = ActionType.ATTACK;
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
    private void OnEnemyClicked(Enemy targetEnemy) {
        if (CurrentState == BattlePlayerTurnState) {
            AddBattleAction(new BattleAction(targetEnemy, _playerSelectedAction, _playerBattleUnit));
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
