using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerTurnState : GenericState<BattleStateMachine.StateKey>, IPlayerTurnListener {
    BattleStateMachine _context;
    private bool _isPlayerTurnDone;
    private int _currentPlayer;

    public BattlePlayerTurnState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _isPlayerTurnDone = false;
        _currentPlayer = 0;

        _context.SetPlayerActionNull();
        _context.OnEnterPlayerTurnState.Invoke(_context.playerBattleUnits[_currentPlayer].Name);
    }

    public override BattleStateMachine.StateKey GetNextState() {
        if (_isPlayerTurnDone) {
            return BattleStateMachine.StateKey.ACTION_SEQUENCE_STATE;
        } else {
            return Key;
        }
        
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    #region IPlayerTurnListener
    
    public void OnEnemyClicked(Enemy targetEnemy) {
        switch (_context.playerSelectedAction) {
            case ActionType.BASIC_ATTACK:
                _context.AddBattleAction(new BasicAttack(targetEnemy, _context.playerBattleUnits[_currentPlayer]));
                
                _currentPlayer += 1;

                if (_currentPlayer == _context.playerBattleUnits.Count) {
                    _isPlayerTurnDone = true;
                    return;
                }
                    
                _context.OnEnterPlayerTurnState.Invoke(_context.playerBattleUnits[_currentPlayer].Name);
                break;
            default:
                break;
        }
    }

    public void OnPlayerSetAction(ActionType action) {
        _context.playerSelectedAction = action;

        switch (_context.playerSelectedAction) {
            case ActionType.HEAL:
                _context.AddBattleAction(new Heal(_context.playerBattleUnits[_currentPlayer]));
                
                _currentPlayer += 1;

                if (_currentPlayer == _context.playerBattleUnits.Count) {
                    _isPlayerTurnDone = true;
                    return;
                }

                _context.OnEnterPlayerTurnState.Invoke(_context.playerBattleUnits[_currentPlayer].Name);
                break;
            default:
                break;
        }
    }

    #endregion
}
