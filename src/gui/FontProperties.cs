//------------------------------------------------------------------------------------------
/*  FONT PROPERTIES
*/
//------------------------------------------------------------------------------------------
using Raylib_cs;

namespace Topdown.GUI {
    /// <summary>
    /// A single struct to contain information about how text is stored. Includes font size, style, color, etc.
    /// </summary>
    public struct FontProperties {
        // PROPERTIES
        //------------------------------------------------------------------------------------------
        public int Size { get; set; }
        public Color Color { get; set; }

        public FontProperties(int size, Color color) {
            Size = size;
            Color = color;
        }
    }
}