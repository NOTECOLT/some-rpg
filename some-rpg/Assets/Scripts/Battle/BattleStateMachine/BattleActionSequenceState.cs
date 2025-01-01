using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionSequenceState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: ACTION SEQUENCE]");
        battle.StartCoroutine(ActionSequence(battle));
    }

    public override void UpdateState(BattleStateMachine battle) { }

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
                        yield return QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, 0.2f);

                        if (QTE.Result >= 0) {
                            damageDealt *= 2; 
                            battleText += "Critical hit!";
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
