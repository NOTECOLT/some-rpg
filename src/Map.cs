//------------------------------------------------------------------------------------------
/* MAP

	TODO: TiledCS is not updated (last updated Dec 2022); so some day,,, i will prolly have to make a custom tmx reader
	Mental note: TiledCS /prolly/ works with Tiled 1.9 the latest (most prolly 1.8)
		take note of any recent changes since then


	I have adjacent maps rendering fine, but how do i properly workout tile collisions?

	Have a list of maps in the scene? and then iterate through them for tile collisions?
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using TiledCS;
using Raylib_cs;
using Topdown.ECS;

namespace Topdown {
	public enum Direction {
		North,
		South,
		West,
		East
	}
	
	/// <summary>
	/// Class for map related functions. Class utilizes TiledCS library
	/// </summary>
    public class Map {

		// STATIC PROPERTIES
		//------------------------------------------------------------------------------------------
		/// <summary>
		/// A static dictionary for all tileset texture files.
		/// </summary>
		public static Dictionary<String, Texture2D> TilesetTextures { get; private set; } = new Dictionary<String, Texture2D>();

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		
		public String FilePath { get; }
		/// <summary>
		/// Marks the origin of the map (top left corner.) Defined in world coordinates
		/// </summary>	
		public Vector2 Origin { get; private set; }
		public TiledMap LoadedMap { get; private set; }
		public Dictionary<int, TiledTileset> LoadedTilesets { get; private set; }
		public Dictionary<Direction, Map> AdjacentMaps { get; set; } = new Dictionary<Direction, Map>();

		/// <summary>
		/// Map Constructor essentially acts as a map loader
		/// </summary>
		/// <param name="path"></param>
		public Map(string path, Vector2 origin) {
			FilePath = path;
			LoadedMap = new TiledMap(path);
			LoadedTilesets = LoadedMap.GetTiledTilesets("resources/maps/");
			// _tilesetTextures = null;

			Origin = origin;
		}

		public Map(string path) {
			FilePath = path;
			LoadedMap = new TiledMap(path);
			LoadedTilesets = LoadedMap.GetTiledTilesets("resources/maps/");
			// _tilesetTextures = null;

			Origin = Vector2.Zero;
		}


		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
		public static void UnloadAllTextures() {
			TilesetTextures = new Dictionary<String, Texture2D>();
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
        /// <summary>
		/// <para>Loads all the textures needed of a map's tileset into a dictionary of texture files.</para>
		/// Is run whenever new textures must be loaded
		/// </summary>
		public void LoadTextures() {
			foreach (KeyValuePair<int, TiledTileset> entry in LoadedTilesets) {
				if (!TilesetTextures.ContainsKey(entry.Value.Name))
					TilesetTextures[entry.Value.Name] = Raylib.LoadTexture("resources/tilesets/" + entry.Value.Image.source);
			}
		}

		public List<Entity> LoadObjectsAsEntities(int tileSize) {
			List<Entity> EntityList = new List<Entity>();

			TiledLayer layer = LoadedMap.Layers.FirstOrDefault(layer => layer.type == TiledLayerType.ObjectLayer, null);
			if (layer is null) return null;

			TiledObject[] tiledObjects = layer.objects;

			foreach (TiledObject obj in tiledObjects) {
				TiledProperty prop = obj.properties.FirstOrDefault(prop => prop.name == "Object Type", null);

				// ew goto statement. Please never let this balloon
				if (prop is null) goto defaultEntity;
				string type = prop.value;
					
				if (type == "signpost") {
					Dialogue dialogue = XMLDialogueParser.LoadDialogueFromFile(obj.properties.First(prop => prop.name == "Dialogue").value);
					Signpost signpost = new Signpost(new Vector2(obj.x / LoadedMap.TileWidth, obj.y / LoadedMap.TileWidth - 1) + (Origin / tileSize), dialogue, ReturnSpriteFromGID(obj.gid));
					EntityList.Add(signpost);

					continue;
				}

				defaultEntity:
					Entity entity = new Entity();
					entity.AddComponent(new ETransform(new Vector2(obj.x / LoadedMap.TileWidth, obj.y / LoadedMap.TileWidth - 1) + (Origin / tileSize), 0, 0, Globals.TILE_SIZE));
					entity.AddComponent(new ESprite(ReturnSpriteFromGID(obj.gid), 0));
					EntityList.Add(entity);


			}

			return EntityList;
		}

		public void LoadAdjacentMaps(int tileSize) {
			List<Direction> directions = new List<Direction>() {Direction.North, Direction.East, Direction.South, Direction.West};
			foreach (Direction dir in directions) {
				TiledProperty prop = LoadedMap.Properties.FirstOrDefault(prop => prop.name == dir.ToString(), null);

				if (prop is null) continue;

				AdjacentMaps[dir] = new Map(prop.value);

				Vector2 newOrigin = Origin;
				
				// sloppy but whatever:
				switch (dir) {
					case Direction.North:
						newOrigin -= new Vector2(0, AdjacentMaps[dir].LoadedMap.Height) * tileSize;
						break;
					case Direction.East:
						newOrigin += new Vector2(LoadedMap.Width, 0) * tileSize;
						break;
					case Direction.South:
						newOrigin += new Vector2(0, LoadedMap.Height) * tileSize;
						break;
					case Direction.West:
						newOrigin -= new Vector2(AdjacentMaps[dir].LoadedMap.Width, 0) * tileSize;
						break;
					default:
						break;
				}

				AdjacentMaps[dir].Origin = newOrigin;
			}
		}

		/// <summary>
        /// <para>Renders Map using TiledCS Library</para>
        /// Reference: https://github.com/TheBoneJarmer/TiledCS
        /// </summary>
        /// <param name="map">Object Representation of tmx file</param>
        /// <param name="tilesets">Object representation of tsx file</param>
        /// <param name="tilesetTextures">Maps the tsx representation to Raylib Textures</param>
        /// <param name="scale"></param>
        public void RenderMap(Camera2D camera, float scale, int tileSize, int screenWidth, int screenHeight) {
			if (LoadedTilesets == null) LoadTextures();

			// Reference: https://github.com/TheBoneJarmer/TiledCS
            foreach (TiledLayer layer in LoadedMap.Layers) {
                for (int y = 0; y < layer.height; y++) {
                    for (int x = 0; x < layer.width; x++) {
                        int gid = layer.data[(y * layer.width) + x];
						if (gid == 0) continue;
						

						Vector2 drawPos = new Vector2(x * tileSize, y * tileSize) + Origin;
						Vector2 screenPos = Raylib.GetWorldToScreen2D(drawPos, camera);
						if (screenPos.X + tileSize < 0 || 
							screenPos.Y + tileSize < 0 ||
							screenPos.X > screenWidth ||
							screenPos.Y > screenHeight) {
							continue;
						}
                        
                        // Represents the retrieved texture & rect as sprite then renders
                        ReturnSpriteFromGID(gid).RenderSprite(drawPos, new Vector2(0, 0), scale, Color.WHITE);;
                    }
                }
            }
        }

		/// <summary>
		/// Checks if a tile is walkable or not. Checks for walkability across ALL layers.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="layer"></param>
		/// <returns>Returns true if there is a collision (i.e. tile not walkable). False otherwise.</returns>
		public bool IsMapCollision(Vector2 tile, int tileSize) {
			tile -= Origin / tileSize;

			// TODO: FIND A BETTER WAY TO DO THIS PART?
			//		EDIT: I dont think there is a better way to simplify this boolean

			// Map boundaries with no connection
			if ((tile.X < 0 && !HasMapConnection(Direction.West)) || 
				(tile.Y < 0 && !HasMapConnection(Direction.North))|| 
				(tile.X >= LoadedMap.Width && !HasMapConnection(Direction.East)) || 
				(tile.Y >= LoadedMap.Height && !HasMapConnection(Direction.South)))
				return true;
				// Map Boundaries with connection
				// 		collision function should give way for other map collision functions
			else if ((tile.X < 0 && HasMapConnection(Direction.West)) || 
				(tile.Y < 0 && HasMapConnection(Direction.North))|| 
				(tile.X >= LoadedMap.Width && HasMapConnection(Direction.East)) || 
				(tile.Y >= LoadedMap.Height && HasMapConnection(Direction.South)))
				return false;

            int idx = ((int)tile.Y * LoadedMap.Width) + (int)tile.X;
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
				TiledProperty walkableProp = ReturnTileFromGID(gid).properties.First(prop => prop.name == "Walkable"); 	
				walkable = Convert.ToBoolean(walkableProp.value);
				// Console.WriteLine($"{gid} Walkable: {walkable}");
				if (!walkable) return true;				
			}				
			return false;
		}

		/// <summary>
		/// Returns true if a map has a connection in a given direcion
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public bool HasMapConnection(Direction dir) {
			// TODO: Improve this, can be better
			TiledProperty prop = null;
			switch (dir) {
				case Direction.North:
					prop = LoadedMap.Properties.FirstOrDefault(prop => prop.name == "North", null);
					break;
				case Direction.East:
					prop = LoadedMap.Properties.FirstOrDefault(prop => prop.name == "East", null);
					break;
				case Direction.West:
					prop = LoadedMap.Properties.FirstOrDefault(prop => prop.name == "West", null);
					break;
				case Direction.South:
					prop = LoadedMap.Properties.FirstOrDefault(prop => prop.name == "South", null);
					break;
				default:
					return false;
			}

			return prop is not null;
		}

		// PRIVATE FUNCTIONS
		//------------------------------------------------------------------------------------------
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
			return new Sprite(TilesetTextures[ts.Name], new Vector2(rect.x, rect.y), new Vector2(rect.width, rect.height), 0);
		}
	

	}
}