#include <stdlib.h>
#include <stdio.h>
#include "raylib.h"
#include "sprite.h"

// FUNCTIONS
//------------------------------------------------------------------------------------------

/** Initializes a Sprite struct
 * 
*/
Sprite* InitSprite(char* path, Vector2 origin, Vector2 size, float scale) {
    Sprite* s = MemAlloc(sizeof(Sprite));
    if (s == NULL) {
		printf("[INIT SPRITE] [MEM ERROR] Failed to allocate memory for sprite at path %s\n", path);
		return NULL;
	}

    s->texture = LoadTexture(path);
    s->size = size;
    s->origin = origin;
    s->scale = scale;
    s->rotation = 0;

    return s;
}

/** Renders a sprite according to the values placed by the struct */
void RenderSprite(Sprite s, Vector2 position, Vector2 offset) {
    Rectangle spriteSrc = {
        s.origin.x,
        s.origin.y,
        s.size.x,
        s.size.y    
    };

    Rectangle spriteDst = {
        position.x,
        position.y,
        s.size.x * s.scale,
        s.size.y * s.scale
    };

    DrawTexturePro(s.texture, spriteSrc, spriteDst, offset, s.rotation, WHITE);
}