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
        
        public static void RenderAllLayers(List<Map> maps, List<EntityRender> entities) {
			// Because of how the Render Queue is set up, tiled layers need to be made in accordance to what layer the player is
			// TODO: Improve by tagging tiled layers with "render above/below entities"?
            for (int i = MinRenderLayer; i < MaxRenderLayer; i++) {
                foreach(Map m in maps) {
                    if (m is null) continue;
                    if (i < m.LoadedMap.Layers.Length)
                        m.RenderMapLayer(Camera, Globals.WORLD_SCALE, i, Globals.SCREEN_WIDTH, Globals.SCREEN_HEIGHT);
                }

                foreach(EntityRender s in entities) {
					if (s.RenderLayer == i)
                    	s.Update();
                }
            }
        }
    }
}