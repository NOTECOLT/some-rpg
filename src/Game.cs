//------------------------------------------------------------------------------------------
/*
TODO: (Engine Stuff)
/	Render Queue (do i need one yet? Not really i guess; but soon i'll have to)
/	Flag System
/ 	Spritesheet animations
/	Debug Info Display
//		Render only whats on screen		<----
//	Multiple Scenes & Scene Switching 	<----
//		Instead of Multiple Scenes for the overworld, how about a map switching
//		If the game is small enough, then I can probably just load all of the tilesets
//		at the start of the game anyway....
//		OR SEAMLESS MAP LOADING?? we need both i think


TODO: (Mechanic / Game Design Stuff)
/	Game Loop
/	Combat System
/	Menu System

TODO: (Low ish priority; aesthetic, skin, etc.)
/	Add more FontProperties 

DONE (BUT COULD BE IMPROVED?)
/	ECS								<-----
/	NPCs / Interactable Objects		<-----
/	Dialogue Box & Text Printing
	/	Create TextObject UIEntity
	/	Adjust Dialogue Box Display

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
	}

    static class Game {
		const bool devMode = true;

        private static void Main() {
			DebugState debugState = DebugState.GAME;

            // WINDOW INITIALIZATION
            //--------------------------------------------------
            Raylib.InitWindow(Globals.ScreenWidth, Globals.ScreenHeight, "Some RPG");
            Raylib.SetTargetFPS(60);

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