using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Regular non-equippable Inventory Items
/// </summary>
public class InventoryItem : IStorable {
    public ItemData Data;

    #region IStorable
    public string GetName() {
        return Data.ItemName;
    }

    public Sprite GetSprite() {
        return Data.Sprite;
    }

    #endregion
}
