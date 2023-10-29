//------------------------------------------------------------------------------------------
/* E(NTITY) DIALOGUE
*/
//------------------------------------------------------------------------------------------
using Topdown.DialogueSystem;

namespace Topdown.ECS {
    /// <summary>
    /// <para> Not to be confused with the Dialogue Class </para>
    /// Connects an Entity to the Dialogue Class
    /// </summary>
    public class EDialogue : Component {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private bool _dialogueActive = false;
        private DialogueManager _dm = null;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
        public Dialogue EntityDialogue { get; private set; } = new Dialogue();

        public EDialogue(Dialogue dialogue, DialogueManager dialogueManager) {
            _dm = dialogueManager;
            EntityDialogue = dialogue;

            // EDialogueSystem.Register(this);
        }

        public override void Destroy() {
            
        }

        public override void Update() {
            if (!_dialogueActive) return;

        }

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
        public void StartDialogue() {
			if (_dialogueActive) return;

			_dm.QueueDialogue(EntityDialogue);
			_dm.NextMessage();		// Plays first message
        }
    }
}