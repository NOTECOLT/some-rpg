#include "raylib.h"
#include "src/player.c"

int main() {
	const int screenWidth = 800;
	const int screenHeight = 450;

	Player player;
	player.position = (Vector2){ .x = 0, .y = 0 };
	player.speed = (Vector2){ .x = 2, .y = 0 };

	InitWindow(screenWidth, screenHeight, "test");
	SetTargetFPS(60);

	while (!WindowShouldClose()) {
        BeginDrawing();

            ClearBackground(RAYWHITE);

            DrawText("you know what go fuck yourself!", 190, 200, 20, LIGHTGRAY);

			UpdatePlayerVectors(&player);

			DrawRectangle(player.position.x, player.position.y, 50, 50, RED);


        EndDrawing();		
	}

	CloseWindow();

    return 0;
}