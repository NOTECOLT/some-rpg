//------------------------------------------------------------------------------------------
/* COMPONENT
*/
//------------------------------------------------------------------------------------------

namespace Topdown.ECS {
	/// <summary>
	/// Components may be added moduarly to different entities
	/// </summary>
	public class Component {
		public Entity entity;

		public virtual void Update() {
			
		}
	}


}