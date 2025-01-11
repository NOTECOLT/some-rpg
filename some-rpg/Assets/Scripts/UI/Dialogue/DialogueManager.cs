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
    [SerializeField] private TextAsset _dialogue;
    private Story _story;

    [SerializeField] private TMP_Text _dialogueText;

    void Awake() {
        _story = new Story(_dialogue.text);

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

        
        // if (_story.currentChoices.Count > 0) {
        //     for (int i = 0; i < _story.currentChoices.Count; i++) {
        //         Choice choice = _story.currentChoices[i];
        //         Debug.Log("     " + (i + 1) + "." + choice.text);
        //     }
        // }        
    }

    void Update() {

    }

    private void OnDialogueNext(InputAction.CallbackContext context) {
        if (_story.canContinue) {
            _dialogueText.text = _story.Continue();
        }
    }
}
