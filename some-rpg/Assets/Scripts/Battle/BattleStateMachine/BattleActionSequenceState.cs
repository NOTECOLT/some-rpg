using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleActionSequenceState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: ACTION SEQUENCE]");

        BuildEnemyActions(battle);
        SortBattleActions(battle);

        foreach (BattleAction action in battle.ActionSequence)
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

            battle.AddBattleAction(new BattleAction(battle.playerBattleUnit, ActionType.ATTACK, enemy));
        }
    }

    /// <summary>
    /// Sorts Battle Actions by priority. At the moment, priority is just defined by player speed
    /// </summary>
    public void SortBattleActions(BattleStateMachine battle) {
        battle.ActionSequence.Sort((x,y) => y.priority.CompareTo(x.priority));
    }

    /// </summary>
    /// Concerned with the player/enemy action execution & animation.
    /// <summary>
    public IEnumerator ActionSequence(BattleStateMachine battle) {
        float gapTime = 2.0f;

        foreach (BattleAction action in battle.ActionSequence) {
            switch (action.ActionType) {
                case ActionType.ATTACK:
                    int damageDealt = action.ActorUnit.CurrentStats.CalculateDamage(action.TargetUnit.CurrentStats);

                    string battleText = "";

                    // Generate QTE if the attacker is a player
                    if (action.ActorUnit is not Enemy) {
                        QuickTimeEvent QTE = new QuickTimeEvent(battle.QTEButton);
                        yield return QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, 0.2f, 0.4f);

                        if (QTE.Result >= 0) {
                            damageDealt *= 2; 
                            battleText += "Critical hit! ";
                        }
                    }

                    battleText += $"{action.ActorUnit.Name} attacked {action.TargetUnit.Name} for {damageDealt} damage!";
                    battle.mainTextbox.text = battleText;

                    action.TargetUnit.DealDamage(damageDealt);
                    yield return new WaitForSeconds(gapTime);

                    if (action.TargetUnit.CurrentStats.HitPoints <= 0)
                        SceneLoader.Instance.LoadOverworld();
                    break;
                default:
                    break;
            }
        }

        battle.ActionSequence = new List<BattleAction>();
        battle.ChangeState(battle.BattlePlayerTurnState);
    }

    // TODO: QUICK TIME EVENT 
    // TODO: https://www.youtube.com/watch?v=pzr1f85xeMc
}
