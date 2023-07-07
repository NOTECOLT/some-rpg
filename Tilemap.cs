//------------------------------------------------------------------------------------------
/* TILEMAP
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    class Tilemap {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private Texture2D _tilemapTexture;
        private Vector2 _size;
        private Vector2 _tileSize;
        private Vector2[] _tilePositions;

		// PROPERTIES
		//------------------------------------------------------------------------------------------      

        public Tilemap(string path, float tileWidth, float tileHeight) {
            Console.WriteLine($"Current Working Directory: {Directory.GetCurrentDirectory()}");
            if (!File.Exists(path)) {
                Console.WriteLine($"[TILEMAP LOADER] [ERROR] {path} does not exist in directory!\nCannot load texture.");
                return;
            }

            _tilemapTexture = Raylib.LoadTexture(path);
            _tileSize = new Vector2(tileWidth, tileHeight);
            _size = new Vector2(_tilemapTexture.width, _tilemapTexture.height);

			// Loads in all tilemap textures right when the tilemap is constructed
            int tileCapacity = (int)(_size.X / _tileSize.X) * (int)(_size.Y / _tileSize.Y);			
            _tilePositions = new Vector2[tileCapacity];
			LoadTilePositions();
        }

		// FUNCTIONS
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// <para>Each tile must be referenced by it's id, then retrieved from the original texture file</para>
		/// By loading in the tile positions from the start, we don't need to calculate the position
		/// of the tile with every call.
		/// </summary>
        private void LoadTilePositions() {
            int n = 0;
            for (int i = 0; i < (_size.Y / _tileSize.Y); i++) {
                for (int j = 0; j < (_size.X / _tileSize.X); j++) {
                    _tilePositions[n] = new Vector2(j * _tileSize.X, i * _tileSize.Y);
                    n++;
				}
            }
        }

		/// <summary>
		/// Returns the sprite of the tile associated with an id in the tilemap.
		/// <para> By default, id's start at 0 and increase from left to right, top to bottom. </para>
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        public Sprite ReturnTileSprite(int id) {
            if (id >= _tilePositions.Length || id < 0) {
                Console.WriteLine($"[TILEMAP] [WARNING] INVALID TILE ID OF {id}");
                return null;
            }

            Sprite spr = new Sprite(_tilemapTexture, _tilePositions[id], _tileSize, 2.0f, 0);
            return spr;
        }
    }
}