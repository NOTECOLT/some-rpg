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
    public Enemy Enemy = null;
    
    public UnityEvent<Enemy> OnEnemyClicked = new UnityEvent<Enemy>();
    
    public void OnPointerClick(PointerEventData eventData) {
        OnEnemyClicked.Invoke(Enemy);
    }
}
