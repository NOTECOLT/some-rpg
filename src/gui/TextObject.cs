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
		public TextStyles TextStyle { get; private set; }

		/// <summary>
		/// Regular Constructor
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="text"></param>
		/// <param name="textStyle"></param>
		/// <param name="bgColor"></param>
		public TextObject(Vector2 pos, Vector2 size, String text, TextStyles textStyle, Color? bgColor) : base(pos, size, bgColor) {
			Text = text;
			TextStyle = textStyle;
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
			switch (TextStyle.VerticalAlign) {	
				case VerticalAlignment.Top:
					drawY += (int)TextStyle.Margin.Y;
					break;
				case VerticalAlignment.Center:
					if (TextStyle.Size <= _absoluteRect.height)
						drawY = (int)(_absoluteRect.y + ((_absoluteRect.height - TextStyle.Size) / 2));
						// drawY += (int)Font.Margin.Y;
					break;
				case VerticalAlignment.Bottom:
					// TODO: This
					throw new Exception("No Vertical Align Bottom Setting");
				default:
					break;
			}

			DrawTextWrap(Text, (int)(_absoluteRect.x + TextStyle.Margin.X), drawY, TextStyle.Size, TextStyle.Color);	
		}

		private static void DrawTextWrap(string text, int posX, int posY, int size, Color color) {
			Raylib.DrawText(text, posX, posY, size, color);
		}
	}
}