using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages battle states and state changes
/// </summary>
public class BattleManager : MonoBehaviour {    
    // These UnityEvents are called upon the start of each BattleState
    public UnityEvent OnPlayerTurnStart;
    public UnityEvent OnPlayerSelectAttackStart;
    public UnityEvent OnEnemyTurnStart;
    public UnityEvent OnActionSequenceStart;

    [SerializeField] private PlayerData _playerData;
    [SerializeField] private GameObject _playerTarget;
    [SerializeField] private TMP_Text _mainTextbox;
    [SerializeField] private GameObject _enemyTargetParent;
    [SerializeField] private GameObject _enemyTargetPrefab;

    // List of enemy targets in the battle
    [SerializeField] private List<Enemy> _enemyList = new List<Enemy>();
    [SerializeField] private List<GameObject> _enemyTargetList = new List<GameObject>();

    private Boolean _stateHasBeenUpdated = true;

    private BattleState _currentState = BattleState.PLAYER_TURN;
    private int _playerSelectedTarget = -1;
    void Start() {
        // Spawn Enemy Targets
        for (int i = 0; i < _enemyList.Count; i++) {
            _enemyList[i].Instantiate(i); // Sets the TargetId of the enemy

            // Create Enemy prefab
            GameObject enemyTarget = Instantiate(_enemyTargetPrefab, _enemyTargetParent.transform);
            enemyTarget.name = _enemyList[i].EnemyType.name + " " + _enemyList[i].TargetId + " (Enemy Target)";
            enemyTarget.GetComponent<SpriteRenderer>().sprite = _enemyList[i].EnemyType.Sprite;

            // UnityEvent listener used for selecting enemies
            enemyTarget.GetComponent<EnemyTarget>().TargetId = i;
            enemyTarget.GetComponent<EnemyTarget>().OnEnemyClicked.AddListener(OnEnemyClicked);

            _enemyTargetList.Add(enemyTarget);
            Debug.Log("[BattleManager] Instantiated EnemyTarget gameObject name=" + enemyTarget.name + "; name=" + _enemyList[i].EnemyType.EnemyName + "; current HP=" + _enemyList[i].CurrentStats.HitPoints + "; target id=" + _enemyList[i].TargetId + ";");
        }

        // Decide who is the first turnholder
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            int states = Enum.GetNames(typeof(BattleState)).Length;

            UpdateState((BattleState)((int)(_currentState + 1) % states));
        }

        // ? I don't know if race conditions are possible due to update() running while state is being updated/other functions happening
        // ? Maybe important to keep this in mind just in case

        // Invokes actions and other calls upon updating the state
        // Was originally 
        if (_stateHasBeenUpdated) {
            _stateHasBeenUpdated = false;

            switch (_currentState) {
                case BattleState.PLAYER_TURN:
                    _mainTextbox.text = "What will player do?";
                    OnPlayerTurnStart.Invoke();
                    Debug.Log($"[BattleManager: PLAYER TURN] EventListeners Triggered: " + OnPlayerTurnStart.GetPersistentEventCount());
                    break;
                case BattleState.PLAYER_SELECT_ATTACK:
                    _mainTextbox.text = "Select an enemy to attack.";
                    OnPlayerSelectAttackStart.Invoke();
                    Debug.Log("[BattleManager: PLAYER SELECT ATTACK] EventListeners Triggered: " + OnPlayerSelectAttackStart.GetPersistentEventCount());
                    break;
                case BattleState.ENEMY_TURN:
                    _mainTextbox.text = _currentState.ToString(); 
                    OnEnemyTurnStart.Invoke();
                    Debug.Log("[BattleManager: ENEMY TURN] EventListeners Triggered: " + OnEnemyTurnStart.GetPersistentEventCount());
                    break;
                case BattleState.ACTION_SEQUENCE:
                    _mainTextbox.text = _currentState.ToString();
                    ActionSequence();
                    OnActionSequenceStart.Invoke();
                    Debug.Log("[BattleManager: ACTION SEQUENCE] EventListeners Triggered: " + OnActionSequenceStart.GetPersistentEventCount());
                    UpdateState(BattleState.PLAYER_TURN);
                    break;
                default:
                    break;
            }
        }


    }

    /// <summary>
    /// Called in order to update the BattleState
    /// </summary>
    public void UpdateState(BattleState nextState) {
        _currentState = nextState;
        _stateHasBeenUpdated = true;
        Debug.Log("[BattleManager] State Updated to " + nextState.ToString());
    }

    // At the moment this is only called when the Attack button in the UI is clicked.
    public void SetStateToPlayerSelectAttack() {
        UpdateState(BattleState.PLAYER_SELECT_ATTACK);
    }

    // Listener Function to be added to every enemy target.
    /// <summary>
    /// Triggers when an enemy is clicked on during enemy target selection
    /// </summary>
    /// <param name="targetId">Passed Target Id of the clicked enemy</param>
    private void OnEnemyClicked(int targetId) {
        if (_currentState == BattleState.PLAYER_SELECT_ATTACK) {
            _playerSelectedTarget = targetId;

            Debug.Log("[BattleManager] Player selected enemy target id=" + _playerSelectedTarget);
            UpdateState(BattleState.ENEMY_TURN);
        }
    }

    /// <summary>
    /// Not to be confused with OnEnemyTurnStart, <br></br>
    /// This function is called after OnEnemyTurnStart, during the ENEMY_TURN BattleState. <br></br>
    /// Concerned with the selection enemy's action for of each turn.
    /// </summary>
    public void EnemyTurn() {
        // TODO: okay, idea: for each enemy, we allow the enemy to store a function in a list of functions
        // This list of functions will then get executed

        for (int i = 0; i < _enemyList.Count; i++) {
            _playerData.CurrentStats.HitPoints -= _enemyList[i].EnemyType.BaseStats.Attack;
        }

        UpdateState(BattleState.ACTION_SEQUENCE);
    }

    /// <summary>
    /// Not to be confused with OnActionSequenceStart, <br></br>
    /// This function is called before OnActionSequenceStart, during the ACTION_SEQUENCE BattleState. <br></br>
    /// Concerned with the player/enemy turn animation and sequencing.
    /// </summary>
    public void ActionSequence() {
        float animationTime = 0.3f; // HP animation time in seconds
        // ENEMY
        for (int i = 0; i < _enemyList.Count; i++) {
            Enemy enemy = _enemyList[i];
            GameObject enemyTarget = _enemyTargetList[i];

            int newHP = enemy.CurrentStats.HitPoints - _playerData.CurrentStats.Attack;
            enemyTarget.GetComponent<EntityInfoUI>().SetHPBar((float)newHP/enemy.EnemyType.BaseStats.HitPoints, animationTime);
            enemy.CurrentStats.HitPoints = newHP;
        }

        // PLAYER
        for (int i = 0; i < _enemyList.Count; i++) {
            Enemy enemy = _enemyList[i];
            GameObject enemyTarget = _enemyTargetList[i];

            int newHP = _playerData.CurrentStats.HitPoints - enemy.CurrentStats.Attack;
            _playerTarget.GetComponent<EntityInfoUI>().SetHPBar((float)newHP/_playerData.BaseStats.HitPoints, animationTime);
            _playerData.CurrentStats.HitPoints = newHP;
        }
    }
}
