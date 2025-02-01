using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to hold references to every item in the game so that external scripts can reference it
/// ? Not my favorite solution but i'll keep it like this for now 
/// </summary>
public class ItemDatabase : MonoBehaviour {
    [SerializeField] private List<Weapon> weaponsList;
    public Dictionary<string, Weapon> Weapons = null;

    void Start() {
        InstantiateWeaponsDB();
    }
    public void InstantiateWeaponsDB() {
        Weapons = new Dictionary<string, Weapon>();

        foreach (Weapon w in weaponsList) {
            if (w.weaponid == "") {
                Debug.LogWarning($"[Item Database] Object {w.name} does not have weaponid. Weapon cannot be referenced in game.");
                continue;
            }
            
            Weapons.Add(w.weaponid, w);
        }
    }
}
