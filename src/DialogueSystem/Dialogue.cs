//------------------------------------------------------------------------------------------
/* DIALOGUE
*/
//------------------------------------------------------------------------------------------
namespace Topdown.DialogueSystem {
    public class Dialogue {
        public List<Message> Messages { get; set; }

        public Dialogue() {
            Messages = new List<Message>();
        }
    }
}