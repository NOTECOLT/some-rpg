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
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private float _movementSpeed = 20f;
    private bool isMoving = false;
    private Animator _animator = null;
    void Start() {
        Application.targetFrameRate = -1;
        if (PlayerData.Instance is not null) {
            StartCell = PlayerData.Instance.Cell;
            Cell = PlayerData.Instance.Cell;
        }

        transform.position = _tileMap.CellToWorld(StartCell) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
        
        _mapManager = FindObjectOfType<MapManager>();

        _animator = GetComponent<Animator>();
    }

    void Update() {
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

            StartCoroutine(MoveTo(Cell + Vector3Int.left, KeyCode.LeftArrow, ANIMATION_DIRECTION_LEFT));
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.right)) return;

            StartCoroutine(MoveTo(Cell + Vector3Int.right, KeyCode.RightArrow, ANIMATION_DIRECTION_RIGHT));
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.up)) return;

            StartCoroutine(MoveTo(Cell + Vector3Int.up, KeyCode.UpArrow, ANIMATION_DIRECTION_UP));
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.down)) return;

            StartCoroutine(MoveTo(Cell + Vector3Int.down, KeyCode.DownArrow, ANIMATION_DIRECTION_DOWN));
        } 

        _mapManager.DoEncounterCheck(transform.position);
    }

    // Coroutine for moving the object to specified worldgrid position
    private IEnumerator MoveTo(Vector3Int target, KeyCode keyInput, int animationDirection) {
        isMoving = true;
        Cell = target;
        PlayerData.Instance.Cell = target;
        
        Vector2 startPosition = transform.position;
        Vector2 endPosition = _tileMap.CellToWorld(target) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
        
        float elapsedTime = 0;
        float movementTime = Vector2.Distance(startPosition, endPosition) / _movementSpeed;
        
        _animator.SetInteger("Direction", animationDirection);
        _animator.SetBool("IsWalking", true);

        while (elapsedTime < movementTime) {
            transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / movementTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (keyInput != 0) transform.position = endPosition;
        isMoving = false;
        _animator.SetBool("IsWalking", false);
    }
}
