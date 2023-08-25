//------------------------------------------------------------------------------------------
/* MAPEDITOR [DEPRECATED]
*/
//------------------------------------------------------------------------------------------
// using System.Numerics;
// using Raylib_cs;
// using Topdown.GUI;
// using Topdown.Engine;

// namespace Topdown {
// 	/// <summary>
// 	/// Houses all logic and code for the MapEditor in Debug Mode
// 	/// </summary>
//     class MapEditorScene : IScene {
// 		// CONSTANTS
// 		//------------------------------------------------------------------------------------------
// 		private const float SIDEBAR_WIDTH = Globals.SCREEN_WIDTH * 0.25f;

// 		// FIELDS
// 		//------------------------------------------------------------------------------------------
//         private Tilemap[] _loadedTilemaps = null;
// 		private Map _loadedMap = null;
// 		private int _selectedSprite = -1;
// 		private Camera2D _camera;

// 		// PROPERTIES
// 		//------------------------------------------------------------------------------------------	
// 		public Map LoadedMap { get { return _loadedMap; } }


// 		// UI ELEMENTS
//         //------------------------------------------------------------------------------------------
// 		private Button ui_saveMapBtn;
// 		private Button ui_loadMapBtn;
// 		private TextField ui_loadMapField;
// 		private Button ui_newMapBtn;

// 		public MapEditorScene() {
// 			_camera = new Camera2D() {
// 				rotation = 0,
// 				zoom = 1
// 			};
// 		}

//         // SCENE FUNCTIONS
//         //------------------------------------------------------------------------------------------
//         public void Load() {
// 			ui_saveMapBtn = new Button(new Vector2(5, 35), new Vector2(150, 40), "Save Map", new FontProperties(30, Color.BLACK), Color.LIGHTGRAY);
// 			ui_loadMapBtn = new Button(new Vector2(160, 35), new Vector2(150, 40), "Load Map", new FontProperties(30, Color.BLACK), Color.LIGHTGRAY);
// 			ui_loadMapField = new TextField(new Vector2(5, 80), new Vector2(300, 40), 200, new FontProperties(30, Color.BLACK), Color.LIGHTGRAY);
// 			ui_newMapBtn = new Button(new Vector2(215, 35), new Vector2(150, 40), "New Map", new FontProperties(30, Color.BLACK), Color.LIGHTGRAY);
// 		}

//         public void Update() {
// 			_camera.target = new Vector2(_loadedMap.Size.X * Globals.TILE_SIZE / 2, _loadedMap.Size.Y * Globals.TILE_SIZE / 2);
// 			_camera.offset = new Vector2((Globals.SCREEN_WIDTH - SIDEBAR_WIDTH) / 2, Globals.SCREEN_HEIGHT / 2);

// 			// 1 - INPUT
// 			//--------------------------------------------------
// 			if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
// 				Vector2 mousePosition = Raylib.GetMousePosition();
// 				if (mousePosition.X >= Globals.SCREEN_WIDTH - SIDEBAR_WIDTH) {
// 					SelectTileInput(mousePosition);
// 				}
// 			}

// 			if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
// 				Vector2 mousePosition = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), _camera);
// 				if (mousePosition.X > 0 && mousePosition.Y > 0 && mousePosition.X < _loadedMap.Size.X * Globals.TILE_SIZE && mousePosition.Y < _loadedMap.Size.Y * Globals.TILE_SIZE) { 
// 					PlaceTileInput(mousePosition);
// 				}
// 			}

// 			// 2 - UI
// 			//--------------------------------------------------			

// 			if (ui_saveMapBtn.IsClicked(Raylib.GetMousePosition())) {
// 				Map.SaveMap(_loadedMap, "resources/maps/testmap.json");
// 			}

// 			if (ui_loadMapBtn.IsClicked(Raylib.GetMousePosition())) {
// 				ui_loadMapField.Enabled = true;
// 				ui_loadMapField.IsAcceptingText = true;
// 			}

// 			if (ui_loadMapField.Enabled) {
// 				if (Raylib.IsKeyReleased(KeyboardKey.KEY_ESCAPE)) {
// 					ui_loadMapField.Enabled = false;
// 					ui_loadMapField.IsAcceptingText = false;
// 					ui_loadMapField.ResetTextField();
// 				}

