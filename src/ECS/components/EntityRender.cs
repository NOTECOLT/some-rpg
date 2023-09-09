//------------------------------------------------------------------------------------------
/* ENTITY RENDER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;
using Topdown.Renderer;

namespace Topdown.ECS {
    /// <summary>
    /// <para>Not to be confused with the Sprite Class. </para>
    /// This Component is meant to couple an entity with a Renderable object
    /// Renderable objects can be Sprites or Spritesheets
    /// </summary>
    public class EntityRender : Component {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public IRenderable RenderObject { get; set; }
		public int RenderLayer { get; }

		// public ESprite(string path, Vector2 origin, Vector2 size, int renderLayer) {
        //     Sprite = new Sprite(path, origin, size, 0);
		// 	RenderLayer = renderLayer;

        //     ESpriteSystem.Register(this);
		// }

        public EntityRender(IRenderable sprite, int renderLayer) {
            RenderObject = sprite;
            RenderLayer = renderLayer;

            ESpriteSystem.Register(this);
        }

        // WARNING THIS FUNCTION USES GLOBALS 
        // TODO I'll need to find a way to take this out methinks?
        /// <summary>
        /// Sprite Component must contain ETransform in order to render
        /// </summary>
        public override void Update() {
            if (entity.GetComponent<ETransform>() is null) return;

            ETransform transform = entity.GetComponent<ETransform>();

            if (RenderObject is null) {
                Raylib.DrawRectangle((int)transform.Position.X, (int)transform.Position.Y, Globals.ScaledTileSize, Globals.ScaledTileSize, Color.MAGENTA);
                return;
            }

            Vector2 sprPos = new Vector2(transform.Position.X + Globals.ScaledTileSize/2, transform.Position.Y + Globals.ScaledTileSize);
            Vector2 offset = new Vector2(RenderObject.Size.X /** scale / 2*/, RenderObject.Size.Y * Globals.WorldScale);

            RenderObject.Render(sprPos, offset, Globals.WorldScale, Color.WHITE);
        }
    }
}