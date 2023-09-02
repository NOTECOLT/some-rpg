//------------------------------------------------------------------------------------------
/*  TEXT FIELD
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;


namespace Topdown.GUI {
	/// <summary>
	/// Allows a user to input a text prompt and submit it
	/// </summary>
    public class TextField : UIEntity {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public bool IsAcceptingText { get; set; } = false;	
		public string Text { get; set; }
		public int CharLimit { get; set; } = 100;
		public TextStyles Font { get; set; }

		/// <summary>
		/// Regular Constructor
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="charLimit"></param>
		/// <param name="font"></param>
		/// <param name="bgColor"></param>
		public TextField(Vector2 pos, Vector2 size, int charLimit, TextStyles font, Color? bgColor) : base(pos, size, bgColor) {
			CharLimit = charLimit;
			Font = font;
		}

		public override void Render() {		
			base.Render();
				
			int drawY = (int)_absoluteRect.y;
			if (Font.Size <= _absoluteRect.height)
				drawY = (int)(_absoluteRect.y + ((_absoluteRect.height - Font.Size) / 2)); // vertical align text
			
			Raylib.DrawText(Text, (int)_absoluteRect.x, drawY, Font.Size, Font.Color);
		}

		public void UpdateTextField() {
			if (!Enabled || !IsAcceptingText) return;
			// I used the Text Field example on raylib's site for this

			int key = Raylib.GetCharPressed();
			while (key > 0) {
				if ((key >= 32) && (key < 125) && (Text.Length < CharLimit)) {
                    Text += (char)key;
				}

				key = Raylib.GetCharPressed();
			}

			if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE) && Text.Length > 0) {
					Text = Text.Remove(Text.Length - 1);
			}
		}

		public void ResetTextField() {
			Text = "";
		}
    }
}
