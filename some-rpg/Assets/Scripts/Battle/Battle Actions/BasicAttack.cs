using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : BattleAction {
    private static float QTE_ACTIVE_TIME = 0.2f;
    private static float QTE_LEAD_TIME = 0.3f;
    public static BasicAttack NULL_ATTACK = new BasicAttack(null, null);
    public BasicAttack(BattleUnit targetUnit, BattleUnit actorUnit) : base("Basic Attack", targetUnit, actorUnit) {
    
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        int damageDealt = ActorUnit.CurrentStats.CalculateDamage(TargetUnit.CurrentStats);
        string battleText = "";
        Vector3 originalActorPosition = ActorUnit.Object.transform.position;

        Vector3 targetPosition = (ActorUnit is Enemy) ? 
            ActorUnit.Object.transform.position + new Vector3(4, 0, 0) :
            ActorUnit.Object.transform.position - new Vector3(4, 0, 0);

        // QTE to perform critical hit, Move player towards target to perform animations
        QuickTimeEvent QTE = new QuickTimeEvent(battle.qteButton);
        battle.StartCoroutine(QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, QTE_ACTIVE_TIME, QTE_LEAD_TIME));
        battle.StartCoroutine(MoveTo(originalActorPosition, targetPosition, QTE_LEAD_TIME));
        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");

        yield return new WaitForSeconds(QTE_LEAD_TIME);

        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Attack");

        while (QTE.Result is null) yield return new WaitForSeconds(Time.deltaTime);

        if ((bool)QTE.Result) {
            if (ActorUnit is Enemy) {
                damageDealt = Mathf.CeilToInt(damageDealt * 0.7f); 
                battleText += "Partial Dodge! ";
            } else {
                damageDealt *= 2; 
                battleText += "Critical hit! ";
            }
        }
        
        if (ActorUnit.Object.GetComponent<Animator>() != null) {
            yield return new WaitForSeconds(ActorUnit.Object.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length);
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");
        }
            
        battle.StartCoroutine(MoveTo(targetPosition, originalActorPosition, QTE_LEAD_TIME));

        yield return new WaitForSeconds(QTE_LEAD_TIME);

        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Idle");
        battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} for {damageDealt} damage!";
        battle.mainTextbox.text = battleText;
        TargetUnit.DealDamage(damageDealt);

        yield return null;
    }

    private IEnumerator MoveTo(Vector3 startPosition, Vector3 finalPosition, float moveTime) {
        float currentTime = moveTime;

        while (currentTime > 0) {
            ActorUnit.Object.transform.position = Vector3.MoveTowards(ActorUnit.Object.transform.position, finalPosition, Vector3.Distance(startPosition, finalPosition)/moveTime * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
            currentTime -= Time.deltaTime;
        }
    }

    // protected override IEnumerator DoNonPlayerAction(BattleStateMachine battle) {
    //     int damageDealt = ActorUnit.CurrentStats.CalculateDamage(TargetUnit.CurrentStats);
    //     Vector3 originalActorPosition = ActorUnit.Object.transform.position;
    //     Vector3 targetPosition = ActorUnit.Object.transform.position + new Vector3(4, 0, 0);
    //     string battleText = "";

    //     // QTE to perform partial dodge
    //     QuickTimeEvent QTE = new QuickTimeEvent(battle.qteButton);
    //     battle.StartCoroutine(QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, QTE_ACTIVE_TIME, QTE_LEAD_TIME));
    //     battle.StartCoroutine(MoveTo(originalActorPosition, targetPosition, QTE_LEAD_TIME));
    //     if (ActorUnit.Object.GetComponent<Animator>() != null)
    //         ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");

    //     yield return new WaitForSeconds(QTE_LEAD_TIME);

    //     if (ActorUnit.Object.GetComponent<Animator>() != null)
    //         ActorUnit.Object.GetComponent<Animator>().SetTrigger("Attack");

    //     while (QTE.Result is null) yield return new WaitForSeconds(Time.deltaTime);

    //     if (ActorUnit.Object.GetComponent<Animator>() != null)
    //         ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");
    //     battle.StartCoroutine(MoveTo(targetPosition, originalActorPosition, QTE_LEAD_TIME));

    //     yield return new WaitForSeconds(QTE_LEAD_TIME);

    //     if (ActorUnit.Object.GetComponent<Animator>() != null)
    //     ActorUnit.Object.GetComponent<Animator>().SetTrigger("Idle");
    //     battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} for {damageDealt} damage!";
    //     battle.mainTextbox.text = battleText;
    //     TargetUnit.DealDamage(damageDealt);

    //     yield return null;
    // }
}
