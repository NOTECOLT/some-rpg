#ifndef SPRITE_H
#define SPRITE_H

#include "sprite.c"

typedef struct sprite Sprite;

Sprite* InitSprite(char* path, Vector2Int origin, Vector2Int size, float scale);
void RenderSprite(Sprite s, Vector2 position);

#endif