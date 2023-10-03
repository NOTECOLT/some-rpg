//------------------------------------------------------------------------------------------
/* PLAYER DATA
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.Xml;
using Raylib_cs;

namespace Topdown {
    public class PlayerData {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
        public String Name { get; set; } = "";
        public String Map { get; set; } = "";

        /// <summary>
        /// Tile saved is relative to the current map's origin (calculated upon loading)
        /// </summary>
        public Vector2 Tile { get; set; } = Vector2.Zero;
        public String FilePath { get; }

        public Dictionary<String, bool> Flags = new Dictionary<string, bool>();
        public PlayerData(string path) {
           Load(path); 
            FilePath = path;
        }

        public PlayerData() { }

		// PUBLIC FUNCTIONS
		//------------------------------------------------------------------------------------------
        public void Load(string path) {
            // if (!File.Exists(path)) {
            //     File.Create(path);
            //     SavePlayerData(path);
            //     return;
            // }
            
            // 1 - FILE READ
			//--------------------------------------------------
			string fileText = File.ReadAllText(path);
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fileText);

			// 2 - XML PARSE
			//--------------------------------------------------
            XmlElement root = xml.DocumentElement;

            foreach (XmlElement node in root) {
                switch (node.Name) {
                    case "name":
                        Name = node.InnerText;
                        Console.WriteLine($"[PLAYER DATA] [Profile {Name}] Loading Player data from {path}");
                        break;
                    case "savedPos":
                        ParseSavedPosition(node);
                        Console.WriteLine($"[PLAYER DATA] [Profile {Name}] Loading player to map {Map} on tile {Tile}");
                        break;
                    case "flags":
                        ParseFlags(node);
                        break;
                    default:
                        break;
                }
            }

            
        }

        public void Save(string path) {
            // if (!File.Exists(path)) {
            //     File.Create(path);
            // }

            XmlDocument xml = new XmlDocument();
            XmlElement playerData = xml.CreateElement("playerdata");

            // NAME
			//--------------------------------------------------
            Console.WriteLine($"[PLAYER DATA] [Profile {Name}] Saving Player data to {path}");
            XmlElement name = xml.CreateElement("name");
            name.InnerText = Name;
            playerData.AppendChild(name);

            // SAVED POSITION
			//--------------------------------------------------
            Console.WriteLine($"[PLAYER DATA] [Profile {Name}] saving player to map {Map} on tile {Tile}");
            XmlElement savedPos = xml.CreateElement("savedPos");

            XmlElement map = xml.CreateElement("map");
            map.InnerText = Map;
            savedPos.AppendChild(map);

            XmlElement tile = xml.CreateElement("tile");
                XmlElement x = xml.CreateElement("x");
                x.InnerText = Convert.ToString(Tile.X);
                tile.AppendChild(x);
                XmlElement y = xml.CreateElement("y");
                y.InnerText = Convert.ToString(Tile.Y);
                tile.AppendChild(y);
                savedPos.AppendChild(tile);
            playerData.AppendChild(savedPos);

            // FLAGS
			//--------------------------------------------------
            XmlElement flags = xml.CreateElement("flags");
            foreach (KeyValuePair<String, bool> flag in Flags) {
                XmlElement flagElement = xml.CreateElement("flag");

                XmlElement nameTag = xml.CreateElement("name");
                nameTag.InnerText = flag.Key;
                flagElement.AppendChild(nameTag);

                XmlElement statusTag = xml.CreateElement("status");
                statusTag.InnerText = Convert.ToString(flag.Value);
                flagElement.AppendChild(statusTag);

                flags.AppendChild(flagElement);
            }
            playerData.AppendChild(flags);

            // Console.WriteLine(playerData.OuterXml);
            File.WriteAllText(path, playerData.OuterXml);
            
        }

        /// <summary>
        /// Resets a player data to the default
        /// </summary>
        public void ResetToDefault() {
            Map = "Test Map 1";
            Name = "Steve";
            Tile = new Vector2(13, 16);
        }
        
		// PRIVATE FUNCTIONS
		//------------------------------------------------------------------------------------------ 
        private void ParseSavedPosition(XmlElement root) {
            foreach (XmlElement node in root) {
                switch (node.Name) {
                    case "map":
                        Map = node.InnerText;
                        break;
                    case "tile":
                        int x = Int32.Parse(node.FirstChild.InnerText);
                        int y = Int32.Parse(node.LastChild.InnerText);

                        Tile = new Vector2(x, y);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ParseFlags(XmlElement root) {
            foreach (XmlElement flag in root) {
                String name = ""; bool status = false;
                foreach (XmlElement node in flag) {
                    switch (node.Name) {
                        case "name":
                            name = node.InnerText;
                            break;
                        case "status":
                            status = Convert.ToBoolean(node.InnerText);
                            break;
                        default:
                            break;
                    }
                }

                if (name != "")
                    Flags[name] = status;
            }
        }


    }
}