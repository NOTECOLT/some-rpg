using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Used to manage the progression of dialogue.
/// We use Ink as a scripting language for dialogue
/// </summary>
public class DialogueManager : MonoBehaviour {
    /// <summary>
    /// Expressed in characters per second
    /// </summary>
    private static float WRITE_SPEED = 70.0f;
    private DialogueAction _inputActions;
    private Story _currentStory = null;
    private bool _isWritingLine = false;
    private bool _isWaitingForChoice = false;
    private bool _autoFinishLine = false;

    [SerializeField] private GameObject _dialogueParent;
    [SerializeField] private GameObject _dialogueChoiceParent;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject _dialogueChoicePrefab;

    public static event System.Action OnDialogueOpen;
    public static event System.Action OnDialogueClose;

    void Awake() {
        _inputActions = new DialogueAction();
        _inputActions.Dialogue.Next.performed += OnNextInput;
    }

    void OnEnable() {
        _inputActions.Enable();
    }

    void OnDisable() {
        _inputActions.Disable();
    }

    void OnDestroy() {
        _inputActions.Dialogue.Next.performed -= OnNextInput;
    }

    void Start() {
        // Initialize the game by disabling dialogue
        DisableDialogueUI();

        // Prototype Code to display choices:     
    }

    void Update() {

    }

    #region Functions

    private static string FUNC_EQUIP_WEAPON = "equipWeapon";

    private void EquipWeapon(string weaponid, int playerid) {
        // Access the ItemDatabase component to be able to reference item ids
        // ? Not my favorite solution but i'll keep it like this for now 
        PlayerDataManager.Instance.Data.PartyStats[playerid].Weapon = new WeaponItem(GameStateMachine.Instance.Weapons[weaponid]);
        Utils.Log($"[Dialogue Manager] Script Function Executed: {FUNC_EQUIP_WEAPON}({weaponid})");
    }
    
    #endregion

    private void DisableDialogueUI() {
        _dialogueParent.SetActive(false);
        _inputActions.Disable();
    }

    private void EnableDialogueUI() {
        _dialogueParent.SetActive(true);
        _inputActions.Enable();

        ClearChoicesUI();
    }

    private void ClearChoicesUI() {
        foreach (Transform child in _dialogueChoiceParent.transform) {
            child.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(child.gameObject);
        }
    }
    
    public void TriggerDialogue(TextAsset inkJson) {
        _currentStory = new Story(inkJson.text);
        EnableDialogueUI();
        OnDialogueOpen?.Invoke();

        _currentStory.BindExternalFunction(FUNC_EQUIP_WEAPON, (string weaponid, int playerid) => EquipWeapon(weaponid, playerid));

        // Display the first line
        ContinueDialogue();
    }

    private void OnNextInput(InputAction.CallbackContext context) {
        if (!_isWritingLine && !_isWaitingForChoice) {
            ContinueDialogue();
        } else if (_isWritingLine) {
            _autoFinishLine = true;
        }
    }

    private void ContinueDialogue() {
        if (_currentStory.canContinue) {
            StartCoroutine(WriteLine(_currentStory.Continue(), WRITE_SPEED));
        } else {
            DisableDialogueUI();
            _currentStory.UnbindExternalFunction(FUNC_EQUIP_WEAPON);
            _currentStory = null;
            OnDialogueClose?.Invoke();
        }
    }

    public void ChooseOption(int choiceIndex) {
        _isWaitingForChoice = false;
        _currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueDialogue();
    }

    /// <summary>
    /// Writes a line character by character
    /// </summary>
    /// <param name="line"></param>
    /// <param name="writeSpeed">Expressed in characters per second</param>
    /// <returns></returns>
    private IEnumerator WriteLine(string line, float writeSpeed) {
        // LINE WRITING
        _isWritingLine = true;
        _dialogueText.text = "";
        foreach (char c in line) {
            _dialogueText.text += c;
            yield return new WaitForSeconds(1/writeSpeed);

            // Auto finish writing the line if the player presses the next key while writing
            if (_autoFinishLine) {
                _autoFinishLine = false;
                _dialogueText.text = line;
                break;
            }
        }

        _isWritingLine = false;

        // DISPLAY CHOICES
        if (_currentStory.currentChoices.Count > 0) {
            ClearChoicesUI();

            _isWaitingForChoice = true;
            List<Choice> choices = _currentStory.currentChoices;
            
            foreach (Choice c in choices) {
                GameObject choiceObject = Instantiate(_dialogueChoicePrefab, _dialogueChoiceParent.transform);
                choiceObject.GetComponentInChildren<TMP_Text>().text = c.text;
                choiceObject.GetComponent<Button>().onClick.AddListener(() => { ChooseOption(c.index); });
            }
        } else {
            ClearChoicesUI();
        }
    }
}
