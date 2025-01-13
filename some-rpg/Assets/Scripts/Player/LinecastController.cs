using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

/// <summary>
/// Gives the player with a raycast which they can use to detect other entities on the map
/// Note that this game does not use any proper physics, but does utilize 2D colliders to detect other game objects not on the tilemap 
/// </summary>
[RequireComponent(typeof(TiledMovementController))] 
public class LinecastController : MonoBehaviour {
    private TiledMovementController _movementController;
    private PlayerInputActions _inputActions;
    [SerializeField] private Tilemap _tileMap;

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

        _movementController = GetComponent<TiledMovementController>();
    }

    void FixedUpdate() {
        Vector2 rayEnd = Vector2.up;

        switch (_movementController.FacingDirection) {
            case Direction.UP:
                rayEnd = Vector2.up;
                break;
            case Direction.RIGHT:
                rayEnd = Vector2.right;
                break;
            case Direction.DOWN:
                rayEnd = Vector2.down;
                break;
            case Direction.LEFT:
                rayEnd = Vector2.left;
                break;
        }
            
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + (Vector3)(rayEnd * _tileMap.cellSize.x));
        Debug.DrawLine(transform.position, transform.position + (Vector3)(rayEnd * _tileMap.cellSize.x), Color.red);

        if (hit.collider != null) {
            Debug.Log("HIT");
        }
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
