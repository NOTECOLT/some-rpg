using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour {
    private int _currentPage = 0;
    private int _maxPages = 3;
    public static DebugUI Instance { get; private set; }
    void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        GetComponent<ScreenLogger>().enabled = false;
    }

    void Update() {
        if (Debug.isDebugBuild) {
            if (Input.GetKeyDown(KeyCode.F3)) {
                _currentPage = (_currentPage + 1) % _maxPages;
                // foreach (Component component in this.gameObject.GetComponents(typeof(Component))) {
                //     if (component == this) continue;
                //     component.enabled = false;
                // }
                GetComponent<ScreenLogger>().enabled = false;
                GetComponent<EncounterGenerator>().enabled = false;

                switch (_currentPage) {
                    case 0:
                        break;
                    case 1:
                        GetComponent<ScreenLogger>().enabled = true;
                        break;
                    case 2:
                        GetComponent<EncounterGenerator>().enabled = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
} 
