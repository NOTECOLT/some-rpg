//------------------------------------------------------------------------------------------
/*  TEXT OBJECT
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.GUI {
    /// <summary>
    /// Text Object
    /// </summary>
    public class TextObject : UIEntity {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public String Text { get; set; }
		public FontProperties Font { get; private set; }

		/// <summary>
		/// Regular Constructor
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <param name="bgColor"></param>
		public TextObject(Vector2 pos, Vector2 size, String text, FontProperties font, Color? bgColor) : base(pos, size, bgColor) {
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
		// public TextObject(UIEntity parent, Vector2 pos, Vector2 size, String text, FontProperties font, Color? bgColor) : base(parent, pos, size, bgColor) {
		// 	Text = text;
		// 	Font = font;
		// }

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public override void Render() {
			base.Render();
			
			int drawY = (int)_absoluteRect.y;

			// TODO: Fix vertical alignment, to work with multiple lines of string
			switch (Font.VerticalAlign) {	
				case VerticalAlignment.Top:
					drawY += (int)Font.Margin.Y;
					break;
				case VerticalAlignment.Center:
					if (Font.Size <= _absoluteRect.height)
						drawY = (int)(_absoluteRect.y + ((_absoluteRect.height - Font.Size) / 2));
						// drawY += (int)Font.Margin.Y;
					break;
				case VerticalAlignment.Bottom:
					// TODO: This
					throw new Exception("No Vertical Align Bottom Setting");
				default:
					break;
			}
			

			
			Raylib.DrawText(Text, (int)(_absoluteRect.x + Font.Margin.X), drawY, Font.Size, Font.Color);
		}
	}
}