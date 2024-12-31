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
    private BattleStateMachine _battleStateMachine;
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private TMP_Text _mainTextbox;
    [SerializeField] private GameObject _enemyObjectParent;
    [SerializeField] private GameObject _enemyObjectPrefab;

    // List of enemy targets in the battle
    [SerializeField] private List<GameObject> _enemyTargetList = new List<GameObject>();
    private ActionType _playerSelectedAction;
    void Start() {
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component!");
            return;
        }

        _battleStateMachine = GetComponent<BattleStateMachine>();
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
            Debug.Log("[BattleManager] Instantiated EnemyTarget gameObject name=" + enemyObject.name + "; name=" + enemy.EnemyType.EnemyName + "; current HP=" + enemy.CurrentStats.HitPoints + ";");
        }

        PlayerData.Instance.BattleUnit = new BattleUnit(PlayerData.Instance.BaseStats, _playerObject, "Player");
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
            _battleStateMachine.AddBattleAction(new BattleAction(targetEnemy, _playerSelectedAction, PlayerData.Instance.BattleUnit));
        }

        _battleStateMachine.ChangeState(_battleStateMachine.BattleActionSequenceState);
    }
}