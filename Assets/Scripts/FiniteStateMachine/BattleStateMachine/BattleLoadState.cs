using UnityEngine;

public class BattleLoadState : GenericState<BattleStateMachine.StateKey> {
    private BattleStateMachine _context;
    private bool _isDoneLoading;
    public BattleLoadState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _isDoneLoading = false;
        
        // We assume the GameStateMachine should be in GameBattleState by now
        _context.gameContext = GameStateMachine.Instance.GetCurrentStateContext<GameBattleState>();
        _context.gameContext.SetBattleContext(_context);

        _context.SetPlayerActionNull();

        foreach (EnemyType enemyType in SceneLoader.Instance.Encounters) {
            GameObject enemyObject = GameObject.Instantiate(_context.enemyObjectPrefab, _context.enemyObjectParent.transform);
            Enemy enemy = new Enemy(enemyType, enemyObject);
            
            enemyObject.name = enemy.Name;
            enemyObject.GetComponent<SpriteRenderer>().sprite = enemyType.Sprite;

            // C# Event listener used for selecting enemies
            enemyObject.GetComponent<EnemyObject>().Enemy = enemy;
            enemyObject.GetComponent<EnemyObject>().OnEnemyClicked += _context.OnEnemyClicked;

            enemyObject.GetComponent<UnitInfoUI>().Instantiate(enemy);
            
            _context.enemyObjectList.Add(enemyObject);
            Debug.Log("[BattleStateMachine] Instantiated EnemyTarget gameObject name=" + enemyObject.name + "; name=" + enemyType.EnemyName + "; current HP=" + enemy.CurrentStats.HitPoints + ";");
        }
        
        // Load all Player Units
        foreach (PlayerData.MemberStats member in PlayerDataManager.Instance.Data.PartyStats) {
            GameObject obj = GameObject.Instantiate(_context.playerUnitPrefab, _context.playerSide.transform);
            BattleUnit memberUnit = new BattleUnit(member.BaseStats, member.CurrentStats, obj, member.Name, member.Weapon.Data);
            _context.playerBattleUnits.Add(memberUnit);
            obj.GetComponent<UnitInfoUI>().Instantiate(memberUnit);
        }
        
        _context.playerSide.GetComponent<ArrangeChildren>().Arrange();
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

    public override void ExitState() { }
}
