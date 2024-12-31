using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract Class from which enemies, bosses, and players will inherit from during battles
/// </summary>
[Serializable]
public abstract class BattleUnit {
    public string Name { get; protected set; }
    /// <summary>
    /// These stats do not change in battle
    /// </summary>
    public EntityStats BaseStats { get; protected set; } = new EntityStats();

    /// <summary>
    /// These stats may change in battle through status effects
    /// </summary>
    public EntityStats CurrentStats { get; set; }= new EntityStats();   

    /// <summary>
    /// Reference to the GameObject that the BattleUnit pertains to
    /// </summary>
    public GameObject Object;
}
