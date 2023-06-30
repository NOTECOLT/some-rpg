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

		// PROPERTIES
		//------------------------------------------------------------------------------------------

        public string name = "";
		public string tilemapPath = "";
		public Tilemap tilemap;
		public Vector2 size;
		public int[,] data;

		public Map(string name, string path, Vector2 size) {
			this.name = name;
			tilemapPath = path;
			tilemap = new Tilemap(path, 32, 32);
			this.size = size;
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
			Console.WriteLine($"{JsonConvert.SerializeObject(mapJSON)}");

			if (mapJSON == null) {
				Console.WriteLine($"[MAP LOADER] [WARNING] Path {path} contains invalid map json");
				return null;
			}

			Map map = new Map(mapJSON.name, mapJSON.tilemapPath, mapJSON.size) {
				data = mapJSON.data,
			};
			return map;
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public void RenderMap() {
			for (int i = 0; i < data.GetLength(0); i++) {
				for (int j = 0; j < data.GetLength(1); j++) {
					tilemap.ReturnTileSprite(data[i,j]).RenderSprite(new Vector2(i * 32, j * 32), new Vector2(0, 0));
				}
			}
		}
    }

	/// <summary>
	/// <para>This class holds all the map data read on the json file.</para>
	/// <para>Needed so there aren't any errors when deserializing json files using JSONConvert </para>
	/// </summary>
	class MapJSON {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public string name = "";
		public string tilemapPath = "";
		public Vector2 size;
		public int[,] data;

		public MapJSON(string name, string path, Vector2 size) {
			this.name = name;
			tilemapPath = path;
			this.size = size;
            data = new int[(int)size.X, (int)size.Y];
		}
	}
}