using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class from which enemies, bosses, and players will inherit from during battles
/// </summary>
[Serializable]
public class BattleUnit {
    private static float ANIMATION_TIME = 0.3f; // HP animation time in seconds

    public string Name { get; protected set; }
    /// <summary>
    /// These stats do not change in battle
    /// </summary>
    public EntityStats BaseStats { get; protected set; } = new EntityStats();
    /// <summary>
    /// These stats may change in battle through status effects
    /// </summary>
    public EntityStats CurrentStats { get; set; } = new EntityStats();

    public WeaponItem WeaponItem { get; protected set; }

    /// <summary>
    /// Backreference to the object that which a BattleUnit pertains to
    /// </summary>
    public GameObject Object;

    public event Action<int, int, int, float> OnHPChange;
    public event Action<int, int, int, float> OnMPChange;
    public event Action<int, int, int, float> OnXPChange;

    public BattleUnit(EntityStats baseStats, EntityStats currentStats, GameObject obj, string name, WeaponItem weaponItem) {
        BaseStats = (EntityStats)baseStats.Clone();
        CurrentStats = (EntityStats)currentStats.Clone();
        WeaponItem = weaponItem;
        Object = obj;
        Name = name;
    }

    /// <summary>
    /// Applies calculated damage onto an enemy's CurrentStats.
    /// Contains Damage Formula.
    /// </summary>
    /// <param name="damageModifier">Damage Modifier is multiplied to the calculated damage</param>
    /// <returns>Returns the damage dealt as integer</returns>
    public int DealDamage(BattleUnit attackingUnit, float damageModifier = 1) {
        int damage = Mathf.CeilToInt(Mathf.Pow(attackingUnit.CurrentStats.Attack, 2) / (1.5f * CurrentStats.Defense) * attackingUnit.WeaponItem.Data.Attack * damageModifier);

        int oldHP = CurrentStats.HitPoints;
        int newHP = (CurrentStats.HitPoints - damage < 0) ? 0 : CurrentStats.HitPoints - damage;
        CurrentStats.HitPoints = newHP;
        OnHPChange?.Invoke(oldHP, newHP, BaseStats.HitPoints, ANIMATION_TIME);

        return damage;
    }

    /// <summary>
    /// Applies very simple heal formula
    /// </summary>
    /// <param name="baseHeal">Amount of HP to heal</param>
    /// <param name="modifier">Modifier multiplied to the base heal</param>
    /// <returns>Returns the amount of HP healed</returns>
    public int HealDamage(int baseHeal, int manaCost, float modifier = 1) {
        if (CurrentStats.ManaPoints < manaCost) return 0;
        int heal = Mathf.CeilToInt(baseHeal * modifier);

        int oldHP = CurrentStats.HitPoints;
        int newHP = (CurrentStats.HitPoints + heal > BaseStats.HitPoints) ? BaseStats.HitPoints : CurrentStats.HitPoints + heal;
        CurrentStats.HitPoints = newHP;

        int oldMP = CurrentStats.ManaPoints;
        int newMP = CurrentStats.ManaPoints - manaCost;
        CurrentStats.ManaPoints = newMP;


        OnHPChange?.Invoke(oldHP, newHP, BaseStats.HitPoints, ANIMATION_TIME);
        OnMPChange?.Invoke(oldMP, newMP, BaseStats.ManaPoints, ANIMATION_TIME);
        return heal;
    }

    /// <summary>
    /// 'wrapper function' for adding experience, calls the add experience function of the weapon
    /// </summary>
    public void AddExperience(int xp) {
        int oldXP = WeaponItem.CurrentStats.LevelXP;
        int newXP = WeaponItem.CurrentStats.LevelXP + xp;

        int totalXP;
        try {
            totalXP = WeaponItem.Data.Levels[WeaponItem.CurrentStats.Level].Experience;
        } catch (ArgumentOutOfRangeException) {
            totalXP = WeaponItem.CurrentStats.LevelXP;
        }
        
        if (totalXP == 0 && newXP == 0) {
            totalXP = 1;
            newXP = 1;
        }

        OnXPChange?.Invoke(oldXP, newXP, totalXP, ANIMATION_TIME);
        WeaponItem.AddExperience(xp);
    }

    public void RemoveAllListeners() {
        OnHPChange = null;
        OnMPChange = null;
        OnXPChange = null;
    }
}
