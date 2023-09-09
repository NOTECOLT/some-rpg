//------------------------------------------------------------------------------------------
/* IRenderable
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.Renderer {
    public interface IRenderable {
        Vector2 Size { get; }
        
        void Render(Vector2 position, Vector2 offset, float scale, Color color) { }
    }
}