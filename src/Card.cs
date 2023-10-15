//------------------------------------------------------------------------------------------
/* CARD
*/
//------------------------------------------------------------------------------------------

using Topdown.Renderer;

namespace Topdown {
	/// <summary>
	/// Data Object for each card
	/// </summary>
	public class Card {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public String Name { get; private set; }
		public Sprite Sprite { get; private set; }
		public String Description { get; private set; }
		public int Quantity { get; set; }

		public Card(string name, Sprite sprite, String description, int quantity) {
			Name = name;
			Sprite = sprite;
			Description = description;
			Quantity = quantity;
		}
	}
}