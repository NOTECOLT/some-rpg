using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object form that is not stored in savedata or serialized. Merely holds all relevant data to item.
/// Inventory Items, Weapons, are stored here
/// </summary>
public class InventoryObjectData : ScriptableObject {
    public string ItemName;
    public Sprite Sprite;

    /// <summary>
    /// A unique identifier used to reference to a certain item in scripts. Used for indexing in the ItemDatabase script
    /// </summary>
    // ? I'm not sure if I want to keep this a permanent solution or not, doesn't seem ideal but what do I know lol
    // ?   having the itemid be a string may make it prone spelling mistakes but it makes it more readable then integer ids
    public string itemid;
}
