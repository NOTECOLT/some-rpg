using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    public TurnHolder CurrentTurn { get; private set; } = TurnHolder.NULL;
    void Start() {
        // Decide who is the first turnholder
    }

    void Update() {
        
    }

    // Called by other entities to switch the current turn
    public void SwitchTurn() {
        CurrentTurn = (CurrentTurn == TurnHolder.PLAYER) ? TurnHolder.ENEMY : TurnHolder.PLAYER;
    }

}
