//------------------------------------------------------------------------------------------
/* MAP

	TODO: TiledCS is not updated (last updated Dec 2022); so some day,,, i will prolly have to make a custom tmx reader
	Mental note: TiledCS /prolly/ works with Tiled 1.9 the latest (most prolly 1.8)
		take note of any recent changes since then
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using TiledCS;
using Raylib_cs;

namespace Topdown {
	/// <summary>
	/// Class for map related functions. Class utilizes TiledCS library
	/// </summary>
    public class Map {
		// FIELDS
		//------------------------------------------------------------------------------------------
		private Dictionary<TiledTileset, Texture2D> _tilesetTextures;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public TiledMap LoadedMap { get; private set; }
		public Dictionary<int, TiledTileset> LoadedTilesets { get; private set; }

		public Map(string path) {
			LoadedMap = new TiledMap(path);
			LoadedTilesets = LoadedMap.GetTiledTilesets("resources/maps/");
			_tilesetTextures = null;
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
        /// <summary>
		/// <para>Loads all the textures needed of a map's tileset into a dictionary of texture files.</para>
		/// Is called once during the load sequence
		/// </summary>
		public void LoadTextures() {
			_tilesetTextures = new Dictionary<TiledTileset, Texture2D>();
			
			foreach (KeyValuePair<int, TiledTileset> entry in LoadedTilesets) {
				_tilesetTextures[entry.Value] = Raylib.LoadTexture("resources/tilesets/" + entry.Value.Image.source);
			}
		}

		public List<Entity> LoadObjectsAsEntities() {
			List<Entity> EntityList = new List<Entity>();

			foreach (TiledObject obj in LoadedMap.Layers.First(layer => layer.type == TiledLayerType.ObjectLayer).objects) {
				Entity entity = new Entity(new Vector2(obj.x / LoadedMap.TileWidth, obj.y / LoadedMap.TileWidth - 1), 0, Globals.TILE_SIZE);
				entity.Sprite = ReturnSpriteFromGID(obj.gid);
				EntityList.Add(entity);
			}

			return EntityList;
		}

		/// <summary>
        /// <para>Renders Map using TiledCS Library</para>
        /// Reference: https://github.com/TheBoneJarmer/TiledCS
        /// </summary>
        /// <param name="map">Object Representation of tmx file</param>
        /// <param name="tilesets">Object representation of tsx file</param>
        /// <param name="tilesetTextures">Maps the tsx representation to Raylib Textures</param>
        /// <param name="scale"></param>
        public void RenderMap(float scale) {
			if (LoadedTilesets == null) LoadTextures();

			// Reference: https://github.com/TheBoneJarmer/TiledCS
            foreach (TiledLayer layer in LoadedMap.Layers) {
                for (int y = 0; y < layer.height; y++) {
                    for (int x = 0; x < layer.width; x++) {
                        int gid = layer.data[(y * layer.width) + x];

                        if (gid == 0) continue;

                        // Represents the retrieved texture & rect as sprite then renders
                        ReturnSpriteFromGID(gid).RenderSprite(new Vector2(x * 32, y * 32), new Vector2(0, 0), scale, Color.WHITE);;
                    }
                }
            }
        }


		/// <summary>
		/// Checks if a tile is walkable or not. Checks for walkability across ALL layers.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="layer"></param>
		/// <returns>Returns true if there is a collision (i.e. tile not walkable). False otherwise.</returns>
		public bool IsMapCollision(Vector2 pos) {
			if (pos.X < 0 || pos.Y < 0 || pos.X >= LoadedMap.Width || pos.Y >= LoadedMap.Height)
				return true;

            int idx = ((int)pos.Y * LoadedMap.Width) + (int)pos.X;
            // int gid = LoadedMap.Layers[layer].data[idx];
			// if (gid == 0) return true;
			bool walkable = true;

			// Checks each layer for collision (rather than one layer)
			// Might not be efficient if we increase the number of layers
			//		(Will run multiple loops with each movement)
			foreach (TiledLayer mapLayer in LoadedMap.Layers) {
				if (mapLayer.type == TiledLayerType.ObjectLayer) continue;

				int gid = mapLayer.data[idx];
				if (gid == 0) continue;

				// Retrieves the Walkable Property; search up LINQ - https://www.tutorialsteacher.com/linq/linq-element-operator-first-firstordefault
				// TiledTileset ts = LoadedTilesets[LoadedMap.GetTiledMapTileset(gid).firstgid];
				TiledProperty walkableProp = ReturnTileFromGID(gid).properties.First(prop => prop.name == "Walkable" ); 	
				walkable = Convert.ToBoolean(walkableProp.value);
				if (!walkable) return true;				
			}				
			return false;
		}

		private TiledTile ReturnTileFromGID(int gid) {
			TiledTileset ts = LoadedTilesets[LoadedMap.GetTiledMapTileset(gid).firstgid];
			return ts.Tiles[gid - LoadedMap.GetTiledMapTileset(gid).firstgid];
		}

		private Sprite ReturnSpriteFromGID(int gid) {
			// Retrieve information from Tiled Object
			//      first line retrieves the tileset that the gid in the map is mapped to
			//      second line gets the source recta
			TiledTileset ts = LoadedTilesets[LoadedMap.GetTiledMapTileset(gid).firstgid];
			TiledSourceRect rect = LoadedMap.GetSourceRect(LoadedMap.GetTiledMapTileset(gid), ts, gid);
			return new Sprite(_tilesetTextures[ts], new Vector2(rect.x, rect.y), new Vector2(rect.width, rect.height), 0);
		}
	}
}


//------------------------------------------------------------------------------------------
/* MAP [DEPRECATED]
*/
//------------------------------------------------------------------------------------------
// namespace Topdown {
// 	/// <summary>
// 	/// Map Object. Does not serialize into json object.
// 	/// </summary>
//     public class Map {
// 		// STATIC VARIABLES
// 		//------------------------------------------------------------------------------------------
// 		static public List<Map> mapList = new List<Map>();

