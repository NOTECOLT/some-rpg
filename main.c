#include <stdlib.h>
#include "raylib.h"
#include "src/entity.h"

int TILE_SIZE = 40;
Vector2Int WORLD_SIZE = {
	.x = 39,
	.y = 22
};

int P_SPEED_WLK = 350;
int P_SPEED_RUN = 425;

/*
TODO:
- Grid Based Movement - FIX THIS YO
- Run Movement
- Placeholder Sprites
- Tiled Map Editor

- Entity List
- Render Queue
- Debug Info Display

*/

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

	// CAMERA INITIALIZATION
	//--------------------------------------------------
	Camera2D camera;
	
	camera.rotation = 0;
	camera.zoom = 1;

	// PLAYER INITIALIZATION
	//--------------------------------------------------
	Entity* player = InitEntity((Vector2Int){.x = 0, .y = 0}, TILE_SIZE);
	SetMovementSpeeds(player, P_SPEED_WLK, P_SPEED_RUN);

	if (player == NULL) {
		// INSERT SOME KIND OF ERROR MESSAGE HERE
		return 1;
	}

	while (!WindowShouldClose()) {
		// 1 - INPUT
		//--------------------------------------------------
		if (!player->isMoving) {
			if (IsKeyDown(KEY_RIGHT))
				player->targetWP = (Vector2Int){.x = player->worldPos.x + 1, .y = player->worldPos.y};
			else if (IsKeyDown(KEY_LEFT))
				player->targetWP = (Vector2Int){.x = player->worldPos.x - 1, .y = player->worldPos.y};
			else if (IsKeyDown(KEY_DOWN))
				player->targetWP = (Vector2Int){.x = player->worldPos.x, .y = player->worldPos.y + 1};
			else if (IsKeyDown(KEY_UP))
				player->targetWP = (Vector2Int){.x = player->worldPos.x, .y = player->worldPos.y - 1};
			else
				player->targetWP = (Vector2Int){.x = player->worldPos.x, .y = player->worldPos.y};
				
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

		RenderWorld();
		RenderEntity(*player, TILE_SIZE);

		EndMode2D();

		DrawText(TextFormat("f: %d; fps: %d", frameCounter, GetFPS()), 5, 5, 30, BLACK);
		DrawEntityDebugText(*player, TILE_SIZE);

        EndDrawing();

		frameCounter++;
	}
	
	free(player);

	CloseWindow();

    return 0;
}

