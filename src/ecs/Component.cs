//------------------------------------------------------------------------------------------
/* COMPONENT

	ECS SYSTEM - https://matthall.codes/blog/ecs/
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
		public virtual void Destroy() { }
		public virtual void Update() { }
	}


}