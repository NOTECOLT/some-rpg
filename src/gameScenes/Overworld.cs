//------------------------------------------------------------------------------------------
/* OVERWORLD SCENE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
using Topdown.ECS;
using Topdown.GUI;
using Topdown.Scene;

namespace Topdown {
	/// <summary>
	/// Scene for when the player is in the overworld.
	/// </summary>
    public class OverworldScene : IScene {
        private Player _player;
        private Camera2D _camera;
		private List<Map> _loadedMaps = new List<Map>();
		private DialogueManager _dialogueManager;
		

		/// <summary>
		/// Overworld Scene must be initialized with a map and player data
		/// </summary>
		/// <param name="player"></param>
		/// <param name="map"></param>
        public OverworldScene(string name) {
            _camera = new Camera2D() {
				rotation = 0,
				zoom = 1
			};
			_loadedMaps.Add(new Map(name, Vector2.Zero));
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
			_player = new Player(new Vector2(8, 14));			
		}

        public void Update() {
            // 1 - INPUT
            //--------------------------------------------------
            ETransform playerT = _player.GetComponent<ETransform>();
			if (!playerT.IsMoving) {
				playerT.IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

				if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) {
					playerT.ChangeDirection(Direction.East);
					playerT.TargetTile = new Vector2(playerT.Tile.X + 1, playerT.Tile.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) {
					playerT.ChangeDirection(Direction.West);
					playerT.TargetTile = new Vector2(playerT.Tile.X - 1, playerT.Tile.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) {
					playerT.ChangeDirection(Direction.South);
					playerT.TargetTile = new Vector2(playerT.Tile.X, playerT.Tile.Y + 1);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) {
					playerT.ChangeDirection(Direction.North);
					playerT.TargetTile = new Vector2(playerT.Tile.X, playerT.Tile.Y - 1);
				} else {
					playerT.TargetTile = new Vector2(playerT.Tile.X, playerT.Tile.Y);
				}	
			}

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

			// Tile Events
			if (playerT.TargetTile != playerT.Tile) {
				// Map Reloading
				// TODO: Improve this? not really the best way to handle it atm
				if (_loadedMaps.Count > 0) {
					Vector2 tile = playerT.TargetTile - _loadedMaps[0].Origin / Globals.TILE_SIZE;

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
				if (_loadedMaps[0].IsMapCollision(playerT.TargetTile, Globals.TILE_SIZE)) {
					// Note: When colliding with a tile on the border of a map, the current map loaded changes (even when it shouldn't in theory)
					//		Atm, this isn't causing any problems, but it may in the future
					playerT.TargetTile = playerT.Tile;
				}

				// Entity Collision
				if (GetEntityListAtTile(playerT.TargetTile).Count > 0) {
					playerT.TargetTile = playerT.Tile;
				}
					
			}

			ETransformSystem.Update();

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);
				_camera.target = new Vector2(playerT.Position.X + (Globals.TILE_SIZE / 2), playerT.Position.Y + (Globals.TILE_SIZE / 2));
				_camera.offset = new Vector2(Globals.SCREEN_WIDTH / 2, Globals.SCREEN_HEIGHT / 2);
				Raylib.BeginMode2D(_camera);

					if (_loadedMaps.Count > 0) {
						foreach (Map m in _loadedMaps) {
							m.RenderMap(_camera, Globals.WORLD_SCALE, Globals.TILE_SIZE, Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT);
						}
					}
						

					ESpriteSystem.Update();

				Raylib.EndMode2D();

				// UI
				//--------------------------------------------------

				Raylib.DrawText($"fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: OVERWORLD", 5, 40, 30, Color.BLACK);
				playerT.DrawEntityDebugText(Globals.TILE_SIZE, new Vector2(5, 75));

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
			_loadedMaps[0].LoadObjectsAsEntities(Globals.TILE_SIZE);

			_loadedMaps[0].LoadAdjacentMaps(Globals.TILE_SIZE);
			foreach (KeyValuePair<Direction, Map> entry in _loadedMaps[0].AdjacentMaps) {
				if (IsMapLoaded(entry.Value)) continue;
				// TODO: FIX THE MAP CHECKING FUCNTION
				entry.Value.LoadTextures();
				entry.Value.LoadObjectsAsEntities(Globals.TILE_SIZE);

				_loadedMaps.Add(entry.Value);
			}
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

		private static List<Entity> GetEntityListAtTile(Vector2 tile) {
			if (ETransformSystem.Components.Count == 0) return null;	
			List<ETransform> transforms = ETransformSystem.Components.Where(c => c.Tile == tile).ToList();
			List<Entity> entityList = new List<Entity>();
			foreach (ETransform t in transforms) {
				entityList.Add(t.entity);
			}

			return entityList;
		}
		
    }
}