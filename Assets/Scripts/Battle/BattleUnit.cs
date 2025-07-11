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
    public event Action<int> OnLevelChange;

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
    public IEnumerator AddExperience(int xp) {
        int remainingXP = xp;

        // ? Kind of not really an elegant solution
        // Implementation takes into account gaining XP through multiple levels
        while (remainingXP > 0) {
            if (MemberData.Weapon.CurrentStats.Level >= MemberData.Weapon.Data.Levels.Count)
                yield break;

            int XPToLevelUp = MemberData.Weapon.Data.Levels[MemberData.Weapon.CurrentStats.Level].Experience - MemberData.Weapon.CurrentStats.LevelXP;

            if (XPToLevelUp > remainingXP) XPToLevelUp = remainingXP;
            remainingXP -= XPToLevelUp;

            int oldXP = MemberData.Weapon.CurrentStats.LevelXP;
            int newXP = MemberData.Weapon.CurrentStats.LevelXP + XPToLevelUp;
            int totalXP = MemberData.Weapon.Data.Levels[MemberData.Weapon.CurrentStats.Level].Experience;

            // Debug.Log($"{MemberData.Weapon.Data.name} {oldXP} {newXP} {totalXP}");

            OnXPChange?.Invoke(oldXP, newXP, totalXP, ANIMATION_TIME * ((float)XPToLevelUp / xp));
            MemberData.Weapon.AddExperience(XPToLevelUp);

            // Wait for animation to finish before snapping the XP bar to final fill ammount
            yield return new WaitForSeconds(ANIMATION_TIME * ((float)XPToLevelUp / xp) + 0.6f);
            OnLevelChange?.Invoke(MemberData.Weapon.CurrentStats.Level);

            if (MemberData.Weapon.CurrentStats.Level < MemberData.Weapon.Data.Levels.Count) {
                OnXPChange?.Invoke(0, MemberData.Weapon.CurrentStats.LevelXP, MemberData.Weapon.Data.Levels[MemberData.Weapon.CurrentStats.Level].Experience, 0);

            }
        }
    }

    public void RemoveAllListeners() {
        OnHPChange = null;
        OnMPChange = null;
        OnXPChange = null;
        OnLevelChange = null;
    }
}
