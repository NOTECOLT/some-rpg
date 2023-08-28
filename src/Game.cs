//------------------------------------------------------------------------------------------
/*
TODO: (Engine Stuff)
/	Render Queue (do i need one yet? Not really i guess; but soon i'll have to)
/	ECS								<-----
/	NPCs / Interactable Objects		<-----
/	Flag System
/	Dialogue Box & Text Printing
/	Debug Info Display

TODOL (Mechanic / Game Design Stuff)
/	Game Loop
/	Combat System
/	Menu System

TODO: (Low ish priority; aesthetic, skin, etc.)
/ 	Spritesheet animations

WELCOME TO C#! lol
*/
//------------------------------------------------------------------------------------------
using System.Collections;
using System.Numerics;
using Raylib_cs;
using Topdown.ECS;

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

			XMLDialogueParser.LoadDialogueFromFile("resources\\dialogue\\test\\testsign.xml");

            // WINDOW INITIALIZATION
            //--------------------------------------------------
            Raylib.InitWindow(Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT, "test");
            Raylib.SetTargetFPS(60);

            // WORLD INITIALIZATION
            //--------------------------------------------------
            Console.WriteLine($"Current Working Directory: {Directory.GetCurrentDirectory()}");
            
            // PLAYER INITIALIZATION
            //--------------------------------------------------
            
            Entity player = new Entity(new Vector2(0, 0), 0, Globals.TILE_SIZE);
            player.SetMovementSpeeds(Globals.PLAYER_WALKSPEED, Globals.PLAYER_RUNSPEED);
            player.SetSprite("resources/sprites/characters/player.png");

            // SCENE INITIALIZATION
            //--------------------------------------------------
			
			SceneLoader sceneLoader = new SceneLoader();
			OverworldScene overworldScene = new OverworldScene(player, "resources/maps/testmap.tmx");

			sceneLoader.LoadScene(overworldScene);

            while (!Raylib.WindowShouldClose()) {
				if (devMode) {
					if (Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) {
						debugState = (DebugState)(((int)debugState + 1) % Enum.GetNames(typeof(DebugState)).Length);
						Console.WriteLine($"[DEVMODE] GAMESTATE CHANGED TO {debugState}");

						switch (debugState) {
							case DebugState.GAME:
								sceneLoader.LoadScene(overworldScene);
								break;
							case DebugState.DEBUG_MAPEDIT:
								// sceneLoader.LoadScene(mapEditorScene);
								break;
							default:
								break;
						}
					}
				}

				sceneLoader.UpdateCurrentScene();
            }

            Raylib.CloseWindow();
        }
	}
}