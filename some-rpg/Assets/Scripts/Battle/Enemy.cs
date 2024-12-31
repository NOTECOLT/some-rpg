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
    public GameObject Object;

    // These stats may change in battle through status effects
    public EntityStats CurrentStats = new EntityStats();   

    public Enemy(EnemyType enemyType, GameObject target) {
        this.EnemyType = enemyType;
        CurrentStats = (EntityStats)EnemyType.BaseStats.Clone();

        this.Object = target;
    }
}
