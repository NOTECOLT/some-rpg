//------------------------------------------------------------------------------------------
/* ENTITY

	ECS SYSTEM - https://matthall.codes/blog/ecs/
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.ECS {
	/// <summary>
	/// <para>An Entity is any interactable/playable object in the game. </para>
	/// Components may be added to the list as needed.
	/// </summary>
	public class Entity {
		public List<Component> Components { get; private set; } = new List<Component>();

		public void AddComponent(Component component) {
			component.entity = this;
			Components.Add(component);
		}

		public T GetComponent<T>() where T : Component {
			foreach (Component c in Components) {
				if (c.GetType().Equals(typeof(T))) {
					return (T)c;
				}
			}
			return null;
		}
	}
}

