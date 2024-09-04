using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Object that holds all data pertaining to an enemy in a battle.
/// </summary>
[Serializable]
public class Enemy {
    public EnemyType EnemyType;
    public int TargetId { get; private set; } = -1;
    public int CurrentHitPoints { get; private set; }

    public void Instantiate(int targetId) {
        CurrentHitPoints = EnemyType.BaseHitPoints;
        TargetId = targetId;
    }
}
