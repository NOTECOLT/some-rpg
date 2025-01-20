using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton Script to hold PlayerData information
/// Actually exists within the scene.
/// </summary>
public class PlayerDataManager : MonoBehaviour {
    public static PlayerDataManager Instance { get; private set; }

    private void Awake()  { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this.gameObject); 
        } else { 
            Instance = this; 
        } 

        DontDestroyOnLoad(this.gameObject);
    }

    public PlayerData DefaultData = new PlayerData();
    public PlayerData Data = new PlayerData();

    void Start() {
        DataPersistenceManager<PlayerData> dataPersistence = new DataPersistenceManager<PlayerData>();
        if (!dataPersistence.Exists("player")) {
            Data = DefaultData;
            dataPersistence.NewData("player", Data);
            Data.CurrentStats = (EntityStats)Data.BaseStats.Clone();
        } else {
            dataPersistence.LoadData("player", ref Data);
        }
    }

    void OnDestroy() {
        DataPersistenceManager<PlayerData> dataPersistence = new DataPersistenceManager<PlayerData>();
        dataPersistence.SaveData("player", Data);
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
    LEFT,
    NULL
}