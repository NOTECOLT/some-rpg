//------------------------------------------------------------------------------------------
/*
TODO: (Engine Stuff)
/	Flag System
/		Save Data
// 	Spritesheet animations
/	Debug Info Display

TODO: (Mechanic / Game Design Stuff)
/	Game Loop
/		Inventory System
/		Experience System
/	Combat System
/	Menu System

TODO: (Low ish priority; aesthetic, skin, etc.)
/	Add more FontProperties 

DONE (BUT COULD BE IMPROVED?)
/	Render Queue (this is done sloppily. See RenderQueue class for more info)
/	ECS								
/	NPCs / Interactable Objects		
/	Dialogue Box & Text Printing
	/	Text Alignment and justify
	/	Choices

WELCOME TO C#! lol
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;
using Topdown.SceneManager;

namespace Topdown {
    enum DebugState {
        GAME,
        DEBUG_MAPEDIT
    }

	/// <summary>
	/// Temporary static class to hold some global constants.
	/// <para> Will /prolly/ dele1te this when I can put this somehwere else</para>
	/// </summary>
	static class Globals {
        // RANDOM CONSTANTS, will eventually move these values
        public const int TileSize = 16;
		public const float WorldScale = 2.0f;

		public static int ScaledTileSize { get { return (int)(TileSize * WorldScale); } }

		public const int ScreenWidth = 960;
		public const int ScreenHeight = 720;
		public const int TargetFPS = 60;
	}

    static class Game {
		const bool devMode = true;

        private static void Main() {
			DebugState debugState = DebugState.GAME;

            // WINDOW INITIALIZATION
            //--------------------------------------------------
            Raylib.InitWindow(Globals.ScreenWidth, Globals.ScreenHeight, "Some RPG");
            Raylib.SetTargetFPS(Globals.TargetFPS);

            // MAP DICTIONARY INITIALIZATION
            //--------------------------------------------------
			Map.CreateMapDictionary();

            // SCENE INITIALIZATION
            //--------------------------------------------------
			OverworldScene overworldScene = new OverworldScene("Test Map 1", new Vector2(25, 15));

			SceneLoader.LoadScene(overworldScene);

            while (!Raylib.WindowShouldClose()) {
				if (devMode) {
					if (Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) {
						debugState = (DebugState)(((int)debugState + 1) % Enum.GetNames(typeof(DebugState)).Length);
						Console.WriteLine($"[DEVMODE] GAMESTATE CHANGED TO {debugState}");

						switch (debugState) {
							case DebugState.GAME:
								SceneLoader.LoadScene(overworldScene);
								break;
							case DebugState.DEBUG_MAPEDIT:
								// sceneLoader.LoadScene(mapEditorScene);
								break;
							default:
								break;
						}
					}
				}

				SceneLoader.UpdateCurrentScene();

				if (SceneLoader.QueuedScene is not null)
					SceneLoader.LoadSceneFromQueue();

            }

            Raylib.CloseWindow();
        }
	}
}