using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : BattleAction {
    public Heal(BattleUnit actorUnit) : base("Heal", null, actorUnit) {
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        string battleText = "";
            
        // Update Text & Entity Info
        // battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} for {damageDealt} damage!";
        battle.mainTextbox.text = battleText;

        yield return null;
    }
}
