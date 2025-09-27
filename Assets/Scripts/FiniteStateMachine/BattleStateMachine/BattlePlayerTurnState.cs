using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerTurnState : GenericState<BattleStateMachine.StateKey> {
    BattleStateMachine _context;
    private bool _isPlayerTurnDone;
    private int _currentPlayer;

    private int _firstPlayer;

    private List<BattleAction> _actionCache; // Temporary list of battle actions that are manipulated before being added to the final list

    public BattlePlayerTurnState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _isPlayerTurnDone = false;

        _actionCache = new List<BattleAction>();

        // Sets the focus to the first party member that is not dead
        _currentPlayer = -1;
        _firstPlayer = SetFocusNextMember();

        _context.SetPlayerActionNull();
        _context.OnResetBattleMenuUI?.Invoke(_context.playerBattleUnits[_currentPlayer].MemberData.Name);
    }

    public override BattleStateMachine.StateKey GetNextState() {
        if (_isPlayerTurnDone) {
            return BattleStateMachine.StateKey.ACTION_SEQUENCE_STATE;
        } else {
            return Key;
        }

    }

    public override void UpdateState() { }

    public override void ExitState() {
        foreach (BattleAction action in _actionCache) {
            _context.PushBattleAction(action);
        }
        _actionCache = new List<BattleAction>();
    }

    public void OnEnemyClicked(Enemy targetEnemy) {
        switch (_context.playerSelectedAction) {
            case ActionType.BASIC_ATTACK:
                _actionCache.Add(new BasicAttack(targetEnemy, _context.playerBattleUnits[_currentPlayer]));
                SetFocusNextMember();

                if (_currentPlayer < _context.playerBattleUnits.Count)
                    _context.OnResetBattleMenuUI?.Invoke(_context.playerBattleUnits[_currentPlayer].MemberData.Name);
                break;
            default:
                break;
        }
    }

    public void OnPlayerSetAction(ActionType action) {
        _context.playerSelectedAction = action;

        switch (_context.playerSelectedAction) {
            case ActionType.HEAL:
                _actionCache.Add(new Heal(_context.playerBattleUnits[_currentPlayer], 8, 3));
                SetFocusNextMember();

                if (_currentPlayer < _context.playerBattleUnits.Count)
                    _context.OnResetBattleMenuUI?.Invoke(_context.playerBattleUnits[_currentPlayer].MemberData.Name);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// "Focus" here refers to which party member is currently being chosen to do an action
    /// </summary>
    private int SetFocusNextMember() {
        int i;
        for (i = _currentPlayer + 1; i < _context.playerBattleUnits.Count; i++) {
            if (_context.playerBattleUnits[i].MemberData.CurrentStats.HitPoints > 0) break;
        }

        _currentPlayer = i;

        if (_currentPlayer == _context.playerBattleUnits.Count) {
            _isPlayerTurnDone = true;
        }

        return i;
    }

    /// <summary>
    /// "Focus" here refers to which party member is currently being chosen to do an action
    /// </summary>
    public void SetFocusPreviousMember() {
        if (_currentPlayer - 1 < _firstPlayer) return;

        int i;
        for (i = _currentPlayer - 1; i >= _firstPlayer; i--) {
            if (_context.playerBattleUnits[i].MemberData.CurrentStats.HitPoints > 0) break;
        }

        _currentPlayer = i;

        if (_actionCache.Count > 0)
            _actionCache.RemoveAt(_actionCache.Count - 1);


        // TODO: FIX THIS, FLOW OF LOGIC GOING BACK N FORTH BETWEEN BSM AND PLAYER TURN STATE
        _context.OnResetBattleMenuUI?.Invoke(_context.playerBattleUnits[_currentPlayer].MemberData.Name);
    }
}
