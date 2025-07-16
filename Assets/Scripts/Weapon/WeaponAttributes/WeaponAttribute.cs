using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that holds information to how a weapon's stats are modified. Is stackable
/// </summary>
[Serializable]
public abstract class WeaponAttribute {
    // public string Name { get; protected set; }
    // public AttrType Type { get; protected set;  }
}

public enum AttrType {
    PASSIVE,
    ACTIVE
}


[Serializable]
public class HealAttr : WeaponAttribute {
    // new public string Name { get; protected set; } = "Heal";
    // new public AttrType Type { get; protected set; } = AttrType.ACTIVE;
    public int HP;
}