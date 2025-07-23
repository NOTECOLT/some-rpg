using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Debug Script which allows the user to generate custom battles on the fly
/// </summary>
public class EncounterGenerator : MonoBehaviour {

    private GUIStyle _style = null;
    private GUIStyle _buttonStyle = null;
    [SerializeField] private List<EnemyType> _enemyTypes = new List<EnemyType>();

    void Awake() {
        _enemyTypes.Sort((a, b) => { return a.EnemyName.CompareTo(b.EnemyName); });
    }

    void OnGUI() {
        if (_style == null) {
            _style = new GUIStyle() {
                fontSize = 25,
                normal = {
                    textColor = Color.white
                },
                wordWrap = true
            };
        }

        if (_buttonStyle == null) {
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.fontSize = 25;
        }

        { // Title
            int startX = 20; int startY = 20;
            int endX = 240; int endY = 70;
            GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
            GUILayout.Label("EncounterGenerator", _style);
            GUILayout.EndArea();
        }

        { // Encounter List
            int startX = 20; int startY = 75;
            int endX = 220; int endY = 175;

            List<string> encounterList = GameStateMachine.Instance.encounters.Select(e => e.EnemyName).ToList();

            GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
            GUILayout.Label($"Encounter List: {string.Join("\n", encounterList)}", _style);
            GUILayout.EndArea();
        }

        if (GameStateMachine.Instance.GetCurrentStateKey() == GameStateMachine.StateKey.OVERWORLD_STATE) { // Text
            {
                int startX = 220; int startY = 75;
                int endX = 420; int endY = 475;
                int buttonSizeY = 50;
                GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
                for (int i = 0; i < _enemyTypes.Count; i++) {
                    bool b = GUI.Button(new Rect(0, i * buttonSizeY, endX - startX, buttonSizeY), $"{_enemyTypes[i].EnemyName}", _buttonStyle);
                    if (b && GameStateMachine.Instance.encounters.Count < 3) {
                        GameStateMachine.Instance.encounters.Add(_enemyTypes[i]);
                    }
                }
                GUILayout.EndArea();
            }
            { // Button List
                int startX = 260; int startY = 20;
                int endX = Screen.width; int endY = 70;
                int buttonSizeX = 240;
                GUILayout.BeginArea(new Rect(startX, startY, endX, endY));
                bool clearEncounters = GUI.Button(new Rect(0, 0, buttonSizeX, endY - startY), "Clear Encounters", _buttonStyle);
                if (clearEncounters) {
                    GameStateMachine.Instance.encounters = new List<EnemyType>();
                }

                bool genEncounter = GUI.Button(new Rect(250, 0, buttonSizeX, endY - startY), "Generate Encounter", _buttonStyle);
                if (genEncounter && GameStateMachine.Instance.encounters.Count > 0) {
                    GameStateMachine.Instance.GetCurrentStateContext<GameOverworldState>().SetBattleTriggered();
                }
                GUILayout.EndArea();
            }
        }
    }
}
