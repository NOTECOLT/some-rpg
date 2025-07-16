using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicAttack : BattleAction {
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
            QTE = new QuickTimeEvent(battle.qteButton, new PressQTE());
        } else {
            QTE = new QuickTimeEvent(battle.qteButton, ActorUnit.MemberData.Weapon.GetQTE());
        }

        WeaponQTE qteAttribute = ActorUnit.MemberData.Weapon.GetQTE();

        battle.StartCoroutine(MoveTo(originalActorPosition, targetPosition, qteAttribute.GetLeadTime()));

        /// <summary>
        /// Counter only used for press QTEs
        /// </summary>
        int failAllowance = 0;
        for (int i = 0; i < qteAttribute.Hits; i++) {
            // The QTE itself
            battle.StartCoroutine(QTE.GenerateQTE(new KeyCode[] { KeyCode.A, KeyCode.S }, qteAttribute.GetWindowTime(), qteAttribute.GetLeadTime()));

            // Perform Attack animation
            if (ActorUnit.Object.GetComponent<Animator>() != null)
                ActorUnit.Object.GetComponent<Animator>().SetTrigger("Attack");

            yield return QTE.WaitForQTEFinish();

            // Perform Attack itself
            if (QTE.Type is PressQTE) {
                int QTEResult = QTE.Result;

                if (QTEResult != QuickTimeEvent.QTE_SUCCESS_RESULT && failAllowance < ((PressQTE)qteAttribute).FailAllowance) {
                    failAllowance++;
                    QTEResult = QuickTimeEvent.QTE_SUCCESS_RESULT;
                }
                PressAttack(battle, QTEResult);
            } else if (QTE.Type is MashQTE) {
                MashAttack(battle, QTE.Result + ((MashQTE)qteAttribute).MashHitBonus);
            } else if (QTE.Type is ReleaseQTE) {
                ReleaseAttack(battle, QTE.Result);
            }

            // Perform Weapon Battle Effects (Done by adding actions to the queue)
            foreach (WeaponAttribute attr in ActorUnit.MemberData.Weapon.GetWeaponLevel(ActorUnit.MemberData.Weapon.CurrentStats.Level).Attributes) {
                // if (attr.Type != AttrType.ACTIVE) continue;

                if (attr is HealAttr) {
                    HealAttr healAttr = (HealAttr)attr;
                    battle.PushBattleActionToNext(new Heal(ActorUnit, healAttr.HP, 0));
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Move unit back
        yield return MoveTo(targetPosition, originalActorPosition, qteAttribute.GetLeadTime());
        
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
        battleText += $"{ActorUnit.MemberData.Name} attacked {TargetUnit.MemberData.Name} using {ActorUnit.MemberData.Weapon.Data.WeaponName} for {damage} damage!";
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
            battleText += $"{ActorUnit.MemberData.Name} attacked {TargetUnit.MemberData.Name} using {ActorUnit.MemberData.Weapon.Data.WeaponName} for {damage} damage!";
        } else {
            battleText = $"Miss! {ActorUnit.MemberData.Name} failed to attack {TargetUnit.MemberData.Name}!";
        }

        battle.mainTextbox.text = battleText;
    }

    private void MashAttack(BattleStateMachine battle, int qteResult) {
        float damageModifier;
        string battleText = "";

        // Check QTE
        if (ActorUnit is Enemy) {
            damageModifier = 1 - Math.Clamp(Mathf.Log(qteResult, 5) - 1, 0, 0.5f);
        } else {
            damageModifier = 1 + Math.Clamp(Mathf.Log(qteResult, 5) - 1, 0, 0.5f);
        }

        // Update Text & Entity Info
        int damage = TargetUnit.DealDamage(ActorUnit, damageModifier);
        battleText += $"{ActorUnit.MemberData.Name} attacked {TargetUnit.MemberData.Name} using {ActorUnit.MemberData.Weapon.Data.WeaponName} for {damage} damage!";
        battle.mainTextbox.text = battleText;
    }

    #endregion


    #region Additional Status Effects

    #endregion
}
