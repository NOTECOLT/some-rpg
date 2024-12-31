using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionSequenceState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: ACTION SEQUENCE]");

        // BuildActionSequence();
        battle.StartCoroutine(ActionSequence(battle));
    }

    public override void ExitState(BattleStateMachine battle) {
    }

    public override void UpdateState(BattleStateMachine battle) {
    }


    /// <summary>
    /// Concerned with the selection building the sequencing of player and enemy's action for of each turn.
    /// </summary>
    public void BuildActionSequence() {
        // Build an "EntityAction" object that can be filled sequentially.
        // This will be executed during the Action Sequence phase.

        // TODO: BUILD ACTION SEQUENCE
    }

    /// Concerned with the player/enemy action execution & animation.
    /// </summary>
    /// <summary>
    public IEnumerator ActionSequence(BattleStateMachine battle) {
        float animationTime = 0.3f; // HP animation time in seconds
        float gapTime = 2.0f;

        foreach (BattleAction action in battle.ActionSequence) {
            switch (action.ActionType) {
                case ActionType.ATTACK:
                    QuickTimeEvent QTE = new QuickTimeEvent(battle.QTEButton);
                    yield return QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, 0.2f);

                    int damageDealt = action.ActorUnit.CalculateDamage(action.TargetUnit.CurrentStats);

                    if (QTE.Result >= 0) {
                        damageDealt *= 2; 

                        battle.mainTextbox.text = "Critical hit! Player attacked " + action.TargetUnit.Name + " for " + damageDealt + " damage!";
                    } else {
                        battle.mainTextbox.text = "Player attacked " + action.TargetUnit.Name + " for " + damageDealt + " damage!";
                    }

                    int newHP = action.TargetUnit.CurrentStats.HitPoints - damageDealt;
                    action.TargetUnit.Object.GetComponent<EntityInfoUI>().SetHPBar((float)newHP/action.TargetUnit.BaseStats.HitPoints, animationTime);
                    action.TargetUnit.CurrentStats.HitPoints = newHP;

                    yield return new WaitForSeconds(gapTime);

                    if (newHP <= 0) {
                        SceneLoader.Instance.LoadOverworld();
                    }

                    break;
                default:
                    break;
            }
        }

        battle.ActionSequence = new List<BattleAction>();

        // // ENEMY ATTACK PLAYER
        // Enemy enemy = enemyList[i];
        // int damageDealt = enemy.CurrentStats.CalculateDamage(enemy.CurrentStats);

        // battle.mainTextbox.text = enemy.EnemyType.EnemyName + " attacked Player for " + damageDealt + " damage!";

        // int newHP = _playerData.CurrentStats.HitPoints - damageDealt;
        // _playerTarget.GetComponent<EntityInfoUI>().SetHPBar((float)newHP/_playerData.BaseStats.HitPoints, animationTime);
        // _playerData.CurrentStats.HitPoints = newHP;

        // yield return new WaitForSeconds(gapTime);

        // if (newHP <= 0) {
        //     SceneLoader.Instance.LoadOverworld();
        // }


        battle.ChangeState(battle.BattlePlayerTurnState);
    }

    // TODO: QUICK TIME EVENT 
    // TODO: https://www.youtube.com/watch?v=pzr1f85xeMc
}
