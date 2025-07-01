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

        // Sets the focus to the first party member that is not dead
        _currentPlayer = -1;
        SetFocusNextMember();

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
                _context.PushBattleAction(new BasicAttack(targetEnemy, _context.playerBattleUnits[_currentPlayer]));  
                SetFocusNextMember();
                
                if (_currentPlayer < _context.playerBattleUnits.Count)
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
                _context.PushBattleAction(new Heal(_context.playerBattleUnits[_currentPlayer], 8, 3));    
                SetFocusNextMember();

                if (_currentPlayer < _context.playerBattleUnits.Count)
                    _context.OnEnterPlayerTurnState.Invoke(_context.playerBattleUnits[_currentPlayer].Name);
                break;
            default:
                break;
        }
    }

    #endregion
    
    /// <summary>
    /// "Focus" here refers to which party member is currently being chosen to do an action
    /// </summary>
    private void SetFocusNextMember() {
        int i;
        for (i = _currentPlayer + 1; i < _context.playerBattleUnits.Count; i++) {
            if (_context.playerBattleUnits[i].CurrentStats.HitPoints > 0) break; 
        }

        _currentPlayer = i;

        if (_currentPlayer == _context.playerBattleUnits.Count) {
            _isPlayerTurnDone = true;
            return;
        }
    }
}
