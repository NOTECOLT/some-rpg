using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that holds information to how a weapon's stats are modified. Is stackable
/// </summary>
[Serializable]
public class WeaponModifier : ICloneable {
    public EffectType Effect;

    public WeaponAttribute GetAttribute(string name) {
        WeaponAttribute ret = Attributes.FirstOrDefault(attr => attr.name == name);
        if (ret != null)
            return ret;
        else
            throw new Exception($"Weapon Attribute {name} does not exist.");
    }

    public List<WeaponAttribute> Attributes = new List<WeaponAttribute>();

    public object Clone() {
        List<WeaponAttribute> attributes = new List<WeaponAttribute>();
        foreach (WeaponAttribute attribute in attributes) {
            attributes.Add((WeaponAttribute)attribute.Clone());
        }

        return new WeaponModifier() {
            Effect = this.Effect,
            Attributes = attributes
        };
    }
}

public enum EffectType {
    HEAL
}

[Serializable]
public class WeaponAttribute : ICloneable {
    public string name;
    public float value;

    public object Clone() {
        return new WeaponAttribute() {
            name = this.name,
            value = this.value
        };
    }
}