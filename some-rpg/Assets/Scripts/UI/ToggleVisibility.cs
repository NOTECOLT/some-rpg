using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleVisibility : MonoBehaviour {
    public void MakeVisible() {
        gameObject.SetActive(true);
    }

    public void MakeInvisible() {
        gameObject.SetActive(false);
    }
}
