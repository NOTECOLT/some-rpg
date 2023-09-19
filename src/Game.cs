﻿//------------------------------------------------------------------------------------------
/*
TODO: (Engine Stuff)
/	Debug Info Display
/	Dialogue System
/		Response Select
/		Dialogue Branches

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
/	Flag System

WELCOME TO C#! lol
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;
using Topdown.SceneManager;

namespace Topdown {
    enum DebugState {
        GAME
        // DEBUG_MAPEDIT
    }

	/// <summary>
	/// Temporary static class to hold some global constants.
	/// <para> Will /prolly/ dele1te this when I can put this somehwere else</para>
	/// </summary>
	static class Globals {
        // RANDOM CONSTANTS, will eventually move these values
		public const string Version = "Alpha 1.0";
		public const bool DevMode = true;

        public const int TileSize = 16;
		public const float WorldScale = 2.0f;

		public static int ScaledTileSize { get { return (int)(TileSize * WorldScale); } }

		public const int ScreenWidth = 960;
		public const int ScreenHeight = 720;
		public const int TargetFPS = 60;
	}

    static class Game {
		public static PlayerData PlayerSaveData;

        private static void Main() {
			// DebugState debugState = DebugState.GAME;

            // WINDOW INITIALIZATION
            //--------------------------------------------------
            Raylib.InitWindow(Globals.ScreenWidth, Globals.ScreenHeight, "Some RPG");
            Raylib.SetTargetFPS(Globals.TargetFPS);

            // MAP DICTIONARY INITIALIZATION
            //--------------------------------------------------
			Map.CreateMapDictionary();

            // PLAYER INITIALIZATION
            //--------------------------------------------------
			PlayerSaveData = new PlayerData("savedata/testdata.xml");
			// playerData.ResetToDefault();
			// playerData.Save("savedata/testdata2.xml");
			// PlayerData newPlayerData = new PlayerData("savedata/testdata2.xml");


            // SCENE INITIALIZATION
            //--------------------------------------------------
			OverworldScene overworldScene = new OverworldScene(PlayerSaveData.Map, PlayerSaveData.Tile);

			SceneLoader.LoadScene(overworldScene);

            while (!Raylib.WindowShouldClose()) {
				// if (Globals.DevMode) {
				// 	if (Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) {
				// 		debugState = (DebugState)(((int)debugState + 1) % Enum.GetNames(typeof(DebugState)).Length);
				// 		Console.WriteLine($"[DEVMODE] GAMESTATE CHANGED TO {debugState}");

				// 		switch (debugState) {
				// 			case DebugState.GAME:
				// 				SceneLoader.LoadScene(overworldScene);
				// 				break;
				// 			case DebugState.DEBUG_MAPEDIT:
				// 				break;
				// 			default:
				// 				break;
				// 		}
				// 	}
				// }

				SceneLoader.UpdateCurrentScene();

				if (SceneLoader.QueuedScene is not null)
					SceneLoader.LoadSceneFromQueue();

            }

            Raylib.CloseWindow();
        }
	}
}