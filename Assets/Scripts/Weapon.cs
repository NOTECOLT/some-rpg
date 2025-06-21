using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equippable items that will affect the type of attack the player will perform.
/// </summary>
[CreateAssetMenu(fileName="Weapon", menuName="Weapon", order=1)]  
public class Weapon : ScriptableObject {
    public WeaponType WeaponType;
    public string WeaponName;

    /// <summary>
    /// A unique identifier used to reference to a certain weapon in scripts. Used for indexing in the ItemDatabase script
    /// </summary>
    // ? I'm not sure if I want to keep this a permanent solution or not, doesn't seem ideal but what do I know lol
    // ?   having the weaponid be a string may make it prone spelling mistakes but it makes it more readable then integer ids
    public string weaponid;
    public Sprite Sprite;
    public float Attack = 1;
    public QTEType QteType;
    [Min(1)] public int Hits = 1;
    
    // TODO: need an elegant way to implement current Level/experience and base experience
    /// <summary>
    /// Additional Effects that are applied to a weapon upon attacking. The size of this list determines the maximum number of levels
    /// </summary>
    public List<WeaponLevel> Levels = new List<WeaponLevel>();
}

[Serializable]
public class WeaponLevel {
    public int Experience = 20;
    public float AttackModifier = 1;
    public List<BattleEffect> Effects = new List<BattleEffect>();
}

public enum BattleEffect {
    HEAL,
    POISON
}