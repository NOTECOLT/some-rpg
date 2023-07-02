//------------------------------------------------------------------------------------------
/* TILEMAP
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown {
    class Tilemap {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
        public Texture2D tilemapTexture;
        public Vector2 size;
        public Vector2 tileSize;
        public Vector2[] tilePositions;

        public Tilemap(string path, float tileWidth, float tileHeight) {
            Console.WriteLine($"Current Working Directory: {Directory.GetCurrentDirectory()}");
            if (!File.Exists(path)) {
                Console.WriteLine($"[TILEMAP LOADER] [ERROR] {path} does not exist in directory!\nCannot load texture.");
                return;
            }

            tilemapTexture = Raylib.LoadTexture(path);
            tileSize = new Vector2(tileWidth, tileHeight);
            size = new Vector2(tilemapTexture.width, tilemapTexture.height);

			// Loads in all tilemap textures right when the tilemap is constructed
            int tileCapacity = (int)(size.X / tileSize.X) * (int)(size.Y / tileSize.Y);			
            tilePositions = new Vector2[tileCapacity];
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
            for (int i = 0; i < (size.Y / tileSize.Y); i++) {
                for (int j = 0; j < (size.X / tileSize.X); j++) {
                    tilePositions[n] = new Vector2(j * tileSize.X, i * tileSize.Y);
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
            if (id >= tilePositions.Length || id < 0) {
                Console.WriteLine($"[TILEMAP] [WARNING] INVALID TILE ID OF {id}");
                return null;
            }

            Sprite spr = new Sprite() {
                texture = tilemapTexture,
				origin = tilePositions[id],
				size = tileSize,
				scale = 2.0f,
				rotation = 0
            };
            return spr;
        }
    }
}