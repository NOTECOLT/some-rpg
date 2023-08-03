//------------------------------------------------------------------------------------------
/* ENTITY
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
	/// <summary>
	/// Defines the type of entity
	/// </summary>
	public enum EntityType {
		PLAYER,
		ENEMY	
	}

	/// <summary>
	/// An Entity is any moving/interactable object in the overworld
	/// </summary>
	public class Entity {
		// FIELDS
		//------------------------------------------------------------------------------------------
		private EntityType _type;
		private Sprite _sprite;

		private Vector2 _position;	// Refers to the position of the player with respect to the screen / global coordinate system  
    	private Vector2 _worldPos;    // Refers to the position of player relative to the world grid
		private Vector2 _targetWP;	// Entity's target world vector. Each entity will constantly move to this location if it is not already.


		private bool _isMoving = false;		// 1 if the entity is moving, 0 otherwise
		private bool _isRunning = false;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
		
		private float _speed = 0;			// Speed of the entity whilst walking
		private float _runSpeed = 0;		// Speed of the entity wihile running

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Vector2 Position { get { return _position; } }
		public Vector2 WorldPos { get { return _worldPos; } }
		public Vector2 TargetWP { get { return _targetWP; } set { _targetWP = value; } }
		public bool IsMoving { get { return _isMoving; } }
		public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } }

		public Entity(Vector2 worldPos, EntityType type, int tileSize) {
			_type = type;

			_worldPos = worldPos;
			_targetWP = worldPos;
			_position = new Vector2(worldPos.X * tileSize, worldPos.Y * tileSize); 
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initialize movement speeds (walk and run speeds) of an entity
		/// </summary>
		/// <param name="walkSpeed"></param>
		/// <param name="runSpeed"></param>
		public void SetMovementSpeeds(float walkSpeed, float runSpeed) {
			_speed = walkSpeed;
			_runSpeed = runSpeed;
		}

		/// <summary>
		/// Initialize sprites of an entity
		/// </summary>
		/// <param name="path"></param>
		public void SetSprite(string path) {
			_sprite = new Sprite(path, new Vector2(18, 22), new Vector2(13, 21), 0);
		}

		/// <summary>
		/// Updates Player Movement by adding speed to position vectors
		/// </summary>
		/// <param name="tileSize"></param>
		public void UpdateEntityVectors(int tileSize) {
			int x = (int)(_targetWP.X - _worldPos.X);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(_targetWP.Y - _worldPos.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);

			// Distance takes the signed distance from target position to position
			//		Positive values indicate that the entity is moving towards target
			float distX = (_targetWP.X * tileSize - _position.X) * signX;
			float distY = (_targetWP.Y * tileSize - _position.Y) * signY;

			// Conditions: If player moves close enough to target position or past the boundaries, snap player to grid
			if ((distX < 0.005 && distY < 0.005 ) || (distX < 0 || distY < 0)) {
				_worldPos = _targetWP;
				_position = new Vector2(_worldPos.X * tileSize, _worldPos.Y * tileSize);
				_isMoving = false;
				return;
			}

			_isMoving = true;

			// If current position of the entity does not match where it needs to be, then move to those coordinates respectively
			// There is no diagonal movement
			if (_worldPos.X * signX < _targetWP.X * signX) {
				if (!_isRunning)
					_position.X += signX * _speed * Raylib.GetFrameTime();
				else
					_position.X += signX * _runSpeed * Raylib.GetFrameTime();
			} else if (_worldPos.Y * signY < _targetWP.Y * signY) {
				if (!_isRunning)
					_position.Y += signY * _speed * Raylib.GetFrameTime();	
				else	
					_position.Y += signY * _runSpeed * Raylib.GetFrameTime();	
			}
		}

		/// <summary>
		/// Renders the entity at the proper world position
		/// </summary>
		/// <param name="tileSize"></param>
		public void RenderEntity(int tileSize, float scale) {
			if (_sprite != null) {
				Vector2 sprPos = new Vector2(_position.X + tileSize/2, _position.Y + tileSize);
				Vector2 offset = new Vector2(_sprite.Size.X /** scale / 2*/, _sprite.Size.Y * scale);

				_sprite.RenderSprite(sprPos, offset, scale, Color.WHITE);
			} else {
				Raylib.DrawRectangle((int)_position.X, (int)_position.Y, tileSize, tileSize, Color.MAGENTA);
			}
		}

		/// <summary>
		/// Renders all debug info relating an entity
		/// </summary>
		/// <param name="tileSize"></param>
		public void DrawEntityDebugText(int tileSize, Vector2 pos) {
			Raylib.DrawText($"position: ({_position.X}, {_position.Y})", (int)pos.X, (int)pos.Y, 30, Color.BLACK);
			Raylib.DrawText($"worldPos: ({_worldPos.X}, {_worldPos.Y})", (int)pos.X, (int)pos.Y + 35, 30, Color.BLACK);
			Raylib.DrawText($"targetWP: ({_targetWP.X}, {_targetWP.Y})", (int)pos.X, (int)pos.Y + 70, 30, Color.BLACK);

			int x = (int)(_targetWP.X - _worldPos.Y);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(_targetWP.Y - _worldPos.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);
			
			float distX = (_targetWP.X * tileSize - _position.X) * signX;
			float distY = (_targetWP.Y * tileSize - _position.Y) * signY;

			Raylib.DrawText($"distance: {distX}, {distY}", (int)pos.X, (int)pos.Y + 105, 30, Color.BLACK);
			Raylib.DrawText($"isMoving: {_isMoving}", (int)pos.X, (int)pos.Y + 140, 30, Color.BLACK);
		}
	}
}

