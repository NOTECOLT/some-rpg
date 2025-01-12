using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object to store either enemy or player statistics
/// </summary>
[Serializable]
public class EntityStats : ICloneable {
    public int hitPoints;
    public int manaPoints;
    public int attack;
    public int defense;
    public int speed;

    public EntityStats() {
        
    }

    public object Clone() {
        return new EntityStats() {
            hitPoints = this.hitPoints,
            manaPoints = this.manaPoints,
            attack = this.attack,
            defense = this.defense,
            speed = this.speed,
        };
    }
}
