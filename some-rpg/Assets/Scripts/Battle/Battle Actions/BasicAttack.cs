using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : BattleAction {
    private static float QTE_ACTIVE_TIME = 0.2f;
    private static float QTE_LEAD_TIME = 0.3f;
    public static BasicAttack NULL_ATTACK = new BasicAttack(null, null);
    public BasicAttack(BattleUnit targetUnit, BattleUnit actorUnit) : base("Basic Attack", targetUnit, actorUnit) {
    
    }

    protected override IEnumerator DoPlayerAction(BattleStateMachine battle) {
        int damageDealt = ActorUnit.CurrentStats.CalculateDamage(TargetUnit.CurrentStats);

        string battleText = "";

        // Generate QTE if the attacker is a player
        QuickTimeEvent QTE = new QuickTimeEvent(battle.qteButton);
        yield return QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, QTE_ACTIVE_TIME, QTE_LEAD_TIME);

        if (QTE.Result >= 0) {
            damageDealt *= 2; 
            battleText += "Critical hit! ";
        }

        battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} for {damageDealt} damage!";
        battle.mainTextbox.text = battleText;

        TargetUnit.DealDamage(damageDealt);

        yield return null;
    }

    protected override IEnumerator DoNonPlayerAction(BattleStateMachine battle) {
        int damageDealt = ActorUnit.CurrentStats.CalculateDamage(TargetUnit.CurrentStats);

        string battleText = "";

        // Generate QTE if the attacker is a player
        QuickTimeEvent QTE = new QuickTimeEvent(battle.qteButton);
        yield return QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, QTE_ACTIVE_TIME, QTE_LEAD_TIME);

        if (QTE.Result >= 0) {
            damageDealt = Mathf.CeilToInt(damageDealt * 0.7f); 
            battleText += "Partial Dodge! ";
        }

        battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} for {damageDealt} damage!";
        battle.mainTextbox.text = battleText;

        TargetUnit.DealDamage(damageDealt);

        yield return null;
    }
}
