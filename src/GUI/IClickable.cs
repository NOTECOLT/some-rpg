//------------------------------------------------------------------------------------------
/* ICLICKABLE
*/
//------------------------------------------------------------------------------------------

using System.Numerics;

namespace Topdown.GUI {
	/// <summary>
	/// Allows entities to be cliked on by the player
	/// </summary>
	public interface IClickable {
		/// <summary>
		/// Triggers when player clicks on a UIEntity
		/// </summary>
		bool OnClick(Vector2 mousePos) {
            return false;
		}
	}
}