//------------------------------------------------------------------------------------------
/** ENTITY.H
 * Contains the entity struct and any/all functions relating to it.
 * Entities are created in the heap
*/
//------------------------------------------------------------------------------------------

#ifndef ENTITY_H
#define ENTITY_H

#include "raylib.h"
#include "sprite.h"

// STRUCTS & ENUMS
//------------------------------------------------------------------------------------------

/** Defines the type of entity */
typedef enum entityType {
	PLAYER,
	ENEMY
} EntityType;

/** An Entity is any moving/interactable object in the overworld */
typedef struct entity {
	EntityType type;
	// Texture2D sprite;
	Sprite* spr;

	Vector2 position;		// Refers to the position of the player with respect to the screen / global coordinate system  
    Vector2 worldPos;    // Refers to the position of player relative to the world grid
	Vector2 targetWP;	// Entity's target world vector. Each entity will constantly move to this location if it is not already.

	int isMoving;		// 1 if the entity is moving, 0 otherwise
	int isRunning;		// 1 if the entity is running, 0 otherwise. isMoving must be set to 1 for this to take effect.
	
	float speed;		// Speed of the entity whilst walking
	float runSpeed;		// Speed of the entity wihile running
} Entity;

// FUNCTIONS
//------------------------------------------------------------------------------------------

Entity* InitEntity(Vector2 worldPos, EntityType type, int tileSize);
void FreeEntity(Entity* e);
void SetMovementSpeeds(Entity* e, float walkSpeed, float runSpeed);
void SetSprite(Entity* e, char* spritePath);
void UpdateEntityVectors(Entity* e, int tileSize);
void RenderEntity(Entity e, int tileSize);

void DrawEntityDebugText(Entity e, int tileSize);

#endif