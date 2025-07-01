using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class Heal : BattleAction {
    private int _amount;
    private int _cost;
    public Heal(BattleUnit actorUnit, int amount, int cost) : base("Heal", actorUnit, actorUnit) {
        _amount = amount;
        _cost = cost;
    }

    public override string ToString() {
        return $"{ActorUnit.Name} {ActionName}; Priority: {priority}";
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        int hpHealed = ActorUnit.HealDamage(_amount, _cost);

        // Update Text & Entity Info
        battle.mainTextbox.text = (hpHealed != 0) ? $"{ActorUnit.Name} healed {hpHealed} HP!" : 
                                                    $"{ActorUnit.Name} could not Heal! Not enough MP.";

        yield return null;
    }
}
