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
    private Vector2 _rayEnd;
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

    void Update() {
        switch (_movementController.FacingDirection) {
            case Direction.UP:
                _rayEnd = Vector2.up;
                break;
            case Direction.RIGHT:
                _rayEnd = Vector2.right;
                break;
            case Direction.DOWN:
                _rayEnd = Vector2.down;
                break;
            default:
                _rayEnd = Vector2.left;
                break;
        }
            
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + (Vector3)(_rayEnd * _tileMap.cellSize.x));
        Debug.DrawLine(transform.position, transform.position + (Vector3)(_rayEnd * _tileMap.cellSize.x), Color.red);

        if (hit.collider != null) {
            Debug.Log("HIT");
        }
    }

    public GameObject GetLinecastHit(Vector3 rayEnd) {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + (rayEnd * _tileMap.cellSize.x));
        Debug.DrawLine(transform.position, transform.position + (rayEnd * _tileMap.cellSize.x), Color.red);

        if (hit.collider != null) {
            return hit.collider.gameObject;
        } else {
            return null;
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
