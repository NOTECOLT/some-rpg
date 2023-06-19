//------------------------------------------------------------------------------------------
/** ENTITY
 * Contains the entity struct and any/all functions relating to it.
 * Entities are created in the heap
*/
//------------------------------------------------------------------------------------------

using System.Numerics;

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
		public EntityType type;
		public Sprite sprite;

		public Vector2 position;	// Refers to the position of the player with respect to the screen / global coordinate system  
    	public Vector2 worldPos;    // Refers to the position of player relative to the world grid
		public Vector2 targetWP;	// Entity's target world vector. Each entity will constantly move to this location if it is not already.


		public int isMoving;		// 1 if the entity is moving, 0 otherwise
		public int isRunning;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
		
		public float speed;			// Speed of the entity whilst walking
		public float runSpeed;		// Speed of the entity wihile running
		
		/// <summary>
		/**	Creates a new entity in the world 
		* - Sets speed & runSpeed variables initialized to 0
 		* - sprite is set to NULL
		*/
		/// </summary>
		public Entity(Vector2 worldPos, EntityType type, int tileSize) {

			this.type = type;
			sprite = null;

			this.worldPos = worldPos;
			targetWP = worldPos;
			position = new Vector2(worldPos.X * tileSize, worldPos.Y * tileSize); 
			//e->position = (Vector2){.x = worldPos.x * tileSize, .y = worldPos.y * tileSize};

			isMoving = 0;
			isRunning = 0;

			speed = 0;
			runSpeed = 0;
		}


		/// <summary>
		/// Initialize movement speeds (walk & run speeds) of an entity
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
		public void SetSprite(String path) {
			sprite = InitSprite(path, new Vector2(18, 22), new Vector2(13, 21), 2.0f);
		}
	}
}

// FUNCTIONS
//------------------------------------------------------------------------------------------

/** Updates Player Movement by adding speed to position vectors */
void UpdateEntityVectors(Entity* e, int tileSize) {
	int x = e->targetWP.x - e->worldPos.x;
	int signX = (x > 0) - (x < 0);
	int y = e->targetWP.y - e->worldPos.y;
	int signY = (y > 0) - (y < 0);

	// Distance takes the signed distance from target position to position
	//		Positive values indicate that the entity is moving towards target
	float distX = (e->targetWP.x * tileSize - e->position.x) * signX;
	float distY = (e->targetWP.y * tileSize - e->position.y) * signY;

	// Conditions: If player moves close enough to target position or past the boundaries, affix player to grid
	if ((distX < 0.005 && distY < 0.005 ) || (distX < 0 || distY < 0)) {
		e->worldPos = e->targetWP;
		e->position = (Vector2){
			.x = e->worldPos.x * tileSize,
			.y = e->worldPos.y * tileSize
		};
		e->isMoving = 0;
		return;
	}

	e->isMoving = 1;

	// If current position of the entity does not match where it needs to be, then move to those coordinates respectively
	// There is no diagonal movement
	if (e->worldPos.x * signX < e->targetWP.x * signX) {
		if (!e->isRunning)
			e->position.x += signX * e->speed * GetFrameTime();
		else
			e->position.x += signX * e->runSpeed * GetFrameTime();
	} else if (e->worldPos.y * signY < e->targetWP.y * signY) {
		if (!e->isRunning)
			e->position.y += signY * e->speed * GetFrameTime();	
		else	
			e->position.y += signY * e->runSpeed * GetFrameTime();	
	}
}

/** Renders the entity at the proper world position */
void RenderEntity(Entity e, int tileSize) {
	if (e.spr != NULL) {
		Vector2 sprPos = {
			.x = e.position.x + (tileSize)/2,
			.y = e.position.y + tileSize
		};
		
		Vector2 offset = {
			.x = (e.spr->size.x * e.spr->scale) / 2, 
			.y = (e.spr->size.y * e.spr->scale)
		};

		RenderSprite(*(e.spr), sprPos, offset);
	} else {
		DrawRectangle(e.position.x, e.position.y, tileSize, tileSize, MAGENTA);
	}
}

/** Renders all debug info relating an entity */
void DrawEntityDebugText(Entity e, int tileSize) {
	DrawText(TextFormat("position: (%.2f, %.2f)", e.position.x, e.position.y), 5, 40, 30, BLACK);
	DrawText(TextFormat("worldPos: (%.0f, %.0f)", e.worldPos.x, e.worldPos.y), 5, 75, 30, BLACK);
	DrawText(TextFormat("targetWP: (%.0f, %.0f)", e.targetWP.x, e.targetWP.y), 5, 110, 30, BLACK);

	int x = e.targetWP.x - e.worldPos.x;
	int signX = (x > 0) - (x < 0);
	int y = e.targetWP.y - e.worldPos.y;
	int signY = (y > 0) - (y < 0);
	
	float distX = (e.targetWP.x * tileSize - e.position.x) * signX;
	float distY = (e.targetWP.y * tileSize - e.position.y) * signY;

	DrawText(TextFormat("distance: (%.2f, %.2f)", distX, distY), 5, 140, 30, BLACK);
	DrawText(TextFormat("isMoving: %d", e.isMoving), 5, 175, 30, BLACK);
}