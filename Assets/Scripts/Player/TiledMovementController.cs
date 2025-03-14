using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

/// <summary>
/// Used to control player movement in a tiled grid
/// </summary>
public class TiledMovementController : MonoBehaviour {
    public Vector3Int Cell { get; private set; } = Vector3Int.zero;
    // public Vector3Int StartCell = Vector3Int.zero;
    public Direction FacingDirection { get; private set; } = Direction.DOWN;
    private GameOverworldState _worldContext;
    private Vector3 _movePoint;
    [SerializeField] private Tilemap _tileMap;
    private PlayerInputActions _inputActions;
    [SerializeField] private float _movementSpeed = 20f;
    private bool _isMoving = false;
    private InputAction.CallbackContext _inputContext;
    private Animator _animator = null;

    void Awake() {
        _inputActions = new PlayerInputActions();

        _inputActions.Player.Walk.started += OnWalkInput;
        _inputActions.Player.Walk.canceled += OnWalkRelease;
    }

    void OnEnable() {
        _inputActions.Enable();
    }

    void OnDisable() {
        _inputActions.Disable();
    }

    void OnDestroy() {
        _inputActions.Player.Walk.started -= OnWalkInput;
        _inputActions.Player.Walk.canceled -= OnWalkRelease;
        GameMenuUI.OnMenuOpen -= DisableInputActions;
        GameMenuUI.OnMenuClose -= EnableInputActions;
        DialogueManager.OnDialogueOpen -= DisableInputActions;
        DialogueManager.OnDialogueClose -= EnableInputActions;

        SceneLoader.Instance.OnEncounterTransition -= DisableInputActions;
    }

    void Start() {
        SceneLoader.Instance.OnEncounterTransition += DisableInputActions;
        GameMenuUI.OnMenuOpen += DisableInputActions;
        GameMenuUI.OnMenuClose += EnableInputActions;
        DialogueManager.OnDialogueOpen += DisableInputActions;
        DialogueManager.OnDialogueClose += EnableInputActions;

        _animator = GetComponent<Animator>();

        // Object waits for the game to be in the GameOverworldState in order to initialize properly
        StartCoroutine(GameStateMachine.Instance.WaitUntilState<GameOverworldState>(() => {
            _worldContext = GameStateMachine.Instance.GetCurrentStateContext<GameOverworldState>();
            SetPosition(PlayerDataManager.Instance.Data.Cell, PlayerDataManager.Instance.Data.Direction);
        }));
    }

    private void SetPosition(Vector3Int cell, Direction direction) {
        Cell = cell;
        FacingDirection = direction;

        transform.position = _tileMap.CellToWorld(Cell) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
        _movePoint = transform.position;
    }

    void Update() {
        _animator.SetBool("IsWalking", _isMoving);
        _animator.SetInteger("Direction", (int)FacingDirection);

        if (!transform.position.Equals(_movePoint)) {
            transform.position = Vector3.MoveTowards(transform.position, _movePoint, _movementSpeed * Time.deltaTime);
            if (transform.position.Equals(_movePoint))
                _worldContext.DoEncounterCheck(transform.position);
        } else {
            GenerateNewMovePoint();
        }
    }
    
    /// We're using the new Unity Input System with this one
    private void OnWalkInput(InputAction.CallbackContext context) {
        _isMoving = true;
        _inputContext = context;

        Direction dir = SetDirection(_inputContext.ReadValue<Vector2>());
        FacingDirection = (dir != Direction.NULL) ? dir : FacingDirection;
    }

    private void OnWalkRelease(InputAction.CallbackContext context) {
        _isMoving = false;
    } 

    private bool IsWalkable(Vector3 direction) {
        if (GetComponent<LinecastController>() is not null) {
            return _worldContext.GetTileIsWalkable(transform.position + direction) &&
                GetComponent<LinecastController>().GetLinecastHit(direction) is null;
        } else {
            return _worldContext.GetTileIsWalkable(transform.position + direction);
        }
    }

    /// <summary>
    /// Generates a new Move Point for the player to move to for as long as the input is being held
    /// </summary>
    private void GenerateNewMovePoint() {
        if (!_isMoving) return;
        
        Vector2 readValue = _inputContext.ReadValue<Vector2>();

        if (!IsWalkable((Vector3)readValue)) return;

        Vector3Int CellAddend = new Vector3Int((int)readValue.x, (int)readValue.y, 0);
        
        Direction dir = SetDirection(_inputContext.ReadValue<Vector2>());
        FacingDirection = (dir != Direction.NULL) ? dir : FacingDirection;

        Cell += CellAddend;
        PlayerDataManager.Instance.Data.Cell = Cell;
        PlayerDataManager.Instance.Data.Direction = FacingDirection;
        
        _movePoint = _tileMap.CellToWorld(Cell) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
    }

    private Direction SetDirection(Vector3 readValue) {
        Vector3Int vector = new Vector3Int((int)readValue.x, (int)readValue.y, 0);
        
        if (vector.Equals(Vector3Int.up)) {
            return Direction.UP;
        } else if (vector.Equals(Vector3Int.right)) {
            return Direction.RIGHT;
        } else if (vector.Equals(Vector3Int.down)) {
            return Direction.DOWN;
        } else if (vector.Equals(Vector3Int.left)) {
            return Direction.LEFT;
        } else {
            return Direction.NULL;
        }
    }

    #region Event Listeners

    public void DisableInputActions() {
        _inputActions.Disable();
    }

    public void EnableInputActions() {
        _inputActions.Enable();
    }

    #endregion
}
