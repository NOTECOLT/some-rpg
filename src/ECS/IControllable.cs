//------------------------------------------------------------------------------------------
/* ICONTROLLABLE
*/
//------------------------------------------------------------------------------------------

namespace Topdown.ECS {
	/// <summary>
	/// Allows entities to be interacted with by the player
	/// </summary>
	public interface IControllable {
		/// <summary>
		/// Triggers when player wants to move an entity
		/// </summary>
		void OnKeyInput() {

		}
	}
}