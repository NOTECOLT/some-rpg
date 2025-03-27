using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndState : GenericState<BattleStateMachine.StateKey> {
    private BattleStateMachine _context;

    public BattleEndState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _context.gameContext.EndBattle();
    }

    public override void ExitState() { 
    }

    public override BattleStateMachine.StateKey GetNextState() {
        return Key;
    }

    public override void UpdateState() { }
}
