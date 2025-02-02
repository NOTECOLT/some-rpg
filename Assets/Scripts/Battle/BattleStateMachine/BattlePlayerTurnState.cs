using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerTurnState : GenericState {
    BattleStateMachine _context;
    public BattlePlayerTurnState(BattleStateMachine context) {
        _context = context;
    }

    public override void EnterState() {
        Debug.Log($"[BattleStateMachine: PLAYER TURN]");

        _context.SetPlayerActionNull();
        _context.OnEnterPlayerTurnState.Invoke();
    }
    public override void UpdateState() {
    
    }
}
