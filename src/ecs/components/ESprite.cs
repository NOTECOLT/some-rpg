//------------------------------------------------------------------------------------------
/* E(NTITY) SPRITE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.ECS {
    /// <summary>
    /// <para>Not to be confused with the Sprite Class. </para>
    /// This Component is meant to couple an entity with a sprite object.
    /// </summary>
    public class ESprite : Component {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Sprite Sprite { get; set; }
		public int RenderLayer { get; }

		public ESprite(string path, int renderLayer) {
            Sprite = new Sprite(path, new Vector2(18, 22), new Vector2(13, 21), 0);
			RenderLayer = renderLayer;

            ESpriteSystem.Register(this);
		}

        public ESprite(Sprite sprite, int renderLayer) {
            Sprite = sprite;
            RenderLayer = renderLayer;

            ESpriteSystem.Register(this);
        }

        // WARNING THIS FUNCTION USES GLOBALS 
        // TODO I'll need to find a way to take this out methinks?
        public override void Update() {
            base.Update();

            if (entity.GetComponent<ETransform>() is null) return;

            ETransform transform = entity.GetComponent<ETransform>();

			if (Sprite != null) {
				Vector2 sprPos = new Vector2(transform.Position.X + Globals.TILE_SIZE/2, transform.Position.Y + Globals.TILE_SIZE);
				Vector2 offset = new Vector2(Sprite.Size.X /** scale / 2*/, Sprite.Size.Y * Globals.WORLD_SCALE);

				Sprite.RenderSprite(sprPos, offset, Globals.WORLD_SCALE, Color.WHITE);
			} else {
				Raylib.DrawRectangle((int)transform.Position.X, (int)transform.Position.Y, Globals.TILE_SIZE, Globals.TILE_SIZE, Color.MAGENTA);
			}
        }
    }
}