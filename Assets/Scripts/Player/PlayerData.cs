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

    [SerializeReference, SubclassSelector] public List<InventoryObject> Inventory;

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

        List<SerializedItem> items = new List<SerializedItem>();
        List<SerializedWeaponItem> weapons = new List<SerializedWeaponItem>();

        foreach (InventoryObject obj in Inventory) {
            if (obj is Item) {
                SerializedItem i = (SerializedItem)((Item)obj).Serialize();
                items.Add(i);
            } else if (obj is WeaponItem) {
                SerializedWeaponItem i = (SerializedWeaponItem)((WeaponItem)obj).Serialize(); 
                weapons.Add(i);
            }
        }

        ret.Items = items;
        ret.WeaponItems = weapons;

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

        List<InventoryObject> inv = new List<InventoryObject>();
        foreach (InventoryObject i in this.Inventory) {
            if (i is Item) {
                inv.Add((Item)((Item)i).Clone());
            } else if (i is WeaponItem) {
                inv.Add((WeaponItem)((WeaponItem)i).Clone());
            }
        }

        pd.Inventory = inv;

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
    public List<SerializedItem> Items;
    public List<SerializedWeaponItem> WeaponItems;

    #region IDeserializable
    public ISerializable Deserialize() {
        PlayerData ret = new PlayerData();

        foreach (SerializedPartyMember serializedMember in PartyStats) {
            ret.PartyStats.Add((PartyMember)serializedMember.Deserialize());
        }

        ret.Cell = new Vector3Int(Cell.x, Cell.y, Cell.z);
        ret.Direction = Direction;
        ret.ExperiencePool = ExperiencePool;

        List<InventoryObject> inventory = new List<InventoryObject>();

        foreach (SerializedItem item in Items) {
            if (GameStateMachine.Instance.Items.ContainsKey(item.ItemId)) {
                return new Item() {
                    Data = (ItemData)GameStateMachine.Instance.Items[item.ItemId],
                };
            } else {
                Debug.LogWarning($"Item {this} does not exist in Items Dictionary! Cannot Deserialize Item data.");
                return new Item() {
                    Data = null,
                };
            }
        }

        foreach (SerializedWeaponItem weapon in WeaponItems) {
            if (GameStateMachine.Instance.Items.ContainsKey(weapon.ItemId)) {
                return new WeaponItem() {
                    Data = (WeaponData)GameStateMachine.Instance.Items[weapon.ItemId],
                };
            } else {
                Debug.LogWarning($"Weapon {this} does not exist in Items Dictionary! Cannot Deserialize Weapon data.");
                return new WeaponItem() {
                    Data = null,
                };
            }
        }

        ret.Inventory = inventory;

        return ret;
    }
    
    #endregion
}