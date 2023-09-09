//------------------------------------------------------------------------------------------
/* OVERWORLD SCENE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
using Topdown.ECS;
using Topdown.GUI;
using Topdown.Renderer;
using Topdown.SceneManager;

namespace Topdown {
	/// <summary>
	/// Scene for when the player is in the overworld.
	/// </summary>
    public class OverworldScene : IScene {
        private Player _player;
		private Vector2 _startingTile;
		private List<Map> _loadedMaps = new List<Map>();
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
			_loadedMaps.Add(new Map(name, Vector2.Zero));
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
			LoadMap(_loadedMaps[0]);

            // 3 - PLAYER LOADING
            //--------------------------------------------------
			_player = new Player(_startingTile);			
		}

        public void Update() {
            // 1 - INPUT
            //--------------------------------------------------
            ETransform playerT = _player.GetComponent<ETransform>();

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
		
			// 2 - PHYSICS
			//--------------------------------------------------

			if (playerT.TargetTile != playerT.Tile) {
				// Map Reloading
				// TODO: Improve this? not really the best way to handle it atm
				if (_loadedMaps.Count > 0) {
					Vector2 tile = playerT.TargetTile - _loadedMaps[0].Origin / Globals.ScaledTileSize;

					if (tile.X < 0 && _loadedMaps[0].HasMapConnection(Direction.West))
						LoadMap(_loadedMaps[0].AdjacentMaps[Direction.West]);
					else if (tile.Y < 0 && _loadedMaps[0].HasMapConnection(Direction.North))
						LoadMap(_loadedMaps[0].AdjacentMaps[Direction.North]);
					else if (tile.X >= _loadedMaps[0].LoadedMap.Width && _loadedMaps[0].HasMapConnection(Direction.East))
						LoadMap(_loadedMaps[0].AdjacentMaps[Direction.East]);
					else if (tile.Y >= _loadedMaps[0].LoadedMap.Height && _loadedMaps[0].HasMapConnection(Direction.South))
						LoadMap(_loadedMaps[0].AdjacentMaps[Direction.South]);
				}

				// Tile Collision
				if (!_loadedMaps[0].IsTileWalkable(playerT.TargetTile)) {
					// Console.WriteLine("YEAH");
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

			ETransformSystem.Update();

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.BLACK);
            	RenderQueue.Camera.target = new Vector2(playerT.Position.X + (Globals.ScaledTileSize), playerT.Position.Y + (Globals.ScaledTileSize));
				RenderQueue.Camera.offset = new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2);
				Raylib.BeginMode2D(RenderQueue.Camera);
					RenderQueue.RenderAllLayers(_loadedMaps, ESpriteSystem.Components);
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
			ESpriteSystem.Unload();
			ETransformSystem.Unload();

			UIEntitySystem.Unload();

			Map.UnloadAllTextures();
        }

		// FUNCTIONS
        //------------------------------------------------------------------------------------------

		/// <summary>
		/// Loads a Map onto the _loadedMaps list, also loads the map's adjacent maps
		/// </summary>
		/// <param name="map"></param>
		private void LoadMap(Map map) {
			if (_loadedMaps[0].Name != map.Name) {
				foreach (Map m in _loadedMaps) {
					if (m.Name == map.Name) {
						_loadedMaps.Remove(m);
						break;
					}
				}
				_loadedMaps.Insert(0, map);
			}

			_loadedMaps[0].LoadTextures();
			_loadedMaps[0].LoadObjectsAsEntities();

			_loadedMaps[0].LoadAdjacentMaps();
			foreach (KeyValuePair<Direction, Map> entry in _loadedMaps[0].AdjacentMaps) {
				if (IsMapLoaded(entry.Value)) continue;
				entry.Value.LoadTextures();
				entry.Value.LoadObjectsAsEntities();

				_loadedMaps.Add(entry.Value);
			}

			Console.WriteLine($"[MAPLOADER] Main Map: {_loadedMaps[0].Name}");
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
			if (ETransformSystem.Components.Count == 0) return null;	
			ETransform transform = ETransformSystem.Components.FirstOrDefault(c => c.Tile == tile, null) ?? null;
			
			if (transform is null) return null;
			else return transform.entity;
		}
    }
}