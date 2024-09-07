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
            Speed = this.Speed
        };
    }
}
