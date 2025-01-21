using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : BattleAction {
    private static float QTE_ACTIVE_TIME = 0.2f;
    private static float QTE_LEAD_TIME = 0.3f;
    public BasicAttack(BattleUnit targetUnit, BattleUnit actorUnit) : base("Basic Attack", targetUnit, actorUnit) {
    
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        Vector3 originalActorPosition = ActorUnit.Object.transform.position;

        Vector3 targetPosition = (ActorUnit is Enemy) ? 
            ActorUnit.Object.transform.position + new Vector3(4, 0, 0) :
            ActorUnit.Object.transform.position - new Vector3(4, 0, 0);

        // QTE to perform critical hit, Move unit towards target to perform animations
        QuickTimeEvent QTE = new QuickTimeEvent(battle.qteButton);
        battle.StartCoroutine(QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, QTE_ACTIVE_TIME, QTE_LEAD_TIME));
        battle.StartCoroutine(MoveTo(originalActorPosition, targetPosition, QTE_LEAD_TIME));
        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");

        yield return new WaitForSeconds(QTE_LEAD_TIME);

        // Perform Attack animation
        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Attack");

        while (QTE.Result is null) yield return new WaitForSeconds(Time.deltaTime);
        
        // Move unit back
        if (ActorUnit.Object.GetComponent<Animator>() != null) {
            yield return new WaitForSeconds(ActorUnit.Object.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");
        }
            
        yield return MoveTo(targetPosition, originalActorPosition, QTE_LEAD_TIME);

        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Idle");

        // Perform weapon dependent battle effects
        switch (ActorUnit.Weapon.Type) {
            case WeaponType.BLADE:
                BladeAttack(battle, (bool)QTE.Result);
                break;
            case WeaponType.ROD:
                RodAttack(battle, (bool)QTE.Result);
                break;
            case WeaponType.RANGED:
                RangedAttack(battle, (bool)QTE.Result);
                break;
            case WeaponType.BLUNT:
                BluntAttack(battle, (bool)QTE.Result);
                break;
            default:
                break;
        }

        yield return null;
    }

    public IEnumerator MoveTo(Vector3 startPosition, Vector3 finalPosition, float moveTime) {
        float currentTime = moveTime;

        while (currentTime > 0) {
            ActorUnit.Object.transform.position = Vector3.MoveTowards(ActorUnit.Object.transform.position, finalPosition, Vector3.Distance(startPosition, finalPosition)/moveTime * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
            currentTime -= Time.deltaTime;
        }
    }

    // I don't like the idea that the logic for all basic attack types is held in this class but for now I'll keep it here
    // Originally wanted weapons to hold their own effect logic but at the moment I dont think thats possible
    // ?    Look into being able to serialize derived classes in json?
    // ?    I believe the problem lies in not being able to save and load weapon data for both scriptable object enemy types and player savedata
    #region Attacks Logic
    private void BladeAttack(BattleStateMachine battle, bool qteResult) {
        float damageModifier = 1;
        string battleText = "";
        // Check QTE
        if (qteResult) {
            if (ActorUnit is Enemy) {
                damageModifier = 0.7f;
                battleText = "Partial Dodge! ";
            } else {
                damageModifier = 2;
                battleText = "Critical hit! ";
            }
        }

        // Update Text & Entity Info
        int damage = TargetUnit.DealDamage(ActorUnit, damageModifier);
        battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} using {ActorUnit.Weapon.WeaponName} for {damage} damage!";
        battle.mainTextbox.text = battleText;
    }

    private void RodAttack(BattleStateMachine battle, bool qteResult) {
        Debug.LogError("RodAttack effect not implemented!");
    }

    private void RangedAttack(BattleStateMachine battle, bool qteResult) {
        float damageModifier = 1;
        string battleText = "";
        // Check QTE
        if (qteResult) {
            if (ActorUnit is Enemy) {
                damageModifier = 0f;
                battleText = "Partial Dodge! ";
            } else {
                damageModifier = 2.5f;
                battleText = "Hit! ";
            }

            // Update Text & Entity Info
            int damage = TargetUnit.DealDamage(ActorUnit, damageModifier);
            battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} using {ActorUnit.Weapon.WeaponName} for {damage} damage!";
        } else {
            battleText = $"Miss! {ActorUnit.Name} failed to attack {TargetUnit.Name}!";
        }

        battle.mainTextbox.text = battleText;
    }

    private void BluntAttack(BattleStateMachine battle, bool qteResult) {
        Debug.LogError("BluntAttack effect not implemented!");
    }

    #endregion


    #region Additional Status Effects

    #endregion
}
