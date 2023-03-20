#include "raylib.h"
#include "src/entity.c"

int TILESIZE = 40;
Vector2 WORLDSIZE = {
	.x = 20,
	.y = 20
};

/** Draws the world **/
int RenderWorld() {
	for (int i = 0; i < WORLDSIZE.x; i++) {
		for (int j = 0; j < WORLDSIZE.y; j++) {
			Color tileColor = ((i + j) % 2) ? LIGHTGRAY : WHITE;
			DrawRectangle(i * TILESIZE, j * TILESIZE, TILESIZE, TILESIZE, tileColor);
		}
	}
}


int main() {
	const int screenWidth = 800;
	const int screenHeight = 450;

	InitWindow(screenWidth, screenHeight, "test");
	SetTargetFPS(60);

	// PLAYER STUFF
	Entity player;
	player.position = (WorldPos){ .x = 0, .y = 0 };
	player.speed = (Vector2){ .x = 0, .y = 0 };

	// WORLD STUFF

	while (!WindowShouldClose()) {
        BeginDrawing();
		ClearBackground(RAYWHITE);

		// INPUT
		if (IsKeyDown(KEY_RIGHT))
			player.position.x += 1;
		else if (IsKeyDown(KEY_LEFT))
			player.position.x -= 1;

		if (IsKeyDown(KEY_DOWN))
			player.position.y += 1;
		else if (IsKeyDown(KEY_UP))
			player.position.y -= 1;


		//UpdateEntityVectors(&player);

		RenderWorld();
		RenderEntity(player, TILESIZE);

        EndDrawing();		
	}

	CloseWindow();

    return 0;
}