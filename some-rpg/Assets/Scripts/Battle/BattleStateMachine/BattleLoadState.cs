using UnityEngine;

public class BattleLoadState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: LOAD GAME]");



        battle.ChangeState(battle.BattlePlayerTurnState);
    }

    public override void ExitState(BattleStateMachine battle) {

    }

    public override void UpdateState(BattleStateMachine battle) {
    
    }
}