// 		// FIELDS
// 		//------------------------------------------------------------------------------------------
//         private string _name = "";
// 		private string[] _tilemapPaths;
// 		private Tilemap[] _tilemaps;
// 		private Vector3 _size;
// 		private int[,,] _data;

// 		// PROPERTIES
// 		//------------------------------------------------------------------------------------------
// 		public Tilemap[] Tilemaps { get { return _tilemaps; } }
// 		public Vector3 Size { get { return _size; } }

// 		/// <summary>
// 		/// Creates a new map object
// 		/// </summary>
// 		/// <param name="name"></param>
// 		/// <param name="path">Specifies the path to the tilemap</param>
// 		/// <param name="size"></param>
// 		public Map(string name, string[] paths, Vector3 size) {
// 			_name = name;
// 			_tilemapPaths = paths;
// 			_tilemaps = new Tilemap[paths.Length];

// 			int runningID = 0;
// 			for (int i = 0; i < paths.Length; i++) {
// 				runningID += (i > 0) ? _tilemaps[i - 1].TileCapacity : 0;
// 				_tilemaps[i] = new Tilemap(paths[i], 16, 16, runningID);
// 			}
				
// 			_size = size;
// 		}

// 		// STATIC FUNCTIONS
// 		//------------------------------------------------------------------------------------------
// 		/// <summary>
// 		/// Deserializes a map.json file into a map object
// 		/// </summary>
// 		/// <param name="path"></param>
// 		/// <returns></returns>
// 		public static Map CreateMapFromPath(string path) {
// 			if (!File.Exists(path)) {
// 				Console.WriteLine($"[MAP LOADER] File {path} not found!");
// 				return null;
// 			}

// 			string fileText = File.ReadAllText(path);
// 			Console.WriteLine($"[MAP LOADER] JSON File Read: \n{fileText}");

// 			MapJSON mapJSON = JsonConvert.DeserializeObject<MapJSON>(fileText); 

// 			/* 	For some reason JsonConvert results this warning message:
// 				"WARNING: FILEIO: File name provided is not valid"
// 				Everything still works fine tho (like it still deserializes)
// 			*/
// 			// Console.WriteLine($"{JsonConvert.SerializeObject(mapJSON)}");

// 			if (mapJSON == null) {
// 				Console.WriteLine($"[MAP LOADER] [WARNING] Path {path} contains invalid map json");
// 				return null;
// 			}

// 			Map map = new Map(mapJSON.Name, mapJSON.TilemapPaths, mapJSON.Size);

