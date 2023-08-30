//------------------------------------------------------------------------------------------
/*  UI ENTITY
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.ComponentModel;
using Raylib_cs;
using Topdown.ECS;

namespace Topdown.GUI {
    /// <summary>
    /// Basic UI Entity Class that all UI elements will inherit from.
	/// Consists of a rectangle and background color
    /// </summary>
    public class UIEntity {
		// FIELDS
		//------------------------------------------------------------------------------------------
		protected Rectangle _absoluteRect;		// Absolute Rect of the UIEntity, (rect of its parents computed)

		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public bool Enabled { get; set; } = true;
		public Rectangle Rect { get; set; }		// Rectangle of the UIEntity relative to any parents
		public UIEntity Parent { get; set; } = null;
		public List<UIEntity> Children { get; set; } = new List<UIEntity>(); 
		public Color? BGColor { get; protected set; } = null;

		/// <summary>
		/// Regular Constructor
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="bgColor"></param>
		public UIEntity(Vector2 pos, Vector2 size, Color? bgColor) {
			_absoluteRect = Rect = new Rectangle(pos.X, pos.Y, size.X, size.Y);
			BGColor = bgColor;

            UIEntitySystem.Register(this);
		}

		// /// <summary>
		// /// With Parent Constructor
		// /// </summary>
		// /// <param name="parent"></param>
		// /// <param name="pos"></param>
		// /// <param name="size"></param>
		// /// <param name="bgColor"></param>
		// public UIEntity(UIEntity parent, Vector2 pos, Vector2 size, Color? bgColor) {
		// 	_absoluteRect = Rect = new Rectangle(pos.X, pos.Y, size.X, size.Y);
		// 	BGColor = bgColor;
		// 	SetParent(parent);
		// }

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public virtual void Render() { 
			if (BGColor != null) // null coalescing operator at the end will never trigger anyway
				Raylib.DrawRectangle((int)_absoluteRect.x, (int)_absoluteRect.y, (int)_absoluteRect.width, (int)_absoluteRect.height, BGColor ?? Color.WHITE);
		}
		
		/// <summary>
		/// Sets the parent of an object.
		/// </summary>
		/// <param name="newParent"></param>
		public void SetParent(UIEntity newParent) {
			if (Parent is not null) {
				Parent.Children.Remove(this);
				Console.WriteLine($"Removed Entity {this} from parent {Parent}");
			} else {
				UIEntitySystem.RemoveEntity(this);
				Console.WriteLine($"Removed Entity {this} from entitylist");
			}

			newParent.Children.Add(this);
			Parent = newParent;

			UIEntity p = Parent;
			while (p is not null) {
				_absoluteRect.x += p.Rect.x;
				_absoluteRect.y += p.Rect.y;
				p = p.Parent;
			}
		}
    }
}