using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that holds data for special tiles. Is filled with some case-specific rules and can be filled with more generic flags as well
/// </summary>
[CreateAssetMenu(fileName="TileData", menuName="TileData", order=2)]
public class TileData : ScriptableObject {
    public List<Tile> tiles;
    public bool isWalkable = true;
}
