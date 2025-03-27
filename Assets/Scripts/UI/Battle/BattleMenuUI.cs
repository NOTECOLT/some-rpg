using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Toggles visibility & manages the state of the Battle UI
/// </summary> 
public class BattleMenuUI : MonoBehaviour {
    [SerializeField] TMP_Text _mainTextbox;
    [SerializeField] GameObject _attackButton;
    [SerializeField] GameObject _healButton;
    [SerializeField] GameObject _cancelButton;
    [SerializeField] GameObject _fleeButton;
    private string _currentPlayerName;

    void Start() {
        FindObjectOfType<BattleStateMachine>().OnEnterPlayerTurnState.AddListener(ResetBattleMenuUI);
        FindObjectOfType<BattleStateMachine>().OnEnterActionSequenceState.AddListener(ClearBattleMenuButtons);
    }
    
    #region Listeners

    public void ResetBattleMenuUI(string playerName) {
        _currentPlayerName = playerName;
        _mainTextbox.text = $"What will {playerName} do?";
        _attackButton.SetActive(true);
        _cancelButton.SetActive(false);
        _fleeButton.SetActive(true);
        _healButton.SetActive(true);
    }

    public void ClearBattleMenuButtons() {
        _attackButton.SetActive(false);
        _cancelButton.SetActive(false);
        _fleeButton.SetActive(false);
        _healButton.SetActive(false);
    }

    #endregion

    #region Button Functions

    public void OnAttackButtonClick() {
        _mainTextbox.text = "Who to attack?";
        FindObjectOfType<BattleStateMachine>().SetPlayerAction(ActionType.BASIC_ATTACK);
        _attackButton.SetActive(false);
        _cancelButton.SetActive(true);
        _fleeButton.SetActive(true);
        _healButton.SetActive(false);
    }

    public void OnHealButtonClick() {
        FindObjectOfType<BattleStateMachine>().SetPlayerAction(ActionType.HEAL);
    }

    public void OnCancelButtonClick() {
        ResetBattleMenuUI(_currentPlayerName);
        FindObjectOfType<BattleStateMachine>().SetPlayerActionNull();
    }

    public void ReturnToOverworldScene() {
        FindObjectOfType<BattleStateMachine>().EndBattle();
    }
    #endregion

}
