#include <stdlib.h>
#include <string.h>
#include "raylib.h"

// STRUCTS & ENUMS
//------------------------------------------------------------------------------------------

/** Vector2 Struct that uses integers instead of vectors */
typedef struct vector2Int {
    int x;
    int y;
} Vector2Int;

/** Defines the type of entity */
typedef enum entityType {
	PLAYER,
	ENEMY
} EntityType;

/** An Entity is any moving/interactable object in the overworld */
typedef struct entity {
	EntityType type;
	Texture2D sprite;

	Vector2 position;		// Refers to the position of the player with respect to the screen / global coordinate system  
    Vector2Int worldPos;    // Refers to the position of player relative to the world grid
	Vector2Int targetWP;	// Entity's target world vector. Each entity will constantly move to this location if it is not already.

	int isMoving;		// 1 if the entity is moving, 0 otherwise
	int isRunning;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
	
	float speed;		// Speed of the entity whilst walking
	float runSpeed;		// Speed of the entity wihile running
} Entity;

// FUNCTIONS
//------------------------------------------------------------------------------------------

/** Creates a new entity in the world 
 * - Sets speed & runSpeed variables initialized to 0
 * - sprite field is left uninitialized
 * 
*/
Entity* InitEntity(Vector2Int worldPos, EntityType type, int tileSize) {
	Entity* e = malloc(sizeof(Entity));
	if (e == NULL) return NULL;

	e->type = type;

	e->worldPos = worldPos;
	e->targetWP = worldPos;
	e->position = (Vector2){.x = worldPos.x * tileSize, .y = worldPos.y * tileSize};

	e->isMoving = 0;
	e->isRunning = 0;

	e->speed = 0;
	e->runSpeed = 0;

	return e;
}

/** Frees an entity and any dynamically allocated memory within the entity (typically strings & arrays)
 * 
*/
void FreeEntity(Entity* e) {
	if (e == NULL) return; // Theres nothing to free

	// we first free any dynamically allocated memory inside the entity then free the entity itself
	//if (e->spritePath != NULL) free(e->spritePath);

	free(e);
}

/** Initialize movement speeds (walk & run speeds) of an entity */
void SetMovementSpeeds(Entity* e, float walkSpeed, float runSpeed) {
	e->speed = walkSpeed;
	e->runSpeed = runSpeed;
}

/** Initialize movement speeds (walk & run speeds) of an entity */
void SetSprite(Entity* e, char* spritePath) {
	// strdup() duplicates a string and dynamically allocates it new data using malloc
	//e->spritePath = strdup(spritePath);

	e->sprite = LoadTexture(spritePath);	
}

/** Updates Player Movement by adding speed to position vectors */
void UpdateEntityVectors(Entity* e, int tileSize) {
	int x = e->targetWP.x - e->worldPos.x;
	int signX = (x > 0) - (x < 0);
	int y = e->targetWP.y - e->worldPos.y;
	int signY = (y > 0) - (y < 0);

	// Distance takes the signed distance from target position to position
	//		Positive values indicate that the entity is moving towards target
	float distX = ((float)e->targetWP.x * tileSize - e->position.x) * signX;
	float distY = ((float)e->targetWP.y * tileSize - e->position.y) * signY;

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

void RenderEntity(Entity e, int tileSize) {
	DrawTextureRec(
		e.sprite, 
		(Rectangle){18, 22, 13, 21}, 
		(Vector2){.x = e.position.x, .y = e.position.y}, 
		WHITE);
	//DrawRectangle(e.position.x, e.position.y, tileSize, tileSize, RED);
}

/** Renders all debug info relating an entity */
void DrawEntityDebugText(Entity e, int tileSize) {
	DrawText(TextFormat("position: (%f, %f)", e.position.x, e.position.y), 5, 40, 30, BLACK);
	DrawText(TextFormat("worldPos: (%d, %d)", e.worldPos.x, e.worldPos.y), 5, 75, 30, BLACK);
	DrawText(TextFormat("targetWP: (%d, %d)", e.targetWP.x, e.targetWP.y), 5, 110, 30, BLACK);

	int x = e.targetWP.x - e.worldPos.x;
	int signX = (x > 0) - (x < 0);
	int y = e.targetWP.y - e.worldPos.y;
	int signY = (y > 0) - (y < 0);
	
	float distX = ((float)e.targetWP.x * tileSize - e.position.x) * signX;
	float distY = ((float)e.targetWP.y * tileSize - e.position.y) * signY;

	DrawText(TextFormat("distance: (%f, %f)", distX, distY), 5, 140, 30, BLACK);
	DrawText(TextFormat("isMoving: %d", e.isMoving), 5, 175, 30, BLACK);
}