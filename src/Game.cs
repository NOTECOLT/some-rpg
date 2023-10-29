//------------------------------------------------------------------------------------------
/*
TODO: (TOP)
/	Combat System
/	ENTITY DIAGRAM

/	Debug Info Display
/	Dialogue System
/		Response Select
/		Dialogue Branches

TODO: FIX
/	FIX MAPDICTIONARY CREATION TO WORK WITH REGEX

TODO: (Other Stuff)
/	Game Loop
/		Inventory System
/		Experience System
/	Menu System

TODO: (Low ish priority; aesthetic, skin, etc.)
/	Add more FontProperties 

DONE (BUT COULD BE IMPROVED?)
/	Render Queue (this is done sloppily. See RenderQueue class for more info)
/	ECS								
/	NPCs / Interactable Objects		
/	Dialogue Box & Text Printing
	/	Text Alignment and justify
/	Flag System

WELCOME TO C#! lol
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;
using Topdown.SceneManager;

namespace Topdown {
    enum DebugState {
        Overworld,
        Battle
    }

	/// <summary>
	/// Temporary static class to hold some global constants.
	/// <para> Will /prolly/ dele1te this when I can put this somehwere else</para>
	/// </summary>
	static class Globals {
        // RANDOM CONSTANTS, will eventually move these values
		public const string VERSION = "Alpha 1.0";
		public const bool DEV_MODE = true;

        public const int TILE_SIZE = 16;
		public const float WORLD_SCALE = 2.0f;

		public static int SCALED_TILE_SIZE { get { return (int)(TILE_SIZE * WORLD_SCALE); } }

		public const int SCREEN_WIDTH = 960;
		public const int SCREEN_HEIGHT = 720;
		public const int TARGET_FPS = 60;
	}

    static class Game {

        private static void Main() {
            // WINDOW INITIALIZATION
            //--------------------------------------------------
            Raylib.InitWindow(Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT, "Some RPG");
            Raylib.SetTargetFPS(Globals.TARGET_FPS);

            // MAP DICTIONARY INITIALIZATION
            //--------------------------------------------------
			Map.CreateMapDictionary();

            // PLAYER INITIALIZATION
            //--------------------------------------------------
			PlayerData playerData = new PlayerData("savedata/testdata.xml");
			// playerData.ResetToDefault();
			// playerData.Save("savedata/testdata2.xml");
			// PlayerData newPlayerData = new PlayerData("savedata/testdata2.xml");


            // SCENE INITIALIZATION
            //--------------------------------------------------
			SceneLoader sceneLoader = new SceneLoader();

			DebugState debugState = DebugState.Overworld;
			OverworldScene overworldScene = new OverworldScene(playerData.Map, playerData.Tile);
			BattleScene battleScene = new BattleScene();

			sceneLoader.LoadScene(overworldScene, playerData);

            while (!Raylib.WindowShouldClose()) {
				if (Globals.DEV_MODE) {
					if (Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) {
						debugState = (DebugState)(((int)debugState + 1) % Enum.GetNames(typeof(DebugState)).Length);
						Console.WriteLine($"[DEVMODE] GAMESTATE CHANGED TO {debugState}");

						switch (debugState) {
							case DebugState.Overworld:
								playerData = new PlayerData("savedata/testdata.xml");
								overworldScene = new OverworldScene(playerData.Map, playerData.Tile);
                                sceneLoader.LoadScene(overworldScene, playerData);
								break;
							case DebugState.Battle:
                                sceneLoader.LoadScene(battleScene, playerData);
								break;
							default:
								break;
						}
					}
				}

                sceneLoader.UpdateCurrentScene();

				if (sceneLoader.isSceneQueued())
                    sceneLoader.LoadSceneFromQueue(playerData);

            }

            Raylib.CloseWindow();
        }
	}
}