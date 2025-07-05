using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleActionSequenceState : GenericState<BattleStateMachine.StateKey> {
    private System.Random _rnd;
    BattleStateMachine _context;
    private bool _isActionSequenceDone;
    private bool _isBattleDone;


    public BattleActionSequenceState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
        _rnd = new System.Random();
    }

    public override void EnterState() {
        _isActionSequenceDone = false;
        _isBattleDone = false;
        _context.OnEnterActionSequenceState?.Invoke();

        BuildEnemyActions();
        SortBattleActions();

        foreach (BattleAction action in _context.actionSequence)
            Debug.Log($"[BattleStateMachine] {action}");

        _context.StartCoroutine(ActionSequence());
    }

    public override void UpdateState() { }

    public override BattleStateMachine.StateKey GetNextState() {
        if (_isBattleDone) {
            return BattleStateMachine.StateKey.END_BATTLE_STATE;
        } else {
            if (_isActionSequenceDone) {
                return BattleStateMachine.StateKey.PLAYER_TURN_STATE;
            } else {
                return Key;
            }     
        }
    }  
    
    public void BuildEnemyActions() {
        // Create Battle Actions for each enemy on the field
        // ? May move this to elsewhere? idk
        // ? Idea: have each enemy facilitate their own battle action?, or maybe just their battle action type
        //      - Would allow for enemy AI
        //      - May be executed with unity events

        List<BattleUnit> aliveMembers = new List<BattleUnit>(_context.playerBattleUnits);
        for (int i = aliveMembers.Count - 1; i >= 0; i--)
            if (aliveMembers[i].MemberData.CurrentStats.HitPoints <= 0)
                aliveMembers.RemoveAt(i);

        foreach (GameObject obj in _context.enemyObjectList) {
            Enemy enemy = obj.GetComponent<EnemyObject>().Enemy;

            _context.PushBattleAction(new BasicAttack(aliveMembers[_rnd.Next(0, aliveMembers.Count)], enemy));
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

        while (_context.actionSequence.Count > 0) {
            BattleAction action = _context.actionSequence[0];
            yield return action.DoAction(_context);
            _context.actionSequence.RemoveAt(0);

            yield return new WaitForSeconds(gapTime);

            // TODO: NEEDS REFACTORING, very sloppy lol
            // On Player Death Sequence
            for (int i = 0; i < _context.playerBattleUnits.Count; i++) {
                BattleUnit player = _context.playerBattleUnits[i];
                if (player.MemberData.CurrentStats.HitPoints <= 0) {
                    // Remove All actions with a dead unit
                    for (int j = _context.actionSequence.Count - 1; j >= 0; j--) {
                        if (_context.actionSequence[j].ActorUnit.Equals(player)) {
                            _context.actionSequence.RemoveAt(j);
                        } else if (_context.actionSequence[j].TargetUnit.Equals(player)) {
                            // Redirect attacks against dead units
                            if (_context.playerBattleUnits.All(p => p.MemberData.CurrentStats.HitPoints <= 0))
                                _context.actionSequence.RemoveAt(j);
                            else
                                _context.actionSequence[j].TargetUnit =_context.playerBattleUnits.First(p => p.MemberData.CurrentStats.HitPoints <= 0);
                        }
                    }
                }
            }
                  
            if (_context.playerBattleUnits.All(p => p.MemberData.CurrentStats.HitPoints <= 0)) {
                _isBattleDone = true;
                yield break;
            }

            // On Enemy Death Sequence
            for (int i = _context.enemyObjectList.Count - 1; i >= 0; i--) {
                GameObject enemyObj = _context.enemyObjectList[i];
                if (enemyObj.GetComponent<EnemyObject>().Enemy.MemberData.CurrentStats.HitPoints <= 0) {
                    enemyObj.GetComponent<EnemyObject>().Enemy.RemoveAllListeners();
                    _context.enemyObjectList.RemoveAt(i);

                    // Remove All actions with a dead unit
                    for (int j = _context.actionSequence.Count - 1; j >= 0; j--) {
                        if (_context.actionSequence[j].ActorUnit.Equals(enemyObj.GetComponent<EnemyObject>().Enemy)) {
                            _context.actionSequence.RemoveAt(j);
                        } else if (_context.actionSequence[j].TargetUnit.Equals(enemyObj.GetComponent<EnemyObject>().Enemy)) {
                            // Redirect attacks against dead units
                            if (_context.enemyObjectList.Count > 0)
                                _context.actionSequence[j].TargetUnit = _context.enemyObjectList[0].GetComponent<EnemyObject>().Enemy;
                            else
                                _context.actionSequence.RemoveAt(j);
                        }
                    }

                    GameObject.Destroy(enemyObj);
                }
            }
        }

        if (_context.enemyObjectList.Count == 0) {
            _isBattleDone = true;
            yield break;
        }

        _context.actionSequence = new List<BattleAction>();
        _isActionSequenceDone = true;
    }
    public override void ExitState() { }
}
