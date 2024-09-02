using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    public BattleState CurrentState { get; private set; } = BattleState.PLAYER_SELECT;

    [SerializeField] private TMP_Text _mainTextbox;
    void Start() {
        // Decide who is the first turnholder
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            UpdateState();
            _mainTextbox.text = CurrentState.ToString();
        }
    }

    // Called by other entities to switch the current turn
    public void UpdateState() {
        int states = Enum.GetNames(typeof(BattleState)).Length;
        CurrentState = (BattleState)((int)(CurrentState + 1) % states);
    }

}
