using System.Numerics;
using Raylib_cs;
using Topdown.Engine;
using TiledCS;

namespace Topdown {
	/// <summary>
	/// Scene for when the player is in the overworld.
	/// </summary>
    public class OverworldScene : IScene {
        private Entity _player;
		private List<Entity> _entityList;	// Contains a list of entities excluding the player
        private Camera2D _camera;
        private Map _map;
		

		/// <summary>
		/// Overworld Scene must be initialized with a map and player data
		/// </summary>
		/// <param name="player"></param>
		/// <param name="map"></param>
        public OverworldScene(Entity player, string mapPath) {
            _player = player;
            _camera = new Camera2D() {
				rotation = 0,
				zoom = 1
			};
			_map = new Map(mapPath);
        }

		// SCENE FUNCTIONS
        //------------------------------------------------------------------------------------------
        public void Load() {
			_map.LoadTextures();
			_entityList = _map.LoadObjectsAsEntities();
        }

        public void Update() {
			// 1 - INPUT
			//--------------------------------------------------
			if (!_player.IsMoving) {
				_player.IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

				if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) {
					_player.TargetTP = new Vector2(_player.TilePos.X + 1, _player.TilePos.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) {
					_player.TargetTP = new Vector2(_player.TilePos.X - 1, _player.TilePos.Y);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) {
					_player.TargetTP = new Vector2(_player.TilePos.X, _player.TilePos.Y + 1);
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) {
					_player.TargetTP = new Vector2(_player.TilePos.X, _player.TilePos.Y - 1);
				} else {
					_player.TargetTP = new Vector2(_player.TilePos.X, _player.TilePos.Y);
				}

				// Collision, movement cancellation
				if (_player.TargetTP != _player.TilePos) {
					if (_map.IsMapCollision(_player.TargetTP) || _entityList.Where(e => e.TilePos == _player.TargetTP).Count() > 0)
						_player.TargetTP = _player.TilePos;
				}
					
			}
		
			// 2 - PHYSICS
			//--------------------------------------------------

			_player.UpdateEntityVectors(Globals.TILE_SIZE, _map);

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);
				_camera.target = new Vector2(_player.Position.X + (Globals.TILE_SIZE / 2), _player.Position.Y + (Globals.TILE_SIZE / 2));
				_camera.offset = new Vector2(Globals.SCREEN_WIDTH / 2, Globals.SCREEN_HEIGHT / 2);
				Raylib.BeginMode2D(_camera);

					if (_map != null)
						_map.RenderMap(Globals.WORLD_SCALE);
					_player.RenderEntity(Globals.TILE_SIZE, Globals.WORLD_SCALE);

					foreach (Entity e in _entityList)
						e.RenderEntity(Globals.TILE_SIZE, Globals.WORLD_SCALE);

				Raylib.EndMode2D();

				Raylib.DrawText($"fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.BLACK);
				Raylib.DrawText($"Mode: OVERWORLD", 5, 40, 30, Color.BLACK);
				_player.DrawEntityDebugText(Globals.TILE_SIZE, new Vector2(5, 75));

			Raylib.EndDrawing();
        }

        public void Unload() {
        }
    }
}