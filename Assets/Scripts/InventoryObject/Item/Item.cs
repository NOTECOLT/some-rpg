using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Regular non-equippable Inventory Items
/// </summary>`
[Serializable]
public class Item : InventoryObject, IStorable, ISerializable, ICloneable {
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

    public object Clone() {
        return new Item() {
            Data = this.Data,
        };
    }
}

[Serializable]
public class SerializedItem : InventoryObject, ICloneable, IDeserializable {
    public string ItemId;


    #region IDeserializable

    public ISerializable Deserialize() {
        if (GameStateMachine.Instance.Items.ContainsKey(ItemId)) {
            return new Item() {
                Data = (ItemData)GameStateMachine.Instance.Items[ItemId],
            };
        } else {
            Debug.LogWarning($"Item {this} does not exist in Item Dictionary! Cannot Deserialize Item data.");
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
}