using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that extends the basic Unity tile to contain more data
/// This can be like collision data or movement effects. 
/// </summary>
[CreateAssetMenu(fileName="SpecialTile", menuName="SpecialTile", order=2)]  
public class SpecialTile : Tile {
    public bool isWalkable = true;
}
