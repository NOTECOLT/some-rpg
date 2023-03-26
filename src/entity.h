//------------------------------------------------------------------------------------------
/** ENTITY.C
 * Contains the entity struct and any/all functions relating to it.
 * Entities are created in the heap
*/
//------------------------------------------------------------------------------------------

#ifndef ENTITY_H
#define ENTITY_H

#include "entity.c"

typedef enum entityType EntityType;
typedef struct entity Entity;

Entity* InitEntity(Vector2 worldPos, EntityType type, int tileSize);
void FreeEntity(Entity* e);
void SetMovementSpeeds(Entity* e, float walkSpeed, float runSpeed);
void UpdateEntityVectors(Entity* e, int tileSize);
void RenderEntity(Entity e, int tileSize);

void DrawEntityDebugText(Entity e, int tileSize);

#endif