using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all information pertaining to enemy encounter rates on overworld tiles.
/// </summary>
[Serializable]
public class EncounterWeight {
    public EnemyType EnemyType;
    public int Weight;
    /*
        EnemyTypes are given weights instead of rates
        Encounter Rate is determined by the ratio between individual weight and the total weight
    */
}
