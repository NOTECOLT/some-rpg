using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverworldState : GenericState<GameStateMachine.StateKey> {
    private static float ENCOUNTER_CHANCE = 0.067f;
    private GameStateMachine _context;
    private bool _isBattleTriggered;
    private MapManager _mapManager;
    private System.Random _rnd = new System.Random();
    public GameOverworldState(GameStateMachine context, GameStateMachine.StateKey key) : base(key) {
        _context = context;
    }
    public override void EnterState() {
        _isBattleTriggered = false;
        
        // If you're reading this, you're probably wondering why this is empty.
        // Normally I would put all overworld loading logic here, but because this gameobject
        //      persists through scene loads (don't destroy on load), what happens is this block
        //      of code ends up running BEFORE or DURING start() method.
        //      Thus, initialization of the following gameobjects is handled by their
        //      respective start methods
        //      - MapManager
        //      - TiledMapController
    }

    public override GameStateMachine.StateKey GetNextState() {
        if (_isBattleTriggered) {
            return GameStateMachine.StateKey.BATTLE_STATE;
        } else {
            return Key;
        }
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    // This is called by the MapManager class
    public void InitMapManager(MapManager mapManager) {
        _mapManager = mapManager;
    }

    /// <summary>
    /// Generates a random encounter based on the internal encounter rate dictionary
    /// Note: Does not determine if the player will even run into a wild encounter at all.
    /// </summary>
    /// <returns></returns>
    private EnemyType GenerateWildEncounter() {
        int generatedValue = _rnd.Next(0, 100);
        int threshold = 0;

        foreach (KeyValuePair<EnemyType, float> entry in _mapManager.EncounterRates) {
            threshold += Mathf.CeilToInt(entry.Value*100);

            if (generatedValue < threshold) return entry.Key;
        }

        return null;
    }

    /// <summary>
    /// Public function that determines if the player will run into a wild encounter and how many. 
    /// Does not generate the encounter itself.   
    /// </summary>
    /// <param name="position"></param>
    public void DoEncounterCheck(Vector3 position) {
        if (!_mapManager.GetTileHasWildEncounters(position)) return;

        // Check if player encounters any wild enemy
        if (_rnd.Next(0, 100) >= Mathf.CeilToInt(ENCOUNTER_CHANCE*100)) return;
        
        int encounters = _rnd.Next(1, 4);
        _context.encounters = new List<EnemyType>();
        string nameList = "{}";

        for (int i = 0; i < encounters; i++) {
            EnemyType encounter = GenerateWildEncounter();
            if (encounter is null) return;
    
            _context.encounters.Add(encounter);
            nameList += $"{encounter.name} ";
        }
        
        Debug.Log($"Encounter with {nameList}}}!");
        _isBattleTriggered = true;
    }

    public bool GetTileIsWalkable(Vector3 position) {
        return _mapManager.GetTileIsWalkable(position);
    }
}