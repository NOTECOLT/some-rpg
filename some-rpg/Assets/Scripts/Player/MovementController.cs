using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class MovementController : MonoBehaviour {
    [SerializeField] private float movementSpeed = 20f;
    private Vector2 movement = Vector2.zero;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            movement.x = -movementSpeed;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            movement.x = movementSpeed;
        } else {
            movement.x = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            movement.y = movementSpeed;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            movement.y = -movementSpeed;
        } else {
            movement.y = 0;
        }

        transform.position += new Vector3(movement.x, movement.y, 0) * Time.deltaTime;
    }
}
