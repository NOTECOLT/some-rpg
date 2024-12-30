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
    [SerializeField] private GameObject _playerTarget;
    [SerializeField] private TMP_Text _mainTextbox;
    [SerializeField] private GameObject _enemyTargetParent;
    [SerializeField] private GameObject _enemyTargetPrefab;

    // List of enemy targets in the battle
    public List<Enemy> enemyList = new List<Enemy>();
    [SerializeField] private List<GameObject> _enemyTargetList = new List<GameObject>();

    private bool _stateHasBeenUpdated = true;
    private BattleState _currentState = BattleState.PLAYER_TURN;
    private int _playerSelectedTarget = -1;
    private bool _qteSuccess = false;
    void Start() {
        // Listener currently used for detecting successful QTE inputs
        GetComponent<QTESystem>().OnQTESuccess.AddListener(SetQTESuccess);

        SceneLoader sceneLoader = FindAnyObjectByType<SceneLoader>();

        if (sceneLoader is null) {
            Debug.LogError("Scene does not have BattleManager Component!");
        } else {
            enemyList = new List<Enemy>();
            foreach (EnemyType enemyType in sceneLoader.Encounters) {
                enemyList.Add(new Enemy(enemyType));
            }
        }

        _playerData = PlayerData.Instance;

        // Spawn encounters
        for (int i = 0; i < enemyList.Count; i++) {
            enemyList[i].Instantiate(i); // Sets the TargetId of the enemy

            // Create Enemy prefab
            GameObject enemyTarget = Instantiate(_enemyTargetPrefab, _enemyTargetParent.transform);
            enemyTarget.name = enemyList[i].EnemyType.name + " " + enemyList[i].TargetId + " (Enemy Target)";
            enemyTarget.GetComponent<SpriteRenderer>().sprite = enemyList[i].EnemyType.Sprite;

            // UnityEvent listener used for selecting enemies
            enemyTarget.GetComponent<EnemyTarget>().TargetId = i;
            enemyTarget.GetComponent<EnemyTarget>().OnEnemyClicked.AddListener(OnEnemyClicked);

            _enemyTargetList.Add(enemyTarget);
            Debug.Log("[BattleManager] Instantiated EnemyTarget gameObject name=" + enemyTarget.name + "; name=" + enemyList[i].EnemyType.EnemyName + "; current HP=" + enemyList[i].CurrentStats.HitPoints + "; target id=" + enemyList[i].TargetId + ";");
        }
    }

    void OnDisable() {
        GetComponent<QTESystem>().OnQTESuccess.RemoveListener(SetQTESuccess);
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            int states = Enum.GetNames(typeof(BattleState)).Length;

            UpdateState((BattleState)((int)(_currentState + 1) % states));
        }

        // ? I don't know if race conditions are possible due to update() running while state is being updated/other functions happening
        // ? Maybe important to keep this in mind just in case

        // Invokes actions and other calls upon updating the state
        if (_stateHasBeenUpdated) {
            _stateHasBeenUpdated = false;

            switch (_currentState) {
                case BattleState.PLAYER_TURN:
                    _mainTextbox.text = "What will player do?";
                    Debug.Log($"[BattleManager: PLAYER TURN]");
                    break;
                case BattleState.PLAYER_SELECT_ATTACK:
                    _mainTextbox.text = "Select an enemy to attack.";
                    Debug.Log("[BattleManager: PLAYER SELECT ATTACK]");
                    break;
                case BattleState.ACTION_SEQUENCE:
                    _mainTextbox.text = _currentState.ToString();
                    Debug.Log("[BattleManager: ACTION SEQUENCE]");

                    BuildActionSequence();
                    StartCoroutine(ActionSequence());
                    break;
                default:
                    break;
            }
        }
    }

    private void SetQTESuccess(float timeLeft) {
        _qteSuccess = true;
    }

    /// <summary>
    /// Called in order to update the BattleState
    /// </summary>
    public void UpdateState(BattleState nextState) {
        _currentState = nextState;
        _stateHasBeenUpdated = true;
        Debug.Log("[BattleManager] State Updated to " + nextState.ToString());
    }

    // BUTTON FUNCTIONS ---------------------------------------------
    // At the moment these are only called when certain buttons in the UI are clicked.
    public void SetStateToPlayerSelectAttack() {
        UpdateState(BattleState.PLAYER_SELECT_ATTACK);
    }

    public void ReturnToOverworldScene() {
        SceneLoader.Instance.LoadOverworld();
    }

    // --------------------------------------------------------------

    // Listener Function to be added to every enemy target.
    /// <summary>
    /// Triggers when an enemy is clicked on during enemy target selection
    /// </summary>
    /// <param name="targetId">Passed Target Id of the clicked enemy</param>
    private void OnEnemyClicked(int targetId) {
        if (_currentState == BattleState.PLAYER_SELECT_ATTACK) {
            _playerSelectedTarget = targetId;

            Debug.Log("[BattleManager] Player selected enemy target id=" + _playerSelectedTarget);
            UpdateState(BattleState.ACTION_SEQUENCE);
        }
    }

    /// <summary>
    /// Concerned with the selection building the sequencing of player and enemy's action for of each turn.
    /// </summary>
    public void BuildActionSequence() {
        // Build an "EntityAction" object that can be filled sequentially.
        // This will be executed during the Action Sequence phase.

        // TODO: BUILD ACTION SEQUENCE
    }

    /// <summary>
    /// Not to be confused with OnActionSequenceStart, <br></br>
    /// This function is called before OnActionSequenceStart, during the ACTION_SEQUENCE BattleState. <br></br>
    /// Concerned with the player/enemy action execution & animation.
    /// </summary>
    public IEnumerator ActionSequence() {
        float animationTime = 0.3f; // HP animation time in seconds
        float gapTime = 2.0f;

        // PLAYER ATTACK ENEMY
        for (int i = 0; i < enemyList.Count; i++) {
            Enemy enemy = enemyList[i];
            GameObject enemyTarget = _enemyTargetList[i];


            yield return GetComponent<QTESystem>().GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, 0.2f);

            int damageDealt = _playerData.CurrentStats.CalculateDamage(enemy.CurrentStats);

            if (_qteSuccess) {
                damageDealt *= 2; 
                _qteSuccess = false;

                _mainTextbox.text = "Critical hit! Player attacked " + enemy.EnemyType.EnemyName + " for " + damageDealt + " damage!";
            } else {
                _mainTextbox.text = "Player attacked " + enemy.EnemyType.EnemyName + " for " + damageDealt + " damage!";
            }

            int newHP = enemy.CurrentStats.HitPoints - damageDealt;
            enemyTarget.GetComponent<EntityInfoUI>().SetHPBar((float)newHP/enemy.EnemyType.BaseStats.HitPoints, animationTime);
            enemy.CurrentStats.HitPoints = newHP;

            yield return new WaitForSeconds(gapTime);

            if (newHP <= 0) {
                SceneLoader.Instance.LoadOverworld();
            }
        }

        // ENEMY ATTACK PLAYER
        for (int i = 0; i < enemyList.Count; i++) {
            Enemy enemy = enemyList[i];
            int damageDealt = enemy.CurrentStats.CalculateDamage(enemy.CurrentStats);

            _mainTextbox.text = enemy.EnemyType.EnemyName + " attacked Player for " + damageDealt + " damage!";

            int newHP = _playerData.CurrentStats.HitPoints - damageDealt;
            _playerTarget.GetComponent<EntityInfoUI>().SetHPBar((float)newHP/_playerData.BaseStats.HitPoints, animationTime);
            _playerData.CurrentStats.HitPoints = newHP;

            yield return new WaitForSeconds(gapTime);

            if (newHP <= 0) {
                SceneLoader.Instance.LoadOverworld();
            }
        }


        UpdateState(BattleState.PLAYER_TURN);
    }

    // TODO: QUICK TIME EVENT 
    // TODO: https://www.youtube.com/watch?v=pzr1f85xeMc
}
