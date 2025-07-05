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

    /// <summary>
    /// Container property that holds all relevant stats, weapons, etc.
    /// I store a PartyMember property instead of keeping all of the nested properties within the BattleUnit class itself for easier compatibility with classes that already interact with the PartyMember class
    /// Anyway, at the moment they store all of the same data anyway.
    /// </summary>
    public PartyMember MemberData;

    /// <summary>
    /// Backreference to the object that which a BattleUnit pertains to
    /// </summary>
    public GameObject Object;

    public event Action<int, int, int, float> OnHPChange;
    public event Action<int, int, int, float> OnMPChange;
    public event Action<int, int, int, float> OnXPChange;

    public BattleUnit(EntityStats baseStats, EntityStats currentStats, GameObject obj, string name, WeaponItem weaponItem) {
        MemberData = new PartyMember() {
            BaseStats = (EntityStats)baseStats.Clone(),
            CurrentStats = (EntityStats)currentStats.Clone(),
            Weapon = weaponItem,
            Name = name
        };

        Object = obj;
    }

    public BattleUnit(PartyMember partyMemberData, GameObject obj) {
        MemberData = (PartyMember)partyMemberData.Clone();

        Object = obj;
    }

    /// <summary>
    /// Applies calculated damage onto an enemy's CurrentStats.
    /// Contains Damage Formula.
    /// </summary>
    /// <param name="damageModifier">Damage Modifier is multiplied to the calculated damage</param>
    /// <returns>Returns the damage dealt as integer</returns>
    public int DealDamage(BattleUnit attackingUnit, float damageModifier = 1) {
        int damage = Mathf.CeilToInt(Mathf.Pow(attackingUnit.MemberData.CurrentStats.Attack, 2) / (1.5f * MemberData.CurrentStats.Defense) * attackingUnit.MemberData.Weapon.Data.Attack * damageModifier);

        int oldHP = MemberData.CurrentStats.HitPoints;
        int newHP = (MemberData.CurrentStats.HitPoints - damage < 0) ? 0 : MemberData.CurrentStats.HitPoints - damage;
        MemberData.CurrentStats.HitPoints = newHP;
        
        OnHPChange?.Invoke(oldHP, newHP, MemberData.BaseStats.HitPoints, ANIMATION_TIME);

        return damage;
    }

    /// <summary>
    /// Applies very simple heal formula
    /// </summary>
    /// <param name="baseHeal">Amount of HP to heal</param>
    /// <param name="modifier">Modifier multiplied to the base heal</param>
    /// <returns>Returns the amount of HP healed</returns>
    public int HealDamage(int baseHeal, int manaCost, float modifier = 1) {
        if (MemberData.CurrentStats.ManaPoints < manaCost) return 0;
        int heal = Mathf.CeilToInt(baseHeal * modifier);

        int oldHP = MemberData.CurrentStats.HitPoints;
        int newHP = (MemberData.CurrentStats.HitPoints + heal > MemberData.BaseStats.HitPoints) ? MemberData.BaseStats.HitPoints : MemberData.CurrentStats.HitPoints + heal;
        MemberData.CurrentStats.HitPoints = newHP;

        int oldMP = MemberData.CurrentStats.ManaPoints;
        int newMP = MemberData.CurrentStats.ManaPoints - manaCost;
        MemberData.CurrentStats.ManaPoints = newMP;


        OnHPChange?.Invoke(oldHP, newHP, MemberData.BaseStats.HitPoints, ANIMATION_TIME);
        OnMPChange?.Invoke(oldMP, newMP, MemberData.BaseStats.ManaPoints, ANIMATION_TIME);
        return heal;
    }

    /// <summary>
    /// 'wrapper function' for adding experience, calls the add experience function of the weapon
    /// </summary>
    public void AddExperience(int xp) {
        int oldXP = MemberData.Weapon.CurrentStats.LevelXP;
        int newXP = MemberData.Weapon.CurrentStats.LevelXP + xp;

        int totalXP;
        try {
            totalXP = MemberData.Weapon.Data.Levels[MemberData.Weapon.CurrentStats.Level].Experience;
        } catch (ArgumentOutOfRangeException) {
            totalXP = MemberData.Weapon.CurrentStats.LevelXP;
        }
        
        if (totalXP == 0 && newXP == 0) {
            totalXP = 1;
            newXP = 1;
        }
        
        OnXPChange?.Invoke(oldXP, newXP, totalXP, ANIMATION_TIME);
        MemberData.Weapon.AddExperience(xp);
    }

    public void RemoveAllListeners() {
        OnHPChange = null;
        OnMPChange = null;
        OnXPChange = null;
    }
}
