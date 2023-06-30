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

        const int tileSize = 32;
        // Vector2 WORLD_SIZE = {
        //     .x = 39,
        //     .y = 22
        // };

        // Map* LOADED_MAP;

        const int playerWalkSpeed = 250;
        const int playerRunSpeed = 350;

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
            Console.WriteLine($"Current Working Directory: {Directory.GetCurrentDirectory()}");
            Map map = Map.LoadMap("resources\\maps\\testmap.json");
            

            // WORLD_SIZE = (Vector2) {
            // 	.x = GetMapWidth(),
            // 	.y = GetMapHeight()
            // };

            // PLAYER INITIALIZATION
            //--------------------------------------------------
            
            Entity player = new Entity(new Vector2(0, 0), EntityType.PLAYER, tileSize);
            //Entity* player = InitEntity((Vector2){.x = 0, .y = 0}, PLAYER, TILE_SIZE);
            player.SetMovementSpeeds(playerWalkSpeed, playerRunSpeed);
            player.SetSprite("resources\\sprites\\characters\\player.png");

            // CAMERA INITIALIZATION
            //--------------------------------------------------
            Camera2D camera;
            
            camera.rotation = 0;
            camera.zoom = 1;

            while (!Raylib.WindowShouldClose()) {
                // 1 - INPUT
                //--------------------------------------------------
                if (!player.isMoving) {
                    player.isRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

                    if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                        player.targetWP = new Vector2(player.worldPos.X + 1, player.worldPos.Y);
                    else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
						player.targetWP = new Vector2(player.worldPos.X - 1, player.worldPos.Y);
                    else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
						player.targetWP = new Vector2(player.worldPos.X, player.worldPos.Y + 1);
                    else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
						player.targetWP = new Vector2(player.worldPos.X, player.worldPos.Y - 1);
                    else
						player.targetWP = new Vector2(player.worldPos.X, player.worldPos.Y);
                }
                
                // 2 - PHYSICS
                //--------------------------------------------------

                player.UpdateEntityVectors(tileSize);

                // 3 - RENDERING
                //--------------------------------------------------

                Raylib.BeginDrawing();
					Raylib.ClearBackground(Color.RAYWHITE);

					camera.target = new Vector2(player.position.X + (tileSize / 2), player.position.Y + (tileSize / 2));
					camera.target = new Vector2(player.position.X + (tileSize / 2), player.position.Y + (tileSize / 2));
					camera.offset = new Vector2(screenWidth / 2, screenHeight / 2);
					Raylib.BeginMode2D(camera);

					// RenderWorld();
                    if (map != null)
					    map.RenderMap();
					player.RenderEntity(tileSize);

					Raylib.EndMode2D();

					Raylib.DrawText($"f: {frameCounter}; fps: {Raylib.GetFPS()}", 5, 5, 30, Color.BLACK);
					player.DrawEntityDebugText(tileSize);

                Raylib.EndDrawing();

                frameCounter++;
            }

            Raylib.CloseWindow();
        }
    }
}