//------------------------------------------------------------------------------------------
/* RENDER Queue
*/
//------------------------------------------------------------------------------------------


using Raylib_cs;
using Topdown.ECS;

namespace Topdown.Renderer {
    /// <summary>
    /// Deals with all in-game world rendering.
    /// </summary>
    public class RenderQueue {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
        // Render Layers should only be between 0 <= x < 16
        public const int MinRenderLayer = 0;
        public const int MaxRenderLayer = 16;

        public static Camera2D Camera = new Camera2D();
        
        public static void RenderAllLayers(List<Map> maps, List<ESprite> entities) {
            for (int i = MinRenderLayer; i < MaxRenderLayer; i++) {
                foreach(Map m in maps) {
                    if (i < m.LoadedMap.Layers.Length) {
                        m.RenderMapLayer(Camera, Globals.WorldScale, i, Globals.ScreenWidth, Globals.ScreenHeight);
                    }
                }

                foreach(ESprite s in entities) {
					if (s.RenderLayer == i)
                    	s.Update();
                }
            }
        }
    }
}