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
        private Camera2D _camera;
        private TiledMap _map;
		private Dictionary<int, TiledTileset> _tilesets;
		private Dictionary<TiledTileset, Texture2D> _tilesetTextures;

		/// <summary>
		/// Overworld Scene must be initialized with a map and player data
		/// </summary>
		/// <param name="player"></param>
		/// <param name="map"></param>
        public OverworldScene(Entity player, TiledMap map, Dictionary<int, TiledTileset> tilesets) {
            _player = player;
            _camera = new Camera2D() {
				rotation = 0,
				zoom = 1
			};
            _map = map;
			_tilesets = tilesets;
        }

		// SCENE FUNCTIONS
        //------------------------------------------------------------------------------------------
        public void Load() {
			_tilesetTextures = new Dictionary<TiledTileset, Texture2D>();
			
			foreach (KeyValuePair<int, TiledTileset> entry in _tilesets) {
				_tilesetTextures[entry.Value] = Raylib.LoadTexture("resources/tilesets/" + entry.Value.Image.source);
			}
        }

        public void Update() {
			// 1 - INPUT
			//--------------------------------------------------
			if (!_player.IsMoving) {
				_player.IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

				if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
					_player.TargetWP = new Vector2(_player.WorldPos.X + 1, _player.WorldPos.Y);
				else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
					_player.TargetWP = new Vector2(_player.WorldPos.X - 1, _player.WorldPos.Y);
				else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
					_player.TargetWP = new Vector2(_player.WorldPos.X, _player.WorldPos.Y + 1);
				else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
					_player.TargetWP = new Vector2(_player.WorldPos.X, _player.WorldPos.Y - 1);
				else
					_player.TargetWP = new Vector2(_player.WorldPos.X, _player.WorldPos.Y);
			}
		
			// 2 - PHYSICS
			//--------------------------------------------------

			_player.UpdateEntityVectors(Globals.TILE_SIZE);

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.RAYWHITE);
				_camera.target = new Vector2(_player.Position.X + (Globals.TILE_SIZE / 2), _player.Position.Y + (Globals.TILE_SIZE / 2));
				_camera.offset = new Vector2(Globals.SCREEN_WIDTH / 2, Globals.SCREEN_HEIGHT / 2);
				Raylib.BeginMode2D(_camera);

					if (_map != null)
						Map.RenderMap(_map, _tilesets, _tilesetTextures, Globals.WORLD_SCALE);
						// _map.RenderMap(Globals.WORLD_SCALE);
					_player.RenderEntity(Globals.TILE_SIZE, Globals.WORLD_SCALE);

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