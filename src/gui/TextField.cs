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
		public FontProperties Font { get; set; }
		public Color BGColor { get; set; }

		public TextField(Vector2 pos, Vector2 size, int charLimit, FontProperties font, Color bgColor) {
			Rect = new Rectangle(pos.X, pos.Y, size.X, size.Y);
			CharLimit = charLimit;
			Font = font;
			BGColor = bgColor;

			UIEntitySystem.Register(this);
		}

		public override void Render() {
			Raylib.DrawRectangle((int)Rect.x, (int)Rect.y, (int)Rect.width, (int)Rect.height, BGColor);
			
			int drawY = (int)Rect.y;
			if (Font.Size <= Rect.height)
				drawY = (int)( Rect.y + ((Rect.height - Font.Size) / 2)); // vertical align text
			
			Raylib.DrawText(Text, (int)Rect.x, drawY, Font.Size, Font.Color);
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
