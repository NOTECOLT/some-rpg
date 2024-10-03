using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System.Linq;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor {
    /// <summary>
    /// Custom Editor function to convert any Tile to a Special Tile object
    /// </summary>
    [MenuItem("Assets/Convert to Special Tile")]
    public static void TileToSpecialTile() {
        if (Selection.objects.Count() == 0) return;

        foreach (Object obj in Selection.objects) {
            if (obj is null || obj.GetType() != typeof(Tile)) continue;
        }

        ConvertSingleTile(Selection.activeObject);
    }

    public static void ConvertSingleTile(Object obj) {
        if (obj is null) return;

        string path = AssetDatabase.GetAssetPath(obj);
        path = path.Substring(0, path.LastIndexOf('/') + 1);
        

        Tile tile = obj as Tile;
        SpecialTile convertedTile = new SpecialTile() {
            sprite = tile.sprite,
            color = tile.color,
            transform = tile.transform,
            gameObject = tile.gameObject,
            flags = tile.flags,
            colliderType = tile.colliderType,
        };
        AssetDatabase.CreateAsset(convertedTile, path + obj.name + "-spec.asset");
        Debug.Log(path);
    }
}
