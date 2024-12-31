using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages battle states and state changes
/// </summary>
public class BattleManager : MonoBehaviour {    
    [SerializeField] private PlayerData _playerData;
    private BattleStateMachine _battleStateMachine;
    [SerializeField] private GameObject _playerTarget;
    [SerializeField] private TMP_Text _mainTextbox;
    [SerializeField] private GameObject _enemyTargetParent;
    [SerializeField] private GameObject _enemyTargetPrefab;

    // List of enemy targets in the battle
    [SerializeField] private List<GameObject> _enemyTargetList = new List<GameObject>();
    private ActionType _playerSelectedAction;
    void Start() {
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component!");
            return;
        }

        _playerData = PlayerData.Instance;
        _battleStateMachine = GetComponent<BattleStateMachine>();
        _playerSelectedAction = ActionType.NULL;

        foreach (EnemyType enemyType in SceneLoader.Instance.Encounters) {
            GameObject enemyTarget = Instantiate(_enemyTargetPrefab, _enemyTargetParent.transform);
            Enemy enemy = new Enemy(enemyType, enemyTarget);
            
            enemyTarget.name = enemy.EnemyType.name + " (Enemy Target)";
            enemyTarget.GetComponent<SpriteRenderer>().sprite = enemy.EnemyType.Sprite;

            // UnityEvent listener used for selecting enemies
            enemyTarget.GetComponent<EnemyTarget>().Enemy = enemy;
            enemyTarget.GetComponent<EnemyTarget>().OnEnemyClicked.AddListener(OnEnemyClicked);

            _enemyTargetList.Add(enemyTarget);
            Debug.Log("[BattleManager] Instantiated EnemyTarget gameObject name=" + enemyTarget.name + "; name=" + enemy.EnemyType.EnemyName + "; current HP=" + enemy.CurrentStats.HitPoints + ";");
        }
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
        if (_battleStateMachine.CurrentState == _battleStateMachine.BattlePlayerTurnState) {
            _battleStateMachine.AddBattleAction(new BattleAction(targetEnemy.CurrentStats, _playerSelectedAction, PlayerData.Instance.CurrentStats) {
                tempRef = targetEnemy
            });
        }

        _battleStateMachine.ChangeState(_battleStateMachine.BattleActionSequenceState);
    }
}