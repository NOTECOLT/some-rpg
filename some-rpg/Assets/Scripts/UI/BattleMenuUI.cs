using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Toggles visibility & manages the state of the Battle UI
/// </summary> 
public class BattleMenuUI : MonoBehaviour {
    [SerializeField] private GameObject playerButtons;
    [SerializeField] private GameObject mainMenuButtons;

    public void SetPlayerTurnMenu() {
        playerButtons.SetActive(true);
        mainMenuButtons.SetActive(true);
    }

    public void SetPlayerSelectAttackMenu() {
        playerButtons.SetActive(true);
        mainMenuButtons.SetActive(false);
    }
}
