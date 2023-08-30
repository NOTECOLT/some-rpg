//------------------------------------------------------------------------------------------
/*  Panel
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.GUI {
    /// <summary>
    /// A non-clickable UI element with text and a background
    /// </summary>
    public class Panel : UIEntity {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public String Text { get; set; }
		public FontProperties Font { get; private set; }
		public Color BGColor { get; private set; }	

		public Panel(Vector2 pos, Vector2 size, String text, FontProperties font, Color bgColor) {
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
	}
}