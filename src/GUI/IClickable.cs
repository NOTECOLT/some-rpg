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
		/// Triggers when player presses the mouse on a UIEntity
		/// </summary>
		void OnMousePress(Vector2 mousePos) { }

		/// <summary>
		/// Triggers when player release the mouse on a UIEntity
		/// </summary>
		void OnMouseRelease(Vector2 mousePos) { }

		/// <summary>
		/// Triggers when player mouse input is down on a UIEntity
		/// </summary>
		void OnMouseDown(Vector2 mousePos) { }
	}
}