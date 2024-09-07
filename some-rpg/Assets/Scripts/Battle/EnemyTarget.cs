using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Clickable enemy target sprite. Attached to enemy prefabs in battlesystem. <br></br>
/// Does not represent or hold any data pertaining to enemy class.
/// </summary>
public class EnemyTarget : MonoBehaviour, IPointerClickHandler {
    public int TargetId = -1;   // Stores the id of the enemy that the target corresponds in the battle
                                // -1 denotes empty target
    public UnityEvent<int> OnEnemyClicked = new UnityEvent<int>();
    
    public void OnPointerClick(PointerEventData eventData) {
        OnEnemyClicked.Invoke(TargetId);
    }
}
