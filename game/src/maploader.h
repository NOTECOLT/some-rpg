#ifndef MAPLOADER_H
#define MAPLOADER_H

typedef struct map Map;


// FUNCTIONS
//------------------------------------------------------------------------------------------

Map* LoadMap(char* path);
void FreeMap(Map* map);


// DEPRECATED 
//------------------------------------------------------------------------------------------

// typedef struct tileset Tileset;

// void LoadMap(char* path);
// void RenderMap();
// int GetMapHeight();
// int GetMapWidth();
// void BuildTileset();
// void FreeTileset();

#endif