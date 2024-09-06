using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all player stats within the scene. <br></br>
/// Will hold loaded PlayerData and PlayerData to be saved from.
/// </summary>
public class PlayerData : MonoBehaviour {
    // BaseStats only change in between battles, through level up or permanent status changes
    public EntityStats BaseStats = new EntityStats(); 

    // CurrentStats may change through status effects in battle
    public EntityStats CurrentStats = new EntityStats();
}
