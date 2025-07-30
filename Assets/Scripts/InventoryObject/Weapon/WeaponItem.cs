using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Inventory Item that stores Weapon data
/// </summary>
[Serializable]
public class WeaponItem : InventoryObject, IStorable, ISerializable, ICloneable {
    public WeaponData Data;
    public WeaponStats CurrentStats;

    public WeaponItem() { }
    public WeaponItem(WeaponData weapon) {
        Data = weapon;
        int level = 1;
        CurrentStats = new WeaponStats(level, 0, 0);
    }

    public WeaponQTE GetQTE() {
        WeaponAttribute attr = GetCurrentWeaponLevel().Attributes.FirstOrDefault(attr => attr is WeaponQTE);
        if (attr == null) {
            attr = Data.QTEAttribute;
        }

        if (attr is PressQTE) {
            return (PressQTE)attr;
        } else if (attr is MashQTE) {
            return (MashQTE)attr;
        } else if (attr is ReleaseQTE) {
            return (ReleaseQTE)attr;
        } else {
            return new PressQTE();
        }
    }

    public int GetWeaponAttack() {
        return GetCurrentWeaponLevel().Attack;
    }

    public WeaponLevel GetCurrentWeaponLevel() {
        return Data.Levels[CurrentStats.Level - 1];
    }

    public WeaponLevel GetNextWeaponLevel() {
        return Data.Levels[CurrentStats.Level];
    }

    public WeaponLevel GetWeaponLevel(int level) {
        if (level - 1 >= Data.Levels.Count || level - 1 < 0) {
            throw new Exception($"Level {level} is an invalid weapon for {Data.ItemName}");
        } else {
            return Data.Levels[level - 1];
        }
    }

    /// <summary>
    /// Add experience to weapon. Levels up the weapon if experience reaches threshold.
    /// </summary>
    public void AddExperience(int amount) {
        CurrentStats.AddExperience(amount, Data);
    }

    #region IStorable

    public string GetName() {
        return Data.ItemName;
    }

    public Sprite GetSprite() {
        return Data.Sprite;
    }

    public string GetID() {
        return Data.itemid;
    }

    #endregion

    #region ISerializable

    public IDeserializable Serialize() {
        return new SerializedWeaponItem() {
            ItemId = Data.itemid,
            CurrentStats = (WeaponStats)CurrentStats.Clone()
        };
            
        // if (GameStateMachine.Instance.Weapons.ContainsValue(Data)) {

        // } else {
        //     Debug.LogWarning($"Weapon {Data.WeaponName} does not exist in Weapons Dictionary! Cannot serialize Weapon data.");
        //     return new SerializedWeaponItem() {
        //         WeaponId = "",
        //         CurrentStats = (WeaponStats)CurrentStats.Clone()
        //     };
        // }
    }

    #endregion

    public object Clone() {
        return new WeaponItem() {
            Data = this.Data,
            CurrentStats = (WeaponStats)this.CurrentStats.Clone()
        };
    }

}

[Serializable]
public class SerializedWeaponItem : InventoryObject, ICloneable, IDeserializable {
    public string ItemId;
    public WeaponStats CurrentStats;

    #region IDeserializable

    public ISerializable Deserialize() {
        if (GameStateMachine.Instance.Items.ContainsKey(ItemId)) {
            return new WeaponItem() {
                Data = (WeaponData)GameStateMachine.Instance.Items[ItemId],
                CurrentStats = (WeaponStats)CurrentStats.Clone()
            };
        } else {
            Debug.LogWarning($"Weapon {this} does not exist in Item Dictionary! Cannot Deserialize Weapon data.");
            return new WeaponItem() {
                Data = null,
                CurrentStats = null
            };
        }
    }

    #endregion

    public object Clone() {
        return new SerializedWeaponItem() {
            ItemId = this.ItemId,
            CurrentStats = (WeaponStats)this.CurrentStats.Clone()
        };
    }
}