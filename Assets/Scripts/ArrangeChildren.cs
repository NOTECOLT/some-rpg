using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uniformly distributes all children between the rect bounded by pointA and pointB
/// </summary>
public class ArrangeChildren : MonoBehaviour {
    public Vector3 pointA;
    public Vector3 pointB;

    public bool snapX = false;
    public bool snapY = false;

    void Start() {
        Arrange();
    }

    void Update() {
        Debug.DrawLine(transform.position+pointA, transform.position+pointB, Color.red);
    }

    public void Arrange() {
        float interval = (pointB.y - pointA.y) / (transform.childCount + 1);
        for (int i = 0; i < transform.childCount; i++) {
            float x,y;
            x = snapX ? (pointA.x + pointB.x) / 2 : pointA.x + interval*(i + 1);
            y = snapY ? (pointA.y + pointB.y) / 2 : pointA.y + interval*(i + 1);
            Vector3 childPos = new Vector3(x, y, pointA.z);
            transform.GetChild(i).localPosition = childPos; 
        }
    }
}
