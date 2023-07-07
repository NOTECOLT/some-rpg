//------------------------------------------------------------------------------------------
/* MAPEDITOR
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    class MapEditor {
		// CONSTANTS
		//------------------------------------------------------------------------------------------
		private const float PANEL_WIDTH = Globals.SCREEN_WIDTH * 0.25f;

		// FIELDS
		//------------------------------------------------------------------------------------------
        private Tilemap _loadedTilemap = null;
		private Map _loadedMap = null;

		// PROPERTIES
		//------------------------------------------------------------------------------------------	
		public Map LoadedMap { get { return _loadedMap; } }

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

				if (_loadedMap != null)
					camera.target = new Vector2((_loadedMap.Size.X * Globals.TILE_SIZE) / 2, (_loadedMap.Size.Y * Globals.TILE_SIZE) / 2);
				else
					camera.target = new Vector2(player.Position.X + (Globals.TILE_SIZE / 2), player.Position.Y + (Globals.TILE_SIZE / 2));
				camera.offset = new Vector2(Globals.SCREEN_WIDTH * 0.375f, Globals.SCREEN_HEIGHT / 2);
				
				Raylib.BeginMode2D(camera);
					
					if (_loadedMap != null) {
						_loadedMap.RenderMap(2.0f);
						Raylib.DrawRectangleLines(0, 0, (int)_loadedMap.Size.X * Globals.TILE_SIZE, (int)_loadedMap.Size.Y * Globals.TILE_SIZE, Color.RED);
					}

				Raylib.EndMode2D();

				Raylib.DrawRectangle((int)(Globals.SCREEN_WIDTH - PANEL_WIDTH), 0, (int)PANEL_WIDTH, Globals.SCREEN_HEIGHT, Color.LIGHTGRAY);

				for (int i = 0; i < _loadedTilemap.Tiles; i++) {
					float posX = (Globals.SCREEN_WIDTH - PANEL_WIDTH) + (Globals.TILE_SIZE * i);
					float posY = Globals.TILE_SIZE * i;
					_loadedTilemap.ReturnTileSprite(i).RenderSprite(new Vector2(posX, posY), new Vector2(0, 0), 2.0f);
				}

				Raylib.DrawText($"f: {frameCounter}; fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: DEBUG MAP EDIT", 5, 40, 30, Color.BLACK);

			Raylib.EndDrawing();
        }

		public void LoadMap(Map map) {
			_loadedMap = map;
			_loadedTilemap = map.Tilemap;
		}
	}
}
