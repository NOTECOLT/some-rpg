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
    [SerializeField] GameObject _cancelButton;
    [SerializeField] GameObject _fleeButton;

    void Start() {
        FindObjectOfType<BattleStateMachine>().OnEnterPlayerTurnState.AddListener(ResetBattleMenuUI);
        FindObjectOfType<BattleStateMachine>().OnEnterActionSequenceState.AddListener(ClearBattleMenuButtons);
    }
    
    // LISTENERS ---------------------------------------------

    public void ResetBattleMenuUI() {
        _mainTextbox.text = "What will player do?";
        _attackButton.SetActive(true);
        _cancelButton.SetActive(false);
        _fleeButton.SetActive(true);
    }

    public void ClearBattleMenuButtons() {
        _attackButton.SetActive(false);
        _cancelButton.SetActive(false);
        _fleeButton.SetActive(false);
    }

    // BUTTON FUNCTIONS ---------------------------------------------

    public void OnAttackButtonClick() {
        _mainTextbox.text = "Who to attack?";
        FindObjectOfType<BattleStateMachine>().SetPlayerAction(ActionType.ATTACK);
        _attackButton.SetActive(false);
        _cancelButton.SetActive(true);
        _fleeButton.SetActive(true);
    }

    public void OnCancelButtonClick() {
        ResetBattleMenuUI();
        FindObjectOfType<BattleStateMachine>().SetPlayerActionNull();
    }

    public void ReturnToOverworldScene() {
        SceneLoader.Instance.LoadOverworld();
    }
    // --------------------------------------------------------------

}
