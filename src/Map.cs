//------------------------------------------------------------------------------------------
/* MAP
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Newtonsoft.Json;
using Raylib_cs;

namespace Topdown {
	/// <summary>
	/// Map Object. Does not serialize into json object.
	/// </summary>
    public class Map {
		// STATIC VARIABLES
		//------------------------------------------------------------------------------------------
		static public List<Map> mapList = new List<Map>();

		// FIELDS
		//------------------------------------------------------------------------------------------
        private string _name = "";
		private string[] _tilemapPaths;
		private Tilemap[] _tilemaps;
		private Vector3 _size;
		private int[,,] _data;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Tilemap[] Tilemaps { get { return _tilemaps; } }
		public Vector3 Size { get { return _size; } }

		/// <summary>
		/// Creates a new map object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path">Specifies the path to the tilemap</param>
		/// <param name="size"></param>
		public Map(string name, string[] paths, Vector3 size) {
			_name = name;
			_tilemapPaths = paths;
			_tilemaps = new Tilemap[paths.Length];

			int runningID = 0;
			for (int i = 0; i < paths.Length; i++) {
				runningID += (i > 0) ? _tilemaps[i - 1].TileCapacity : 0;
				_tilemaps[i] = new Tilemap(paths[i], 16, 16, runningID);
			}
				
			_size = size;
		}

		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
		/// <summary>
		/// Deserializes a map.json file into a map object
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Map LoadMap(string path) {
			string fileText = File.ReadAllText(path);
			Console.WriteLine($"[MAP LOADER] JSON File Read: \n{fileText}");

			MapJSON mapJSON = JsonConvert.DeserializeObject<MapJSON>(fileText); 

			/* 	For some reason JsonConvert results this warning message:
				"WARNING: FILEIO: File name provided is not valid"
				Everything still works fine tho (like it still deserializes)
			*/
			// Console.WriteLine($"{JsonConvert.SerializeObject(mapJSON)}");

			if (mapJSON == null) {
				Console.WriteLine($"[MAP LOADER] [WARNING] Path {path} contains invalid map json");
				return null;
			}

			Map map = new Map(mapJSON.Name, mapJSON.TilemapPaths, mapJSON.Size);

			// In case that the size of the map in the JSON is not equal to the actual dimensions of the data,
			//		We just create a map with the specified size, and then copy the data from the JSON to the map obj
			map._data = new int[(int)mapJSON.Size.X, (int)mapJSON.Size.Y, (int)mapJSON.Size.Z];
			CopyMapData(mapJSON.data, new Vector3 (mapJSON.data.GetLength(0), mapJSON.data.GetLength(1), mapJSON.data.GetLength(2)), ref map._data, map._size);

			Console.WriteLine($"[MAP LOADER] Map succesfully loaded from file at {path}.");

			return map;
		}

		/// <summary>
		/// Saves map object to a json file at the specified path
		/// </summary>
		/// <param name="map"></param>
		/// <param name="path"></param>
		public static void SaveMap(Map map, string path) {
			MapJSON json = new MapJSON(map._name, map._tilemapPaths, map._size);
			json.data = map._data;

			string text = JsonConvert.SerializeObject(json);
			
			if (!File.Exists(path))
				Console.WriteLine($"[MAP SAVER] File at {path} does not exist! Will create new file.");
			else
				Console.WriteLine($"[MAP SAVER] File at {path} does exist! Will overwrite.");

			File.WriteAllText(path, text);
		}

		/// <summary>
		/// Copies Map Data from one array to another, and fills any remaining overflow data with empty cells (-1)
		/// </summary>
		/// <param name="srcData"></param>
		/// <param name="srcSize"></param>
		/// <param name="dstData"></param>
		/// <param name="dstSize"></param>
		public static void CopyMapData(int[,,] srcData, Vector3 srcSize, ref int[,,] dstData, Vector3 dstSize) {
			for (int i = 0; i < dstSize.X; i++) {
				for (int j = 0; j < dstSize.Y; j++) {
					for (int k = 0; k < dstSize.Z; k++) {
						if (i < srcSize.X && j < srcSize.Y && k < srcSize.Z)
							dstData[i,j,k] = srcData[i,j,k];
						else
							dstData[i,j,k] = -1;
					}
				}
			}
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public void RenderMap(float scale) {
			for (int i = 0; i < _data.GetLength(0); i++) {
				for (int j = 0; j < _data.GetLength(1); j++) {
					for (int k = 0; k < _data.GetLength(2); k++) {
						if (_data[i,j,k] != -1)
							Tilemap.ReturnTileSpriteFromArray(_tilemaps, _data[i,j,k]).RenderSprite(new Vector2(i * 32, j * 32), new Vector2(0, 0), scale, Color.WHITE);
							//_tilemaps.ReturnTileSprite();
					}
				}
			}
		}

		public void SetTile(int x, int y, int z, int id) {
			if (x >= 0 && y >= 0 && x < _size.X && y < _size.Y)
				if (id >= -1 && id < Tilemap.GetTotalTileArrayCapacity(_tilemaps))
					_data[x,y,z] = id;
		}
    }

	/// <summary>
	/// <para>This class holds all the map data read on the json file.</para>
	/// <para>Needed so there aren't any errors when deserializing json files using JSONConvert </para>
	/// </summary>
	class MapJSON {
		// PUBLIC PROPERTIES
		//------------------------------------------------------------------------------------------
		public string Name = "";
		public string[] TilemapPaths;
		public Vector3 Size;
		public int[,,] data; // TODO: ADJUST EVERYTHING TO FIT THIS

		/// <summary>
		/// Initializes a new mapJSON object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="paths">Specifies the path to the tilemap</param>
		/// <param name="size"></param>
		public MapJSON(string name, string[] paths, Vector3 size) {
			Name = name;
			TilemapPaths = paths;
			Size = size;
            data = new int[(int)size.X, (int)size.Y, (int)size.Z];
			// NOTE: Array representation is very different from the representation seen ingame
			//	First axis denotes Y-axis
			// 	Second axis denotes X-axis
			// 	Third axis denotes layer
		}
	}
}