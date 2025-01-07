using UnityEngine;

public class BattleLoadState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: LOAD GAME]");

        battle.SetPlayerActionNull();

        foreach (EnemyType enemyType in SceneLoader.Instance.Encounters) {
            GameObject enemyObject = GameObject.Instantiate(battle.enemyObjectPrefab, battle.enemyObjectParent.transform);
            Enemy enemy = new Enemy(enemyType, enemyObject);
            
            enemyObject.name = enemy.Name;
            enemyObject.GetComponent<SpriteRenderer>().sprite = enemy.EnemyType.Sprite;

            // UnityEvent listener used for selecting enemies
            enemyObject.GetComponent<EnemyObject>().Enemy = enemy;
            enemyObject.GetComponent<EnemyObject>().OnEnemyClicked.AddListener(battle.OnEnemyClicked);

            enemyObject.GetComponent<EntityInfoUI>().Instantiate(enemy);
            
            battle.enemyObjectList.Add(enemyObject);
            Debug.Log("[BattleStateMachine] Instantiated EnemyTarget gameObject name=" + enemyObject.name + "; name=" + enemy.EnemyType.EnemyName + "; current HP=" + enemy.CurrentStats.HitPoints + ";");
        }

        battle.playerBattleUnit = new BattleUnit(PlayerData.Instance.BaseStats, battle.playerObject, "Player");
        battle.playerBattleUnit.Object.GetComponent<EntityInfoUI>().Instantiate(battle.playerBattleUnit);

        battle.qteButton.SetActive(false);

        battle.ChangeState(battle.battlePlayerTurnState);
    }

    public override void UpdateState(BattleStateMachine battle) {
    
    }
}
