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
        int damage = Mathf.CeilToInt(Mathf.Pow(attackingUnit.CurrentStats.attack, 2) / (1.5f*CurrentStats.defense) * damageModifier);

        int oldHP = CurrentStats.hitPoints;
        int newHP = (CurrentStats.hitPoints - damage < 0) ? 0 : CurrentStats.hitPoints - damage;
        CurrentStats.hitPoints = newHP;
        Object.GetComponent<EntityInfoUI>().SetHPBar(oldHP, newHP, BaseStats.hitPoints, ANIMATION_TIME);

        return damage;
    }
    
    /// <summary>
    /// Applies very simple heal formula
    /// </summary>
    /// <param name="baseHeal">Amount of HP to heal</param>
    /// <param name="modifier">Modifier multiplied to the base heal</param>
    /// <returns>Returns the amount of HP healed</returns>
    public int HealDamage(int baseHeal, int manaCost, float modifier = 1) {
        int heal = Mathf.CeilToInt(baseHeal * modifier);

        int oldHP = CurrentStats.hitPoints;
        int newHP = (CurrentStats.hitPoints + heal > BaseStats.hitPoints) ? BaseStats.hitPoints : CurrentStats.hitPoints + heal;
        CurrentStats.hitPoints = newHP;

        int oldMP = CurrentStats.manaPoints;
        int newMP = CurrentStats.manaPoints - manaCost;
        CurrentStats.manaPoints = newMP;

        Object.GetComponent<EntityInfoUI>().SetHPBar(oldHP, newHP, BaseStats.hitPoints, ANIMATION_TIME);
        Object.GetComponent<EntityInfoUI>().SetMPBar(oldMP, newMP, BaseStats.manaPoints, ANIMATION_TIME);
        return heal;
    }
}
