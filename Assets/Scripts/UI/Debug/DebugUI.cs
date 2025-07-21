using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour {
    private GUIStyle _style = null;
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

    void OnGUI() {
        if (_style == null) {
            _style = new GUIStyle() {
                fontSize = 25,
                normal = {
                    textColor = Color.white
                },
                wordWrap = true,
            };
        }
        
        if (Debug.isDebugBuild) {
            int startX = Screen.width - 450; int startY = 10;
            int endX = Screen.width - 10; int endY = 50;

            GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
            GUILayout.Label($"Development Build version: {Application.version}", _style);

            GUILayout.EndArea();

        }
    }
} 
