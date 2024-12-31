using System;
using UnityEngine;

/// <summary>
/// Object that holds all data pertaining to an enemy in a battle.
/// </summary>
[Serializable]
public class Enemy : BattleUnit {
    public EnemyType EnemyType;
    
    public Enemy(EnemyType enemyType, GameObject obj) : base((EntityStats)enemyType.BaseStats.Clone(), obj, enemyType.EnemyName) {
        EnemyType = enemyType;
    }
}
