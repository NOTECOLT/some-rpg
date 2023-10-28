//------------------------------------------------------------------------------------------
/*  IMAGE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;
using Topdown.Renderer;

namespace Topdown.GUI {
	public class Image : UIEntity {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public IRenderable RenderObject { get; set; }

		public Image(Vector2 pos, Vector2 size, String name, Color color, IRenderable renderObject) : base(pos, size, name, color) {
			RenderObject = renderObject;
		}

        // FUNCTIONS
        //------------------------------------------------------------------------------------------
        public override void Render() {
            base.Render();
			if (RenderObject is null) return;
			else if (RenderObject is Sprite) {
				Sprite s = RenderObject as Sprite;

				// DO I really wanna stretch textures?
				s.RenderStretch(_absoluteRect, Vector2.Zero, Color.WHITE);
				// s.Render(new Vector2(_absoluteRect.x, _absoluteRect.y), Vector2.Zero, 1.0f, Color.WHITE);
			}
        }
    }
}