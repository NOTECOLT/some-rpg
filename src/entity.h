#include "raylib.h"
#include "entity.c"

typedef struct vector2Int Vector2Int;
typedef struct entity Entity;

Entity* InitEntity(Vector2Int worldPos, int tileSize);
void SetMovementSpeeds(Entity* e, float walkSpeed, float runSpeed);
void UpdateEntityVectors(Entity* e, int tileSize);
void RenderEntity(Entity e, int tileSize);

void DrawEntityDebugText(Entity e, int tileSize);