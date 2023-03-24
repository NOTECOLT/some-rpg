#include <stdlib.h>
#include "raylib.h"

typedef struct vector2Int {
    int x;
    int y;
} Vector2Int;

/** An Entity is any moving/interactable object in the overworld */
typedef struct entity {
	Vector2 position;		// Refers to the position of the player with respect to the screen / global coordinate system  
    Vector2Int worldPos;    // Refers to the position of player relative to the world grid

	Vector2Int targetWP;	// Entity's target world vector. Each entity will constantly move to this location if it is not already

	int isMoving;
	int isRunning;
	
	float speed;
	float runSpeed;
} Entity;

/** Creates a new entity in the world */
Entity* InitEntity(Vector2Int worldPos, int tileSize) {
	Entity* e = malloc(sizeof(Entity));
	e->worldPos = worldPos;
	e->targetWP = worldPos;
	e->position = (Vector2){.x = worldPos.x * tileSize, .y = worldPos.y * tileSize};

	e->isMoving = 0;
	e->isRunning = 0;

	e->speed = 0;
	e->runSpeed = 0;

	return e;
}

void SetMovementSpeeds(Entity* e, float walkSpeed, float runSpeed) {
	e->speed = walkSpeed;
	e->runSpeed = runSpeed;
}

/** Updates Player Movement by adding speed to position vectors */
void UpdateEntityVectors(Entity* e, int tileSize) {
	int x = e->targetWP.x - e->worldPos.x;
	int signX = (x > 0) - (x < 0);
	int y = e->targetWP.y - e->worldPos.y;
	int signY = (y > 0) - (y < 0);

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
	DrawRectangle(e.position.x, e.position.y, tileSize, tileSize, RED);
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