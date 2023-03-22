#include "raylib.h"
#include "src/entity.c"

int TILE_SIZE = 40;
Vector2Int WORLD_SIZE = {
	.x = 39,
	.y = 22
};

int P_SPEED_WLK = 350;
int P_SPEED_RUN = 425;

/*
IDEAS:
- Grid Based Movement
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
	const int screenWidth = 1600;
	const int screenHeight = 900;

	InitWindow(screenWidth, screenHeight, "test");
	SetTargetFPS(60);
	int frameCounter = 0;

	// PLAYER STUFF
	Entity* player = NewEntity((Vector2Int){.x = 0, .y = 0}, TILE_SIZE, P_SPEED_WLK);

	// WORLD STUFF

	while (!WindowShouldClose()) {
		// 1 - INPUT
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

		UpdateEntityVectors(player, TILE_SIZE);

		// 3 - RENDERING

        BeginDrawing();
		ClearBackground(RAYWHITE);

		RenderWorld();
		RenderEntity(*player, TILE_SIZE);
		DrawText(TextFormat("f: %d; fps: %d", frameCounter, GetFPS()), 5, 5, 30, BLACK);
		DrawText(TextFormat("position: (%f, %f)", player->position.x, player->position.y), 5, 40, 30, BLACK);
		DrawText(TextFormat("worldPos: (%d, %d)", player->worldPos.x, player->worldPos.y), 5, 75, 30, BLACK);
		DrawText(TextFormat("targetWP: (%d, %d)", player->targetWP.x, player->targetWP.y), 5, 110, 30, BLACK);
		float distX = abs(player->position.x - (float)player->targetWP.x * TILE_SIZE);
		float distY = abs(player->position.y - (float)player->targetWP.y * TILE_SIZE);
		DrawText(TextFormat("distance: (%f, %f)", distX, distY), 5, 140, 30, BLACK);
		DrawText(TextFormat("isMoving: %d", player->isMoving), 5, 175, 30, BLACK);

        EndDrawing();

		frameCounter++;
	}

	free(player);

	CloseWindow();

    return 0;
}