//------------------------------------------------------------------------------------------
/*
TODO: (Engine Stuff)
/	Render Queue (do i need one yet? Not really i guess; but soon i'll have to)
//	ECS								<-----
//	NPCs / Interactable Objects		<-----
//	Dialogue Box & Text Printing
	/	Add more FontProperties
	//	Create TextObject UIEntity
	/	Adjust Dialogue Box Display
/	Flag System
/	Multiple Scenes & Scene Switching
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
using Raylib_cs;
using Topdown.Scene;

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

            // SCENE INITIALIZATION
            //--------------------------------------------------
			
			SceneLoader sceneLoader = new SceneLoader();
			OverworldScene overworldScene = new OverworldScene("resources/maps/testmap.tmx");

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