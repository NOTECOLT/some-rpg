using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Responsible for Game Menu input done in the overworld (separate from battle menu ui)
/// </summary>
public class GameMenuUI : MonoBehaviour {
    private GameMenuAction _inputActions;
    private bool _isMenuOpen = false;
    private bool _isPartyOpen = false;
    private bool _isInventoryOpen = false;

    [SerializeField] private GameObject _menuParent;
    [SerializeField] private GameObject _partyParent;
    [SerializeField] private GameObject _inventoryParent;
    [SerializeField] private UnitInfoMenuUI[] _playerInfo;

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

        DialogueManager.OnDialogueOpen -= DisableInputActions;
        DialogueManager.OnDialogueClose -= EnableInputActions;
    }

    void Start() {
        SceneLoader.Instance.OnEncounterTransition += OnSceneTransition;
        DialogueManager.OnDialogueOpen += DisableInputActions;
        DialogueManager.OnDialogueClose += EnableInputActions;

        _isMenuOpen = false;
        _menuParent.SetActive(_isMenuOpen);

        SetPartyMenuVisible(false);
        SetInventoryMenuVisible(false);
    }

    void OnMenuInput(InputAction.CallbackContext context) {
        _isMenuOpen = !_isMenuOpen;
        _menuParent.SetActive(_isMenuOpen);

        SetPartyMenuVisible(false);
        SetInventoryMenuVisible(false);

        if (_isMenuOpen) OnMenuOpen?.Invoke();
        else OnMenuClose?.Invoke();
    }

    public void SaveGame() {
        PlayerDataManager.Instance.SaveData();
    }

    public void ReloadGame() {
        Destroy(FindObjectOfType<GameStateMachine>().gameObject);
        SceneLoader.Instance.LoadOverworld();
    }

    public void ExitMenu() {
        _isMenuOpen = false;
        _menuParent.SetActive(_isMenuOpen);    
        OnMenuClose?.Invoke();    
    }

    public void SetPartyMenuVisible(bool isActive) {
        _isPartyOpen = isActive;
        _partyParent.SetActive(_isPartyOpen);
    }

    public void SetInventoryMenuVisible(bool isActive) {
        _isInventoryOpen = isActive;
        _inventoryParent.SetActive(_isInventoryOpen);
    }

    public void PartyMenu() {
        SetPartyMenuVisible(!_isPartyOpen);
        SetInventoryMenuVisible(false);

        if (_isPartyOpen) {
            PlayerData playerData = PlayerDataManager.Instance.Data;
            for (int i = 0; i < PlayerDataManager.Instance.Data.PartyStats.Count; i++) {
                _playerInfo[i].Instantiate(playerData.PartyStats[i]);
            }
        }
    }

    public void InventoryMenu() {
        SetInventoryMenuVisible(!_isInventoryOpen);
        SetPartyMenuVisible(false);
    }





    private void DisableInputActions() {
        _inputActions.Disable();
    }

    private void EnableInputActions() {
        _inputActions.Enable();
    }
}
