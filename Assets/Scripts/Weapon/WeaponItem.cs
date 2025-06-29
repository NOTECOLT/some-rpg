using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object that stores a weapon's data & stats. Stored in PlayerData structures. A serialized version of the class exists in savedata
/// </summary>
[Serializable]
public class WeaponItem : ICloneable {
    public Weapon Data;
    public WeaponStats CurrentStats;

    /// <summary>
    /// Add experience to weapon. Levels up the weapon if experience reaches threshold.
    /// </summary>
    public void AddExperience(int amount) {
        CurrentStats.AddExperience(amount, Data);
    }

    public object Clone() {
        return new WeaponItem() {
            Data = this.Data,
            CurrentStats = (WeaponStats)this.CurrentStats.Clone()
        };
    }
}
