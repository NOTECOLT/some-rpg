//------------------------------------------------------------------------------------------
/*  UI CARD
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.GUI {
	/// <summary>
	/// The UICard is a special UIEntity used for displaying cards in-battle
	/// </summary>
	public class UICard : UIEntity, IClickable {
		// STATIC FIELDS
		//------------------------------------------------------------------------------------------
		private static float CARD_WIDTH = 125;
		private static float CARD_HEIGHT = 175;

		// FIELDS
		//------------------------------------------------------------------------------------------
		private Card _card;
		private Vector2 _lastMousePos = Vector2.Zero;
		
		public UICard(Vector2 pos, Card card) : base(pos, new Vector2(CARD_WIDTH, CARD_HEIGHT), card.Name + " Card", Color.WHITE) {
			_card = card;

			Image img = new Image(new Vector2(10, 10), new Vector2(105, 75), card.Name + " Image", Color.WHITE, card.Sprite);
			img.CullMouseClick = false;
			img.SetParent(this);

			TextObject txt = new TextObject(new Vector2(10, 95), new Vector2(105, 75), card.Description, card.Name + " Description", new TextStyles(18, Color.BLACK), null);
			txt.CullMouseClick = false;
			txt.SetParent(this);
		}
		
		// ICLICKABLE
		//------------------------------------------------------------------------------------------
		public void OnMousePress(Vector2 mousePos) { 
			_lastMousePos = mousePos;
		}
	
		public void OnMouseRelease(Vector2 mousePos) { }

		public void OnMouseDown(Vector2 mousePos) { 
			Vector2 offset = mousePos - _lastMousePos;
			_lastMousePos = mousePos;

			_absoluteRect.x += offset.X;
			_absoluteRect.y += offset.Y;

			foreach (UIEntity child in Children) {
				child.RelativeRect = new Rectangle() {
					x = child.RelativeRect.x + offset.X,
					y = child.RelativeRect.y + offset.Y,
					width = child.RelativeRect.width,
					height = child.RelativeRect.height
				};
			}
		}
	}
}