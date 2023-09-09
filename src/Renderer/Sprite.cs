//------------------------------------------------------------------------------------------
/* SPRITE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.Renderer {
    /// <summary>
    /// <para>A sprite not only features the texture, but also other information pertaining to it.</para>
	/// <para>Sprite class should take care of some of the math required to render sprites on to the screen.</para>
    /// </summary>
    public class Sprite  : IRenderable {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private Texture2D _texture;   	
        private Vector2 _origin = Vector2.Zero;		// Start of the spritesheet
		private Vector2 _margin = Vector2.Zero;		// Distance between each frame


		private float _rotation; 
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Vector2 Size { get; }				// Height and Width of the texture
        public Sprite(string path, Vector2 origin, Vector2 size, float rotation) {
			_texture = Raylib.LoadTexture(path);
			Size = size;
			_origin = origin;
			_rotation = rotation;
        }

        public Sprite(Texture2D texture, Vector2 origin, Vector2 size, float rotation) {
			_texture = texture;
			Size = size;
			_origin = origin;
			_rotation = rotation;
        }



		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Renders at position with given offset and scale. In most situations offset is 0 vector.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="offset"></param>
		public void Render(Vector2 position, Vector2 offset, float scale, Color color) {
			Rectangle spriteSrc = new Rectangle(_origin.X, _origin.Y, Size.X, Size.Y);
			Rectangle spriteDst = new Rectangle(position.X, position.Y, Size.X * scale, Size.Y * scale);

			Raylib.DrawTexturePro(_texture, spriteSrc, spriteDst, offset, _rotation, color);
		}
    }
}