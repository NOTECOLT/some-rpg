//------------------------------------------------------------------------------------------
/*  BUTTON
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.GUI {
	/// <summary>
	/// A clickable UI element
	/// </summary>
    public class Button : UIEntity {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public String Text { get; private set; }
		public FontProperties Font { get; private set; }
		public Color BGColor { get; private set; }	

		public Button(Vector2 pos, Vector2 size, String text, FontProperties font, Color bgColor) {
			Rect = new Rectangle(pos.X, pos.Y, size.X, size.Y);
			Text = text;
			Font = font;
			BGColor = bgColor;

			UIEntitySystem.Register(this);
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public override void Render() {
			Raylib.DrawRectangle((int)Rect.x, (int)Rect.y, (int)Rect.width, (int)Rect.height, BGColor);
			
			int drawY = (int)Rect.y;
			if (Font.Size <= Rect.height)
				drawY = (int)( Rect.y + ((Rect.height - Font.Size) / 2)); // vertical align text
			
			Raylib.DrawText(Text, (int)Rect.x, drawY, Font.Size, Font.Color);
		}
		
		public bool OnClick(Vector2 mousePos) {
			if (!Enabled) return false;
			return Raylib.CheckCollisionPointRec(mousePos, Rect) && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
		}
	}
}