using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : BattleAction {
    public static BasicAttack NULL_ATTACK = new BasicAttack(null, null);
    public BasicAttack(BattleUnit targetUnit, BattleUnit actorUnit) : base("Basic Attack", targetUnit, actorUnit) {
    
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        int damageDealt = ActorUnit.CurrentStats.CalculateDamage(TargetUnit.CurrentStats);

        string battleText = "";

        // Generate QTE if the attacker is a player
        if (ActorUnit is not Enemy) {
            QuickTimeEvent QTE = new QuickTimeEvent(battle.qteButton);
            yield return QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, 0.2f, 0.4f);

            if (QTE.Result >= 0) {
                damageDealt *= 2; 
                battleText += "Critical hit! ";
            }
        }

        battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} for {damageDealt} damage!";
        battle.mainTextbox.text = battleText;

        TargetUnit.DealDamage(damageDealt);

        yield return null;
    }
}
