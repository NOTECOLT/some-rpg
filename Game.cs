//------------------------------------------------------------------------------------------
/*
TODO:
- Movement: DO MOVEMENT BETTER? But i can put this on the backburner naman i think
- simple tilemap editor? Or something
-   Need something to handle collisions and stuff
	- Scene Loader
	
- Spritesheet animations

- Entity List
- Render Queue
- Debug Info Display

WELCOME TO C#! lol
*/
//------------------------------------------------------------------------------------------
using System.Collections;
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    enum GameState {
        OVERWORLD,
        DEBUG_MAPEDIT
    }
    static class Game {
		const bool devMode = true;

        // RANDOM CONSTANTS, will eventually move these values
        const int tileSize = 32;
        const int playerWalkSpeed = 200;
        const int playerRunSpeed = 300;

		const int screenWidth = 960;
		const int screenHeight = 720;

        public static void Main() {
			GameState gameState = GameState.OVERWORLD;

            // WINDOW INITIALIZATION
            //--------------------------------------------------
            Raylib.InitWindow(screenWidth, screenHeight, "test");
            Raylib.SetTargetFPS(60);
            int frameCounter = 0;

            // WORLD INITIALIZATION
            //--------------------------------------------------
            Console.WriteLine($"Current Working Directory: {Directory.GetCurrentDirectory()}");
            Map map = Map.LoadMap("resources\\maps\\testmap.json");
            
            // PLAYER INITIALIZATION
            //--------------------------------------------------
            
            Entity player = new Entity(new Vector2(0, 0), EntityType.PLAYER, tileSize);
            player.SetMovementSpeeds(playerWalkSpeed, playerRunSpeed);
            player.SetSprite("resources\\sprites\\characters\\player.png");

            // CAMERA INITIALIZATION
            //--------------------------------------------------
            Camera2D camera = new Camera2D();
            
            camera.rotation = 0;
            camera.zoom = 1;

            while (!Raylib.WindowShouldClose()) {
				if (devMode) {
					if (Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) {
						gameState = (GameState)(((int)gameState + 1) % Enum.GetNames(typeof(GameState)).Length);
						Console.WriteLine($"[DEVMODE] GAMESTATE CHANGED TO {gameState} on frame {frameCounter}");
					}
				}
				
				switch (gameState) {
					case GameState.OVERWORLD:
						OverworldGameLoop(ref player, ref camera, ref map, ref frameCounter);
						break;
					case GameState.DEBUG_MAPEDIT:
						DebugMapEditLoop(ref player, ref camera, ref map, ref frameCounter);
						break;
					default:
						break;
				}
            }

            Raylib.CloseWindow();
        }

		// GAME STATE LOOPS
		//--------------------------------------------------

		/// <summary>
		/// Main Overworld Game Loop.
		/// <para>In this state, the player can see their avatar walking around. </para>
		/// </summary>
		/// <param name="player"></param>
		/// <param name="camera"></param>
		/// <param name="map"></param>
		/// <param name="frameCounter"></param>
        public static void DebugMapEditLoop(ref Entity player, ref Camera2D camera, ref Map map, ref int frameCounter) {
			// 1 - INPUT
			//--------------------------------------------------
			
			// 2 - PHYSICS
			//--------------------------------------------------

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);

				camera.target = new Vector2(player.position.X + (tileSize / 2), player.position.Y + (tileSize / 2));
				camera.target = new Vector2(player.position.X + (tileSize / 2), player.position.Y + (tileSize / 2));
				camera.offset = new Vector2(screenWidth / 2, screenHeight / 2);
				Raylib.BeginMode2D(camera);

					if (map != null)
						map.RenderMap();

				Raylib.EndMode2D();

				Raylib.DrawText($"f: {frameCounter}; fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: DEBUG MAP EDIT", 5, 40, 30, Color.BLACK);

			Raylib.EndDrawing();

			frameCounter++;
        }
		public static void OverworldGameLoop(ref Entity player, ref Camera2D camera, ref Map map, ref int frameCounter) {
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

					if (map != null)
						map.RenderMap();
					player.RenderEntity(tileSize);

				Raylib.EndMode2D();

				Raylib.DrawText($"f: {frameCounter}; fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: OVERWORLD", 5, 40, 30, Color.BLACK);
				player.DrawEntityDebugText(tileSize, new Vector2(5, 75));

			Raylib.EndDrawing();

			frameCounter++;
		}
	}
}