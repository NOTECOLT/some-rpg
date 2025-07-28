using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Singleton Script to hold PlayerData information
/// Actually exists within the scene.
/// </summary>
public class PlayerDataManager : MonoBehaviour {
    public static PlayerDataManager Instance { get; private set; }
    private void Awake() {
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

    public void LoadPlayerData() {
        DataPersistenceManager<SerializedPlayerData> dpm = new DataPersistenceManager<SerializedPlayerData>();
        if (!dpm.Exists("player")) {
            NewSaveData();
        } else {
            SerializedPlayerData serializedData = null;
            int ret = dpm.LoadData("player", ref serializedData);
            if (ret == DataPersistenceManager<SerializedPlayerData>.RET_SAVE_WRONG_VERSION) {
                NewSaveData();
            } else {
                Data = (PlayerData)serializedData.Deserialize();
            }
        }
    }

    private void NewSaveData() {
        DataPersistenceManager<SerializedPlayerData> dpm = new DataPersistenceManager<SerializedPlayerData>();

        Data = (PlayerData)DefaultData.Clone();

        foreach (PartyMember player in Data.PartyStats) {
            player.CurrentStats = (EntityStats)player.BaseStats.Clone();
        }

        SerializedPlayerData serializedData = (SerializedPlayerData)Data.Serialize();

        dpm.NewData("player", serializedData); 
        dpm.SaveData("player", serializedData);
    }

    public void SaveData() {
        DataPersistenceManager<SerializedPlayerData> dpm = new DataPersistenceManager<SerializedPlayerData>();
        SerializedPlayerData serializedData = (SerializedPlayerData)Data.Serialize();
        
        dpm.SaveData("player", serializedData);
    }
}