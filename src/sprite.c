#include "raylib.h"
#include "vector.h"

// STRUCTS & ENUMS
//------------------------------------------------------------------------------------------

typedef struct sprite {
    Texture2D texture;
    Vector2Int size;     // Height and Width of the texture
    Vector2Int origin;
    float scale;
} Sprite;


// FUNCTIONS
//------------------------------------------------------------------------------------------

/** Initializes a Sprite struct
 * 
*/
Sprite* InitSprite(char* path, Vector2Int origin, Vector2Int size, float scale) {
    Sprite* s = MemAlloc(sizeof(Sprite));
    if (s == NULL) {
		printf("[INIT SPRITE] [MEM ERROR] Failed to allocate memory for sprite at path %s\n", path);
		return NULL;
	}

    s->texture = LoadTexture(path);
    s->size = size;
    s->origin = origin;
    s->scale = scale;

    return s;
}

void RenderSprite(Sprite s, Vector2 position) {
    Rectangle spriteSrc = {
        s.origin.x,
        s.origin.y,
        s.origin.x + s.size.x,
        s.origin.y + s.size.y    
    };

    Rectangle spriteDst = {
        position.x,
        position.y,
        s.size.x * s.scale,
        s.size.y * s.scale
    };

    DrawTexturePro(s.texture, spriteSrc, spriteDst, (Vector2){0, 0}, 0, WHITE);
}