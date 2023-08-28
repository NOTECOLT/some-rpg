//------------------------------------------------------------------------------------------
/* XMLDIALOGUEPARSER
*/
//------------------------------------------------------------------------------------------
using System.Xml;

namespace Topdown {
	public static class XMLDialogueParser {
		/// <summary>
		/// Parses an XML file and converts it into a dialogue object.
		/// </summary>
		/// <param name="path"></param>
        public static Dialogue LoadDialogueFromFile(string path) {
			if (!File.Exists(path)) {
				Console.WriteLine($"[DIALOGUE MANAGER] File {path} not found!");
				return null;
			}

			// 1 - FILE READ
			//--------------------------------------------------
			string fileText = File.ReadAllText(path);
			// Console.WriteLine($"[DIALOGUE MANAGER] XML File Read: \n{fileText}");

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

			string name = "", text = "";
			foreach (XmlNode child in node.ChildNodes) {
				if (child.Name == "name") name = child.InnerText;
				if (child.Name == "text") text = child.InnerText;
			}

			return new Message(text, name);
        }
    }

}