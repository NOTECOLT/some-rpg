using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class WeaponStats : ICloneable {
    public int Level = 1;
    public List<float> Experience = new List<float>();

    public object Clone() {
        return new WeaponStats() {
            Level = this.Level,
            Experience = new List<float>(this.Experience)
        };
    }
}