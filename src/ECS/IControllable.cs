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
		/// Main "Update" function. Will get called every frame
		/// </summary>
		void OnKeyInput() {

		}

		/// <summary>
		/// Helper function which should return true if correct input triggers have been met
		/// </summary>
		bool CheckKeys() {
			return false;
		}
	}
}