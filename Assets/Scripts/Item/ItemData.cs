using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory Items
/// Scriptable Object form that is not stored in savedata or serialized. Merely holds all relevant data to item.
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ItemData", order = 2)]
public class ItemData : ScriptableObject {
    public string ItemName;
    public Sprite Sprite;
}
