using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equippable items that will affect the type of attack the player will perform.
/// </summary>
[CreateAssetMenu(fileName="Weapon", menuName="Weapon", order=1)]  
public class Weapon : ScriptableObject {
    private static float QTE_ACTIVE_TIME = 0.2f;
    private static float QTE_LEAD_TIME = 0.3f;
    public WeaponType Type;
    public string WeaponName;
    public Sprite Sprite;

    public IEnumerator AttackAction(BattleStateMachine battle, BasicAttack attack) {
        switch (Type) {
            case WeaponType.BLADE:
                yield return BladeAttack(battle, attack);
                break;
            default:
                break;
        }
    }

    private IEnumerator BladeAttack(BattleStateMachine battle, BasicAttack attack) {
        float modifier = 1;
        string battleText = "";
        Vector3 originalActorPosition = attack.ActorUnit.Object.transform.position;

        Vector3 targetPosition = (attack.ActorUnit is Enemy) ? 
            attack.ActorUnit.Object.transform.position + new Vector3(4, 0, 0) :
            attack.ActorUnit.Object.transform.position - new Vector3(4, 0, 0);

        // QTE to perform critical hit, Move unit towards target to perform animations
        QuickTimeEvent QTE = new QuickTimeEvent(battle.qteButton);
        battle.StartCoroutine(QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, QTE_ACTIVE_TIME, QTE_LEAD_TIME));
        battle.StartCoroutine(attack.MoveTo(originalActorPosition, targetPosition, QTE_LEAD_TIME));
        if (attack.ActorUnit.Object.GetComponent<Animator>() != null)
            attack.ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");

        yield return new WaitForSeconds(QTE_LEAD_TIME);

        // Perform Attack animation
        if (attack.ActorUnit.Object.GetComponent<Animator>() != null)
            attack.ActorUnit.Object.GetComponent<Animator>().SetTrigger("Attack");

        while (QTE.Result is null) yield return new WaitForSeconds(Time.deltaTime);

        // Check QTE
        if ((bool)QTE.Result) {
            if (attack.ActorUnit is Enemy) {
                modifier = 0.7f; 
                battleText += "Partial Dodge! ";
            } else {
                modifier = 2; 
                battleText += "Critical hit! ";
            }
        }
        
        // Move unit back
        if (attack.ActorUnit.Object.GetComponent<Animator>() != null) {
            yield return new WaitForSeconds(attack.ActorUnit.Object.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
            attack.ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");
        }
            
        battle.StartCoroutine(attack.MoveTo(targetPosition, originalActorPosition, QTE_LEAD_TIME));

        yield return new WaitForSeconds(QTE_LEAD_TIME);

        // Update Text & Entity Info
        if (attack.ActorUnit.Object.GetComponent<Animator>() != null)
            attack.ActorUnit.Object.GetComponent<Animator>().SetTrigger("Idle");
        int damage = attack.TargetUnit.DealDamage(attack.ActorUnit, modifier);
        battleText += $"{attack.ActorUnit.Name} attacked {attack.TargetUnit.Name} for {damage} damage!";
        battle.mainTextbox.text = battleText;

        yield return null;
    }
}

public enum WeaponType {
    BLADE,
    BLUNT,
    ROD,
    RANGED,
}
