using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Action that is used to describe an attack, heal, or other battle effect
/// </summary>
[Serializable]
public class BattleAction {
    public BattleUnit TargetUnit { get; protected set; }
    public BattleUnit ActorUnit { get; protected set; }
    public string ActionName { get; protected set; }

    /// <summary>
    /// Defines which order in the action sequence an action will take place. Higher priority actions will move first.
    /// At the moment, priority is defined by the speed stat for the ActorUnit.
    /// </summary>
    public int priority;

    public BattleAction(string actionName, BattleUnit targetUnit, BattleUnit actorUnit) {
        ActionName = actionName;
        TargetUnit = targetUnit;
        ActorUnit = actorUnit;

        priority = (ActorUnit is not null) ? ActorUnit.CurrentStats.speed : 0;
    }

    public override string ToString() {
        return $"{ActorUnit.Name} {ActionName} on {TargetUnit.Name}; Priority: {priority}";
    }

    /// <summary>
    /// This coroutine performs all the calculations, effects, and animations. Must be overrided by any battle action that inherits from it.
    /// This function is public-facing and redirects to either DoPlayerAction or DoNonPlayerAction depending on the ActorUnit
    /// </summary>
    public virtual IEnumerator DoAction(BattleStateMachine battle) {
        Debug.LogWarning($"Action {ActionName} 'DoPlayerAction' has not been implemented!");
        yield return null;
    }
}