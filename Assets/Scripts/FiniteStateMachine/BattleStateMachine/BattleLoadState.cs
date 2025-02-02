using UnityEngine;

public class BattleLoadState : GenericState<BattleStateMachine.StateKey> {
    private BattleStateMachine _context;
    private bool _isDoneLoading;
    public BattleLoadState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _isDoneLoading = false;
        Debug.Log($"[BattleStateMachine: LOAD GAME]");

        _context.SetPlayerActionNull();

        foreach (EnemyType enemyType in SceneLoader.Instance.Encounters) {
            GameObject enemyObject = GameObject.Instantiate(_context.enemyObjectPrefab, _context.enemyObjectParent.transform);
            Enemy enemy = new Enemy(enemyType, enemyObject);
            
            enemyObject.name = enemy.Name;
            enemyObject.GetComponent<SpriteRenderer>().sprite = enemyType.Sprite;

            // C# Event listener used for selecting enemies
            enemyObject.GetComponent<EnemyObject>().Enemy = enemy;
            enemyObject.GetComponent<EnemyObject>().OnEnemyClicked += _context.OnEnemyClicked;

            enemyObject.GetComponent<EntityInfoUI>().Instantiate(enemy);
            
            _context.enemyObjectList.Add(enemyObject);
            Debug.Log("[BattleStateMachine] Instantiated EnemyTarget gameObject name=" + enemyObject.name + "; name=" + enemyType.EnemyName + "; current HP=" + enemy.CurrentStats.HitPoints + ";");
        }

        _context.playerBattleUnit = new BattleUnit(PlayerDataManager.Instance.Data.BaseStats, PlayerDataManager.Instance.Data.CurrentStats, _context.playerObject, "Player", PlayerDataManager.Instance.Data.Weapon);
        _context.playerBattleUnit.Object.GetComponent<EntityInfoUI>().Instantiate(_context.playerBattleUnit);

        _context.qteButton.SetActive(false);
        _isDoneLoading = true;
    }

    public override void UpdateState() { } 

    public override BattleStateMachine.StateKey GetNextState() {
        if (_isDoneLoading) {
            return BattleStateMachine.StateKey.PLAYER_TURN_STATE;
        } else {
            return Key;
        }
    }    
}
