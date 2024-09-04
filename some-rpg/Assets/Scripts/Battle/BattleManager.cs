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
    public BattleState CurrentState { get; private set; } = BattleState.PLAYER_TURN;
    public List<Enemy> EnemyList = new List<Enemy>();

    // These UnityEvents are called upon the start of each BattleState
    public UnityEvent OnPlayerTurnStart;
    public UnityEvent OnPlayerSelectAttackStart;
    public UnityEvent OnEnemyTurnStart;
    public UnityEvent OnActionSequenceStart;

    [SerializeField] private TMP_Text _mainTextbox;
    [SerializeField] private GameObject _enemyTargetParent;
    [SerializeField] private GameObject _enemyTargetPrefab;
    void Start() {
        int enemyCount = 0;
        // Spawn Enemies
        foreach (Enemy e in EnemyList) {
            e.Instantiate(enemyCount++);

            GameObject enemyTarget = Instantiate(_enemyTargetPrefab, _enemyTargetParent.transform);
            enemyTarget.name = e.EnemyType.name + " " + e.TargetId + " (Enemy Target)";
            enemyTarget.GetComponent<SpriteRenderer>().sprite = e.EnemyType.Sprite;

            Debug.Log("gameObject name=" + enemyTarget.name + "; name=" + e.EnemyType.EnemyName + "; current HP=" + e.CurrentHitPoints + "; target id=" + e.TargetId + ";");
        }

        // Decide who is the first turnholder
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            int states = Enum.GetNames(typeof(BattleState)).Length;

            UpdateState((BattleState)((int)(CurrentState + 1) % states));
        }
    }

    // Called by other entities to switch the current turn
    public void UpdateState(BattleState nextState) {
        CurrentState = nextState;
        Debug.Log("[BattleManager] State Updated to " + CurrentState.ToString());
        
        switch (CurrentState) {
            case BattleState.PLAYER_TURN:
                _mainTextbox.text = "What will player do?";
                OnPlayerTurnStart.Invoke();
                Debug.Log("[BattleManager] EventListeners Triggered: " + OnPlayerTurnStart.GetPersistentEventCount());
                break;
            case BattleState.PLAYER_SELECT_ATTACK:
                _mainTextbox.text = "Select an enemy to attack.";
                OnPlayerSelectAttackStart.Invoke();
                Debug.Log("[BattleManager] EventListeners Triggered: " + OnPlayerSelectAttackStart.GetPersistentEventCount());
                break;
            case BattleState.ENEMY_TURN:
                _mainTextbox.text = CurrentState.ToString();  
                OnEnemyTurnStart.Invoke();
                Debug.Log("[BattleManager] EventListeners Triggered: " + OnEnemyTurnStart.GetPersistentEventCount());
                break;
            case BattleState.ACTION_SEQUENCE:
                _mainTextbox.text = CurrentState.ToString();
                OnActionSequenceStart.Invoke();
                Debug.Log("[BattleManager] EventListeners Triggered: " + OnActionSequenceStart.GetPersistentEventCount());
                break;
            default:
                break;
        }
    }

    public void SetStateToPlayerSelectAttack() {
        UpdateState(BattleState.PLAYER_SELECT_ATTACK);
    }

}
