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
		public TextStyles Font { get; private set; }

		/// <summary>
		/// Regular Constructor
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="bgColor"></param>
		public Button(Vector2 pos, Vector2 size, String text, TextStyles font, Color? bgColor) : base(pos, size, bgColor) {
			Text = text;
			Font = font;
		}

		// /// <summary>
		// /// With Parent Constructor
		// /// </summary>
		// /// <param name="parent"></param>
		// /// <param name="pos"></param>
		// /// <param name="size"></param>
		// /// <param name="text"></param>
		// /// <param name="font"></param>
		// /// <param name="bgColor"></param>
		// public Button(UIEntity parent, Vector2 pos, Vector2 size, String text, FontProperties font, Color? bgColor) : base(parent, pos, size, bgColor) {
		// 	Text = text;
		// 	Font = font;
		// }

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public override void Render() {
			base.Render();
			
			int drawY = (int)_absoluteRect.y;
			if (Font.Size <= _absoluteRect.height)
				drawY = (int)( _absoluteRect.y + ((_absoluteRect.height - Font.Size) / 2)); // vertical align text
			
			Raylib.DrawText(Text, (int)_absoluteRect.x, drawY, Font.Size, Font.Color);
		}
		
		public bool OnClick(Vector2 mousePos) {
			if (!Enabled) return false;
			return Raylib.CheckCollisionPointRec(mousePos, Rect) && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
		}
	}
}