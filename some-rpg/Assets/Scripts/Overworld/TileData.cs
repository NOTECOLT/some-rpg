using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that holds data for special tiles. Is filled with some case-specific rules and can be filled with more generic flags as well
/// </summary>
[CreateAssetMenu(fileName="TileData", menuName="TileData", order=3)]
public class TileData : ScriptableObject {
    // TileDatas can be used to group multiple tiles under a single rule.
    // But no single TileBase should appear in more than one TileData
    public List<TileBase> Tiles;
    public bool IsWalkable = true;
    public bool HasWildEncounters = false;
}
