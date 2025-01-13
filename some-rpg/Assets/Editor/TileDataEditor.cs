using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TileData))]
public class TileDataEditor : Editor {
    SerializedProperty Tiles;
    SerializedProperty IsWalkable;
    SerializedProperty HasWildEncounters;

    void OnEnable() {
        Tiles = serializedObject.FindProperty("Tiles");
        IsWalkable = serializedObject.FindProperty("IsWalkable");
        HasWildEncounters = serializedObject.FindProperty("HasWildEncounters");
    }

    public override void OnInspectorGUI() {
        

        serializedObject.Update();

        EditorGUILayout.PropertyField(IsWalkable);
        EditorGUILayout.PropertyField(HasWildEncounters);
        EditorGUILayout.PropertyField(Tiles);
        DrawTileListPreview();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTileListPreview() {
        TileData tileData = (TileData)target;
        if (tileData.Tiles is null) return;

        float spriteSize = 32;
        int n = 0;
        
        while (n < tileData.Tiles.Count) {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < Mathf.FloorToInt(Screen.width / (spriteSize * 2)); i++) {
                if (n >= tileData.Tiles.Count) break;
                
                if (tileData.Tiles[n] is not null)
                    DrawTile((Tile)tileData.Tiles[n], spriteSize);
                n++;
            }
            GUILayout.EndHorizontal();
        }
    }

    private void DrawTile(Tile tile, float spriteSize) {
        Texture2D texture = AssetPreview.GetAssetPreview(tile.sprite);
        GUILayout.Label(texture, GUILayout.Height(spriteSize), GUILayout.Width(spriteSize));
    }
}
