//------------------------------------------------------------------------------------------
/*  BUTTON
*/
//------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;

namespace Topdown.GUI {
	/// <summary>
	/// A clickable UI element. Nothing more than a regular UIEntity with an IClickable interface
	/// </summary>
    public class Button : UIEntity, IClickable {
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
		public Button(Vector2 pos, Vector2 size, String text, String name, TextStyles font, Color? bgColor) : base(pos, size, name, bgColor) {
			Text = text;
			Font = font;
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public override void Render() {
			base.Render();
			
			int drawY = (int)_absoluteRect.y;
			if (Font.Size <= _absoluteRect.height)
				drawY = (int)( _absoluteRect.y + ((_absoluteRect.height - Font.Size) / 2)); // vertical align text
			
			Raylib.DrawText(Text, (int)_absoluteRect.x, drawY, Font.Size, Font.Color);
		}

		// ICLICKABLE
		//------------------------------------------------------------------------------------------
		public void OnMousePress(Vector2 mousePos) { }
	
		public void OnMouseRelease(Vector2 mousePos) {
			Console.WriteLine($"{Name} Clicked!");
		}

		public void OnMouseDown(Vector2 mousePos) { }
	}
}