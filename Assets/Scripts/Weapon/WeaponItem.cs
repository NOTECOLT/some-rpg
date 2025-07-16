using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Object that stores a weapon's data & stats. Stored in PlayerData structures. A serialized version of the class exists in savedata
/// </summary>
[Serializable]
public class WeaponItem : ICloneable {
    public Weapon Data;
    public WeaponStats CurrentStats;

    public WeaponItem() { }
    public WeaponItem(Weapon weapon) {
        Data = weapon;
        int level = 1;
        CurrentStats = new WeaponStats(level, 0, 0);
    }

    public WeaponQTE GetQTE() {
        WeaponAttribute attr = GetCurrentWeaponLevel().Attributes.FirstOrDefault(attr => attr is WeaponQTE);
        if (attr == null) {
            attr = Data.QTEAttribute;
        }

        if (attr is PressQTE) {
            return (PressQTE)attr;
        } else if (attr is MashQTE) {
            return (MashQTE)attr;
        } else if (attr is ReleaseQTE) {
            return (ReleaseQTE)attr;
        } else {
            return new PressQTE();
        }
    }

    public int GetWeaponAttack() {
        return GetCurrentWeaponLevel().Attack;
    }

    public WeaponLevel GetCurrentWeaponLevel() {
        return Data.Levels[CurrentStats.Level - 1];
    }

    public WeaponLevel GetNextWeaponLevel() {
        return Data.Levels[CurrentStats.Level];
    }

    public WeaponLevel GetWeaponLevel(int level) {
        if (level - 1 >= Data.Levels.Count || level - 1 < 0) {
            throw new Exception($"Level {level} is an invalid weapon for {Data.WeaponName}");
        } else {
            return Data.Levels[level - 1];
        }
    }

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
