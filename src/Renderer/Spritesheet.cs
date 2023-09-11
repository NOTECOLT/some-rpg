//------------------------------------------------------------------------------------------
/* SPRITE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.Renderer {
    /// <summary>
    /// <para></para>
    /// </summary>
    public class Spritesheet : IRenderable {
		// FIELDS
		//------------------------------------------------------------------------------------------
		private Texture2D _texture;  
        private Vector2 _origin;			// Distance of the first frame from the top left corner of the texture
		private Vector2 _margin;			// Distance between every frame
        private float _rotation; 
		private int _frame;					// The current frame that is being played. 0 Indexed
		private int _frameBuffer;			// How many (game) frames each (sprite) frame will stay on screen
		private Animation _currentAnimation;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Vector2 Size { get; }	// Height and Width of a single frame
		public bool FlipX { get; set; } = false;
		/// <summary>
		/// Contains a list of all animations with their reference tags
		/// </summary>
		public Dictionary<String, Animation> AnimDictionary;
        public Spritesheet(string path, Vector2 origin, Vector2 size, Vector2 margin, int frameBuffer, float rotation) {
			_texture = Raylib.LoadTexture(path);
			Size = size;
			_origin = origin;
			_rotation = rotation;
			_margin = margin;
			_frameBuffer = frameBuffer;

			_frame = 0;

			AnimDictionary = new Dictionary<String, Animation>();
			AnimDictionary["null"] = new Animation {
				StartFrame = 0,
				FrameCount = 1
			};
			SetAnimation("null");
        }

        public Spritesheet(Texture2D texture, Vector2 origin, Vector2 size,Vector2 margin, int frameBuffer, float rotation) {
			_texture = texture;
			Size = size;
			_origin = origin;
			_rotation = rotation;
			_margin = margin;
			_frameBuffer = frameBuffer;
			

			_frame = 0;

			AnimDictionary = new Dictionary<String, Animation>();
			AnimDictionary["null"] = new Animation {
				StartFrame = 0,
				FrameCount = 1
			};
			SetAnimation("null");
        }
		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Renders at position with given offset and scale. In most situations offset is 0 vector.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="offset"></param>
		public void Render(Vector2 position, Vector2 offset, float scale, Color color) {
			// Convert the buffered frames into the actual frame to get on the texture
			//	A frameBuffer = 2 means sprites will change every 2 frames
			_frame = (_frame + 1) % (_currentAnimation.FrameCount * _frameBuffer);
			int spriteFrame = _currentAnimation.StartFrame + (_frame / _frameBuffer);

			// Console.WriteLine($"{_frame} / {_currentAnimation.FrameCount * _frameBuffer} ; {spriteFrame - _currentAnimation.StartFrame}");
			
			// Using all available spritesheet information, calculate the position of the frame to draw
			int framesInRow = (int)MathF.Ceiling((_texture.width - _origin.X) / (Size.X + _margin.X));

			int frameX = spriteFrame % framesInRow;
			int frameY = (int)MathF.Floor(spriteFrame / framesInRow);

			Vector2 src = new Vector2() {
				X = frameX * (Size.X + _margin.X) + _origin.X,
				Y = frameY * (Size.Y + _margin.Y) + _origin.Y
			};
            
            Rectangle spriteSrc;

			if (!FlipX)
				spriteSrc = new Rectangle(src.X, src.Y, Size.X, Size.Y);
			else
				spriteSrc = new Rectangle(src.X, src.Y, -Size.X, Size.Y);

			Rectangle spriteDst = new Rectangle(position.X, position.Y, Size.X * scale, Size.Y * scale);
			Raylib.DrawTexturePro(_texture, spriteSrc, spriteDst, offset, _rotation, color);
		}

		public void SetAnimation(String name) {
			// The null animation sets a default state in case of invalid animation keys
			if (!AnimDictionary.ContainsKey(name)) {
				_currentAnimation = AnimDictionary["null"];
				return;
			}

			if (_currentAnimation.FrameCount == AnimDictionary[name].FrameCount
				&& _currentAnimation.StartFrame == AnimDictionary[name].StartFrame) return;

			_currentAnimation = AnimDictionary[name];
			_frame = 0;
		}
		
		/// <summary>
		/// Modifies frame buffer in terms of frames per second
		/// </summary>
		/// <param name="fps"></param>
		public void SetFPS(int fps) {
			_frameBuffer = (Raylib.GetFPS() == 0) ? Globals.TargetFPS / fps : Raylib.GetFPS() / fps;
		}
    }

	/// <summary>
	/// An animation merely dictates the start and frame count within a spritesheet.
	/// </summary>
	public struct Animation {
		public int StartFrame;
		public int FrameCount;
	}
}