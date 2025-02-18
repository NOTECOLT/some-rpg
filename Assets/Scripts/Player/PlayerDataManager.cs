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
    private class SerializedPlayerData {
        [Serializable]
        public class MemberStats {
            // BaseStats only change in between battles, through level up or permanent status changes
            public EntityStats BaseStats = new EntityStats();

            // CurrentStats may change through status effects in battle
            public EntityStats CurrentStats = new EntityStats();

            /// <summary>
            /// Weapon Keys must be stored instead of scriptable object instances.
            /// </summary>
            public string Weapon;

            public object Clone() {
                return new MemberStats() {
                    BaseStats = (EntityStats)this.BaseStats.Clone(),
                    CurrentStats = (EntityStats)this.CurrentStats.Clone(),
                    Weapon = this.Weapon
                };
            }
        } 
        public List<MemberStats> PartyStats = new List<MemberStats>();
        public Vector3Int Cell = Vector3Int.zero;
        public Direction Direction = Direction.DOWN;

        public SerializedPlayerData(PlayerData pd) {
            this.PartyStats = new List<MemberStats>();

            foreach (PlayerData.MemberStats dsStats in pd.PartyStats) {
                MemberStats stats = new MemberStats() {
                    BaseStats = (EntityStats)dsStats.BaseStats.Clone(),
                    CurrentStats = (EntityStats)dsStats.CurrentStats.Clone(),
                };


                if (GameStateMachine.Instance.Weapons.ContainsValue(dsStats.Weapon)) {
                    stats.Weapon = GameStateMachine.Instance.Weapons.First(x => x.Value == dsStats.Weapon).Key;
                } else {
                    Debug.LogWarning($"Weapon {dsStats.Weapon.WeaponName} does not exist in Weapons Dictionary! Cannot serialize Weapon data.");
                    stats.Weapon = "";                
                }

                this.PartyStats.Add(stats);
            }

            this.Cell = pd.Cell;
            this.Direction = pd.Direction;
        }

        public PlayerData DeserializePlayerData() {
            PlayerData pd = new PlayerData();
            
            foreach (MemberStats sStats in PartyStats) {
                PlayerData.MemberStats stats = new PlayerData.MemberStats() {
                    BaseStats = (EntityStats)sStats.BaseStats.Clone(),
                    CurrentStats = (EntityStats)sStats.CurrentStats.Clone()
                };
                
                if (GameStateMachine.Instance.Weapons.ContainsKey(sStats.Weapon)) {
                    stats.Weapon = GameStateMachine.Instance.Weapons[sStats.Weapon];
                } else {
                    Debug.LogWarning($"Weapon {sStats.Weapon} does not exist in Weapons Dictionary! Cannot Deserialize Weapon data.");
                    stats.Weapon = null;                
                } 

                pd.PartyStats.Add(stats);
            }

            pd.Cell = new Vector3Int(this.Cell.x, this.Cell.y, this.Cell.z);
            pd.Direction = this.Direction;

            return pd;
        }
    }

    #endregion

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
                Data = serializedData.DeserializePlayerData();
            }
        }
    }

    private void NewSaveData() {
        DataPersistenceManager<SerializedPlayerData> dpm = new DataPersistenceManager<SerializedPlayerData>();

        Data = (PlayerData)DefaultData.Clone();

        foreach (PlayerData.MemberStats player in Data.PartyStats) {
            player.CurrentStats = (EntityStats)player.BaseStats.Clone();
        }

        SerializedPlayerData serializedData = new SerializedPlayerData(Data);

        dpm.NewData("player", serializedData); 
        dpm.SaveData("player", serializedData);
    }

    public void SaveData() {
        DataPersistenceManager<SerializedPlayerData> dpm = new DataPersistenceManager<SerializedPlayerData>();
        SerializedPlayerData serializedData = new SerializedPlayerData(Data);
        
        dpm.SaveData("player", serializedData);
    }
}