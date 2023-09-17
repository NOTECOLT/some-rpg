//------------------------------------------------------------------------------------------
/* DIALOGUEPARSER
*/
//------------------------------------------------------------------------------------------
using System.Xml;

namespace Topdown.DialogueSystem {
	public static class DialogueParser {
		/// <summary>
		/// Parses an XML file and converts it into a dialogue object.
		/// </summary>
		/// <param name="path"></param>
        public static Dialogue LoadDialogueFromFile(string path) {
			if (!File.Exists(path)) {
				throw new FileLoadException($"{path} does not exist");
			}

			// 1 - FILE READ
			//--------------------------------------------------
			string fileText = File.ReadAllText(path);

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fileText);

			// 2 - XML PARSE
			//--------------------------------------------------
			XmlElement root = xml.DocumentElement;
			Dialogue dialogue = new Dialogue();
			
			foreach (XmlNode node in root) {
				Message msg = ParseMessage(node);
				if (msg is not null) dialogue.Messages.Add(msg);
			}

			return dialogue;
		}

        public static Message ParseMessage(XmlNode node) {
			if (node.Name != "message") return null;

			Message msg = new Message("");

			foreach (XmlNode child in node.ChildNodes) {
				switch (child.Name) {
					case "name":
						msg.Name = child.InnerText;
						break;
					case "text":
						msg.Text = child.InnerText;
						break;
					case "flagChecks":
						foreach (XmlElement flag in child)
							msg.FlagChecks[flag.GetAttribute("name")] = Convert.ToBoolean(flag.GetAttribute("status"));
						break;
					default:
						break;
				}
			}


			return msg;
        }
    }

}