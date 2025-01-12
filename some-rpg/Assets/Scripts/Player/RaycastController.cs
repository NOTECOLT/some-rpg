using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gives the player with a raycast which they can use to detect other entities on the map
/// Note that this game does not use any proper physics, but does utilize 2D colliders to detect other game objects not on the tilemap 
/// </summary>
public class MapRaycaster : MonoBehaviour {
    private PlayerInputActions _inputActions;

    void Awake() {
        _inputActions = new PlayerInputActions();

        _inputActions.Player.Interact.performed += OnInteractInput;
    }

    void OnEnable() {
        _inputActions.Enable();
    }

    void OnDisable() {
        _inputActions.Disable();
    }

    void OnDestroy() {
        _inputActions.Player.Interact.performed -= OnInteractInput;
        GameMenuUI.OnMenuOpen -= DisableInputActions;
        GameMenuUI.OnMenuClose -= EnableInputActions;

        SceneLoader.Instance.OnEncounterTransition -= DisableInputActions;
    }

    void Start() {
        SceneLoader.Instance.OnEncounterTransition += DisableInputActions;
        GameMenuUI.OnMenuOpen += DisableInputActions;
        GameMenuUI.OnMenuClose += EnableInputActions;
    }

    void Update() {
        
    }

    private void OnInteractInput(InputAction.CallbackContext context) {
        
    }

    #region Event Listeners

    void DisableInputActions() {
        _inputActions.Disable();
    }

    private void EnableInputActions() {
        _inputActions.Enable();
    }

    #endregion
}
