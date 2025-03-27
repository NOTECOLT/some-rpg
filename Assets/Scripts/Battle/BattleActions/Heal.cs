using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : BattleAction {    
    public Heal(BattleUnit actorUnit) : base("Heal", actorUnit, actorUnit) {
    }

    public override string ToString() {
        return $"{ActorUnit.Name} {ActionName}; Priority: {priority}";
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        int hpHealed = ActorUnit.HealDamage(8, 3);

        // Update Text & Entity Info
        battle.mainTextbox.text = (hpHealed != 0) ? $"{ActorUnit.Name} healed {hpHealed} HP!" : 
                                                    $"{ActorUnit.Name} could not Heal! Not enough MP.";

        yield return null;
    }
}
