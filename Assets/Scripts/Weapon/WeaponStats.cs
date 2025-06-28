using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class WeaponStats : ICloneable {
    public int Level = 1;

    /// <summary>
    /// Amount of (cumulative) experience a weapon has.
    /// Cumulative -- counting all experience gained in each level together.
    /// </summary>
    public float Experience = 0;

    public List<WeaponModifier> ActiveModifiers = new List<WeaponModifier>();

    /// <summary>
    /// Add experience to weapon. Levels up the weapon if experience reaches threshold.
    /// </summary>
    public void AddExperience(float amount, Weapon weapon) {
        Experience += amount;
        if (Level >= weapon.Levels.Count) return;

        if (Experience >= weapon.Levels[Level].Experience) {
            foreach (WeaponModifier modifier in weapon.Levels[Level].Modifiers)
                ActiveModifiers.Add(modifier);
            
            Level += 1;
        }
    }

    public object Clone() {
        List<WeaponModifier> activeModifiers = new List<WeaponModifier>();
        foreach (WeaponModifier modifier in activeModifiers) {
            activeModifiers.Add((WeaponModifier)modifier.Clone());
        }

        return new WeaponStats() {
            Level = this.Level,
            Experience = this.Experience,
            ActiveModifiers = activeModifiers
        };
    }
}