using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.InputSystem;

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
        DisableDialogue();

        // Prototype Code to display choices:     
    }

    void Update() {

    }

    private void DisableDialogue() {
        _dialogueParent.SetActive(false);
        _inputActions.Disable();
        _currentStory = null;
    }

    private void EnableDialogue() {
        _dialogueParent.SetActive(true);
        _inputActions.Enable();
    }
    
    public void TriggerDialogue(TextAsset inkJson) {
        _currentStory = new Story(inkJson.text);
        EnableDialogue();
        OnDialogueOpen.Invoke();

        foreach (Transform child in _dialogueChoiceParent.transform) {
            Destroy(child.gameObject);
        }

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
            DisableDialogue();
            OnDialogueClose.Invoke();
        }
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
            _isWaitingForChoice = true;
            List<Choice> choices = _currentStory.currentChoices;
            
            foreach (Choice c in choices) {
                GameObject choiceObject = Instantiate(_dialogueChoicePrefab, _dialogueChoiceParent.transform);
                choiceObject.GetComponentInChildren<TMP_Text>().text = c.text;
            }
        } else {
            _isWaitingForChoice = false;
            foreach (Transform child in _dialogueChoiceParent.transform) {
                Destroy(child.gameObject);
            }
        }
    }

    private void DisplayChoices() {

    }
}
