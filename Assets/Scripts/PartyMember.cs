using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PartyMember {
    /// <summary>
    /// These stats do not change in battle. BaseStats only change in between battles, through level up or permanent status changes
    /// </summary>
    public EntityStats BaseStats = new EntityStats();
    
    /// <summary>
    /// These stats may change in battle through status effects
    /// </summary>
    public EntityStats CurrentStats = new EntityStats();
    public string Name;
    public WeaponItem Weapon;

    public object Clone() {
        return new PartyMember() {
            BaseStats = (EntityStats)this.BaseStats.Clone(),
            CurrentStats = (EntityStats)this.CurrentStats.Clone(),
            Weapon = (WeaponItem)this.Weapon.Clone(),
            Name = this.Name
        };
    }
} 