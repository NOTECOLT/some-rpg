using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Object that stores all player data information. Not a Monobehaviour and thus must be loaded using the PlayerDataManager
/// </summary>
[Serializable]
public class PlayerData : ISerializable {
    public List<PartyMember> PartyStats = new List<PartyMember>();
    public Vector3Int Cell = Vector3Int.zero;
    public Direction Direction = Direction.DOWN;
    public int ExperiencePool = 0;

    public List<IStorable> Inventory;

    #region ISerializable

    public IDeserializable Serialize() {
        SerializedPlayerData ret = new SerializedPlayerData();

        ret.PartyStats = new List<SerializedPartyMember>();
        foreach (PartyMember deserializedMember in this.PartyStats) {
            ret.PartyStats.Add((SerializedPartyMember)deserializedMember.Serialize());
        }

        ret.Cell = this.Cell;
        ret.Direction = this.Direction;
        ret.ExperiencePool = this.ExperiencePool;

        return ret;
    }

    #endregion
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

/// <summary>
/// Serialized version of PlayerData. PlayerData must be converted into this and vice versa when saving & loading
/// </summary>
[Serializable]
public class SerializedPlayerData : IDeserializable {
    public List<SerializedPartyMember> PartyStats = new List<SerializedPartyMember>();
    public Vector3Int Cell = Vector3Int.zero;
    public Direction Direction = Direction.DOWN;
    public int ExperiencePool = 0;
    public List<IStorable> Inventory;

    #region IDeserializable
    public ISerializable Deserialize() {
        PlayerData ret = new PlayerData();

        foreach (SerializedPartyMember serializedMember in PartyStats) {
            ret.PartyStats.Add((PartyMember)serializedMember.Deserialize());
        }

        ret.Cell = new Vector3Int(Cell.x, Cell.y, Cell.z);
        ret.Direction = Direction;
        ret.ExperiencePool = ExperiencePool;

        return ret;
    }
    
    #endregion
}