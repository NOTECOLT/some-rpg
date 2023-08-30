//------------------------------------------------------------------------------------------
/* E(NTITY) TRANSFORM
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.ECS {
	public enum Direction {
		Up,
		Down,
		Left,
		Right
	}
	/// <summary>
	/// Allows the entity to exist within the game world
	/// </summary>
	public class ETransform : Component {
		// FIELDS
		//------------------------------------------------------------------------------------------
		private Vector2 _position;	    // Refers to the position of the player with respect to the screen / global coordinate system  
    	private Vector2 _tilePos;       // Refers to the position of player relative to the world grid
		private Vector2 _targetTP;	    // Entity's target world vector. Each entity will constantly move to this location if it is not already.	

		private float _speed = 0;		// Speed of the entity whilst walking
		private float _runSpeed = 0;	// Speed of the entity wihile running

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Vector2 Position { get { return _position; } }
		public Vector2 TilePos { get { return _tilePos; } }
		public Vector2 TargetTP { get { return _targetTP; } set { _targetTP = value; } }
		public bool IsMoving { get; private set; } = false;	// 1 if the entity is moving, 0 otherwise
		public bool IsRunning { get; set; } = false;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
		public Direction Facing { get; private set; } = Direction.Down;

        public ETransform(Vector2 tilePos, float walkSpeed, float runSpeed, int tileSize) {
			_tilePos = tilePos;
			_targetTP = tilePos;
			_position = new Vector2(tilePos.X * tileSize, tilePos.Y * tileSize); 

            _speed = walkSpeed;
			_runSpeed = runSpeed;
			
			ETransformSystem.Register(this);
        }


        // WARNING THIS FUNCTION USES GLOBALS 
        // TODO I'll need to find a way to take this out methinks?
        /// <summary>
		/// Updates Player Movement by adding speed to position vectors.
		/// This function is meant to update position vectors every frame.
		/// </summary>
		public override void Update() {
			int x = (int)(_targetTP.X - _tilePos.X);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(_targetTP.Y - _tilePos.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);

			// Distance takes the signed distance from target position to position
			//		Positive values indicate that the entity is moving towards target
			float distX = (_targetTP.X * Globals.TILE_SIZE - _position.X) * signX;
			float distY = (_targetTP.Y * Globals.TILE_SIZE - _position.Y) * signY;

			// Conditions: If player moves close enough to target position or past the boundaries, snap player to grid
			if ((distX < 0.005 && distY < 0.005 ) || (distX < 0 || distY < 0)) {
				_tilePos = _targetTP;
				_position = new Vector2(_tilePos.X * Globals.TILE_SIZE, _tilePos.Y * Globals.TILE_SIZE);
				IsMoving = false;
				return;
			}

			IsMoving = true;

			// If current position of the entity does not match where it needs to be, then move to those coordinates respectively
			// There is no diagonal movement
			if (_tilePos.X * signX < _targetTP.X * signX) {
				if (!IsRunning)
					_position.X += signX * _speed * Raylib.GetFrameTime();
				else
					_position.X += signX * _runSpeed * Raylib.GetFrameTime();
			} else if (_tilePos.Y * signY < _targetTP.Y * signY) {
				if (!IsRunning)
					_position.Y += signY * _speed * Raylib.GetFrameTime();	
				else	
					_position.Y += signY * _runSpeed * Raylib.GetFrameTime();	
			}
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public void ChangeDirection(Direction dir) {
			Facing = dir;
		}
		
		
		/// <summary>
		/// Renders all debug info relating an entity
		/// </summary>
		/// <param name="tileSize"></param>
		public void DrawEntityDebugText(int tileSize, Vector2 pos) {
			if (!Enabled) return;

			Raylib.DrawText($"position: ({_position.X}, {_position.Y})", (int)pos.X, (int)pos.Y, 30, Color.BLACK);
			Raylib.DrawText($"worldPos: ({_tilePos.X}, {_tilePos.Y})", (int)pos.X, (int)pos.Y + 35, 30, Color.BLACK);
			Raylib.DrawText($"targetWP: ({_targetTP.X}, {_targetTP.Y})", (int)pos.X, (int)pos.Y + 70, 30, Color.BLACK);

			int x = (int)(_targetTP.X - _tilePos.Y);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(_targetTP.Y - _tilePos.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);
			
			float distX = (_targetTP.X * tileSize - _position.X) * signX;
			float distY = (_targetTP.Y * tileSize - _position.Y) * signY;

			Raylib.DrawText($"distance: {distX}, {distY}", (int)pos.X, (int)pos.Y + 105, 30, Color.BLACK);
			Raylib.DrawText($"isMoving: {IsMoving}", (int)pos.X, (int)pos.Y + 140, 30, Color.BLACK);
		}
	}


}