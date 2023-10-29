//------------------------------------------------------------------------------------------
/* DIALOGUEMANAGER
	SINGLETON
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.Collections.Generic;
using Topdown.GUI;
using Raylib_cs;

namespace Topdown.DialogueSystem {
    public class DialogueManager {
		// FIELDS
		//------------------------------------------------------------------------------------------	
		private UIEntity _dialoguePanel;
		private TextObject _dialogueText;
		private PlayerData _playerData;
		

		// STATIC PROPERTIES
		//------------------------------------------------------------------------------------------
        private static Message _currentMessage;
		public static bool DialogueActive { get; private set; } = false;
        public static Queue<Message> MessageQueue { get; private set; } = new Queue<Message>();

		public DialogueManager(PlayerData playerData) {
			_playerData = playerData;

			TextStyles ts = new TextStyles(30, Color.BLACK) {
				VerticalAlign = Alignment.Top,
				Margin = new Vector4(10, 10, 0, 0)
			};

			_dialoguePanel = new UIEntity(new Vector2(Globals.SCREEN_WIDTH * 0.025f, Globals.SCREEN_HEIGHT * 0.775f), new Vector2(Globals.SCREEN_WIDTH * 0.95f, Globals.SCREEN_HEIGHT * 0.2f), "Dialogue Panel", Color.LIGHTGRAY);
			_dialogueText = new TextObject(new Vector2(0, 0), new Vector2(Globals.SCREEN_WIDTH * 0.95f, Globals.SCREEN_HEIGHT * 0.2f), "Test Text", "Dialogue Text", ts, null);
			_dialogueText.SetParent(_dialoguePanel);

			_dialoguePanel.Enabled = false;
		}

		public void UpdateDialogueBox() {
			if (!DialogueActive) { 
				_dialoguePanel.Enabled = false;
				return;
			}

			_dialoguePanel.Enabled = true;
			_dialogueText.Text = $"{_currentMessage.Name}: {_currentMessage.Text}";

		}

		public void test() {

		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
        public void QueueDialogue(Dialogue dialogue) {
			if (MessageQueue is null) MessageQueue = new Queue<Message>();

			foreach (Message msg in dialogue.Messages) {
				// Only enqueue if the message matches all the flag checks
				//	If the flag checks dictionary is empty, then load the message anyway
				bool meetsFlags = true;

				foreach (KeyValuePair<String, bool> flag in msg.FlagChecks) {
					if (_playerData.Flags[flag.Key] != flag.Value) meetsFlags = false;
				}

				if (meetsFlags) MessageQueue.Enqueue(msg);
			}
        }

        public void NextMessage() {
			if (MessageQueue is null || MessageQueue.Count() == 0) {
				DialogueActive = false;
				return;
			}
			DialogueActive = MessageQueue.Count() != 0;
			_currentMessage = MessageQueue.Dequeue();
		}
    }
}