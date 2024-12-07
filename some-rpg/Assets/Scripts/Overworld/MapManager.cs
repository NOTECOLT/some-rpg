using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour {
    [SerializeField] private List<Tilemap> _tilemapLayers;
    [SerializeField] private List<TileData> _tileDatas;

    // List of Encounter Rates is handled per map
    [SerializeField] private List<EncounterWeight> _encounterWeights = new List<EncounterWeight>();
    public Dictionary<EnemyType, float> EncounterRates { get; private set; } = new Dictionary<EnemyType, float>();

    // This dictionary will be used as a lookup table for the MapManager
    // This is built on Awake()
    private Dictionary<TileBase, TileData> _tileDictionary; 
    void Awake() {
        // Building the Tile Dictionary

        _tileDictionary = new Dictionary<TileBase, TileData>();
        foreach (TileData tileData in _tileDatas) {
            foreach (TileBase tileBase in tileData.Tiles) {
                if (_tileDictionary.ContainsKey(tileBase)) {
                    Debug.LogWarning($"Tile Dictionary already contains definition for {tileBase.name} from {_tileDictionary[tileBase].name}. Ignoring new definition from {tileData.name}.");
                    continue;
                }
                _tileDictionary.Add(tileBase, tileData);
            }
        }

        // Building the Encounter Rates
        
        int totalWeight = 0;
        foreach (EncounterWeight encounterWeight in _encounterWeights) {
            totalWeight += encounterWeight.Weight;
        }

        foreach (EncounterWeight encounterWeight in _encounterWeights) {
            EncounterRates.Add(encounterWeight.EnemyType, encounterWeight.Weight / totalWeight);
        }
    }

    public bool GetTileIsWalkable(Vector3 worldPosition) {
        foreach (Tilemap layer in _tilemapLayers) {
            Vector3Int cellPosition = layer.WorldToCell(worldPosition);
            TileBase tile = layer.GetTile(cellPosition);
            if (tile is null) continue;
            if (!_tileDictionary.ContainsKey(tile)) continue;
            if (!_tileDictionary[tile].IsWalkable) return false;
        }
        
        return true;
    }

    public bool GetTileHasWildEncounters(Vector3 worldPosition) {
        foreach (Tilemap layer in _tilemapLayers) {
            Vector3Int cellPosition = layer.WorldToCell(worldPosition);
            TileBase tile = layer.GetTile(cellPosition);
            if (tile is null) continue;
            if (!_tileDictionary.ContainsKey(tile)) continue;
            if (_tileDictionary[tile].HasWildEncounters) return true;
        }
        
        return false;     
    }
}
