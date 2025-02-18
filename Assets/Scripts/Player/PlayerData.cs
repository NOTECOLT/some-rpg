using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Object that stores all player data information. Not a Monobehaviour and thus must be loaded using the PlayerDataManager
/// </summary>
[Serializable] 
public class PlayerData {
    [Serializable]
    public class MemberStats {
        // BaseStats only change in between battles, through level up or permanent status changes
        public EntityStats BaseStats = new EntityStats();
        
        // CurrentStats may change through status effects in battle
        public EntityStats CurrentStats = new EntityStats();
        public Weapon Weapon;

        public object Clone() {
            return new MemberStats() {
                BaseStats = (EntityStats)this.BaseStats.Clone(),
                CurrentStats = (EntityStats)this.CurrentStats.Clone(),
                Weapon = this.Weapon
            };
        }
    } 

    public List<MemberStats> PartyStats = new List<MemberStats>();
    public Vector3Int Cell = Vector3Int.zero;
    public Direction Direction = Direction.DOWN;

    public object Clone() {
        return new PlayerData() {
            PartyStats = new List<MemberStats>(this.PartyStats),
            Cell = new Vector3Int(this.Cell.x, this.Cell.y, this.Cell.z),
            Direction = this.Direction,
        };
    }
}
