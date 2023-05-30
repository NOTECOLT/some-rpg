#include <stdlib.h>
#define CUTE_TILED_IMPLEMENTATION
#include "lib\cute_tiled.h"

#include "raylib.h"
#include "maploader.h"
#include "sprite.h"


// STRUCTS, ENUMS, & GLOBALS
//------------------------------------------------------------------------------------------

typedef struct cute_tiled_map_t TileMap;
typedef struct cute_tiled_layer_t MapLayer;
typedef struct cute_tiled_tileset_t CuteTileset;

/** Tileset object, for internal usage; different from CuteTileset Object
 * - Tiles are built with their gid serving as their array index
 * - Each tile points to its corresponding source texture in the srcTiles array
*/
typedef struct tileset {
    int size;

	/** Each tile points to its corresponding source texture in the srcTiles array 
	 * NOTE: Tiles are 0 INDEXED, when converting from tilemap JSON GID (given GID = N), their corresponding tileset index is always N - 1
	*/
	Texture2D** tiles;
	Vector2* tilePos;	// The position of each tile on their corresponding source texture file

    Vector2 tileSize;
    Texture2D* srcTiles;
} Tileset;

TileMap* LOADED_MAP = NULL;
CuteTileset* LOADED_CUTE_TILESET = NULL;
Tileset* CURRENT_TILESET = NULL;

// FUNCTIONS
//------------------------------------------------------------------------------------------

void LoadMap(char* path) {
    TileMap* map = cute_tiled_load_map_from_file(path, 0);

    LOADED_MAP = map;
    LOADED_CUTE_TILESET = map->tilesets;

    CuteTileset* current = LOADED_CUTE_TILESET;

    BuildTileset((Vector2) {.x = 16, .y = 16});
}

void BuildTileset(Vector2 tileSize) {
    if (CURRENT_TILESET == NULL)
        CURRENT_TILESET = malloc(sizeof(Tileset));

    if (LOADED_CUTE_TILESET == NULL) {
        printf("[MAPLOADER] No Tileset currently loaded. Cannot build Tileset.\n");
        return;
    }

	// SET TILESET SIZES & SRC FILES1
	// I want do this instead of dynamically reallocating memory with each tileset image
	// Effectively though this does mean we're looping through the loaded_tilesets twice.
	//--------------------------------------------------

    CuteTileset* set = LOADED_CUTE_TILESET;

    int sz = 0;
	int setCount = 0;
	
	while (set) {
		
		sz += set->tilecount;
		

		printf("[TILESET BUILDER] tileset [%d], tiles:[%d] - > [%d]\n", setCount, set->firstgid - 1, sz - 1);

		setCount += 1;
		set = set->next;

		
	}

	CURRENT_TILESET->size = sz;
    CURRENT_TILESET->tiles = malloc(sizeof(Texture2D*) * sz);
    CURRENT_TILESET->srcTiles = malloc(sizeof(Texture2D) * setCount);
	CURRENT_TILESET->tilePos = malloc(sizeof(Vector2*) * sz);

	CURRENT_TILESET->tileSize = tileSize;

	printf("[TILESET BUILDER] Building tile set of size %d\n", CURRENT_TILESET->size);

	// BUILDING TILESET OBJECT
	//--------------------------------------------------

	set = LOADED_CUTE_TILESET;

	setCount = 0;
    while (set) {
		printf("[TILESET BUILDER] TEXTURE: %s\n", set->image.ptr);
		
		CURRENT_TILESET->srcTiles[setCount] = LoadTexture(set->image.ptr);

		int x = set->firstgid;
		for (int i = 0; i < set->imageheight; i += set->tileheight) {
			for (int j = 0; j < set->imagewidth; j += set->tilewidth) {
				CURRENT_TILESET->tiles[x - 1] = &(CURRENT_TILESET->srcTiles[setCount]);
				// CURRENT_TILESET->tilePos[x - 1] = malloc(sizeof(Vector2));
				CURRENT_TILESET->tilePos[x - 1] = (Vector2) {
					.x = i,
					.y = j
				};
				
							// TODO TILE POSITIONS NOT BEING SAVED HERE
							// 		MALLOC ???
							//		Also check tiles srcTiles array if textures are even being saved
				//printf("[TILESET BUILDER] tiles[%d] = \t(%.0f, %.0f) \tat file (%d)\n", x - 1, CURRENT_TILESET->tilePos[x - 1].x, CURRENT_TILESET->tilePos[x - 1].y, setCount);
				printf("[TILESET BUILDER] tiles[%d] = \tfile %p\n", x - 1, &(CURRENT_TILESET->srcTiles[setCount]));
				x += 1;

				
			}
		}

		setCount += 1;
        set = set->next;
    }

    
}

void RenderMap() {
    if (LOADED_MAP == NULL) {
        printf("[MAPLOADER] No Map currently loaded.\n");
        return;
    }

	if (CURRENT_TILESET == NULL) {
		printf("[MAPLOADER] No Tileset currently loaded.\n");
	}
    
    MapLayer* layer = LOADED_MAP->layers;

	int mapWidth = GetMapWidth();

    //while (layer) {
		int* data = layer->data;
		int dataCount = layer->data_count;
		int h = layer->height;
		int w = layer->width;

		Sprite spr;
		for (int i = 0; i < dataCount; i++) {
			if (data[i] != 0) {
				spr = (Sprite) {
					.texture = *(CURRENT_TILESET->tiles[data[i] - 1]),
					.origin = (CURRENT_TILESET->tilePos[data[i] - 1]),
					.size = CURRENT_TILESET->tileSize,
					.scale = 2.0f,
					.rotation = 0
				};

				RenderSprite(spr, (Vector2){(i % w)* 32, (i / w) * 32}, (Vector2){0, 0});
				//printf("%d, (file %p; pos: (%.0f, %.0f)) \n", data[i] - 1, CURRENT_TILESET->tiles[data[i] - 1], CURRENT_TILESET->tilePos[data[i] - 1].x, CURRENT_TILESET->tilePos[data[i] - 1].y);
			}
		}

		//layer = layer->next;
	//}
}

void FreeTileset() {
	if (CURRENT_TILESET == NULL) { 
		printf("[FREE ENTITY] No memory at %p to be freed\n", CURRENT_TILESET);
		return;
	}

	free(CURRENT_TILESET->srcTiles);
	free(CURRENT_TILESET->tilePos);
	free(CURRENT_TILESET->tiles);

	free(CURRENT_TILESET);
	CURRENT_TILESET = NULL;
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