//------------------------------------------------------------------------------------------
/* COMPONENT SYSTEM

	ECS SYSTEM - https://matthall.codes/blog/ecs/
*/
//------------------------------------------------------------------------------------------

using System.Diagnostics.Contracts;

namespace Topdown.ECS {
    public class ComponentSystem<T> where T : Component {
		// STATIC FIELDS
		//------------------------------------------------------------------------------------------
        protected static List<T> _components = new List<T>();

        public static List<T> Components { get { return _components; } set { _components = value; } }
        
        public static void Register(T component) {
            _components.Add(component);
        }

        public static void Update() {
            foreach (T c in _components) {
                if (c.Enabled) c.Update();
            }
        }

        /// <summary>
        /// Because these systems use static lsits, they must be unloaded at the end of each scene.
        /// </summary>
        public static void Unload() {
            _components = new List<T>();
        }
    }   

    // SYSTEMS
    // Systems only exist for components that need to be updated constantly
    //------------------------------------------------------------------------------------------
    public class TileTransformSystem : ComponentSystem<TileTransform> { }
    public class EntityRenderSystem : ComponentSystem<EntityRender> { }
    // public class EDialogueSystem : ComponentSystem<EDialogue> { }
}