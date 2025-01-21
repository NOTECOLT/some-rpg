using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object that stores all player data information. Not a Monobehaviour and thus must be loaded using the PlayerDataManager
/// </summary>
[Serializable] 
public class PlayerData {
    // BaseStats only change in between battles, through level up or permanent status changes
    public EntityStats BaseStats = new EntityStats(); 

    // CurrentStats may change through status effects in battle
    public EntityStats CurrentStats = new EntityStats();
    public Vector3Int Cell = Vector3Int.zero;
    public Direction Direction = Direction.DOWN;
    public Weapon Weapon;
}
