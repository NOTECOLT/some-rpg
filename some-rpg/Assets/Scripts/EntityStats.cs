using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object to store either enemy or player statistics
/// </summary>
[Serializable]
public class EntityStats {
    public int HitPoints;
    public int Attack;
    public int Defense;
    public int Speed;

    public EntityStats() {
        
    }
}
