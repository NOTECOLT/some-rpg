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

WELCOME TO C#! lol
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    static class Game {

        // int TILE_SIZE = 40;
        // Vector2 WORLD_SIZE = {
        //     .x = 39,
        //     .y = 22
        // };

        // Map* LOADED_MAP;

        // int P_SPEED_WLK = 350;
        // int P_SPEED_RUN = 450;

        public static void Main() {

            // WINDOW INITIALIZATION
            //--------------------------------------------------
            const int screenWidth = 960;
            const int screenHeight = 720;

            Raylib.InitWindow(screenWidth, screenHeight, "test");
            Raylib.SetTargetFPS(60);
            int frameCounter = 0;

            // WORLD INITIALIZATION
            //--------------------------------------------------
            
            //LOADED_MAP = ("resources\\maps\\testmap.json");
            
            // WORLD_SIZE = (Vector2) {
            // 	.x = GetMapWidth(),
            // 	.y = GetMapHeight()
            // };

            // PLAYER INITIALIZATION
            //--------------------------------------------------
            
            Entity player = new Entity(new Vector2(0, 0), EntityType.PLAYER, TILE_SIZE);
            //Entity* player = InitEntity((Vector2){.x = 0, .y = 0}, PLAYER, TILE_SIZE);
            player.SetMovementSpeeds(P_SPEED_WLK, P_SPEED_RUN);
            player.SetSprite("resources\\sprites\\characters\\player.png");

            // CAMERA INITIALIZATION
            //--------------------------------------------------
            Camera2D camera;
            
            camera.rotation = 0;
            camera.zoom = 1;

            while (!Raylib.WindowShouldClose()) {
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

                Raylib.BeginDrawing();
                Raylib.ClearBackground(RAYWHITE);

                camera.target = (Vector2){.x = player->position.x + (TILE_SIZE / 2), .y = player->position.y + (TILE_SIZE / 2),};
                camera.offset = (Vector2){.x = screenWidth / 2, .y = screenHeight / 2};
                Raylib.BeginMode2D(camera);

                // RenderWorld();
                // RenderMap();
                RenderEntity(*player, TILE_SIZE);

                Raylib.EndMode2D();

                Raylib.DrawText(Raylib.TextFormat("f: %d; fps: %d", frameCounter, Raylib.GetFPS()), 5, 5, 30, Color.BLACK);
                //DrawEntityDebugText(*player, TILE_SIZE);

                Raylib.EndDrawing();

                frameCounter++;
            }

            Raylib.CloseWindow();
        }
    }
}