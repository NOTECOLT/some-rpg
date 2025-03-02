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

    public Weapon Weapon { get; protected set; }

    /// <summary>
    /// Backreference to the object that which a BattleUnit pertains to
    /// </summary>
    public GameObject Object;

    public BattleUnit(EntityStats baseStats, EntityStats currentStats, GameObject obj, string name, Weapon weapon) {
        BaseStats = (EntityStats)baseStats.Clone();
        CurrentStats = (EntityStats)currentStats.Clone();
        Weapon = weapon;
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
        int damage = Mathf.CeilToInt(Mathf.Pow(attackingUnit.CurrentStats.Attack, 2) / (1.5f*CurrentStats.Defense) * attackingUnit.Weapon.Attack * damageModifier);

        int oldHP = CurrentStats.HitPoints;
        int newHP = (CurrentStats.HitPoints - damage < 0) ? 0 : CurrentStats.HitPoints - damage;
        CurrentStats.HitPoints = newHP;
        Object.GetComponent<UnitInfoUI>().SetHPBar(oldHP, newHP, BaseStats.HitPoints, ANIMATION_TIME);
        
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

        Object.GetComponent<UnitInfoUI>().SetHPBar(oldHP, newHP, BaseStats.HitPoints, ANIMATION_TIME);
        Object.GetComponent<UnitInfoUI>().SetMPBar(oldMP, newMP, BaseStats.ManaPoints, ANIMATION_TIME);
        return heal;
    }
}
