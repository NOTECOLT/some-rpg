using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object to store either enemy or player statistics
/// </summary>
[Serializable]
public class EntityStats : ICloneable {
    public int HitPoints;
    public int ManaPoints;
    public int Attack;
    public int Defense;
    public int Speed;

    public EntityStats() {
        
    }

    public object Clone() {
        return new EntityStats() {
            HitPoints = this.HitPoints,
            Attack = this.Attack,
            Defense = this.Defense,
            Speed = this.Speed,
        };
    }

    /// <summary>
    /// This function contains the damage formula on a battle unit and applies it on an entity's CurrentStats
    /// </summary>
    /// <param name="defendingStats"></param>
    /// <returns></returns>
    public static int CalculateDamage(EntityStats attackingStats, EntityStats defendingStats) {
        float dmg = Mathf.Pow(attackingStats.Attack, 2) / (1.5f*defendingStats.Defense);

        return Mathf.RoundToInt(dmg);
    }
}
