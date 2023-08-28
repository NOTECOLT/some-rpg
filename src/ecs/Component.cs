//------------------------------------------------------------------------------------------
/* COMPONENT
*/
//------------------------------------------------------------------------------------------

namespace Topdown.ECS {
	/// <summary>
	/// <para> This class provides a basic framework that component classes may inherit from </para>
	/// Components may be added moduarly to different entities.
	/// </summary>
	public class Component {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public Entity entity;
		public bool Enabled = true;

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public virtual void Update() {
			if (!Enabled) return;
		}
	}


}