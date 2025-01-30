using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
