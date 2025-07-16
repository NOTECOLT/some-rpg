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

    #region SerializedPlayerData
    /// <summary>
    /// Serialized version of PlayerData. PlayerData must be converted into this and vice versa when saving & loading
    /// </summary>
    [Serializable] 
    private class SerializedPlayerData {
        [Serializable]
        public class SerializedPartyMember {
            // BaseStats only change in between battles, through level up or permanent status changes
            public EntityStats BaseStats = new EntityStats();

            // CurrentStats may change through status effects in battle
            public EntityStats CurrentStats = new EntityStats();
            public string Name;

            /// <summary>
            /// Weapon Keys must be stored instead of scriptable object instances.
            /// </summary>
            public SerializedWeaponItem Weapon;

            public object Clone()
            {
                return new SerializedPartyMember()
                {
                    BaseStats = (EntityStats)this.BaseStats.Clone(),
                    CurrentStats = (EntityStats)this.CurrentStats.Clone(),
                    Weapon = (SerializedWeaponItem)this.Weapon.Clone(),
                    Name = this.Name
                };
            }
        } 
        public List<SerializedPartyMember> PartyStats = new List<SerializedPartyMember>();
        public Vector3Int Cell = Vector3Int.zero;
        public Direction Direction = Direction.DOWN;

        public SerializedPlayerData(PlayerData pd) {
            this.PartyStats = new List<SerializedPartyMember>();

            foreach (PartyMember dsStats in pd.PartyStats) {
                SerializedPartyMember stats = new SerializedPartyMember() {
                    BaseStats = (EntityStats)dsStats.BaseStats.Clone(),
                    CurrentStats = (EntityStats)dsStats.CurrentStats.Clone(),
                    Name = dsStats.Name
                };


                if (GameStateMachine.Instance.Weapons.ContainsValue(dsStats.Weapon.Data)) {
                    stats.Weapon = new SerializedWeaponItem() {
                        Data = GameStateMachine.Instance.Weapons.First(x => x.Value == dsStats.Weapon.Data).Key,
                        CurrentStats = (WeaponStats)dsStats.Weapon.CurrentStats.Clone()
                    };
                } else {
                    Debug.LogWarning($"Weapon {dsStats.Weapon.Data.WeaponName} does not exist in Weapons Dictionary! Cannot serialize Weapon data.");
                    stats.Weapon = new SerializedWeaponItem() {
                        Data = "",
                        CurrentStats = (WeaponStats)dsStats.Weapon.CurrentStats.Clone()
                    };             
                }

                this.PartyStats.Add(stats);
            }

            this.Cell = pd.Cell;
            this.Direction = pd.Direction;
        }

        public PlayerData DeserializePlayerData() {
            PlayerData pd = new PlayerData();
            
            foreach (SerializedPartyMember sStats in PartyStats) {
                PartyMember stats = new PartyMember() {
                    BaseStats = (EntityStats)sStats.BaseStats.Clone(),
                    CurrentStats = (EntityStats)sStats.CurrentStats.Clone(),
                    Name = sStats.Name
                };
                
                if (GameStateMachine.Instance.Weapons.ContainsKey(sStats.Weapon.Data)) {
                    stats.Weapon = new WeaponItem() {
                        Data = GameStateMachine.Instance.Weapons[sStats.Weapon.Data], 
                        CurrentStats = (WeaponStats)sStats.Weapon.CurrentStats.Clone()
                    };
                } else {
                    Debug.LogWarning($"Weapon {sStats.Weapon} does not exist in Weapons Dictionary! Cannot Deserialize Weapon data.");
                    stats.Weapon = new WeaponItem() {
                        Data = null, 
                        CurrentStats = null
                    };             
                } 

                pd.PartyStats.Add(stats);
            }

            pd.Cell = new Vector3Int(this.Cell.x, this.Cell.y, this.Cell.z);
            pd.Direction = this.Direction;

            return pd;
        }
    }

    #endregion

    #region Serialized Weapon Item
    [Serializable]
    public class SerializedWeaponItem : ICloneable {
        public string Data;
        public WeaponStats CurrentStats;

        public object Clone() {
            return new SerializedWeaponItem() {
                Data = this.Data,
                CurrentStats = (WeaponStats)this.CurrentStats.Clone()
            };
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

        foreach (PartyMember player in Data.PartyStats) {
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