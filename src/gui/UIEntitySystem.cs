//------------------------------------------------------------------------------------------
/*  UI ENTITY SYSTEM
*/
//------------------------------------------------------------------------------------------

namespace Topdown.GUI {
	/// <summary>
	/// Holds all UIEntities similar to the ComponentSystems in the ECS.
	/// </summary>
	public class UIEntitySystem {
		/// <summary>
		/// EntityList must contain only parent Elements
		/// </summary>
		private static List<UIEntity> _entityList = new List<UIEntity>();

		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
		public static void Register(UIEntity entity) {
			_entityList.Add(entity);
		}

		public static void RemoveEntity(UIEntity entity) {
			_entityList.Remove(entity);
		}

		/// <summary>
		/// Renders all UIEntities in pre-order traversal (V-L-R)
		/// </summary>
		public static void RenderAll() {
			RenderInList(_entityList);
		}

		private static void RenderInList(List<UIEntity> list) {
			foreach (UIEntity e in list) {
				if (e.Enabled) {
					e.Render();
					RenderInList(e.Children);
				}
			}
		}

		/// <summary>
		/// Because the EntityList is a static list, it must be unloaded at the end of ever scene
		/// </summary>
		public static void Unload() {
			_entityList = new List<UIEntity>();
		}
	}
}