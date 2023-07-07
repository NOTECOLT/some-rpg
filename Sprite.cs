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
		// FIELDS
		//------------------------------------------------------------------------------------------
        private Texture2D _texture;
        private Vector2 _size;     // Height and Width of the texture
        private Vector2 _origin;
        private float _scale;
        private float _rotation; 

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Vector2 Size { get { return _size; } set { _size = value; } }
		public float Scale { get { return _scale; } set { _scale = value; } }

        public Sprite(string path, Vector2 origin, Vector2 size, float scale, float rotation) {
			_texture = Raylib.LoadTexture(path);
			_size = size;
			_origin = origin;
			_scale = scale;
			_rotation = rotation;
        }

        public Sprite(Texture2D texture, Vector2 origin, Vector2 size, float scale, float rotation) {
			_texture = texture;
			_size = size;
			_origin = origin;
			_scale = scale;
			_rotation = rotation;
        }
		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Renders a sprite according to the values placed by the struct
		/// </summary>
		/// <param name="position"></param>
		/// <param name="offset"></param>
		public void RenderSprite(Vector2 position, Vector2 offset) {
			Rectangle spriteSrc = new Rectangle(_origin.X, _origin.Y, _size.X, _size.Y);
			Rectangle spriteDst = new Rectangle(position.X, position.Y, _size.X * _scale, _size.Y * _scale);

			Raylib.DrawTexturePro(_texture, spriteSrc, spriteDst, offset, _rotation, Color.WHITE);
		}
    }
}