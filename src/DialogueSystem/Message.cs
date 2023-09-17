//------------------------------------------------------------------------------------------
/* MESSAGE
*/
//------------------------------------------------------------------------------------------
namespace Topdown.DialogueSystem {
    public class Message {
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
        public Dictionary<String, bool> FlagChecks = new Dictionary<String, bool>();

        public Message(string text, string name = "") {
            Text = text;
            Name = name;
        }
    }
}