//------------------------------------------------------------------------------------------
/*  UI ENTITY
*/
//------------------------------------------------------------------------------------------
using Raylib_cs;
using Topdown.ECS;

namespace Topdown.GUI {
    /// <summary>
    /// Basic UI Entity Class that all UI elements will inherit from
    /// </summary>
    public class UIEntity {
		public bool Enabled { get; set; } = false;
		public Rectangle Rect { get; set; }

		public virtual void Render() { }

    }

	/// <summary>
	/// Holds all UIEntities similar to the ComponentSystems in the ECS.
	/// </summary>
	public class UIEntitySystem {
		private static List<UIEntity> _entityList = new List<UIEntity>();

		public static void Register(UIEntity entity) {
			_entityList.Add(entity);
		}

		public static void RenderAll() {
			foreach (UIEntity e in _entityList) {
				if (e.Enabled) e.Render();
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