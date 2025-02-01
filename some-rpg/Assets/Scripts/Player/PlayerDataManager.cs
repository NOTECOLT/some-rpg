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
            NewSaveData(dataPersistence);
        } else {
            if (dataPersistence.LoadData("player", ref Data) == DataPersistenceManager<PlayerData>.RET_SAVE_WRONG_VERSION) {
                NewSaveData(dataPersistence);
            }
        }

        FindObjectOfType<TiledMovementController>().SetPosition(Data.Cell, Data.Direction);
    }

    private void NewSaveData(DataPersistenceManager<PlayerData> dpm) {
        Data = (PlayerData)DefaultData.Clone();
        dpm.NewData("player", Data);
        Data.CurrentStats = (EntityStats)Data.BaseStats.Clone();
        dpm.SaveData("player", Data);    
    }
}