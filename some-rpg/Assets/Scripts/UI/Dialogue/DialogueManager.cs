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
    private DialogueAction _inputActions;
    private Story _currentStory = null;

    [SerializeField] private GameObject _dialogueParent;
    [SerializeField] private TMP_Text _dialogueText;

    public static event System.Action OnDialogueOpen;
    public static event System.Action OnDialogueClose;

    void Awake() {
        _inputActions = new DialogueAction();
        _inputActions.Dialogue.Next.performed += OnDialogueNext;
    }

    void OnEnable() {
        _inputActions.Enable();
    }

    void OnDisable() {
        _inputActions.Disable();
    }

    void OnDestroy() {
        _inputActions.Dialogue.Next.performed -= OnDialogueNext;
    }

    void Start() {
        // Initialize the game by disabling dialogue
        DisableDialogue();

        // Prototype Code to display choices:
        // if (_story.currentChoices.Count > 0) {
        //     for (int i = 0; i < _story.currentChoices.Count; i++) {
        //         Choice choice = _story.currentChoices[i];
        //         Debug.Log("     " + (i + 1) + "." + choice.text);
        //     }
        // }        
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
        // Display the first line
        if (_currentStory.canContinue)
            _dialogueText.text = _currentStory.Continue();
    }

    private void OnDialogueNext(InputAction.CallbackContext context) {
        if (_currentStory.canContinue) {
            _dialogueText.text = _currentStory.Continue();
        } else {
            DisableDialogue();
            OnDialogueClose.Invoke();
        }
    }

    private IEnumerator DisplayLine() {
        yield return null;
    }
}
