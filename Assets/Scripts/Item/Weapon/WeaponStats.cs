using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class WeaponStats : ICloneable {
    public int Level = 1;

    /// <summary>
    /// Amount of XP the weapon has at each level
    /// </summary>
    public int LevelXP = 0;

    /// <summary>
    /// Amount of (cumulative) experience a weapon has.
    /// Cumulative -- counting all experience gained in each level together.
    /// </summary>
    public int CumulativeXP = 0;

    public WeaponStats() { }

    public WeaponStats(int level, int levelXP, int cumulativeXP) {
        this.Level = level;
        this.LevelXP = levelXP;
        this.CumulativeXP = cumulativeXP;
    }

    /// <summary>
    /// Add experience to weapon. Levels up the weapon if experience reaches threshold.
    /// </summary>
    public void AddExperience(int amount, WeaponData weapon) {
        if (Level >= weapon.Levels.Count) return;

        LevelXP = Mathf.Clamp(LevelXP + amount, 0, weapon.Levels[Level].Experience);
        CumulativeXP += amount;

        if (LevelXP >= weapon.Levels[Level].Experience) {
            // foreach (WeaponModifier modifier in weapon.Levels[Level].Modifiers) {
            //     Debug.Log($"effect: {modifier.Effect}");
            //     Modifiers.Add(modifier);
            // }

            Level += 1;
            LevelXP = 0;
        }
    }

    public object Clone() {
        // List<WeaponModifier> modifiers = new List<WeaponModifier>();
        // foreach (WeaponModifier modifier in modifiers) {
        //     modifiers.Add((WeaponModifier)modifier.Clone());
        // }

        return new WeaponStats() {
            Level = this.Level,
            LevelXP = this.LevelXP,
            CumulativeXP = this.CumulativeXP,
            // Modifiers = modifiers
        };
    }
}