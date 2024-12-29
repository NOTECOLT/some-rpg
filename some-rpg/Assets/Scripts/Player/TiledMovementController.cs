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
    public Vector3Int Cell { get; private set; } = Vector3Int.zero;
    public Vector3Int StartCell = Vector3Int.zero;
    private MapManager _mapManager;
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private float _movementSpeed = 20f;
    private bool isMoving = false;
    void Start() {
        transform.position = _tileMap.CellToWorld(StartCell) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
        Debug.Log(transform.position);
        
        _mapManager = FindObjectOfType<MapManager>();
    }

    void Update() {
        if (isMoving) return;
        
        CheckMovementInput();
    }

    private void CheckMovementInput() {
        if (!Input.GetKey(KeyCode.LeftArrow) &&
            !Input.GetKey(KeyCode.RightArrow) &&
            !Input.GetKey(KeyCode.UpArrow) &&
            !Input.GetKey(KeyCode.DownArrow)) return;

        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.left)) return;

            StartCoroutine(MoveTo(Cell + Vector3Int.left, KeyCode.LeftArrow));
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.right)) return;

            StartCoroutine(MoveTo(Cell + Vector3Int.right, KeyCode.RightArrow));
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.up)) return;

            StartCoroutine(MoveTo(Cell + Vector3Int.up, KeyCode.UpArrow));
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            if (!_mapManager.GetTileIsWalkable(transform.position + Vector3Int.down)) return;

            StartCoroutine(MoveTo(Cell + Vector3Int.down, KeyCode.DownArrow));
        } 

        _mapManager.DoEncounterCheck(transform.position);
    }

    // Coroutine for moving the object to specified worldgrid position
    private IEnumerator MoveTo(Vector3Int target, KeyCode keyInput) {
        isMoving = true;
        Cell = target;
        
        Vector2 startPosition = transform.position;
        Vector2 endPosition = _tileMap.CellToWorld(target) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
        
        float elapsedTime = 0;
        float movementTime = Vector2.Distance(startPosition, endPosition) / _movementSpeed;

        while (elapsedTime < movementTime) {
            transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / movementTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (keyInput != 0) transform.position = endPosition;
        isMoving = false;
    }
}
