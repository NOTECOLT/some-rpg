using UnityEngine;

public class BattleLoadState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: LOAD GAME]");

        battle.playerSelectedAction = ActionType.NULL;

        foreach (EnemyType enemyType in SceneLoader.Instance.Encounters) {
            GameObject enemyObject = GameObject.Instantiate(battle.enemyObjectPrefab, battle.enemyObjectParent.transform);
            Enemy enemy = new Enemy(enemyType, enemyObject);
            
            enemyObject.name = enemy.Name + " (Enemy Target)";
            enemyObject.GetComponent<SpriteRenderer>().sprite = enemy.EnemyType.Sprite;

            // UnityEvent listener used for selecting enemies
            enemyObject.GetComponent<EnemyObject>().Enemy = enemy;
            enemyObject.GetComponent<EnemyObject>().OnEnemyClicked.AddListener(battle.OnEnemyClicked);

            battle.enemyObjectList.Add(enemyObject);
            Debug.Log("[BattleStateMachine] Instantiated EnemyTarget gameObject name=" + enemyObject.name + "; name=" + enemy.EnemyType.EnemyName + "; current HP=" + enemy.CurrentStats.HitPoints + ";");
        }

        battle.playerBattleUnit = new BattleUnit(PlayerData.Instance.BaseStats, battle.playerObject, "Player");

        battle.QTEButton.SetActive(false);

        battle.ChangeState(battle.BattlePlayerTurnState);
    }

    public override void UpdateState(BattleStateMachine battle) {
    
    }
}
