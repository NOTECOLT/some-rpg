
//------------------------------------------------------------------------------------------
/*
TODO:
- Tilemap
	- I realize i need to do a tilemap system instead of loading a single png for the entire map
	- 		Question is, do i want to build it from scratch? :thinking:
	- Scene Loader
	
- Spritesheet animations

- Entity List
- Render Queue
- Debug Info Display

TODO: Fix VSCode Tasks.json so that I can debug within the editor lol

NOTE TO SELF: Uncomment -Wl,--subsystem,windows in the Makefile to hide the console window
*/
//------------------------------------------------------------------------------------------
#include <stdlib.h>
#include <stdio.h>
#include "raylib.h"
#include "entity.h"
#include "maploader.h"

int TILE_SIZE = 40;
Vector2 WORLD_SIZE = {
	.x = 39,
	.y = 22
};

Map* LOADED_MAP;

int P_SPEED_WLK = 350;
int P_SPEED_RUN = 450;

/** Draws the world */
void RenderWorld() {
	for (int i = 0; i < WORLD_SIZE.x; i++) {
		for (int j = 0; j < WORLD_SIZE.y; j++) {
			Color tileColor = ((i + j) % 2) ? LIGHTGRAY : WHITE;
			DrawRectangle(i * TILE_SIZE, j * TILE_SIZE, TILE_SIZE, TILE_SIZE, tileColor);
		}
	}
}

int main() {
	// WINDOW INITIALIZATION
	//--------------------------------------------------
	const int screenWidth = 960;
	const int screenHeight = 720;

	InitWindow(screenWidth, screenHeight, "test");
	SetTargetFPS(60);
	int frameCounter = 0;

	// WORLD INITIALIZATION
	//--------------------------------------------------
	
	printf("WORKING DIRECTORY %s\n", GetWorkingDirectory());
	LOADED_MAP = ("resources\\maps\\testmap.json");
	
	// WORLD_SIZE = (Vector2) {
	// 	.x = GetMapWidth(),
	// 	.y = GetMapHeight()
	// };

	// PLAYER INITIALIZATION
	//--------------------------------------------------
	Entity* player = InitEntity((Vector2){.x = 0, .y = 0}, PLAYER, TILE_SIZE);
	SetMovementSpeeds(player, P_SPEED_WLK, P_SPEED_RUN);
	SetSprite(player, "resources\\sprites\\characters\\player.png");

	// CAMERA INITIALIZATION
	//--------------------------------------------------
	Camera2D camera;
	
	camera.rotation = 0;
	camera.zoom = 1;

	while (!WindowShouldClose()) {
		// 1 - INPUT
		//--------------------------------------------------
		if (!player->isMoving) {
			player->isRunning = (IsKeyDown(KEY_LEFT_SHIFT)) ? 1 : 0;

			if (IsKeyDown(KEY_RIGHT)) 
				player->targetWP = (Vector2){.x = player->worldPos.x + 1, .y = player->worldPos.y};
			else if (IsKeyDown(KEY_LEFT))
				player->targetWP = (Vector2){.x = player->worldPos.x - 1, .y = player->worldPos.y};
			else if (IsKeyDown(KEY_DOWN))
				player->targetWP = (Vector2){.x = player->worldPos.x, .y = player->worldPos.y + 1};
			else if (IsKeyDown(KEY_UP))
				player->targetWP = (Vector2){.x = player->worldPos.x, .y = player->worldPos.y - 1};
		 	else
				player->targetWP = (Vector2){.x = player->worldPos.x, .y = player->worldPos.y};
		}
		
		// 2 - PHYSICS
		//--------------------------------------------------

		UpdateEntityVectors(player, TILE_SIZE);
		camera.target = (Vector2){.x = player->position.x, .y = player->position.y};

		// 3 - RENDERING
		//--------------------------------------------------

        BeginDrawing();
		ClearBackground(RAYWHITE);

		camera.target = (Vector2){.x = player->position.x + (TILE_SIZE / 2), .y = player->position.y + (TILE_SIZE / 2),};
		camera.offset = (Vector2){.x = screenWidth / 2, .y = screenHeight / 2};
		BeginMode2D(camera);

		// RenderWorld();
		// RenderMap();
		RenderEntity(*player, TILE_SIZE);

		EndMode2D();

		DrawText(TextFormat("f: %d; fps: %d", frameCounter, GetFPS()), 5, 5, 30, BLACK);
		DrawEntityDebugText(*player, TILE_SIZE);

        EndDrawing();

		frameCounter++;
	}

	FreeMap(LOADED_MAP);
	FreeEntity(player);

	CloseWindow();

    return 0;
}

