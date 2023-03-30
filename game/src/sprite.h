#ifndef SPRITE_H
#define SPRITE_H

// STRUCTS & ENUMS
//------------------------------------------------------------------------------------------

/** A sprite not only features the texture, but also other information pertaining to it */
typedef struct sprite {
    Texture2D texture;
    Vector2 size;     // Height and Width of the texture
    Vector2 origin;
    float scale;
    float rotation;
} Sprite;

// FUNCTIONS
//------------------------------------------------------------------------------------------

Sprite* InitSprite(char* path, Vector2 origin, Vector2 size, float scale);
void RenderSprite(Sprite s, Vector2 position, Vector2 offset);

#endif