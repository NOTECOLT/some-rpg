//------------------------------------------------------------------------------------------
/* E(NTITY) DIALOGUE
*/
//------------------------------------------------------------------------------------------
namespace Topdown.ECS {
    /// <summary>
    /// <para> Not to be confused with the Dialogue Class </para>
    /// Connects an Entity to the Dialogue Class
    /// </summary>
    public class EDialogue : Component {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private bool _dialogueActive = false;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
        public Dialogue EntityDialogue { get; private set; } = new Dialogue();

        public EDialogue(Dialogue dialogue) {
            EntityDialogue = dialogue;

            EDialogueSystem.Register(this);
        }

        public override void Update() {
            base.Update();
            if (!_dialogueActive) return;

            // TODO: ADD IINTERACTABLE INTERFACE?
        }

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
        public void StartDialogue() {
			if (_dialogueActive) return;

            // DialogueActive = true;
			foreach (Message msg in EntityDialogue.Messages) {
				Console.WriteLine($"{msg.Name}: {msg.Text}");
            }
            
        }
    }
}