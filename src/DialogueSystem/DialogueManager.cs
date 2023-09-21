//------------------------------------------------------------------------------------------
/* DIALOGUEMANAGER
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
		private static Message _currentMessage;

		// STATIC PROPERTIES
		//------------------------------------------------------------------------------------------
        public static bool DialogueActive { get; private set; } = false;
        public static Queue<Message> MessageQueue { get; private set; } = new Queue<Message>();

		public DialogueManager() {
			TextStyles fp = new TextStyles(30, Color.BLACK) {
				VerticalAlign = Alignment.Top,
				Margin = new Vector4(10, 10, 0, 0)
			};


			_dialoguePanel = new UIEntity(new Vector2(Globals.ScreenWidth * 0.025f, Globals.ScreenHeight * 0.775f), new Vector2(Globals.ScreenWidth * 0.95f, Globals.ScreenHeight * 0.2f), Color.LIGHTGRAY);
			_dialogueText = new TextObject(new Vector2(0, 0), new Vector2(Globals.ScreenWidth * 0.95f, Globals.ScreenHeight * 0.2f), "Test Text", fp, null);
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

		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
        public static void QueueDialogue(Dialogue dialogue) {
			if (MessageQueue is null) MessageQueue = new Queue<Message>();

			foreach (Message msg in dialogue.Messages) {
				// Only enqueue if the message matches all the flag checks
				//	If the flag checks dictionary is empty, then load the message anyway
				bool meetsFlags = true;

				foreach (KeyValuePair<String, bool> flag in msg.FlagChecks) {
					if (Game.PlayerSaveData.Flags[flag.Key] != flag.Value) meetsFlags = false;
				}

				if (meetsFlags) MessageQueue.Enqueue(msg);
			}
        }

        public static void NextMessage() {
			if (MessageQueue is null || MessageQueue.Count() == 0) {
				DialogueActive = false;
				return;
			}
			DialogueActive = MessageQueue.Count() != 0;
			_currentMessage = MessageQueue.Dequeue();
			
		}
    }
}