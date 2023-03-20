#include "raylib.h"

typedef struct worldPos {
    int x;
    int y;
} WorldPos;

typedef struct entity {
	//Vector2 position;
    WorldPos position;
	Vector2 speed;
} Entity;

/** Updates Player Movement by adding speed to position vectors **/
void UpdateEntityVectors(Entity* e) {
	e->position.x += e->speed.x;
	e->position.y += e->speed.y;
}

void RenderEntity(Entity e, int tileSize) {
    DrawRectangle(e.position.x * tileSize, e.position.y * tileSize, tileSize, tileSize, RED);
}