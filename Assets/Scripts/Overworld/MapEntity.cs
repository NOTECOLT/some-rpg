using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEntity : MonoBehaviour, IInteractable {
    public Vector3Int Cell { get; private set; } = Vector3Int.zero;
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private TextAsset _interactDialogue;

    // Start is called before the first frame update
    void Start() {
        Cell = _tileMap.WorldToCell(transform.position);
        // Make sure we snap the object to the grid
        transform.position = _tileMap.CellToWorld(Cell) + new Vector3(_tileMap.cellSize.x / 2, _tileMap.cellSize.y / 2, 0);
    }

    public void OnInteract() {
        FindObjectOfType<DialogueManager>().TriggerDialogue(_interactDialogue);
    }
}
