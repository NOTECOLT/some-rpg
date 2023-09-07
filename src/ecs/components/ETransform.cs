//------------------------------------------------------------------------------------------
/* E(NTITY) TRANSFORM
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.ECS {

	/// <summary>
	/// Allows the entity to exist within the game world
	/// </summary>
	public class ETransform : Component {
		private const int DefaultWalkSpeed = 50;
		private const int DefaultRunSpeed = 100;
		// FIELDS
		//------------------------------------------------------------------------------------------
		private Vector2 _position;	    // Refers to the position of the player with respect to the screen / global coordinate system  
    	private Vector2 _tile;       // Refers to the position of player relative to the world grid
		private Vector2 _targetTile;	    // Entity's target world vector. Each entity will constantly move to this location if it is not already.	

		private float _speed = 0;		// Speed of the entity whilst walking
		private float _runSpeed = 0;	// Speed of the entity wihile running

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Vector2 Position { get { return _position; } }
		public Vector2 Tile { get { return _tile; } }
		public Vector2 TargetTile { get { return _targetTile; } set { _targetTile = value; } }
		public bool IsMoving { get; private set; } = false;	// 1 if the entity is moving, 0 otherwise
		public bool IsRunning { get; set; } = false;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
		public Direction Facing { get; private set; } = Direction.South;

        public ETransform(Vector2 tile, float walkSpeed = DefaultWalkSpeed, float runSpeed = DefaultRunSpeed) {
			_tile = tile;
			_targetTile = tile;
			_position = new Vector2(tile.X * Globals.ScaledTileSize, tile.Y * Globals.ScaledTileSize); 

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

			int x = (int)(_targetTile.X - _tile.X);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(_targetTile.Y - _tile.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);

			// Distance takes the signed distance from target position to position
			//		Positive values indicate that the entity is moving towards target
			float distX = (_targetTile.X * Globals.ScaledTileSize - _position.X) * signX;
			float distY = (_targetTile.Y * Globals.ScaledTileSize - _position.Y) * signY;

			// Conditions: If player moves close enough to target position or past the boundaries, snap player to grid
			if ((distX < 0.005 && distY < 0.005 ) || (distX < 0 || distY < 0)) {
				_tile = _targetTile;
				_position = new Vector2(_tile.X * Globals.ScaledTileSize, _tile.Y * Globals.ScaledTileSize);
				IsMoving = false;
				return;
			}

			IsMoving = true;

			// If current position of the entity does not match where it needs to be, then move to those coordinates respectively
			// There is no diagonal movement
			if (_tile.X * signX < _targetTile.X * signX) {
				if (!IsRunning)
					_position.X += signX * _speed * Raylib.GetFrameTime();
				else
					_position.X += signX * _runSpeed * Raylib.GetFrameTime();
			} else if (_tile.Y * signY < _targetTile.Y * signY) {
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
		public void DrawEntityDebugText(Vector2 pos) {
			if (!Enabled) return;

			Raylib.DrawText($"position: ({_position.X}, {_position.Y})", (int)pos.X, (int)pos.Y, 30, Color.RAYWHITE);
			Raylib.DrawText($"Tile: ({_tile.X}, {_tile.Y})", (int)pos.X, (int)pos.Y + 35, 30, Color.RAYWHITE);
			// Raylib.DrawText($"targetTile: ({_targetTile.X}, {_targetTile.Y})", (int)pos.X, (int)pos.Y + 70, 30, Color.BLACK);

			// int x = (int)(_targetTile.X - _tile.Y);
			// int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			// int y = (int)(_targetTile.Y - _tile.Y);
			// int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);
			
			// float distX = (_targetTile.X * tileSize - _position.X) * signX;
			// float distY = (_targetTile.Y * tileSize - _position.Y) * signY;

			// Raylib.DrawText($"distance: {distX}, {distY}", (int)pos.X, (int)pos.Y + 105, 30, Color.BLACK);
			// Raylib.DrawText($"isMoving: {IsMoving}", (int)pos.X, (int)pos.Y + 140, 30, Color.BLACK);
		}
	}


}