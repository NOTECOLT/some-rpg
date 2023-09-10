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
using System.Xml;
using TiledCS;
using Raylib_cs;
using Topdown.ECS;
using Topdown.Renderer;
using Topdown.DialogueSystem;

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
		private static int _tileSize = Globals.ScaledTileSize;

		// STATIC PROPERTIES
		//------------------------------------------------------------------------------------------
		/// <summary>
		/// A static dictionary for all tileset texture files.
		/// </summary>
		public static Dictionary<String, Texture2D> TilesetTextures { get; private set; } = new Dictionary<String, Texture2D>();

		/// <summary>
		/// References file for all map names and their file paths
		/// </summary>
		public static Dictionary<string, string> MapDictionary = new Dictionary<string, string>();

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public String Name { get; }
		// public String FilePath { get; }
		/// <summary>
		/// Marks the origin of the map (top left corner.) Defined in world coordinates
		/// </summary>	
		public Vector2 Origin { get; private set; }
		public TiledMap LoadedMap { get; private set; }
		public Dictionary<int, TiledTileset> LoadedTilesets { get; private set; }
		public Dictionary<Direction, Map> AdjacentMaps { get; set; } = new Dictionary<Direction, Map>();

		/// <summary>
		/// Map Constructor loads the map from the file, but does not load any textures nor entities into the game
		/// </summary>
		/// <param name="path"></param>
		public Map(String name, Vector2 origin) {
			LoadedMap = new TiledMap(MapDictionary[name]);
			LoadedTilesets = LoadedMap.GetTiledTilesets("resources/maps/");

			Name = name;
			Origin = origin;
		}

		/// <summary>
		/// Map Constructor loads the map from the file, but does not load any textures nor entities into the game
		/// </summary>
		/// <param name="path"></param>
		public Map(String name) {
			LoadedMap = new TiledMap(MapDictionary[name]);
			LoadedTilesets = LoadedMap.GetTiledTilesets("resources/maps/");

			Name = name;
			Origin = Vector2.Zero;
		}


		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
		public static void UnloadAllTextures() {
			TilesetTextures = new Dictionary<String, Texture2D>();
		}

        public static void CreateMapDictionary() {
			string dictPath = "resources/maps/mapdictionary.xml";
			if (!File.Exists(dictPath)) {
				throw new FileLoadException($"Map Dictionary at {dictPath} does not exist");
			}

			// 1 - FILE READ
			//--------------------------------------------------
			string fileText = File.ReadAllText(dictPath);

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fileText);

			// 2 - XML PARSE
			//--------------------------------------------------
			XmlElement root = xml.DocumentElement;
			
			foreach (XmlNode node in root) {
				if (node.Name != "map") continue;;

				string name = "", path = "";
				foreach (XmlNode child in node.ChildNodes) {
					if (child.Name == "name") name = child.InnerText;
					if (child.Name == "path") path = child.InnerText;
				}

				MapDictionary[name] = path;
			}
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

		public List<Entity> LoadObjectsAsEntities() {
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
					Signpost signpost = new Signpost(new Vector2(obj.x / LoadedMap.TileWidth, obj.y / LoadedMap.TileWidth - 1) + (Origin / _tileSize), dialogue, ReturnSpriteFromGID(obj.gid));
					signpost.SetTiledProperties(obj.properties);
					EntityList.Add(signpost);

					continue;
				}

				defaultEntity:
					Entity entity = new Entity();
					entity.AddComponent(new ETransform(new Vector2(obj.x / LoadedMap.TileWidth, obj.y / LoadedMap.TileWidth - 1) + (Origin / _tileSize)));
					entity.AddComponent(new EntityRender(ReturnSpriteFromGID(obj.gid), 0));
					entity.SetTiledProperties(obj.properties);
					EntityList.Add(entity);


			}

			return EntityList;
		}

		public void LoadAdjacentMaps() {
			List<Direction> directions = new List<Direction>() {Direction.North, Direction.East, Direction.South, Direction.West};
			foreach (Direction dir in directions) {
				TiledProperty prop = LoadedMap.Properties.FirstOrDefault(prop => prop.name == dir.ToString(), null);

				if (prop is null) continue;

				AdjacentMaps[dir] = new Map(prop.value);

				Vector2 newOrigin = Origin;
				
				// sloppy but whatever:
				switch (dir) {
					case Direction.North:
						newOrigin -= new Vector2(0, AdjacentMaps[dir].LoadedMap.Height) * _tileSize;
						break;
					case Direction.East:
						newOrigin += new Vector2(LoadedMap.Width, 0) * _tileSize;
						break;
					case Direction.South:
						newOrigin += new Vector2(0, LoadedMap.Height) * _tileSize;
						break;
					case Direction.West:
						newOrigin -= new Vector2(AdjacentMaps[dir].LoadedMap.Width, 0) * _tileSize;
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
        public void RenderMapLayer(Camera2D camera, float scale, int layer, int screenWidth, int screenHeight) {
			if (LoadedTilesets == null) LoadTextures();
			if (LoadedMap.Layers[layer].type == TiledLayerType.ObjectLayer) return;

			// Reference: https://github.com/TheBoneJarmer/TiledCS
            // foreach (TiledLayer layer in LoadedMap.Layers) {
                for (int y = 0; y < LoadedMap.Layers[layer].height; y++) {
                    for (int x = 0; x < LoadedMap.Layers[layer].width; x++) {
                        int gid = LoadedMap.Layers[layer].data[(y * LoadedMap.Layers[layer].width) + x];
						if (gid == 0) continue;
						

						Vector2 drawPos = new Vector2(x * _tileSize, y * _tileSize) + Origin;
						Vector2 screenPos = Raylib.GetWorldToScreen2D(drawPos, camera);
						if (screenPos.X + _tileSize < 0 || 
							screenPos.Y + _tileSize < 0 ||
							screenPos.X > screenWidth ||
							screenPos.Y > screenHeight) {
							continue;
						}
                        
                        // Represents the retrieved texture & rect as sprite then renders
                        ReturnSpriteFromGID(gid).Render(drawPos, new Vector2(0, 0), scale, Color.WHITE);;
                    }
                }
            // }
        }

		/// <summary>
		/// Checks if a tile is walkable or not. Checks for walkability across ALL layers.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="layer"></param>
		/// <returns>Returns true if there is a collision (i.e. tile not walkable). False otherwise.</returns>
		public bool IsTileWalkable(Vector2 tile) {
			tile -= Origin / _tileSize;

			// TODO: FIND A BETTER WAY TO DO THIS PART?
			//		EDIT: I dont think there is a better way to simplify this boolean

			// Map boundaries with no connection
			if ((tile.X < 0 && !HasMapConnection(Direction.West)) || 
				(tile.Y < 0 && !HasMapConnection(Direction.North))|| 
				(tile.X >= LoadedMap.Width && !HasMapConnection(Direction.East)) || 
				(tile.Y >= LoadedMap.Height && !HasMapConnection(Direction.South)))
				return false;
				// Map Boundaries with connection
				// 		collision function should give way for other map collision functions
			else if ((tile.X < 0 && HasMapConnection(Direction.West)) || 
				(tile.Y < 0 && HasMapConnection(Direction.North))|| 
				(tile.X >= LoadedMap.Width && HasMapConnection(Direction.East)) || 
				(tile.Y >= LoadedMap.Height && HasMapConnection(Direction.South)))
				return true;

			return !TileContainsPropertyValue(tile, "Walkable", false);
		}

		public (String, Vector2)? IsTileWarpable(Vector2 tile) {
			tile -= Origin / _tileSize;

			String mapName = ReturnFirstPropertyFromTile<String>(tile, "Warp Map", null);
			Vector2 warpCoords = new Vector2() {
				X = ReturnFirstPropertyFromTile<int>(tile, "Warp X", int.MaxValue),
				Y = ReturnFirstPropertyFromTile<int>(tile, "Warp Y", int.MaxValue)
			};

			if (mapName is null || warpCoords.X == int.MaxValue || warpCoords.Y == int.MaxValue) return null;
			else {
				return (mapName, warpCoords);
			}
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
		/// <summary>
		/// <para>Searches for the first property in a tile and returns the value. </para>
		/// Works best for properties that you know will only appear once in a given tile
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tile"></param>
		/// <param name="propertyName"></param>
		/// <param name="def">Default value if property does not exist in tile</param>
		/// <returns></returns>
		public T ReturnFirstPropertyFromTile<T>(Vector2 tile, String propertyName, T d = default) {
			// TILE CHECKS
			//--------------------------------------------------
			int idx = ((int)tile.Y * LoadedMap.Width) + (int)tile.X;

			// Checks each layer for property (rather than one layer)
			// Might not be efficient if we increase the number of layers
			//		(Will run multiple loops with each movement)
			foreach (TiledLayer mapLayer in LoadedMap.Layers) {
				if (mapLayer.type == TiledLayerType.ObjectLayer) continue;

				int gid = mapLayer.data[idx];
				if (gid == 0) continue;

                // Retrieves property; search up LINQ - https://www.tutorialsteacher.com/linq/linq-element-operator-first-firstordefault
                TiledProperty property = ReturnTileFromGID(gid).properties.FirstOrDefault((Func<TiledProperty, bool>)(prop => prop.name == propertyName), null);

				if (property is null) continue;
				else return (T)Convert.ChangeType(property.value, typeof(T));			
			}	

			// ENTITY CHECKS
			//--------------------------------------------------
			// Stupid but, entities are stored at the converted tile position (with Origin)
			List<Entity> entities = GetEntityListAtTile(tile + (Origin / _tileSize));

			foreach (Entity e in entities) {
				if (e.TiledProperties is null) continue;

				TiledProperty property = e.TiledProperties.FirstOrDefault((Func<TiledProperty, bool>)(prop => prop.name == propertyName), null);

				if (property is null) continue;
				else return (T)Convert.ChangeType(property.value, typeof(T));
			}	

			return d;
		}

		/// <summary>
		/// Checks if a tile contains a property with a certain value. Includes entities
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="tile"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns>True if tile contains values. False otherwise. No property returns false</returns>
		public bool TileContainsPropertyValue<T>(Vector2 tile, String propertyName, T value) {
			// TILE CHECKS
			//--------------------------------------------------
			int idx = ((int)tile.Y * LoadedMap.Width) + (int)tile.X;

			// Checks each layer for property (rather than one layer)
			// Might not be efficient if we increase the number of layers
			//		(Will run multiple loops with each movement)
			foreach (TiledLayer mapLayer in LoadedMap.Layers) {
				if (mapLayer.type == TiledLayerType.ObjectLayer) continue;

				TiledProperty property = null;
					int gid = mapLayer.data[idx];
					if (gid == 0) continue;

					// Retrieves property; search up LINQ - https://www.tutorialsteacher.com/linq/linq-element-operator-first-firstordefault
					property = ReturnTileFromGID(gid).properties.FirstOrDefault((Func<TiledProperty, bool>)(prop => prop.name == propertyName), null);

				if (property is null) continue;
				else {
					T a = (T)Convert.ChangeType(property.value, typeof(T));
					if (EqualityComparer<T>.Default.Equals(a, value)) {
						return true;
					}
				}		
			}

			// ENTITY CHECKS
			//--------------------------------------------------
			// Stupid but, entities are stored at the converted tile position (with Origin)
			List<Entity> entities = GetEntityListAtTile(tile + (Origin / _tileSize));

			foreach (Entity e in entities) {
				if (e.TiledProperties is null) continue;

				TiledProperty property = e.TiledProperties.FirstOrDefault((Func<TiledProperty, bool>)(prop => prop.name == propertyName), null);

				if (property is null) continue;
				else {
					T a = (T)Convert.ChangeType(property.value, typeof(T));
					if (EqualityComparer<T>.Default.Equals(a, value)) {
						return true;
					}
				}	
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
			return new Sprite(TilesetTextures[ts.Name], new Vector2(rect.x, rect.y), new Vector2(rect.width, rect.height), 0);
		}

		private static List<Entity> GetEntityListAtTile(Vector2 tile) {
			if (ETransformSystem.Components.Count == 0) return null;	
			List<ETransform> transforms = ETransformSystem.Components.Where(c => c.Tile == tile).ToList();
			List<Entity> entityList = new List<Entity>();
			foreach (ETransform t in transforms) {
				entityList.Add(t.entity);
			}

			return entityList;
		}
	}
}