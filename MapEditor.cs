//------------------------------------------------------------------------------------------
/* MAPEDITOR
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    class MapEditor {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private Tilemap loadedTilemap = null;
		private Map loadedMap = null;

		// PROPERTIES
		//------------------------------------------------------------------------------------------	
		public Map LoadedMap { get { return loadedMap; } }

		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Main Map Editor Loop
		/// </summary>
		/// <param name="player"></param>
		/// <param name="camera"></param>
		/// <param name="loadedMap"></param>
		/// <param name="frameCounter"></param>
        public void MapEditorLoop(Entity player, Camera2D camera, int frameCounter) {
			// 1 - INPUT
			//--------------------------------------------------
			
			// 2 - PHYSICS
			//--------------------------------------------------

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);

				if (loadedMap != null)
					camera.target = new Vector2((loadedMap.Size.X * Globals.TILE_SIZE) / 2, (loadedMap.Size.Y * Globals.TILE_SIZE) / 2);
				else
					camera.target = new Vector2(player.Position.X + (Globals.TILE_SIZE / 2), player.Position.Y + (Globals.TILE_SIZE / 2));
				camera.offset = new Vector2(Globals.SCREEN_WIDTH / 2, Globals.SCREEN_HEIGHT / 2);
				Raylib.BeginMode2D(camera);
					
					if (loadedMap != null) {
						loadedMap.RenderMap();
						Raylib.DrawRectangleLines(0, 0, (int)loadedMap.Size.X * Globals.TILE_SIZE, (int)loadedMap.Size.Y * Globals.TILE_SIZE, Color.RED);
					}

				Raylib.EndMode2D();

				Raylib.DrawText($"f: {frameCounter}; fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: DEBUG MAP EDIT", 5, 40, 30, Color.BLACK);

			Raylib.EndDrawing();
        }

		public void LoadMap(Map map) {
			loadedMap = map;
			loadedTilemap = map.Tilemap;
		}
	}
}