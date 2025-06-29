using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : BattleAction {
    private static float QTE_WINDOW = 0.2f;
    private static float QTE_MASH_WINDOW = 1.0f;
    private static float QTE_RELEASE_WINDOW = 0.3f;

    private static float QTE_LEAD_TIME = 0.3f;
    

    public BasicAttack(BattleUnit targetUnit, BattleUnit actorUnit) : base("Basic Attack", targetUnit, actorUnit) {
    
    }

    public override IEnumerator DoAction(BattleStateMachine battle) {
        Vector3 originalActorPosition = ActorUnit.Object.transform.position;

        Vector3 targetPosition = (ActorUnit is Enemy) ? 
            ActorUnit.Object.transform.position + new Vector3(4, 0, 0) :
            ActorUnit.Object.transform.position - new Vector3(4, 0, 0);

        // Setup weapon-specific QTEs to perform critical hit, 
        //  & Move unit towards target to perform animations
        QuickTimeEvent QTE;
        if (ActorUnit is Enemy) {
            QTE = new QuickTimeEvent(battle.qteButton, QTEType.PRESS);
        } else {
            QTE = new QuickTimeEvent(battle.qteButton, ActorUnit.WeaponItem.Data.QteType);
        }
        
        float qteWindow;
        switch (ActorUnit.WeaponItem.Data.QteType) {
            case QTEType.RELEASE:
                qteWindow = QTE_RELEASE_WINDOW;
                break;
            case QTEType.MASH:
                qteWindow = QTE_MASH_WINDOW;
                break;
            default:
                qteWindow = QTE_WINDOW;
                break;
        }

        battle.StartCoroutine(MoveTo(originalActorPosition, targetPosition, QTE_LEAD_TIME));

        for (int i = 0; i < ActorUnit.WeaponItem.Data.Hits; i++) {
            battle.StartCoroutine(QTE.GenerateQTE(new KeyCode[] {KeyCode.A, KeyCode.S}, qteWindow, QTE_LEAD_TIME));

            // Perform Attack animation
            if (ActorUnit.Object.GetComponent<Animator>() != null)
                ActorUnit.Object.GetComponent<Animator>().SetTrigger("Attack");

            yield return QTE.WaitForQTEFinish();

            // Perform weapon dependent battle effects
            switch (QTE.Type) {
                case QTEType.PRESS:
                    PressAttack(battle, QTE.Result);
                    break;
                case QTEType.MASH:
                    MashAttack(battle, QTE.Result);
                    break;
                case QTEType.RELEASE:
                    ReleaseAttack(battle, QTE.Result);
                    break;
                default:
                    break;
            }
            
            yield return new WaitForSeconds(0.5f);
        }

        // Move unit back
        yield return MoveTo(targetPosition, originalActorPosition, QTE_LEAD_TIME);
        
        yield return null;
    }

    public IEnumerator MoveTo(Vector3 startPosition, Vector3 finalPosition, float moveTime) {
        float currentTime = moveTime;

        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Walk");

        while (currentTime > 0) {
            ActorUnit.Object.transform.position = Vector3.MoveTowards(ActorUnit.Object.transform.position, finalPosition, Vector3.Distance(startPosition, finalPosition)/moveTime * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
            currentTime -= Time.deltaTime;
        }

        if (ActorUnit.Object.GetComponent<Animator>() != null)
            ActorUnit.Object.GetComponent<Animator>().SetTrigger("Idle");
    }

    // I don't like the idea that the logic for all basic attack types is held in this class but for now I'll keep it here
    // Originally wanted weapons to hold their own effect logic but at the moment I dont think thats possible
    // ?    Look into being able to serialize derived classes in json?
    // ?    I believe the problem lies in not being able to save and load weapon data for both scriptable object enemy types and player savedata
    #region Attacks Logic
    private void PressAttack(BattleStateMachine battle, int qteResult) {
        float damageModifier = 1;
        string battleText = "";
        // Check QTE
        if (qteResult == QuickTimeEvent.QTE_SUCCESS_RESULT) {
            if (ActorUnit is Enemy) {
                damageModifier = 0.7f;
                battleText = "Partial Dodge! ";
            } else {
                damageModifier = 1.5f;
                battleText = "Critical hit! ";
            }
        }

        // Update Text & Entity Info
        int damage = TargetUnit.DealDamage(ActorUnit, damageModifier);
        battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} using {ActorUnit.WeaponItem.Data.WeaponName} for {damage} damage!";
        battle.mainTextbox.text = battleText;
    }
    private void ReleaseAttack(BattleStateMachine battle, int qteResult) {
        float damageModifier;
        string battleText;
        // Check QTE
        if (qteResult == QuickTimeEvent.QTE_SUCCESS_RESULT) {
            if (ActorUnit is Enemy) {
                damageModifier = 0f;
                battleText = "Partial Dodge! ";
            } else {
                damageModifier = 2.5f;
                battleText = "Hit! ";
            }

            // Update Text & Entity Info
            int damage = TargetUnit.DealDamage(ActorUnit, damageModifier);
            battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} using {ActorUnit.WeaponItem.Data.WeaponName} for {damage} damage!";
        } else {
            battleText = $"Miss! {ActorUnit.Name} failed to attack {TargetUnit.Name}!";
        }

        battle.mainTextbox.text = battleText;
    }

    private void MashAttack(BattleStateMachine battle, int qteResult) {
        float damageModifier;
        string battleText = "";

        // Check QTE
        if (ActorUnit is Enemy) {
            damageModifier = 1 - Math.Clamp(Mathf.Log(qteResult, 4) - 1, 0, 0.5f);
        } else {
            damageModifier = 1 + Math.Clamp(Mathf.Log(qteResult, 4) - 1, 0, 0.5f);
        }

        // Update Text & Entity Info
        int damage = TargetUnit.DealDamage(ActorUnit, damageModifier);
        battleText += $"{ActorUnit.Name} attacked {TargetUnit.Name} using {ActorUnit.WeaponItem.Data.WeaponName} for {damage} damage!";
        battle.mainTextbox.text = battleText;
    }

    #endregion


    #region Additional Status Effects

    #endregion
}
