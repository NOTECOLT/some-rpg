//------------------------------------------------------------------------------------------
/* DIALOGUE
*/
//------------------------------------------------------------------------------------------
namespace Topdown {
    public class Dialogue {
        public List<Message> Messages { get; set; }

        public Dialogue() {
            Messages = new List<Message>();
        }
    }

//------------------------------------------------------------------------------------------
/* MESSAGE
*/
//------------------------------------------------------------------------------------------

    public class Message {
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
        
        public Message(string text, string name = "") {
            Text = text;
            Name = name;
        }
    }
}