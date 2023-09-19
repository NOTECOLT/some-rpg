//------------------------------------------------------------------------------------------
/* OVERWORLD SCENE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;
using Topdown.ECS;
using Topdown.GUI;
using Topdown.Renderer;
using Topdown.SceneManager;
using Topdown.DialogueSystem;

namespace Topdown {
	/// <summary>
	/// Scene for when the player is in the overworld.
	/// </summary>
    public class OverworldScene : IScene {
        private Player _player;
		private Vector2 _startingTile;
		// private List<Map> _loadedMaps = new List<Map>();
		private Map[] _loadedMaps = new Map[5];
		private DialogueManager _dialogueManager;
		

		/// <summary>
		/// Overworld Scene must be initialized with a map and player data
		/// </summary>
		/// <param name="player"></param>
		/// <param name="map"></param>
        public OverworldScene(string name, Vector2 startingTile) {
			if (!Map.MapDictionary.ContainsKey(name)) {
				throw new Exception($"Map {name} not found in Map Dictionary!");
			}

            RenderQueue.Camera = new Camera2D() {
				rotation = 0,
				zoom = 1
			};
			_loadedMaps[0] = new Map(name, Vector2.Zero);
			// _loadedMaps.Add();
			_startingTile = startingTile;
        }

		// SCENE FUNCTIONS
        //------------------------------------------------------------------------------------------
        public void Load() {
			// 1 - SYSTEM LOADING
			//--------------------------------------------------
			// most DialogueManager functions uses static objects, but an object is needed for the UI elements
			_dialogueManager = new DialogueManager();

			// 2 - MAP LOADING
			//--------------------------------------------------
			LoadMap(_loadedMaps[0], true);

            // 3 - PLAYER LOADING
            //--------------------------------------------------
			_player = new Player(_startingTile);			
		}

        public void Update() {
            TileTransform playerT = _player.GetComponent<TileTransform>();

            // 1 - INPUT
            //--------------------------------------------------

			// Player Movement
			_player.OnKeyInput();

			// Player Interaction
			if (Raylib.IsKeyReleased(KeyboardKey.KEY_SPACE)) {
				Vector2 target = playerT.Tile;
				switch (playerT.Facing) {
					case Direction.North:
						target -= Vector2.UnitY;
						break;
					case Direction.South:
						target += Vector2.UnitY;
						break;
					case Direction.West:
						target -= Vector2.UnitX;
						break;
					case Direction.East:
						target += Vector2.UnitX;
						break;
					default:
						break;
				}

				if (DialogueManager.DialogueActive) {
					DialogueManager.NextMessage();
				} else {
					if (GetEntityAtTile(target) is not null && GetEntityAtTile(target) is IInteractable) {
						IInteractable i = GetEntityAtTile(target) as IInteractable;
						i.OnInteract();
					}	
				}
			}

			// Debug Keys
			if (Globals.DevMode && Raylib.IsKeyDown(KeyboardKey.KEY_F3)) {
				if (Raylib.IsKeyReleased(KeyboardKey.KEY_S)) {
					SavePlayerData();
				}
			}
		
			// 2 - PHYSICS
			//--------------------------------------------------

			if (playerT.TargetTile != playerT.Tile) {
				// Map Reloading
				// TODO: Improve this? not really the best way to handle it atm
				if (_loadedMaps.Length > 0) {
					Vector2 tile = playerT.TargetTile - _loadedMaps[0].Origin / Globals.ScaledTileSize;

					if (tile.Y < 0 && _loadedMaps[0].HasMapConnection(Direction.North))
						LoadMap(_loadedMaps[1]);
					else if (tile.X >= _loadedMaps[0].LoadedMap.Width && _loadedMaps[0].HasMapConnection(Direction.East))
						LoadMap(_loadedMaps[2]);
					else if (tile.Y >= _loadedMaps[0].LoadedMap.Height && _loadedMaps[0].HasMapConnection(Direction.South))
						LoadMap(_loadedMaps[3]);
					else if (tile.X < 0 && _loadedMaps[0].HasMapConnection(Direction.West))
						LoadMap(_loadedMaps[4]);
				}

				// Tile Collision
				if (!_loadedMaps[0].IsTileWalkable(playerT.TargetTile)) {
					// Note: When colliding with a tile on the border of a map, the current map loaded changes (even when it shouldn't in theory)
					//		Atm, this isn't causing any problems, but it may in the future
					playerT.TargetTile = playerT.Tile;

				// Tile Events
				} else {
					(String map, Vector2 tile)? warpTuple = _loadedMaps[0].IsTileWarpable(playerT.TargetTile);

					if (warpTuple is not null) {
						SceneLoader.QueueScene(new OverworldScene(warpTuple.Value.map, warpTuple.Value.tile));
					}	
				}	
			}

			TileTransformSystem.Update();

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.BLACK);
            	RenderQueue.Camera.target = new Vector2(playerT.Position.X + (Globals.ScaledTileSize), playerT.Position.Y + (Globals.ScaledTileSize));
				RenderQueue.Camera.offset = new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2);
				Raylib.BeginMode2D(RenderQueue.Camera);
				RenderQueue.RenderAllLayers(_loadedMaps.ToList<Map>(), EntityRenderSystem.Components);
				Raylib.EndMode2D();

				// UI
				//--------------------------------------------------

				Raylib.DrawText($"fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.RAYWHITE);
				Raylib.DrawText($"Mode: OVERWORLD", 5, 40, 30, Color.RAYWHITE);
				playerT.DrawEntityDebugText(new Vector2(5, 75));

				UIEntitySystem.RenderAll();
				_dialogueManager.Render();

			Raylib.EndDrawing();
        }

        public void Unload() {
			// Unloading Component Systems once the scene ends because the list is static
			EntityRenderSystem.Unload();
			TileTransformSystem.Unload();

			UIEntitySystem.Unload();

			Map.UnloadAllTextures();
        }

		// FUNCTIONS
        //------------------------------------------------------------------------------------------
		
		/// <summary>
		/// Loads a Map onto the _loadedMaps list, also loads the map's adjacent maps
		/// </summary>
		/// <param name="map"></param>
		/// <param name="firstMap">True if this is the first map to be loaded onto the scene</param>
		private void LoadMap(Map map, bool firstMap = false) {
			String oldMap = _loadedMaps[0].Name;
			
			if (_loadedMaps[0].Name != map.Name) {
				_loadedMaps[0] = map;
			}

			// LOAD ADJACENT MAPS
			//--------------------------------------------------
			// KEY
			// [0] - main map
			// [1] - north map
			// [2] - east map
			// [3] - west map
			// [4] - south map
			if (_loadedMaps[0].HasMapConnection(Direction.North)) {
				Map northMap = new Map(_loadedMaps[0].AdjacentMaps[Direction.North], Vector2.Zero);
				Vector2 newOrigin = _loadedMaps[0].Origin - new Vector2(0, northMap.LoadedMap.Height) * Globals.ScaledTileSize;
				northMap.Origin = newOrigin;
				_loadedMaps[1] = northMap;
			}

			if (_loadedMaps[0].HasMapConnection(Direction.East)) {
				Map eastMap = new Map(_loadedMaps[0].AdjacentMaps[Direction.East], Vector2.Zero);
				Vector2 newOrigin = _loadedMaps[0].Origin + new Vector2(_loadedMaps[0].LoadedMap.Width, 0) * Globals.ScaledTileSize;
				eastMap.Origin = newOrigin;
				_loadedMaps[2] = eastMap;
			}

			if (_loadedMaps[0].HasMapConnection(Direction.South)) {
				Map southMap = new Map(_loadedMaps[0].AdjacentMaps[Direction.South], Vector2.Zero);
				Vector2 newOrigin = _loadedMaps[0].Origin + new Vector2(0, _loadedMaps[0].LoadedMap.Height) * Globals.ScaledTileSize;
				southMap.Origin = newOrigin;
				_loadedMaps[3] = southMap;
			}

			if (_loadedMaps[0].HasMapConnection(Direction.West)) {
				Map westMap = new Map(_loadedMaps[0].AdjacentMaps[Direction.South], Vector2.Zero);
				Vector2 newOrigin = _loadedMaps[0].Origin - new Vector2(westMap.LoadedMap.Width, 0) * Globals.ScaledTileSize;
				westMap.Origin = newOrigin;
				_loadedMaps[4] = westMap;
			}

			for (int i = TileTransformSystem.Components.Count - 1; i >= 0; i--) {
				if (TileTransformSystem.Components[i].entity is Player) continue;

				bool shouldBeLoaded = false;
				foreach (Map m in _loadedMaps) {
					if (m is null) continue;
					if (TileTransformSystem.Components[i].entity.Map == m.Name) shouldBeLoaded = true; 
				}
				
				if (!shouldBeLoaded) TileTransformSystem.Components[i].entity.Destroy();
			}

			// LOAD ALL TEXTURES & ENTITIES
			//--------------------------------------------------
			foreach (Map m in _loadedMaps) {
				if (m is null) continue;
				m.LoadTextures();

				// Idea is that, there should only be 4 maps loaded at a time
				//		if you are moving from one map to the other, then the only entities you should
				//		be retaining are the ones from the old map and the new map
				if ((m.Name == oldMap || m.Name == map.Name )&& !firstMap) continue;
				m.LoadObjectsAsEntities();
			}

			Console.WriteLine($"[MAPLOADER] Current Loaded Map: {_loadedMaps[0].Name}");
			Console.WriteLine($"[MAPLOADER] Number of Entities: {TileTransformSystem.Components.Count}");
		}

		/// <summary>
		/// Checks if a map is loaded in the map list. Uses filepath for equality check
		/// </summary>
		/// <param name="map"></param>
		/// <returns></returns>
		private bool IsMapLoaded(Map map) {
			foreach (Map m in _loadedMaps) {
				if (m.Name == map.Name) return true;
			}

			return false;
		}
		
		private static Entity GetEntityAtTile(Vector2 tile) {
			if (TileTransformSystem.Components.Count == 0) return null;	
			TileTransform transform = TileTransformSystem.Components.FirstOrDefault(c => c.Tile == tile, null) ?? null;
			
			if (transform is null) return null;
			else return transform.entity;
		}
    
		private void SavePlayerData() {
			Game.PlayerSaveData.Map = _loadedMaps[0].Name;
			Game.PlayerSaveData.Tile = _player.GetComponent<TileTransform>().Tile;
			Game.PlayerSaveData.Save(Game.PlayerSaveData.FilePath);
		}
	
	}
}