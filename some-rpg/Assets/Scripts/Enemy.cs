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

    // These stats may change in battle through status effects
    public EntityStats CurrentStats { get; private set; } = new EntityStats();   

    public Enemy(EnemyType enemyType) {
        this.EnemyType = enemyType;
    }

    public void Instantiate(int targetId) {
        CurrentStats = (EntityStats)EnemyType.BaseStats.Clone();
        TargetId = targetId;
    }
}
