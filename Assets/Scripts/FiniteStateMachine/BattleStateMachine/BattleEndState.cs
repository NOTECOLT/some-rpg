using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndState : GenericState<BattleStateMachine.StateKey> {
    private BattleStateMachine _context;

    public BattleEndState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        // Add experience for remaining party members if all enemies are dead
        if (_context.enemyObjectList.Count == 0)
            foreach (BattleUnit member in _context.playerBattleUnits) {
                if (member.CurrentStats.HitPoints <= 0) continue;

                member.WeaponItem.AddExperience(5);
            }
        _context.gameContext.EndBattle();
    }

    public override void ExitState() {
    }

    public override BattleStateMachine.StateKey GetNextState() {
        return Key;
    }

    public override void UpdateState() { }
}
