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
    class Map {
		// STATIC VARIABLES
		//------------------------------------------------------------------------------------------
		static public List<Map> mapList = new List<Map>();

		// FIELDS
		//------------------------------------------------------------------------------------------
        private string _name = "";
		private string _tilemapPath = "";
		private Tilemap _tilemap;
		private Vector2 _size;
		private int[,] _data;

		// Properties
		//------------------------------------------------------------------------------------------
		public Tilemap Tilemap { get { return _tilemap; } }
		public Vector2 Size { get { return _size; } }

		/// <summary>
		/// Creates a new map object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path">Specifies the path to the tilemap</param>
		/// <param name="size"></param>
		public Map(string name, string path, Vector2 size) {
			_name = name;
			_tilemapPath = path;
			_tilemap = new Tilemap(path, 16, 16);
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

			Map map = new Map(mapJSON.Name, mapJSON.TilemapPath, mapJSON.Size);

			// In case that the size of the map in the JSON is not equal to the actual dimensions of the data,
			//		We just create a map with the specified size, and then copy the data from the JSON to the map obj
			map._data = new int[(int)mapJSON.Size.X,(int)mapJSON.Size.Y];
			for (int i = 0; i < map._size.X; i++) {
				for (int j = 0; j < map._size.Y; j++) {
					if (i < mapJSON.Data.GetLength(0) && j < mapJSON.Data.GetLength(1))
						map._data[i,j] = mapJSON.Data[i,j];
					else
						map._data[i,j] = -1;
				}
			}

			Console.WriteLine($"[MAP LOADER] Map succesfully loaded from file at {path}.");

			return map;
		}

		/// <summary>
		/// Saves map object to a json file at the specified path
		/// </summary>
		/// <param name="map"></param>
		/// <param name="path"></param>
		public static void SaveMap(Map map, string path) {
			MapJSON json = new MapJSON(map._name, map._tilemapPath, map._size);
			json.Data = map._data;

			string text = JsonConvert.SerializeObject(json);
			
			if (!File.Exists(path))
				Console.WriteLine($"[MAP SAVER] File at {path} does not exist! Will create new file.");
			else
				Console.WriteLine("$[MAP SAVER] File at {path} does exist! Will overwrite.");

			File.WriteAllText(path, text);
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public void RenderMap(float scale) {
			for (int i = 0; i < _data.GetLength(0); i++) {
				for (int j = 0; j < _data.GetLength(1); j++) {
					if (_data[i,j] != -1)
						_tilemap.ReturnTileSprite(_data[i,j]).RenderSprite(new Vector2(i * 32, j * 32), new Vector2(0, 0), scale, Color.WHITE);
				}
			}
		}

		public void SetTile(int x, int y, int id) {
			if (x >= 0 && y >= 0 && x < _size.X && y < _size.Y)
				if (id >= -1 && id < _tilemap.Tiles)
					_data[x,y] = id;
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
		public string TilemapPath = "";
		public Vector2 Size;
		public int[,] Data;

		/// <summary>
		/// Initializes a new mapJSON object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path">Specifies the path to the tilemap</param>
		/// <param name="size"></param>
		public MapJSON(string name, string path, Vector2 size) {
			Name = name;
			TilemapPath = path;
			Size = size;
            Data = new int[(int)size.X, (int)size.Y];
		}
	}
}