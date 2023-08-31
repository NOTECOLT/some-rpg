//------------------------------------------------------------------------------------------
/*  FONT PROPERTIES
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.GUI {
    public enum VerticalAlignment {
        Top,
        Center,
        Bottom
    }
    /// <summary>
    /// A single struct to contain information about how text is stored. Includes font size, style, color, etc.
    /// </summary>
    public struct TextStyles {
        // PROPERTIES
        //------------------------------------------------------------------------------------------
        public int Size { get; set; }
        public Color Color { get; set; }
        public VerticalAlignment VerticalAlign { get; set; } = VerticalAlignment.Top;
        
        /// <summary>
        /// Ordered: (X: Left, Y: Up, Z: Right, W: Down)
        /// </summary>
        public Vector4 Margin { get; set; } = new Vector4(0, 0, 0, 0);
        public Font Font { get; set; } = Raylib.GetFontDefault();

        public TextStyles(int size, Color color) {
            Size = size;
            Color = color;
        }
    }
}