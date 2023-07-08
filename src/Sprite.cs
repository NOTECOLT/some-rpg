//------------------------------------------------------------------------------------------
/* SPRITE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    /// <summary>
    /// <para>A sprite not only features the texture, but also other information pertaining to it.</para>
	/// <para>Sprite class should take care of some of the math required to render sprites on to the screen.</para>
    /// </summary>
    class Sprite {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private Texture2D _texture;
        private Vector2 _size;     // Height and Width of the texture
        private Vector2 _origin;
        private float _rotation; 

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Texture2D Texture { get { return _texture; } }
		public Vector2 Size { get { return _size; } set { _size = value; } }
        public Sprite(string path, Vector2 origin, Vector2 size, float rotation) {
			_texture = Raylib.LoadTexture(path);
			_size = size;
			_origin = origin;
			_rotation = rotation;
        }

        public Sprite(Texture2D texture, Vector2 origin, Vector2 size, float rotation) {
			_texture = texture;
			_size = size;
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
		public void RenderSprite(Vector2 position, Vector2 offset, float scale, Color color) {
			Rectangle spriteSrc = new Rectangle(_origin.X, _origin.Y, _size.X, _size.Y);
			Rectangle spriteDst = new Rectangle(position.X, position.Y, _size.X * scale, _size.Y * scale);

			Raylib.DrawTexturePro(_texture, spriteSrc, spriteDst, offset, _rotation, color);
		}
    }
}