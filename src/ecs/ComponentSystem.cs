//------------------------------------------------------------------------------------------
/* COMPONENT SYSTEM
*/
//------------------------------------------------------------------------------------------

namespace Topdown.ECS {
    public class ComponentSystem<T> where T : Component {
		// STATIC FIELDS
		//------------------------------------------------------------------------------------------
        protected static List<T> _components = new List<T>();
        
        public static void Register(T component) {
            _components.Add(component);
        }

        public static void Update() {
            foreach (T c in _components) {
                c.Update();
            }
        }
    }   

    // SYSTEMS
    // Systems only exist for components that need to be updated constantly
    //------------------------------------------------------------------------------------------
    public class ETransformSystem : ComponentSystem<ETransform> { }
    public class ESpriteSystem : ComponentSystem<ESprite> { }
}