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
		private float _speed = 0;		// Speed of the entity whilst walking
		private float _runSpeed = 0;	// Speed of the entity wihile running

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Vector2 Position { get; private set; }		// Refers to the position of the player with respect to the screen / global coordinate system  
		public Vector2 Tile { get; private set; }			// Refers to the position of player relative to the world grid
		public Vector2 TargetTile { get; set; }				// Entity's target world vector. Each entity will constantly move to this location if it is not already.	
		public bool IsMoving { get; private set; } = false;	// 1 if the entity is moving, 0 otherwise
		public bool IsRunning { get; set; } = false;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
		public Direction Facing { get; private set; } = Direction.South;

        public ETransform(Vector2 tile, float walkSpeed = DefaultWalkSpeed, float runSpeed = DefaultRunSpeed) {
			Tile = tile;
			TargetTile = tile;
			Position = new Vector2(tile.X * Globals.ScaledTileSize, tile.Y * Globals.ScaledTileSize); 

            _speed = walkSpeed;
			_runSpeed = runSpeed;
			
			ETransformSystem.Register(this);
        }

        public override void Destroy() {
            ETransformSystem.Components.Remove(this);
        }

        public override void Update() {
			MoveToTargetTile();
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public void ChangeDirection(Direction dir) {
			Facing = dir;
		}

        // WARNING THIS FUNCTION USES GLOBALS 
        // TODO I'll need to find a way to take this out methinks?
        /// <summary>
		/// Updates Player Movement by adding speed to position vectors.
		/// This function is meant to update position vectors every frame.
		/// </summary>	
		public void MoveToTargetTile() {
			int x = (int)(TargetTile.X - Tile.X);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(TargetTile.Y - Tile.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);

			// Distance takes the signed distance from target position to position
			//		Positive values indicate that the entity is moving towards target
			float distX = (TargetTile.X * Globals.ScaledTileSize - Position.X) * signX;
			float distY = (TargetTile.Y * Globals.ScaledTileSize - Position.Y) * signY;

			// Conditions: If player moves close enough to target position or past the boundaries, snap player to grid
			if ((distX < 0.005 && distY < 0.005 ) || (distX < 0 || distY < 0)) {
				// CHECK AGAIN IF KEY IS STILL BEING PRESSED TO NEGATE THIS

				Tile = TargetTile;
				Position = new Vector2(Tile.X * Globals.ScaledTileSize, Tile.Y * Globals.ScaledTileSize);
				
				IsMoving = false;

				// Once the transform reaches the target tile, another check needs to be done
				//		if the entity is controllable. If the keys are still being pressed,
				//		the entity must keep moving. This is to prevent stuttery movement
				if (entity is IControllable) {
					IControllable e = entity as IControllable;

					IsMoving = e.CheckKeys();
				} else {
					IsMoving = false;
				}

				return;
			}

			IsMoving = true;

			// If current position of the entity does not match where it needs to be, then move to those coordinates respectively
			// There is no diagonal movement
			if (Tile.X * signX < TargetTile.X * signX) {
				if (!IsRunning)
					Position += Vector2.UnitX * signX * _speed * Raylib.GetFrameTime();
				else 
					Position += Vector2.UnitX * signX * _runSpeed * Raylib.GetFrameTime();
			} else if (Tile.Y * signY < TargetTile.Y * signY) {
				if (!IsRunning)
					Position += Vector2.UnitY * signY * _speed * Raylib.GetFrameTime();
				else	
					Position += Vector2.UnitY * signY * _runSpeed * Raylib.GetFrameTime();
	
			}
		}
		
		/// <summary>
		/// Renders all debug info relating an entity
		/// </summary>
		/// <param name="tileSize"></param>
		public void DrawEntityDebugText(Vector2 pos) {
			if (!Enabled) return;

			Raylib.DrawText($"position: ({Position.X}, {Position.Y})", (int)pos.X, (int)pos.Y, 30, Color.RAYWHITE);
			Raylib.DrawText($"Tile: ({Tile.X}, {Tile.Y})", (int)pos.X, (int)pos.Y + 35, 30, Color.RAYWHITE);
			// Raylib.DrawText($"targetTile: ({TargetTile.X}, {TargetTile.Y})", (int)pos.X, (int)pos.Y + 70, 30, Color.WHITE);

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