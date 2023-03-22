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

	int isMoving;
	Vector2Int targetWP;	// Entity's target world vector. Each entity will constantly move to this location if it is not already
	float speed;
	//Vector2 speed;
} Entity;

Entity* NewEntity(Vector2Int worldPos, int tileSize, float speed);
void UpdateEntityVectors(Entity* e, int tileSize);
void RenderEntity(Entity e, int tileSize);




/** Creates a new entity in the world */
Entity* NewEntity(Vector2Int worldPos, int tileSize, float speed) {
	Entity* e = malloc(sizeof(Entity));
	e->worldPos = worldPos;
	e->targetWP = worldPos;
	e->position = (Vector2){.x = worldPos.x * tileSize, .y = worldPos.y * tileSize};
	e->isMoving = 0;
	e->speed = speed;

	return e;
}

/** Updates Player Movement by adding speed to position vectors */
void UpdateEntityVectors(Entity* e, int tileSize) {
	float distX = abs(e->position.x - (float)e->targetWP.x * tileSize);
	float distY = abs(e->position.y - (float)e->targetWP.y * tileSize);

	// Second Condition is just a bandaid solution, if ever movement glitches & player moves past their target position
	//		Idea is that a the target position will never be more than 1 tile away, so if ever it does, 
	//			then something is wrong
	// @NOTECOLT pls find a better way to do this
	if (distX < 0.005 && distY < 0.05 || (distX > tileSize + 0.05 || distY > tileSize + 0.05)) {
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

	int x = e->targetWP.x - e->worldPos.x;
	int signX = (x > 0) - (x < 0);
	int y = e->targetWP.y - e->worldPos.y;
	int signY = (y > 0) - (y < 0);

	if (e->worldPos.x * signX < e->targetWP.x * signX) {
		e->position.x += signX * e->speed * GetFrameTime();
	} else if (e->worldPos.y * signY < e->targetWP.y * signY) {
		e->position.y += signY * e->speed * GetFrameTime();	
	}
	// if (e->worldPos.x != e->targetWP.x) {
	// 	int x = e->targetWP.x - e->worldPos.x;
	// 	int sign = (x > 0) - (x < 0);
	// 	e->position.x += sign * e->speed * GetFrameTime();
	// } else if (e->worldPos.y != e->targetWP.y) {
	// 	int y = e->targetWP.y - e->worldPos.y;
	// 	int sign = (y > 0) - (y < 0);
	// 	e->position.y += sign * e->speed * GetFrameTime();	
	// }
}

void RenderEntity(Entity e, int tileSize) {
	DrawRectangle(e.position.x, e.position.y, tileSize, tileSize, RED);
}