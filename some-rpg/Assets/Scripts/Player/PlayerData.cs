using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all player stats within the scene. <br></br>
/// Will hold loaded PlayerData and PlayerData to be saved from.
/// </summary>
public class PlayerData : MonoBehaviour {
    public static PlayerData Instance { get; private set; }
    // ! I DONT WANT THIS TO BE A SINGLETON, THERE MUST BE A BETTER WAY
    // ! IN THE FUTURE: LOOK INTO DEPENDENCY INJECTION???
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

    void Start() {
        CurrentStats = (EntityStats)BaseStats.Clone();
    }
}
