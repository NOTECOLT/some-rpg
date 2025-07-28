using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Regular non-equippable Inventory Items
/// </summary>`
public class Item : IStorable, ISerializable {
    public ItemData Data;

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
        return new SerializedItem() {
            ItemId = Data.itemid
        };
    }

    #endregion
}

[Serializable]
public class SerializedItem : ICloneable, IDeserializable, IStorable {
    public string ItemId;


    #region IDeserializable

    public ISerializable Deserialize() {
        if (GameStateMachine.Instance.Items.ContainsKey(ItemId)) {
            return new Item() {
                Data = GameStateMachine.Instance.Items[ItemId],
            };
        } else {
            Debug.LogWarning($"Weapon {this} does not exist in Weapons Dictionary! Cannot Deserialize Weapon data.");
            return new Item() {
                Data = null,
            };
        }
    }

    #endregion

    public object Clone() {
        return new SerializedItem() {
            ItemId = this.ItemId
        };
    }

    public string GetID() {
        throw new NotImplementedException();
    }

    public string GetName() {
        throw new NotImplementedException();
    }

    public Sprite GetSprite() {
        throw new NotImplementedException();
    }
}