using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform focus;

    // Update is called once per frame
    void Update() {
        if (focus == null) return;

        transform.position = new Vector3(focus.position.x, focus.position.y, transform.position.z);
    }
}
