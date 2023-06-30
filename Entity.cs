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
	enum EntityType {
		PLAYER,
		ENEMY	
	}

	/// <summary>
	/// An Entity is any moving/interactable object in the overworld
	/// </summary>
	class Entity {
		// PROPERTIES
		//------------------------------------------------------------------------------------------

		public EntityType type;
		public Sprite sprite;

		public Vector2 position;	// Refers to the position of the player with respect to the screen / global coordinate system  
    	public Vector2 worldPos;    // Refers to the position of player relative to the world grid
		public Vector2 targetWP;	// Entity's target world vector. Each entity will constantly move to this location if it is not already.


		public bool isMoving = false;		// 1 if the entity is moving, 0 otherwise
		public bool isRunning = false;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
		
		public float speed = 0;			// Speed of the entity whilst walking
		public float runSpeed = 0;		// Speed of the entity wihile running
		
		public Entity(Vector2 worldPos, EntityType type, int tileSize) {
			this.type = type;

			this.worldPos = worldPos;
			targetWP = worldPos;
			position = new Vector2(worldPos.X * tileSize, worldPos.Y * tileSize); 
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initialize movement speeds (walk and run speeds) of an entity
		/// </summary>
		/// <param name="walkSpeed"></param>
		/// <param name="runSpeed"></param>
		public void SetMovementSpeeds(float walkSpeed, float runSpeed) {
			speed = walkSpeed;
			this.runSpeed = runSpeed;
		}

		/// <summary>
		/// Initialize sprites of an entity
		/// </summary>
		/// <param name="path"></param>
		public void SetSprite(string path) {
			sprite = new Sprite(path, new Vector2(18, 22), new Vector2(13, 21), 2.0f);
		}

		/// <summary>
		/// Updates Player Movement by adding speed to position vectors
		/// </summary>
		/// <param name="tileSize"></param>
		public void UpdateEntityVectors(int tileSize) {
			int x = (int)(targetWP.X - worldPos.Y);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(targetWP.Y - worldPos.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);

			// Distance takes the signed distance from target position to position
			//		Positive values indicate that the entity is moving towards target
			float distX = (targetWP.X * tileSize - position.X) * signX;
			float distY = (targetWP.Y * tileSize - position.Y) * signY;

			// Conditions: If player moves close enough to target position or past the boundaries, affix player to grid
			if ((distX < 0.005 && distY < 0.005 ) || (distX < 0 || distY < 0)) {
				worldPos = targetWP;
				position = new Vector2(worldPos.X * tileSize, worldPos.Y * tileSize);
				isMoving = false;
				return;
			}

			isMoving = true;

			// If current position of the entity does not match where it needs to be, then move to those coordinates respectively
			// There is no diagonal movement
			if (worldPos.X * signX < targetWP.X * signX) {
				if (isRunning)
					position.X += signX * speed * Raylib.GetFrameTime();
				else
					position.X += signX * runSpeed * Raylib.GetFrameTime();
			} else if (worldPos.Y * signY < targetWP.Y * signY) {
				if (!isRunning)
					position.Y += signY * speed * Raylib.GetFrameTime();	
				else	
					position.Y += signY * runSpeed * Raylib.GetFrameTime();	
			}
		}

		/// <summary>
		/// Renders the entity at the proper world position
		/// </summary>
		/// <param name="tileSize"></param>
		public void RenderEntity(int tileSize) {
			if (sprite != null) {
				Vector2 sprPos = new Vector2(position.X + tileSize/2, position.Y + tileSize);
				Vector2 offset = new Vector2(sprite.size.X * sprite.scale / 2, sprite.size.Y * sprite.scale);
			
				sprite.RenderSprite(sprPos, offset);
			} else {
				Raylib.DrawRectangle((int)position.X, (int)position.Y, tileSize, tileSize, Color.MAGENTA);
			}
		}

		/// <summary>
		/// Renders all debug info relating an entity
		/// </summary>
		/// <param name="tileSize"></param>
		public void DrawEntityDebugText(int tileSize) {
			Raylib.DrawText($"position: ({position.X}, {position.Y})", 5, 40, 30, Color.BLACK);
			Raylib.DrawText($"worldPos: ({worldPos.X}, {worldPos.Y})", 5, 75, 30, Color.BLACK);
			Raylib.DrawText($"targetWP: ({targetWP.X}, {targetWP.Y})", 5, 110, 30, Color.BLACK);

			int x = (int)(targetWP.X - worldPos.Y);
			int signX = Convert.ToInt32(x > 0) - Convert.ToInt32(x < 0);		// remember false = 0, true = 1
			int y = (int)(targetWP.Y - worldPos.Y);
			int signY = Convert.ToInt32(y > 0) - Convert.ToInt32(y < 0);
			
			float distX = (targetWP.X * tileSize - position.X) * signX;
			float distY = (targetWP.Y * tileSize - position.Y) * signY;

			Raylib.DrawText($"distance: {distX}, {distY}", 5, 140, 30, Color.BLACK);
			Raylib.DrawText($"isMoving: {isMoving}", 5, 175, 30, Color.BLACK);
		}
	}
}

