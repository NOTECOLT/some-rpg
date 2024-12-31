using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Action that is used to describe an attack, heal, or other battle effect
/// </summary>
[Serializable]
public class BattleAction {
    public EntityStats TargetStats;
    public EntityStats ActorStats;
    public ActionType ActionType;

    public Enemy tempRef; // TODO: Remove this variable
    public BattleAction(EntityStats target, ActionType actionType, EntityStats actor) {
        TargetStats = target;
        ActionType = actionType;
        ActorStats = actor;
    }
}

public enum ActionType {
    NULL, 
    ATTACK
}