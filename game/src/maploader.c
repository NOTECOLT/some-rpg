#include <stdlib.h>
#define CUTE_TILED_IMPLEMENTATION
#include "lib\cute_tiled.h"

#include "raylib.h"
#include "maploader.h"


// STRUCTS, ENUMS, & GLOBALS
//------------------------------------------------------------------------------------------

typedef struct cute_tiled_map_t TileMap;
typedef struct cute_tiled_layer_t MapLayer;
typedef struct cute_tiled_tileset_t CuteTileset;

typedef struct tileset {
// IDK
} Tileset;

TileMap* LOADED_MAP = NULL;
CuteTileset* LOADED_TILESET = NULL;

// FUNCTIONS
//------------------------------------------------------------------------------------------

void LoadMap(char* path) {
    TileMap* map = cute_tiled_load_map_from_file(path, 0);

    LOADED_MAP = map;
    LOADED_TILESET = map->tilesets;
}



void RenderMap() {
    if (LOADED_MAP == NULL) {
        printf("[MAPLOADER] No Map currently loaded.\n");
        return;
    }
    
    MapLayer* layer = LOADED_MAP->layers;

	int mapWidth = GetMapWidth();

    while (layer) {
		int* data = layer->data;
		int dataCount = layer->data_count;

		printf("[LAYER %s]\n", layer->name.ptr);
		for (int i = 0; i < dataCount; i++) {
			printf("%d, ", data[i]);

			if ((i + 1) % mapWidth == 0) printf("\n");
		}


		layer = layer->next;
	}


    // printf("%d\n", LOADED_MAP->height);
}

int GetMapHeight() {
    if (LOADED_MAP == NULL) {
        printf("[MAPLOADER] No Map currently loaded.\n");
        return -1;
    }
	
	return LOADED_MAP->height;
}

int GetMapWidth() {
    if (LOADED_MAP == NULL) {
        printf("[MAPLOADER] No Map currently loaded.\n");
        return -1;
    }
	
	return LOADED_MAP->width;
}