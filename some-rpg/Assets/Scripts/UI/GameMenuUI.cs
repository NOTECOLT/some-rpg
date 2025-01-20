using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Responsible for Game Menu input done in the overworld (separate from battle menu ui)
/// </summary>
public class GameMenuUI : MonoBehaviour {
    private GameMenuAction _inputActions;
    private bool _isMenuOpen = false;
    [SerializeField] private GameObject _menuParent;
    [SerializeField] private EntityInfoUI _playerInfo;

    /// <summary>
    /// Unity Event that gets sent whenever the menu is opened
    /// </summary>
    public static event System.Action OnMenuOpen;

    /// <summary>
    /// Unity Event that gets sent whenever the menu is closed
    /// </summary>
    public static event System.Action OnMenuClose;

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

        if (_isMenuOpen) {
            OnMenuOpen.Invoke();
            _playerInfo.Instantiate(PlayerDataManager.Instance.name, PlayerDataManager.Instance.Data.CurrentStats, PlayerDataManager.Instance.Data.BaseStats);
        }
        else OnMenuClose.Invoke();
    }
}
