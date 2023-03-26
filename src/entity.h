#include "raylib.h"
#include "entity.c"

typedef struct vector2Int Vector2Int;
typedef enum entityType EntityType;
typedef struct entity Entity;

Entity* InitEntity(Vector2Int worldPos, EntityType type, int tileSize);
void FreeEntity(Entity* e);
void SetMovementSpeeds(Entity* e, float walkSpeed, float runSpeed);
void UpdateEntityVectors(Entity* e, int tileSize);
void RenderEntity(Entity e, int tileSize);

void DrawEntityDebugText(Entity e, int tileSize);