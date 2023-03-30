#ifndef MAPLOADER_H
#define MAPLOADER_H

typedef struct tileset Tileset;

void LoadMap(char* path);
void RenderMap();
int GetMapHeight();
int GetMapWidth();

#endif