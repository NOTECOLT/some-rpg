using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script to be attached to the Scene Transition Canvas Prefab. Has reference to children objects
/// </summary>
public class SceneTransitionCanvas : MonoBehaviour {
    public static SceneTransitionCanvas Instance;

    // If there is an instance, and it's not me, delete myself.
    // The only reason i'm making this a singleton is so that the SceneLoader can easily access this reference
    void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(this.gameObject); 
        } else { 
            Instance = this; 
        } 

        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject panelFade;
}
