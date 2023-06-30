//------------------------------------------------------------------------------------------
/* SPRITE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    /// <summary>
    /// A sprite not only features the texture, but also other information pertaining to it
    /// </summary>
    class Sprite {
		// PROPERTIES
		//------------------------------------------------------------------------------------------

        public Texture2D texture;
        public Vector2 size;     // Height and Width of the texture
        public Vector2 origin;
        public float scale;
        public float rotation; 

		public Sprite() {
			
		}

        public Sprite(string path, Vector2 origin, Vector2 size, float scale) {
			texture = Raylib.LoadTexture(path);
			this.size = size;
			this.origin = origin;
			this.scale = scale;
			rotation = 0;
        }
		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Renders a sprite according to the values placed by the struct
		/// </summary>
		/// <param name="position"></param>
		/// <param name="offset"></param>
		public void RenderSprite(Vector2 position, Vector2 offset) {
			Rectangle spriteSrc = new Rectangle(origin.X, origin.Y, size.X, size.Y);
			Rectangle spriteDst = new Rectangle(position.X, position.Y, size.X * scale, size.Y * scale);

			Raylib.DrawTexturePro(texture, spriteSrc, spriteDst, offset, rotation, Color.WHITE);
		}
    }
}