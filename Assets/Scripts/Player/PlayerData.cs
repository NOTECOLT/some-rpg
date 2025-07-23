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
    public List<PartyMember> PartyStats = new List<PartyMember>();
    public Vector3Int Cell = Vector3Int.zero;
    public Direction Direction = Direction.DOWN;
    public int ExperiencePool = 0;

    public object Clone() {
        PlayerData pd = new PlayerData() {
            Cell = new Vector3Int(this.Cell.x, this.Cell.y, this.Cell.z),
            Direction = this.Direction,
            ExperiencePool = this.ExperiencePool,
        };

        List<PartyMember> ps = new List<PartyMember>();

        foreach (PartyMember member in this.PartyStats) {
            ps.Add((PartyMember)member.Clone());
        }

        pd.PartyStats = ps;

        return pd;
    }
}