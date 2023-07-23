//------------------------------------------------------------------------------------------
/*
TODO:
//	simple tilemap editor? Or something
//	multiple tilemaps per map
/// TODO: layers
/	Need something to handle collisions and stuff
	Scene Loader
	
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
    enum DebugState {
        GAME,
        DEBUG_MAPEDIT
    }

	/// <summary>
	/// Temporary static class to hold some global constants.
	/// <para> Will /prolly/ delete this when I can put this somehwere else</para>
	/// </summary>
	static class Globals {
        // RANDOM CONSTANTS, will eventually move these values
        public const int TILE_SIZE = 32;
        public const int PLAYER_WALKSPEED = 200;
        public const int PLAYER_RUNSPEED = 300;

		public const int SCREEN_WIDTH = 960;
		public const int SCREEN_HEIGHT = 720;

		public const float WORLD_SCALE = 2.0f;	// This value scales sprites to fit with the tilesize
	}

    static class Game {
		const bool devMode = true;

        public static void Main() {
			DebugState debugState = DebugState.GAME;

            // WINDOW INITIALIZATION
            //--------------------------------------------------
            Raylib.InitWindow(Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT, "test");
            Raylib.SetTargetFPS(60);
            int frameCounter = 0;

            // WORLD INITIALIZATION
            //--------------------------------------------------
            Console.WriteLine($"Current Working Directory: {Directory.GetCurrentDirectory()}");
            Map map = Map.LoadMap("resources/maps/testmap.json");
            
            // PLAYER INITIALIZATION
            //--------------------------------------------------
            
            Entity player = new Entity(new Vector2(0, 0), EntityType.PLAYER, Globals.TILE_SIZE);
            player.SetMovementSpeeds(Globals.PLAYER_WALKSPEED, Globals.PLAYER_RUNSPEED);
            player.SetSprite("resources/sprites/characters/player.png");

            // CAMERA INITIALIZATION
            //--------------------------------------------------
            Camera2D camera = new Camera2D();
            
            camera.rotation = 0;
            camera.zoom = 1;

            // MAP EDITOR
            //--------------------------------------------------
			MapEditor mapEditor = new MapEditor();
			mapEditor.LoadMap(map);

            while (!Raylib.WindowShouldClose()) {
				if (devMode) {
					if (Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) {
						if (debugState == DebugState.DEBUG_MAPEDIT) {
							map = mapEditor.LoadedMap;
							Map.SaveMap(map, "resources/maps/testmap.json");
						}
							
						debugState = (DebugState)(((int)debugState + 1) % Enum.GetNames(typeof(DebugState)).Length);
						Console.WriteLine($"[DEVMODE] GAMESTATE CHANGED TO {debugState}");
					}
				}
				
				switch (debugState) {
					case DebugState.GAME:
						OverworldGameLoop(ref player, ref camera, ref map, frameCounter);
						break;
					case DebugState.DEBUG_MAPEDIT:
						mapEditor.MapEditorLoop(ref camera);
						break;
					default:
						break;
				}

				frameCounter++;
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
		public static void OverworldGameLoop(ref Entity player, ref Camera2D camera, ref Map map, int frameCounter) {
			// 1 - INPUT
			//--------------------------------------------------
			if (!player.IsMoving) {
				player.IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

				if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
					player.TargetWP = new Vector2(player.WorldPos.X + 1, player.WorldPos.Y);
				else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
					player.TargetWP = new Vector2(player.WorldPos.X - 1, player.WorldPos.Y);
				else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
					player.TargetWP = new Vector2(player.WorldPos.X, player.WorldPos.Y + 1);
				else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
					player.TargetWP = new Vector2(player.WorldPos.X, player.WorldPos.Y - 1);
				else
					player.TargetWP = new Vector2(player.WorldPos.X, player.WorldPos.Y);
			}

			// if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
			// 	Vector2 mousePosition = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
			// 	Console.WriteLine($"({mousePosition.X}, {mousePosition.Y})");
			// }

			
			// 2 - PHYSICS
			//--------------------------------------------------

			player.UpdateEntityVectors(Globals.TILE_SIZE);

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);
				camera.target = new Vector2(player.Position.X + (Globals.TILE_SIZE / 2), player.Position.Y + (Globals.TILE_SIZE / 2));
				camera.offset = new Vector2(Globals.SCREEN_WIDTH / 2, Globals.SCREEN_HEIGHT / 2);
				Raylib.BeginMode2D(camera);

					if (map != null)
						map.RenderMap(Globals.WORLD_SCALE);
					player.RenderEntity(Globals.TILE_SIZE, Globals.WORLD_SCALE);

				Raylib.EndMode2D();

				Raylib.DrawText($"f: {frameCounter}; fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: OVERWORLD", 5, 40, 30, Color.BLACK);
				player.DrawEntityDebugText(Globals.TILE_SIZE, new Vector2(5, 75));

			Raylib.EndDrawing();
		}
	}
}