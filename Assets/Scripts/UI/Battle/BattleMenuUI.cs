using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Toggles visibility & manages the state of the Battle UI
/// </summary> 
public class BattleMenuUI : MonoBehaviour {
    private bool _isInventoryOpen = false;

    [SerializeField] TMP_Text _mainTextbox;
    [SerializeField] GameObject _attackButton;
    [SerializeField] GameObject _invButton;
    [SerializeField] GameObject _cancelButton;
    [SerializeField] GameObject _backButton;
    [SerializeField] GameObject _fleeButton;

    [SerializeField] GameObject _inventoryParent;
    [SerializeField] BattleStateMachine _battleStateMachine;

    
    private string _currentPlayerName;

    void Start() {
        _battleStateMachine.OnResetBattleMenuUI.AddListener(ResetBattleMenuUI);
        _battleStateMachine.OnEnterActionSequenceState.AddListener(ClearBattleMenuButtons);
    }

    #region Listeners

    public void ResetBattleMenuUI(string playerName) {
        _currentPlayerName = playerName;
        _mainTextbox.text = $"What will {playerName} do?";
        _attackButton.SetActive(true);
        _cancelButton.SetActive(false);
        _fleeButton.SetActive(true);
        _invButton.SetActive(true);
        _backButton.SetActive(true);
    }

    public void ClearBattleMenuButtons() {
        _attackButton.SetActive(false);
        _cancelButton.SetActive(false);
        _fleeButton.SetActive(false);
        _invButton.SetActive(false);
        _backButton.SetActive(false);
    }

    #endregion

    #region Button Functions

    public void OnAttackButtonClick() {
        _mainTextbox.text = "Who to attack?";
        _battleStateMachine.SetPlayerAction(ActionType.BASIC_ATTACK);
        _attackButton.SetActive(false);
        _cancelButton.SetActive(true);
        _fleeButton.SetActive(true);
        _invButton.SetActive(false);
        _backButton.SetActive(false);
    }

    public void OnInventoryButtonClick() {
        _isInventoryOpen = !_isInventoryOpen;
        _inventoryParent.SetActive(_isInventoryOpen);
    }

    public void OnCancelButtonClick() {
        ResetBattleMenuUI(_currentPlayerName);
        _battleStateMachine.SetPlayerActionNull();
    }

    public void OnBackButtonClick() {
        _battleStateMachine.CancelCurrentPlayerAction();
    }

    public void ReturnToOverworldScene() {
        _battleStateMachine.EndBattle();
    }
    #endregion

}
