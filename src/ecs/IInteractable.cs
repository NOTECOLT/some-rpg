//------------------------------------------------------------------------------------------
/* ENTITY

	ECS SYSTEM - https://matthall.codes/blog/ecs/
*/
//------------------------------------------------------------------------------------------

namespace Topdown.ECS {
	/// <summary>
	/// Allows entities to be interacted with by the player
	/// </summary>
	public interface IInteractable {
		/// <summary>
		/// Triggers when player interacts with an entity
		/// </summary>
		void OnInteract() {

		}
	}
}