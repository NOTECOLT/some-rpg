﻿//------------------------------------------------------------------------------------------
/*
TODO:
//	simple tilemap editor? Or something
//	multiple tilemaps per map
/// TODO: layers
/// TODO: UI Elements (mainly buttons)
/// TODO: Scene Loader
	-- Why? Need to be initialize UI elements in a scene before its main render loop
	-- Otherwise we would be constantly re-initializing UI elements with every frame
/	Need something to handle collisions and stuff
	
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
using Topdown.GUI;
using Topdown.Engine;

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

            // WORLD INITIALIZATION
            //--------------------------------------------------
            Console.WriteLine($"Current Working Directory: {Directory.GetCurrentDirectory()}");
            Map map = Map.LoadMap("resources/maps/testmap.json");
            
            // PLAYER INITIALIZATION
            //--------------------------------------------------
            
            Entity player = new Entity(new Vector2(0, 0), EntityType.PLAYER, Globals.TILE_SIZE);
            player.SetMovementSpeeds(Globals.PLAYER_WALKSPEED, Globals.PLAYER_RUNSPEED);
            player.SetSprite("resources/sprites/characters/player.png");

            // SCENE INITIALIZATION
            //--------------------------------------------------
			
			SceneLoader sceneLoader = new SceneLoader();

			OverworldScene overworldScene = new OverworldScene(player, map);
			MapEditorScene mapEditorScene = new MapEditorScene();
			mapEditorScene.LoadMap(map);

            while (!Raylib.WindowShouldClose()) {
				if (devMode) {
					if (Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) {
						if (debugState == DebugState.DEBUG_MAPEDIT) {
							map = mapEditorScene.LoadedMap;
							Map.SaveMap(map, "resources/maps/testmap.json");
						}
							
						debugState = (DebugState)(((int)debugState + 1) % Enum.GetNames(typeof(DebugState)).Length);
						Console.WriteLine($"[DEVMODE] GAMESTATE CHANGED TO {debugState}");
					}
				}

				sceneLoader.UpdateCurrentScene();
				
				switch (debugState) {
					case DebugState.GAME:
						sceneLoader.LoadScene(overworldScene);
						break;
					case DebugState.DEBUG_MAPEDIT:
						sceneLoader.LoadScene(mapEditorScene);
						break;
					default:
						break;
				}
            }

            Raylib.CloseWindow();
        }
	}
}