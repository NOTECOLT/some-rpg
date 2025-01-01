using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Action that is used to describe an attack, heal, or other battle effect
/// </summary>
[Serializable]
public class BattleAction {
    public BattleUnit TargetUnit;
    public BattleUnit ActorUnit;
    public ActionType ActionType;

    /// <summary>
    /// Defines which order in the action sequence an action will take place. Higher priority actions will move first.
    /// At the moment, priority is defined by the speed stat for the ActorUnit.
    /// </summary>
    public int priority;

    public BattleAction(BattleUnit targetUnit, ActionType actionType, BattleUnit actorUnit) {
        TargetUnit = targetUnit;
        ActionType = actionType;
        ActorUnit = actorUnit;


        priority = ActorUnit.CurrentStats.Speed;
    }

    public override string ToString() {
        return $"{ActorUnit.Name} {ActionType} on {TargetUnit.Name}; Priority: {priority}";
    }
}

public enum ActionType {
    NULL, 
    ATTACK
}