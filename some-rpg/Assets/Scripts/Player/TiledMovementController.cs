using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Used to control player movement in a tiled grid
/// </summary>
public class TiledMovementController : MonoBehaviour {
    // Animation Directions integers as defined by the animator controller parameters
    // Integers follow the order "Never (North, Up) Eat (East, Right) Soggy (South, Down) Waffles (West, Left)"
    private static int ANIMATION_DIRECTION_UP = 0;
    private static int ANIMATION_DIRECTION_RIGHT = 1;
    private static int ANIMATION_DIRECTION_DOWN = 2;
    private static int ANIMATION_DIRECTION_LEFT = 3;

    public Vector3Int Cell { get; private set; } = Vector3Int.zero;
    public Vector3Int StartCell = Vector3Int.zero;
    private MapManager _mapManager;
    private Vector3 _movePoint;
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private float _movementSpeed = 20f;
    private bool isMoving = false;
    private Animator _animator = null;
    void Start() {
        Application.targetFrameRate = 60;
        if (PlayerData.Instance is not null) {
            StartCell = PlayerData.Instance.Cell;
            Cell = PlayerData.Instance.Cell;
        }

        transform.position = _tileMap.CellToWorld(StartCell) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
        
        _mapManager = FindObjectOfType<MapManager>();
        _animator = GetComponent<Animator>();

        _movePoint = transform.position;
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, _movePoint, _movementSpeed * Time.deltaTime);
        if (transform.position.Equals(_movePoint)) {
            isMoving = false;
            _animator.SetBool("IsWalking", false);
        }


        if (isMoving) return;
        CheckMovementInput();
    }

    private void CheckMovementInput() {
        if (!Input.GetKey(KeyCode.LeftArrow) &&
            !Input.GetKey(KeyCode.RightArrow) &&
            !Input.GetKey(KeyCode.UpArrow) &&
            !Input.GetKey(KeyCode.DownArrow)) {
            return;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.left)) return;

            SetNewMovePoint(Cell + Vector3Int.left, ANIMATION_DIRECTION_LEFT);
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.right)) return;

            SetNewMovePoint(Cell + Vector3Int.right, ANIMATION_DIRECTION_RIGHT);
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.up)) return;

            SetNewMovePoint(Cell + Vector3Int.up, ANIMATION_DIRECTION_UP);
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.down)) return;

            SetNewMovePoint(Cell + Vector3Int.down, ANIMATION_DIRECTION_DOWN);
        } 

        _mapManager.DoEncounterCheck(transform.position);
    }
    
    private void SetNewMovePoint(Vector3Int target, int animationDirection) {
        isMoving = true;
        Cell = target;
        PlayerData.Instance.Cell = target;
        
        _movePoint = _tileMap.CellToWorld(target) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
        
        if (_animator.GetBool("IsWalking") != true)
            _animator.SetBool("IsWalking", true);

        if (_animator.GetInteger("Direction") != animationDirection)
            _animator.SetInteger("Direction", animationDirection);
    }
}
