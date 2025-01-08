using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : BattleAction {    
    public Heal(BattleUnit actorUnit) : base("Heal", null, actorUnit) {
    }

    public override string ToString() {
        return $"{ActorUnit.Name} {ActionName}; Priority: {priority}";
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        string battleText = "";
            
        int hpHealed = ActorUnit.HealDamage(5, 2);
        // Update Text & Entity Info
        battleText += $"{ActorUnit.Name} healed {hpHealed} HP!";
        battle.mainTextbox.text = battleText;

        yield return null;
    }
}
