using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene loading & transitions. Note that SceneLoader sghould be a singleton, and there should be one that persists in every scene regardless. 
/// This object does not get destroyed on load.
/// </summary>
public class SceneLoader : MonoBehaviour {
    private static int OVERWORLD_SCENE = 0;
    private static int BATTLE_SCENE = 1; 

    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _panelFade;

    public List<EnemyType> Encounters { get; private set; }
    public static SceneLoader Instance { get; private set; }

    /// <summary>
    /// Triggers upon Scene Transition into an encounter
    /// </summary>
    public event System.Action OnEncounterTransition;

    private void Awake()  { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this.gameObject); 
        } else { 
            Instance = this; 
        } 

        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        _camera = Camera.main.gameObject;
        _panelFade = SceneTransitionCanvas.Instance.panelFade.gameObject;
    }

    // Function to load a battle scene with a specific encounter
    public void LoadEncounter(List<EnemyType> encounters) {
        Encounters = encounters;
        StartCoroutine(EncounterTransition());
    }

    public IEnumerator EncounterTransition() {
        OnEncounterTransition();

        _camera.GetComponent<Animator>().SetTrigger("ZoomIn");
        yield return new WaitForSeconds(_camera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        
        yield return SceneFadeTransition(BATTLE_SCENE);
    }

    public void LoadOverworld() {
        StartCoroutine(SceneFadeTransition(OVERWORLD_SCENE));
    }

    public IEnumerator SceneFadeTransition(int scene) {
        // Panel Fade Out Old Scene
        _panelFade.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(_camera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        SceneManager.LoadScene(scene);

        // Panel Fade In New Scene
        _panelFade.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(_camera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}
