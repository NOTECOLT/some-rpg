//------------------------------------------------------------------------------------------
/*  TEXT FIELD
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;


namespace Topdown.GUI {
    public class TextField {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private bool _enabled = false;
		private bool _isAcceptingText = false;
		private Rectangle _rect;
		private String _text = "";
		private int _charLimit = 100;
		private FontProperties _font;
		private Color _bgColor;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public bool Enabled { get { return _enabled; } set { _enabled = value; } }
		public bool IsAcceptingText { get { return _isAcceptingText; } set { _isAcceptingText = value; }}
		public Rectangle Rect { get { return _rect; } set { _rect = value; } }
		public string Text { get { return _text; } set { _text = value; } }
		public int CharLimit { get { return _charLimit; } }
		public FontProperties Font { get { return _font; } set { _font = value; } }
		public Color BGColor { get { return _bgColor; } set { _bgColor = value; } }	

		public TextField(Vector2 pos, Vector2 size, int charLimit, FontProperties font, Color bgColor) {
			_rect = new Rectangle(pos.X, pos.Y, size.X, size.Y);
			_charLimit = charLimit;
			_font = font;
			_bgColor = bgColor;
		}

		public void Render() {
			if (!_enabled)
				return;
			Raylib.DrawRectangle((int)_rect.x, (int)_rect.y, (int)_rect.width, (int)_rect.height, _bgColor);
			
			int drawY = (int)_rect.y;
			if (_font.Size <= _rect.height)
				drawY = (int)( _rect.y + ((_rect.height - _font.Size) / 2)); // vertical align text
			
			Raylib.DrawText(_text, (int)_rect.x, drawY, _font.Size, _font.Color);
		}

		public void UpdateTextField() {
			if (!_enabled || !_isAcceptingText) return;
			// I used the Text Field example on raylib's site for this

			int key = Raylib.GetCharPressed();
			while (key > 0) {
				if ((key >= 32) && (key < 125) && (_text.Length < _charLimit)) {
                    _text += (char)key;
				}

				key = Raylib.GetCharPressed();
			}

			if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE) && _text.Length > 0) {
					_text = _text.Remove(_text.Length - 1);
			}
		}

		public void ResetTextField() {
			_text = "";
		}
    }
}
