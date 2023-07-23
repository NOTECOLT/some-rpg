//------------------------------------------------------------------------------------------
/* MAPEDITOR
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
	/// <summary>
	/// Houses all logic and code for the MapEditor in Debug Mode
	/// </summary>
    class MapEditor {
		// CONSTANTS
		//------------------------------------------------------------------------------------------
		private const float SIDEBAR_WIDTH = Globals.SCREEN_WIDTH * 0.25f;

		// FIELDS
		//------------------------------------------------------------------------------------------
        private Tilemap[] _loadedTilemaps = null;
		private Map _loadedMap = null;
		private int _selectedSprite = -1;

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
        public void MapEditorLoop(ref Camera2D camera) {
			camera.target = new Vector2(_loadedMap.Size.X * Globals.TILE_SIZE / 2, _loadedMap.Size.Y * Globals.TILE_SIZE / 2);
			camera.offset = new Vector2((Globals.SCREEN_WIDTH - SIDEBAR_WIDTH) / 2, Globals.SCREEN_HEIGHT / 2);

			// 1 - INPUT
			//--------------------------------------------------
			if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
				Vector2 mousePosition = Raylib.GetMousePosition();
				if (mousePosition.X >= Globals.SCREEN_WIDTH - SIDEBAR_WIDTH) {
					SelectTileInput(mousePosition);
				}
			}

			if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
				Vector2 mousePosition = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
				// Console.WriteLine($"({mousePosition.X}, {mousePosition.Y})");
				if (mousePosition.X > 0 && mousePosition.Y > 0 && mousePosition.X < _loadedMap.Size.X * Globals.TILE_SIZE && mousePosition.Y < _loadedMap.Size.Y * Globals.TILE_SIZE) { 
					PlaceTileInput(mousePosition);
				}
			}

			// 2 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);
				
				Raylib.BeginMode2D(camera);
					
					if (_loadedMap != null) {
						_loadedMap.RenderMap(2.0f);
						Raylib.DrawRectangleLines(0, 0, (int)_loadedMap.Size.X * Globals.TILE_SIZE, (int)_loadedMap.Size.Y * Globals.TILE_SIZE, Color.RED);
					}

				Raylib.EndMode2D();

				RenderSidebar();

				Raylib.DrawText($"fps: {Raylib.GetFPS()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: DEBUG MAP EDIT", 5, 40, 30, Color.BLACK);

			Raylib.EndDrawing();
        }

		private void RenderSidebar() {
			Raylib.DrawRectangle((int)(Globals.SCREEN_WIDTH - SIDEBAR_WIDTH), 0, (int)SIDEBAR_WIDTH, Globals.SCREEN_HEIGHT, Color.LIGHTGRAY);

			// TILEMAP TILES
			//--------------------------------------------------
			for (int i = 0; i < Tilemap.GetTotalTileArrayCapacity(_loadedTilemaps); i++) { 
				float tilePosX = (Globals.SCREEN_WIDTH - SIDEBAR_WIDTH) + (i % MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE) * Globals.TILE_SIZE);
				float tilePosY = MathF.Floor(i / MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE)) * Globals.TILE_SIZE;
				Color color = (i == _selectedSprite) ? Color.RED : Color.WHITE;
				Tilemap.ReturnTileSpriteFromArray(_loadedTilemaps, i).RenderSprite(new Vector2(tilePosX, tilePosY), new Vector2(0, 0), 2.0f, color);
			}
		}

		/// <summary>
		/// Receives input on a mouse click and interpolates it to output a selected tile
		/// <para>Onle requires reverse computation of what it took to render the tiles in the sidebar</para>
		/// </summary>
		/// <param name="mousePosition"></param>
		private void SelectTileInput(Vector2 mousePosition) {	
			float hitX = MathF.Floor(((mousePosition.X - (Globals.SCREEN_WIDTH - SIDEBAR_WIDTH)) / Globals.TILE_SIZE) % MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE));
			float hitY = MathF.Floor(mousePosition.Y / Globals.TILE_SIZE);
			//Console.WriteLine($"{hitX}, {hitY}");
			
			int i = (int)MathF.Floor(hitX + (MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE) * hitY));

			if ( i >= 0 && i < Tilemap.GetTotalTileArrayCapacity(_loadedTilemaps)) {
				//Console.WriteLine($"Tile Selected: {i}");
				_selectedSprite = i;
			} else {
				_selectedSprite = -1;
			}
		}

		private void PlaceTileInput(Vector2 mousePosition) {
			int hitX = (int)MathF.Floor(mousePosition.X / Globals.TILE_SIZE);
			int hitY = (int)MathF.Floor(mousePosition.Y / Globals.TILE_SIZE);
			// Console.WriteLine($"MousePos: ({mousePosition.X}, {mousePosition.Y}); Tile: ({hitX},{hitY})");
			LoadedMap.SetTile(hitX, hitY, 0, _selectedSprite);
		}

		public void LoadMap(Map map) {
			_loadedMap = map;
			_loadedTilemaps = map.Tilemaps;
		}
	}
}