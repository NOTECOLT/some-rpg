//------------------------------------------------------------------------------------------
/*  UI ENTITY SYSTEM
*/
//------------------------------------------------------------------------------------------
using Raylib_cs;
using System.Numerics;

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
		/// Entities at the front of the list are rendered on top
		/// </summary>
		public static void RenderAll() {
			RenderInList(_entityList);
		}
		
		private static void RenderInList(List<UIEntity> list) {
			for (int i = list.Count - 1; i >= 0; i--) {
				if (list[i].Enabled) {
					list[i].Render();
					RenderInList(list[i].Children);
				}
			}
		}

		/// <summary>
		/// Handles mouse input checks for all ui entities
		/// </summary>
		/// <param name="mousePos"></param>
		public static void DoMouseInputAll(Vector2 mousePos) {
			DoMouseInputInList(_entityList, mousePos);
		}
		
		private static bool DoMouseInputInList(List<UIEntity> list, Vector2 mousePos) {
			foreach (UIEntity uie in list) {
				bool inChildren = DoMouseInputInList(uie.Children, mousePos);
				if (!uie.CullMouseClick) continue; // Ignore UIEntities that do not cull mouse clicks

				if (inChildren) return true;

				if (uie.Enabled && uie is IClickable) {
					IClickable ic = uie as IClickable;
					if (uie.IsMouseInRect(mousePos) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
						ic.OnMousePress(mousePos);
						Console.WriteLine($"[UIENTITY] OnMousePress Triggered for {(ic as UIEntity).Name} at position {mousePos}");
						return true;
					}

					if (uie.IsMouseInRect(mousePos) && Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
						ic.OnMouseDown(mousePos);
						// Console.WriteLine($"[UIENTITY] OnMouseDown Triggered for {(ic as UIEntity).Name} at position {mousePos}");
						return true;
					}

					if (uie.IsMouseInRect(mousePos) && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
						ic.OnMouseRelease(mousePos);
						Console.WriteLine($"[UIENTITY] OnMouseRelease Triggered for {(ic as UIEntity).Name} at position {mousePos}");
						return true;
					}
				}
			}

			return false;
		}

		public static void MoveEntityOnList(UIEntity e, int newIdx) {
			if (!_entityList.Contains(e)) return;

			_entityList.Remove(e);
			_entityList.Insert(newIdx, e);
		}

		/// <summary>
		/// Because the EntityList is a static list, it must be unloaded at the end of ever scene
		/// </summary>
		public static void Unload() {
			_entityList = new List<UIEntity>();
			
		}
	}
}