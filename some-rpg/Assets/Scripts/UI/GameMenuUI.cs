using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuUI : MonoBehaviour {
    private GameMenuAction _inputActions;
    private bool _isMenuOpen = false;
    [SerializeField] private GameObject _menuParent;

    void Awake() {
        _inputActions = new GameMenuAction();

        _inputActions.GameMenu.OpenGameMenu.started += OnMenuInput;
    }

    void OnEnable() {
        _inputActions.Enable();
    }

    void OnSceneTransition() {
        _inputActions.Disable();
    }

    void OnDisable() {
        _inputActions.Disable();
    }

    void OnDestroy() {
        _inputActions.GameMenu.OpenGameMenu.started -= OnMenuInput;
        SceneLoader.Instance.OnEncounterTransition -= OnSceneTransition;
    }

    void Start() {
        SceneLoader.Instance.OnEncounterTransition += OnSceneTransition;

        _isMenuOpen = false;
        _menuParent.SetActive(_isMenuOpen);
    }

    void OnMenuInput(InputAction.CallbackContext context) {
        _isMenuOpen = !_isMenuOpen;
        _menuParent.SetActive(_isMenuOpen);
    }
}
