//------------------------------------------------------------------------------------------
/*  BUTTON
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.GUI {
    public class Button {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private bool _enabled = true;
		private Rectangle _rect;
		private String _text;
		private FontProperties _font;
		private Color _bgColor;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public bool Enabled { get { return _enabled; } set { _enabled = value; } }
		public Rectangle Rect { get { return _rect; } set { _rect = value; } }
		public String Text { get { return _text; } set { _text = value; } }
		public FontProperties Font { get { return _font; } set { _font = value; } }
		public Color BGColor { get { return _bgColor; } set { _bgColor = value; } }
		

		public Button(Vector2 pos, Vector2 size, String text, FontProperties font, Color bgColor) {
			_rect = new Rectangle(pos.X, pos.Y, size.X, size.Y);
			_text = text;
			_font = font;
			_bgColor = bgColor;
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public void Render() {
			if (!_enabled)
				return;
			Raylib.DrawRectangle((int)_rect.x, (int)_rect.y, (int)_rect.width, (int)_rect.height, _bgColor);
			
			int drawY = (int)_rect.y;
			if (_font.Size <= _rect.height)
				drawY = (int)( _rect.y + ((_rect.height - _font.Size) / 2)); // vertical align text
			
			Raylib.DrawText(_text, (int)_rect.x, drawY, _font.Size, _font.Color);
		}
		
		public bool IsClicked(Vector2 mousePos) {
			if (!_enabled)
				return false;
			return Raylib.CheckCollisionPointRec(mousePos, _rect) && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
		}
	}
}