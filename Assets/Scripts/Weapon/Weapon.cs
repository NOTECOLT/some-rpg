using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Equippable items that will affect the type of attack the player will perform.
/// </summary>
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
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
    /// Additional Effects that are applied to a weapon upon attacking. The size of this list determines the maximum number of levels a weapon has. <br></br>
    /// A weapon with level = n means that all weapon effects within the list of index [0, n] are obtained. 
    /// </summary>
    public List<WeaponLevel> Levels = new List<WeaponLevel>();
}

/// <summary>
/// More used as a 'tuple class' to hold the amount of experience required to surpass a level and the effects associated with it
/// </summary>
[Serializable]
public class WeaponLevel {
    /// <summary>
    /// The amount of experience required to reach that level and obtain all weapon modifiers
    /// </summary>
    public int Experience = 20;

    // public WeaponAttribute GetModifier(string name) {
    //     WeaponAttribute ret = Attributes.FirstOrDefault(attr => attr.Name == name);
    //     if (ret != null)
    //         return ret;
    //     else
    //         throw new Exception($"Weapon Attribute {name} does not exist.");
    // }
    
    [SerializeReference, SubclassSelector]
    public List<WeaponAttribute> Attributes = new List<WeaponAttribute>();
}