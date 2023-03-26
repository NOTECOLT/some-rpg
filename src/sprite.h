#ifndef SPRITE_H
#define SPRITE_H

#include "sprite.c"

typedef struct sprite Sprite;

Sprite* InitSprite(char* path, Vector2 origin, Vector2 size, float scale);
void RenderSprite(Sprite s, Vector2 position, Vector2 offset);

#endif