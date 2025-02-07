using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    #region SerializedPlayerData
    /// <summary>
    /// Serialized version of PlayerData. PlayerData must be converted into this and vice versa when saving & loading
    /// </summary>
    [Serializable] 
    private class SerializedPlayerdata {
        public EntityStats BaseStats = new EntityStats(); 
        public EntityStats CurrentStats = new EntityStats();
        public Vector3Int Cell = Vector3Int.zero;
        public Direction Direction = Direction.DOWN;

        /// <summary>
        /// Weapon Keys must be stored instead of scriptable object instances.
        /// </summary>
        public string Weapon;

        public SerializedPlayerdata(PlayerData pd) {
            this.BaseStats = (EntityStats)pd.BaseStats.Clone();
            this.CurrentStats = (EntityStats)pd.CurrentStats.Clone();
            this.Cell = pd.Cell;
            this.Direction = pd.Direction;

            if (GameStateMachine.Instance.Weapons.ContainsValue(pd.Weapon)) {
                this.Weapon = GameStateMachine.Instance.Weapons.First(x => x.Value == pd.Weapon).Key;
            } else {
                Debug.LogWarning($"Weapon {pd.Weapon.WeaponName} does not exist in Weapons Dictionary! Cannot serialize Weapon data.");
                this.Weapon = "";                
            }
        }

        public PlayerData DeserializePlayerData() {
            PlayerData pd = new PlayerData();

            pd.BaseStats = (EntityStats)this.BaseStats.Clone();
            pd.CurrentStats = (EntityStats)this.CurrentStats.Clone();
            pd.Cell = new Vector3Int(this.Cell.x, this.Cell.y, this.Cell.z);
            pd.Direction = this.Direction;

            if (GameStateMachine.Instance.Weapons.ContainsKey(this.Weapon)) {
                pd.Weapon = GameStateMachine.Instance.Weapons[this.Weapon];
            } else {
                Debug.LogWarning($"Weapon {this.Weapon} does not exist in Weapons Dictionary! Cannot Deserialize Weapon data.");
                pd.Weapon = null;                
            } 

            return pd;
        }
    }

    #endregion

    public PlayerData DefaultData = new PlayerData();
    public PlayerData Data = new PlayerData();

    public void LoadPlayerData() {
        DataPersistenceManager<SerializedPlayerdata> dpm = new DataPersistenceManager<SerializedPlayerdata>();
        if (!dpm.Exists("player")) {
            NewSaveData();
        } else {
            SerializedPlayerdata serializedData = null;
            int ret = dpm.LoadData("player", ref serializedData);
            if (ret == DataPersistenceManager<SerializedPlayerdata>.RET_SAVE_WRONG_VERSION) {
                NewSaveData();
            } else {
                Data = serializedData.DeserializePlayerData();
            }
        }
    }

    private void NewSaveData() {
        DataPersistenceManager<SerializedPlayerdata> dpm = new DataPersistenceManager<SerializedPlayerdata>();

        Data = (PlayerData)DefaultData.Clone();
        Data.CurrentStats = (EntityStats)Data.BaseStats.Clone();

        SerializedPlayerdata serializedData = new SerializedPlayerdata(Data);

        dpm.NewData("player", serializedData); 
        dpm.SaveData("player", serializedData);
    }

    public void SaveData() {
        DataPersistenceManager<SerializedPlayerdata> dpm = new DataPersistenceManager<SerializedPlayerdata>();
        dpm.SaveData("player", new SerializedPlayerdata(Data));
    }
}