// 			// In case that the size of the map in the JSON is not equal to the actual dimensions of the data,
// 			//		We just create a map with the specified size, and then copy the data from the JSON to the map obj
// 			map._data = new int[(int)mapJSON.Size.X, (int)mapJSON.Size.Y, (int)mapJSON.Size.Z];
// 			CopyMapData(mapJSON.data, new Vector3 (mapJSON.data.GetLength(0), mapJSON.data.GetLength(1), mapJSON.data.GetLength(2)), ref map._data, map._size);

// 			Console.WriteLine($"[MAP LOADER] Map succesfully loaded from file at {path}.");

// 			return map;
// 		}

// 		/// <summary>
// 		/// Saves map object to a json file at the specified path
// 		/// </summary>
// 		/// <param name="map"></param>
// 		/// <param name="path"></param>
// 		public static void SaveMap(Map map, string path) {
// 			MapJSON json = new MapJSON(map._name, map._tilemapPaths, map._size);
// 			json.data = map._data;

// 			string text = JsonConvert.SerializeObject(json);
			
// 			if (!File.Exists(path))
// 				Console.WriteLine($"[MAP SAVER] File at {path} does not exist! Will create new file.");
// 			else
// 				Console.WriteLine($"[MAP SAVER] File at {path} does exist! Will overwrite.");

// 			File.WriteAllText(path, text);
// 		}

// 		/// <summary>
// 		/// Copies Map Data from one array to another, and fills any remaining overflow data with empty cells (-1)
// 		/// </summary>
// 		/// <param name="srcData"></param>
// 		/// <param name="srcSize"></param>
// 		/// <param name="dstData"></param>
// 		/// <param name="dstSize"></param>
// 		public static void CopyMapData(int[,,] srcData, Vector3 srcSize, ref int[,,] dstData, Vector3 dstSize) {
// 			for (int i = 0; i < dstSize.X; i++) {
// 				for (int j = 0; j < dstSize.Y; j++) {
// 					for (int k = 0; k < dstSize.Z; k++) {
// 						if (i < srcSize.X && j < srcSize.Y && k < srcSize.Z)
// 							dstData[i,j,k] = srcData[i,j,k];
// 						else
// 							dstData[i,j,k] = -1;
// 					}
// 				}
// 			}
// 		}

// 		// FUNCTIONS
// 		//------------------------------------------------------------------------------------------
// 		public void RenderMap(float scale) {
// 			for (int i = 0; i < _data.GetLength(0); i++) {
// 				for (int j = 0; j < _data.GetLength(1); j++) {
// 					for (int k = 0; k < _data.GetLength(2); k++) {
// 						if (_data[i,j,k] != -1)
// 							Tilemap.ReturnTileSpriteFromArray(_tilemaps, _data[i,j,k]).RenderSprite(new Vector2(i * 32, j * 32), new Vector2(0, 0), scale, Color.WHITE);
// 							//_tilemaps.ReturnTileSprite();
// 					}
// 				}
// 			}
// 		}

// 		public void SetTile(int x, int y, int z, int id) {
// 			if (x >= 0 && y >= 0 && x < _size.X && y < _size.Y)
// 				if (id >= -1 && id < Tilemap.GetTotalTileArrayCapacity(_tilemaps))
// 					_data[x,y,z] = id;
// 		}
//     }

// 	/// <summary>
// 	/// <para>This class holds all the map data read on the json file.</para>
// 	/// <para>Needed so there aren't any errors when deserializing json files using JSONConvert </para>
// 	/// </summary>
// 	class MapJSON {
// 		// PUBLIC PROPERTIES
// 		//------------------------------------------------------------------------------------------
// 		public string Name = "";
// 		public string[] TilemapPaths;
// 		public Vector3 Size;
// 		public int[,,] data; // TODO: ADJUST EVERYTHING TO FIT THIS

// 		/// <summary>
// 		/// Initializes a new mapJSON object
// 		/// </summary>
// 		/// <param name="name"></param>
// 		/// <param name="paths">Specifies the path to the tilemap</param>
// 		/// <param name="size"></param>
// 		public MapJSON(string name, string[] paths, Vector3 size) {
// 			Name = name;
// 			TilemapPaths = paths;
// 			Size = size;
//             data = new int[(int)size.X, (int)size.Y, (int)size.Z];
// 			// NOTE: Array representation is very different from the representation seen ingame
// 			//	First axis denotes Y-axis
// 			// 	Second axis denotes X-axis
// 			// 	Third axis denotes layer
// 		}
// 	}
// }