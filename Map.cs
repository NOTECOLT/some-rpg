//------------------------------------------------------------------------------------------
/* MAP
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Newtonsoft.Json;
using Raylib_cs;

namespace Topdown {
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

			Map map = new Map(mapJSON.Name, mapJSON.TilemapPath, mapJSON.Size) {
				_data = mapJSON.Data,
			};
			return map;
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public void RenderMap(float scale) {
			for (int i = 0; i < _data.GetLength(0); i++) {
				for (int j = 0; j < _data.GetLength(1); j++) {
					if (_data[i,j] != -1)
						_tilemap.ReturnTileSprite(_data[i,j]).RenderSprite(new Vector2(i * 32, j * 32), new Vector2(0, 0), scale);
				}
			}
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

		public MapJSON(string name, string path, Vector2 size) {
			Name = name;
			TilemapPath = path;
			Size = size;
            Data = new int[(int)size.X, (int)size.Y];
		}
	}
}