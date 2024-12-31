using System;
using UnityEngine;

/// <summary>
/// Object that holds all data pertaining to an enemy in a battle.
/// </summary>
[Serializable]
public class Enemy : BattleUnit {
    public EnemyType EnemyType;
    
    public Enemy(EnemyType enemyType, GameObject obj) {
        this.EnemyType = enemyType;

        Name = enemyType.EnemyName;
        BaseStats = (EntityStats)enemyType.BaseStats.Clone();
        CurrentStats = (EntityStats)enemyType.BaseStats.Clone();
        this.Object = obj;
    }
}
