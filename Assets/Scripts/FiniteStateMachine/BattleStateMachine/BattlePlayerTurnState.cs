using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerTurnState : GenericState<BattleStateMachine.StateKey>, IStateListener {
    BattleStateMachine _context;
    private bool _isPlayerTurnDone;
    public BattlePlayerTurnState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _isPlayerTurnDone = false;
        Debug.Log($"[BattleStateMachine: PLAYER TURN]");

        _context.SetPlayerActionNull();
        _context.OnEnterPlayerTurnState.Invoke();
    }

    public override BattleStateMachine.StateKey GetNextState() {
        if (_isPlayerTurnDone) {
            return BattleStateMachine.StateKey.ACTION_SEQUENCE_STATE;
        } else {
            return Key;
        }
        
    }

    public override void UpdateState() { }

    public void OnEnemyClicked(Enemy targetEnemy) {
        switch (_context.playerSelectedAction) {
            case ActionType.BASIC_ATTACK:
                _context.AddBattleAction(new BasicAttack(targetEnemy, _context.playerBattleUnit));
                
                _isPlayerTurnDone = true;
                break;
            default:
                break;
        }
    }

    public void OnPlayerSetAction(ActionType action) {
        _context.playerSelectedAction = action;

        switch (_context.playerSelectedAction) {
            case ActionType.HEAL:
                _context.AddBattleAction(new Heal(_context.playerBattleUnit));
                
                _isPlayerTurnDone = true;
                break;
            default:
                break;
        }
    }
}
