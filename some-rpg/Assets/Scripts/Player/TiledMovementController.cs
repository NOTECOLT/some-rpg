using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

// Used to control tiled player movement on a grid
public class TiledMovementController : MonoBehaviour {
    public Vector2Int GridPosition { get; private set; } = Vector2Int.zero;
    [SerializeField] private Grid _worldGrid;
    [SerializeField] private float _movementSpeed = 20f;
    private bool isMoving = false;
    void Start() {
        transform.position = new Vector3(GridPosition.x * _worldGrid.cellSize.x, GridPosition.y * _worldGrid.cellSize.y,0);
    }

    void Update() {
        if (isMoving) return;

        if (Input.GetKey(KeyCode.LeftArrow)) {
            StartCoroutine(MoveTo(GridPosition + Vector2Int.left, KeyCode.LeftArrow));
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            StartCoroutine(MoveTo(GridPosition + Vector2Int.right, KeyCode.RightArrow));
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            StartCoroutine(MoveTo(GridPosition + Vector2Int.up, KeyCode.UpArrow));
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            StartCoroutine(MoveTo(GridPosition + Vector2Int.down, KeyCode.DownArrow));
        } 
    }

    // Coroutine for moving the object to specified worldgrid position
    private IEnumerator MoveTo(Vector2Int target, KeyCode inputCode) {
        isMoving = true;
        GridPosition = target;
        
        Vector2 startPos = transform.position;
        Vector2 endPos = new Vector2(target.x * _worldGrid.cellSize.x, target.y * _worldGrid.cellSize.y);
        
        float elapsedTime = 0;
        float movementTime = Vector2.Distance(startPos, endPos) / _movementSpeed;

        while (elapsedTime < movementTime) {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / movementTime;

            transform.position = Vector2.Lerp(startPos, endPos, percent);
            yield return null;
        }

        if (!Input.GetKey(inputCode))
            transform.position = endPos;
        isMoving = false;
    }
}