// 				if (Raylib.IsKeyReleased(KeyboardKey.KEY_ENTER)) {
// 					string filepath = ui_loadMapField.Text;
// 					Map map = Map.LoadMapFromPath(filepath);
// 					LoadMap(map);

// 					ui_loadMapField.Enabled = false;
// 					ui_loadMapField.IsAcceptingText = false;
// 					ui_loadMapField.ResetTextField();
// 				}
// 			}
			
// 			ui_loadMapField.UpdateTextField();

// 			// 3 - RENDERING
// 			//--------------------------------------------------

// 			Raylib.BeginDrawing();
// 				Raylib.ClearBackground(Color.RAYWHITE);
				
// 				Raylib.BeginMode2D(_camera);
					
// 					if (_loadedMap != null) {
// 						_loadedMap.RenderMap(2.0f);
// 						Raylib.DrawRectangleLines(0, 0, (int)_loadedMap.Size.X * Globals.TILE_SIZE, (int)_loadedMap.Size.Y * Globals.TILE_SIZE, Color.RED);
// 					}

// 				Raylib.EndMode2D();

// 				RenderSidebar();
// 				ui_saveMapBtn.Render();
// 				ui_loadMapBtn.Render();
// 				ui_loadMapField.Render();

// 				Raylib.DrawText($"fps: {Raylib.GetFPS()}; Mode: DEBUG MAP EDIT", 5, 5, 24, Color.BLACK);

// 			Raylib.EndDrawing();
//         }

//         public void Unload() {
//         }
//         // FUNCTIONS
//         //------------------------------------------------------------------------------------------
//         private void RenderSidebar() {
// 			Raylib.DrawRectangle((int)(Globals.SCREEN_WIDTH - SIDEBAR_WIDTH), 0, (int)SIDEBAR_WIDTH, Globals.SCREEN_HEIGHT, Color.LIGHTGRAY);

// 			// TILEMAP TILES
// 			//--------------------------------------------------
// 			for (int i = 0; i < Tilemap.GetTotalTileArrayCapacity(_loadedTilemaps); i++) { 
// 				float tilePosX = (Globals.SCREEN_WIDTH - SIDEBAR_WIDTH) + (i % MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE) * Globals.TILE_SIZE);
// 				float tilePosY = MathF.Floor(i / MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE)) * Globals.TILE_SIZE;
// 				Color color = (i == _selectedSprite) ? Color.RED : Color.WHITE;
// 				Tilemap.ReturnTileSpriteFromArray(_loadedTilemaps, i).RenderSprite(new Vector2(tilePosX, tilePosY), new Vector2(0, 0), 2.0f, color);
// 			}
// 		}

// 		/// <summary>
// 		/// Receives input on a mouse click and interpolates it to output a selected tile
// 		/// <para>Onle requires reverse computation of what it took to render the tiles in the sidebar</para>
// 		/// </summary>
// 		/// <param name="mousePosition"></param>
// 		private void SelectTileInput(Vector2 mousePosition) {	
// 			float hitX = MathF.Floor(((mousePosition.X - (Globals.SCREEN_WIDTH - SIDEBAR_WIDTH)) / Globals.TILE_SIZE) % MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE));
// 			float hitY = MathF.Floor(mousePosition.Y / Globals.TILE_SIZE);
			
// 			int i = (int)MathF.Floor(hitX + (MathF.Floor(SIDEBAR_WIDTH / Globals.TILE_SIZE) * hitY));

// 			if ( i >= 0 && i < Tilemap.GetTotalTileArrayCapacity(_loadedTilemaps)) {
// 				//Console.WriteLine($"Tile Selected: {i}");
// 				_selectedSprite = i;
// 			} else {
// 				_selectedSprite = -1;
// 			}
// 		}

// 		private void PlaceTileInput(Vector2 mousePosition) {
// 			int hitX = (int)MathF.Floor(mousePosition.X / Globals.TILE_SIZE);
// 			int hitY = (int)MathF.Floor(mousePosition.Y / Globals.TILE_SIZE);
// 			// Console.WriteLine($"MousePos: ({mousePosition.X}, {mousePosition.Y}); Tile: ({hitX},{hitY})");
// 			LoadedMap.SetTile(hitX, hitY, 0, _selectedSprite);
// 		}

// 		public void LoadMap(Map map) {
// 			if (map == null) return;
// 			_loadedMap = map;
// 			_loadedTilemaps = map.Tilemaps;
// 		}
// 	}
// }