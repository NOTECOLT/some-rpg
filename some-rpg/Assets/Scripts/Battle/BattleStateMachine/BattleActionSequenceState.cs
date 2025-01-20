using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleActionSequenceState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: ACTION SEQUENCE]");

        battle.OnEnterActionSequenceState.Invoke();

        BuildEnemyActions(battle);
        SortBattleActions(battle);

        foreach (BattleAction action in battle.actionSequence)
            Debug.Log($"[BattleStateMachine] {action}");

        battle.StartCoroutine(ActionSequence(battle));
    }

    public override void UpdateState(BattleStateMachine battle) { }

    public void BuildEnemyActions(BattleStateMachine battle) {
        // Create Battle Actions for each enemy on the field
        // ? May move this to elsewhere? idk
        // ? Idea: have each enemy facilitate their own battle action?, or maybe just their battle action type
        //      - Would allow for enemy AI
        //      - May be executed with unity events
        foreach (GameObject obj in battle.enemyObjectList) {
            Enemy enemy = obj.GetComponent<EnemyObject>().Enemy;

            battle.AddBattleAction(new BasicAttack(battle.playerBattleUnit, enemy));
        }
    }

    /// <summary>
    /// Sorts Battle Actions by priority. At the moment, priority is just defined by player speed
    /// </summary>
    public void SortBattleActions(BattleStateMachine battle) {
        battle.actionSequence.Sort((x,y) => y.priority.CompareTo(x.priority));
    }

    /// </summary>
    /// Concerned with the player/enemy action execution & animation.
    /// <summary>
    public IEnumerator ActionSequence(BattleStateMachine battle) {
        float gapTime = 1.0f;

        foreach (BattleAction action in battle.actionSequence) {
            yield return action.DoAction(battle);

            yield return new WaitForSeconds(gapTime);

            // Check after each action if either the player or all enemies have died
            if (PlayerDataManager.Instance.Data.CurrentStats.hitPoints <= 0) {
                battle.EndBattle();
                yield break;
            }
                

            foreach (GameObject enemy in battle.enemyObjectList) {
                if (enemy.GetComponent<EnemyObject>().Enemy.CurrentStats.hitPoints > 0)
                    break;
                battle.EndBattle();
                yield break;
            }
        }

        battle.actionSequence = new List<BattleAction>();
        battle.ChangeState(battle.battlePlayerTurnState);
    }

    // TODO: QUICK TIME EVENT 
    // TODO: https://www.youtube.com/watch?v=pzr1f85xeMc
}
