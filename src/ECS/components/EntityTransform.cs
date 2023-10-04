//------------------------------------------------------------------------------------------
/* WORLD TRANSFORM
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Raylib_cs;

namespace Topdown.ECS {
	/// <summary>
	/// Allows the entity to exist within the game world
	/// </summary>
	public class EntityTransform : Component {
		// FIELDS
		//------------------------------------------------------------------------------------------            

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		/// <summary>
		/// Refers to the position of the player with respect to the screen / global coordinate system  
		/// </summary>
		public Vector2 Position { get; protected set; }

        public EntityTransform(Vector2 position) {
			Position = position; 
			
			EntityTransformSystem.Register(this);
        }

        public override void Destroy() {
            EntityTransformSystem.Components.Remove(this);
        }

        public override void Update() {
			
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		
		/// <summary>
		/// Renders all debug info relating an entity
		/// </summary>
		public virtual void DrawEntityDebugText(Vector2 pos) {
			if (!Enabled) return;

			Raylib.DrawText($"position: ({Position.X}, {Position.Y})", (int)pos.X, (int)pos.Y, 30, Color.RAYWHITE);
			// Raylib.DrawText($"targetTile: ({TargetTile.X}, {TargetTile.Y})", (int)pos.X, (int)pos.Y + 70, 30, Color.WHITE);
		}
	}


}