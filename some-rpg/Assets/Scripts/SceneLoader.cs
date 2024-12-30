using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour {
    private static int OVERWORLD_SCENE = 0;
    private static int BATTLE_SCENE = 1; 

    public List<EnemyType> Encounters { get; private set; }

    public static SceneLoader Instance { get; private set; }

    private void Awake()  { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this.gameObject); 
        } else { 
            Instance = this; 
        } 

        DontDestroyOnLoad(this.gameObject);
    }

    // Function to load a battle scene with a specific encounter
    public void LoadEncounter(List<EnemyType> encounters) {
        Encounters = encounters;
        SceneManager.LoadScene(BATTLE_SCENE);
    }

    public void LoadOverworld() {
        SceneManager.LoadScene(OVERWORLD_SCENE);
    }
}
