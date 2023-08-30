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
		// private List<Entity> _entityList;	// Contains a list of entities excluding the player
        private Camera2D _camera;
        private Map _map;
		private DialogueManager _dialogueManager;
		

		/// <summary>
		/// Overworld Scene must be initialized with a map and player data
		/// </summary>
		/// <param name="player"></param>
		/// <param name="map"></param>
        public OverworldScene(string mapPath) {
            _camera = new Camera2D() {
				rotation = 0,
				zoom = 1
			};
			_map = new Map(mapPath);
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
			_map.LoadTextures();
			_map.LoadObjectsAsEntities();

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
					playerT.ChangeDirection(Direction.Right);
					playerT.TargetTP = new Vector2(playerT.TilePos.X + 1, playerT.TilePos.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) {
					playerT.ChangeDirection(Direction.Left);
					playerT.TargetTP = new Vector2(playerT.TilePos.X - 1, playerT.TilePos.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) {
					playerT.ChangeDirection(Direction.Down);
					playerT.TargetTP = new Vector2(playerT.TilePos.X, playerT.TilePos.Y + 1);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) {
					playerT.ChangeDirection(Direction.Up);
					playerT.TargetTP = new Vector2(playerT.TilePos.X, playerT.TilePos.Y - 1);
				} else {
					playerT.TargetTP = new Vector2(playerT.TilePos.X, playerT.TilePos.Y);
				}

				// Collision, movement cancellation
				if (playerT.TargetTP != playerT.TilePos) {
					if (_map.IsMapCollision(playerT.TargetTP) || GetEntityListAtTilePos(playerT.TargetTP).Count() > 0)
						playerT.TargetTP = playerT.TilePos;
				}
					
			}

			// Player Interaction
			if (Raylib.IsKeyReleased(KeyboardKey.KEY_SPACE)) {
				Vector2 target = playerT.TilePos;
				switch (playerT.Facing) {
					case Direction.Up:
						target -= Vector2.UnitY;
						break;
					case Direction.Down:
						target += Vector2.UnitY;
						break;
					case Direction.Left:
						target -= Vector2.UnitX;
						break;
					case Direction.Right:
						target += Vector2.UnitX;
						break;
					default:
						break;
				}

				if (DialogueManager.DialogueActive) {
					DialogueManager.NextMessage();
				} else {
					if (GetEntityAtTilePos(target) is not null && GetEntityAtTilePos(target) is IInteractable) {
						IInteractable i = GetEntityAtTilePos(target) as IInteractable;
						i.OnInteract();
					}	
				}
			}
		
			// 2 - PHYSICS
			//--------------------------------------------------

			ETransformSystem.Update();

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);
				_camera.target = new Vector2(playerT.Position.X + (Globals.TILE_SIZE / 2), playerT.Position.Y + (Globals.TILE_SIZE / 2));
				_camera.offset = new Vector2(Globals.SCREEN_WIDTH / 2, Globals.SCREEN_HEIGHT / 2);
				Raylib.BeginMode2D(_camera);

					if (_map != null)
						_map.RenderMap(Globals.WORLD_SCALE);

					ESpriteSystem.Update();

				Raylib.EndMode2D();

				// UI
				//--------------------------------------------------

				Raylib.DrawText($"fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: OVERWORLD", 5, 40, 30, Color.BLACK);
				// playerT.DrawEntityDebugText(Globals.TILE_SIZE, new Vector2(5, 75));

				UIEntitySystem.RenderAll();
				_dialogueManager.Render();

			Raylib.EndDrawing();
        }

        public void Unload() {
			// Unloading Component Systems once the scene ends because the list is static
			ESpriteSystem.Unload();
			ETransformSystem.Unload();

			UIEntitySystem.Unload();
        }

		// FUNCTIONS
        //------------------------------------------------------------------------------------------
		public Entity GetEntityAtTilePos(Vector2 tilePos) {
			if (ETransformSystem.Components.Count() == 0) return null;	
			ETransform transform = ETransformSystem.Components.FirstOrDefault(c => c.TilePos == tilePos, null) ?? null;
			
			if (transform is null) return null;
			else return transform.entity;
			// return _entityList.FirstOrDefault(
		}

		public List<Entity> GetEntityListAtTilePos(Vector2 tilePos) {
			if (ETransformSystem.Components.Count() == 0) return null;	
			List<ETransform> transforms = ETransformSystem.Components.Where(c => c.TilePos == tilePos).ToList();
			List<Entity> entityList = new List<Entity>();
			foreach (ETransform t in transforms) {
				entityList.Add(t.entity);
			}

			return entityList;
			// return _entityList.Where(e => e.GetComponent<ETransform>().TilePos == tilePos);
		}
    }
}