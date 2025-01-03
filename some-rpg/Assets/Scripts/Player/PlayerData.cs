using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all player stats within the scene. <br></br>
/// Will hold loaded PlayerData and PlayerData to be saved from.
/// </summary>
public class PlayerData : MonoBehaviour {
    public static PlayerData Instance { get; private set; }

    private void Awake()  { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this.gameObject); 
        } else { 
            Instance = this; 
        } 

        DontDestroyOnLoad(this.gameObject);
    }

    // BaseStats only change in between battles, through level up or permanent status changes
    public EntityStats BaseStats = new EntityStats(); 

    // CurrentStats may change through status effects in battle
    public EntityStats CurrentStats = new EntityStats();
    public Vector3Int Cell = Vector3Int.zero;
    public Direction Direction = Direction.DOWN;

    void Start() {
        CurrentStats = (EntityStats)BaseStats.Clone();
    }
}

/// <summary>
/// Animation Directions integers as defined by the animator controller parameters
/// Integers follow the order "Never (North, Up) Eat (East, Right) Soggy (South, Down) Waffles (West, Left)"
/// </summary>
public enum Direction {
    UP,
    RIGHT,
    DOWN,
    LEFT
}