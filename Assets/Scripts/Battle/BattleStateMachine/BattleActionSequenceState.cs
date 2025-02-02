using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleActionSequenceState : GenericState {
    BattleStateMachine _context;
    public BattleActionSequenceState(BattleStateMachine context) {
        _context = context;
    }

    public override void EnterState() {
        Debug.Log($"[BattleStateMachine: ACTION SEQUENCE]");

        _context.OnEnterActionSequenceState.Invoke();

        BuildEnemyActions();
        SortBattleActions();

        foreach (BattleAction action in _context.actionSequence)
            Debug.Log($"[BattleStateMachine] {action}");

        _context.StartCoroutine(ActionSequence());
    }

    public override void UpdateState() { }

    public void BuildEnemyActions() {
        // Create Battle Actions for each enemy on the field
        // ? May move this to elsewhere? idk
        // ? Idea: have each enemy facilitate their own battle action?, or maybe just their battle action type
        //      - Would allow for enemy AI
        //      - May be executed with unity events
        foreach (GameObject obj in _context.enemyObjectList) {
            Enemy enemy = obj.GetComponent<EnemyObject>().Enemy;

            _context.AddBattleAction(new BasicAttack(_context.playerBattleUnit, enemy));
        }
    }

    /// <summary>
    /// Sorts Battle Actions by priority. At the moment, priority is just defined by player speed
    /// </summary>
    public void SortBattleActions() {
        _context.actionSequence.Sort((x,y) => y.priority.CompareTo(x.priority));
    }

    /// </summary>
    /// Concerned with the player/enemy action execution & animation.
    /// <summary>
    public IEnumerator ActionSequence() {
        float gapTime = 1.0f;

        foreach (BattleAction action in _context.actionSequence) {
            yield return action.DoAction(_context);

            yield return new WaitForSeconds(gapTime);

            // Check after each action if either the player or all enemies have died
            if (_context.playerBattleUnit.CurrentStats.HitPoints <= 0) {
                _context.EndBattle();
                yield break;
            }
                

            foreach (GameObject enemy in _context.enemyObjectList) {
                if (enemy.GetComponent<EnemyObject>().Enemy.CurrentStats.HitPoints > 0)
                    break;
                _context.EndBattle();
                yield break;
            }
        }

        _context.actionSequence = new List<BattleAction>();
        _context.ChangeState(_context.States[BattleStateMachine.StateKey.PLAYER_TURN_STATE]);
    }
}
