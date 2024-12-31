using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerTurnState : BattleBaseState {
    public override void EnterState(BattleStateMachine battle) {
        Debug.Log($"[BattleStateMachine: PLAYER TURN]");

        battle.mainTextbox.text = "What will player do?";
    }

    public override void ExitState(BattleStateMachine battle) {

    }

    public override void UpdateState(BattleStateMachine battle) {
    
    }
}
