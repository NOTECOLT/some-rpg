//------------------------------------------------------------------------------------------
/* DIALOGUEMANAGER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.Collections.Generic;
using Topdown.GUI;
using Raylib_cs;

namespace Topdown {
    public class DialogueManager {
		// FIELDS
		//------------------------------------------------------------------------------------------	
		private Panel _dialoguePanel;
		private static Message _currentMessage;

		// STATIC PROPERTIES
		//------------------------------------------------------------------------------------------
        public static bool DialogueActive { get; private set; } = false;
        public static Queue<Message> MessageQueue { get; private set; } = new Queue<Message>();

		public DialogueManager() {
			_dialoguePanel = new Panel(new Vector2(Globals.SCREEN_WIDTH * 0.025f, Globals.SCREEN_HEIGHT * 0.775f), new Vector2(Globals.SCREEN_WIDTH * 0.95f, Globals.SCREEN_HEIGHT * 0.2f), "Test Text", new FontProperties(30, Color.BLACK), Color.LIGHTGRAY);
		}

		public void Render() {
			if (!DialogueActive) { 
				_dialoguePanel.Enabled = false;
				return;
			}

			_dialoguePanel.Enabled = true;
			_dialoguePanel.Text = $"{_currentMessage.Name}: {_currentMessage.Text}";

		}

		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
        public static void QueueDialogue(Dialogue dialogue) {
			if (MessageQueue is null) MessageQueue = new Queue<Message>();

			foreach (Message msg in dialogue.Messages) {
				MessageQueue.Enqueue(msg);
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