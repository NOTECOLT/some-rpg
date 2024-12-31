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
    public EntityStats ActorUnit;
    public ActionType ActionType;

    public BattleAction(BattleUnit targetUnit, ActionType actionType, EntityStats actorUnit) {
        TargetUnit = targetUnit;
        ActionType = actionType;
        ActorUnit = actorUnit;
    }
}

public enum ActionType {
    NULL, 
    ATTACK
}