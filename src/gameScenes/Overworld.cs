//------------------------------------------------------------------------------------------
/* OVERWORLD SCENE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;
using Topdown.ECS;
using Topdown.Scene;

namespace Topdown {
	/// <summary>
	/// Scene for when the player is in the overworld.
	/// </summary>
    public class OverworldScene : IScene {
        private Player _player;
		private List<Entity> _entityList;	// Contains a list of entities excluding the player
        private Camera2D _camera;
        private Map _map;
		

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
			// 1- MAP LOADING
			//--------------------------------------------------
			_map.LoadTextures();
			_entityList = _map.LoadObjectsAsEntities();

            // 2- PLAYER LOADING
            //--------------------------------------------------
            ECS.ETransform pTransform = new ECS.ETransform(new Vector2(0, 0), Globals.PLAYER_WALKSPEED, Globals.PLAYER_RUNSPEED, Globals.TILE_SIZE);
            ESprite pSprite = new ESprite("resources/sprites/characters/player.png", 0);
			_player = new Player(pTransform, pSprite);
        }

        public void Update() {
            // 1 - INPUT
            //--------------------------------------------------
            ETransform playerT = _player.GetComponent<ECS.ETransform>();
			if (!playerT.IsMoving) {
				playerT.IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

				if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) {
					playerT.TargetTP = new Vector2(playerT.TilePos.X + 1, playerT.TilePos.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) {
					playerT.TargetTP = new Vector2(playerT.TilePos.X - 1, playerT.TilePos.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) {
					playerT.TargetTP = new Vector2(playerT.TilePos.X, playerT.TilePos.Y + 1);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) {
					playerT.TargetTP = new Vector2(playerT.TilePos.X, playerT.TilePos.Y - 1);
				} else {
					playerT.TargetTP = new Vector2(playerT.TilePos.X, playerT.TilePos.Y);
				}

				// Collision, movement cancellation
				if (playerT.TargetTP != playerT.TilePos) {
					if (_map.IsMapCollision(playerT.TargetTP) || _entityList.Where(e => e.GetComponent<ETransform>().TilePos == playerT.TargetTP).Count() > 0)
						playerT.TargetTP = playerT.TilePos;
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
				playerT.DrawEntityDebugText(Globals.TILE_SIZE, new Vector2(5, 75));

			Raylib.EndDrawing();
        }

        public void Unload() {
        }
    }
}