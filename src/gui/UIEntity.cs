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
		protected Rectangle _relativeRect;		// Relative Rect, relative to its parent
		protected Alignment _verticalAlign = Alignment.None;
		protected Alignment _horizontalAlign = Alignment.None;
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public bool Enabled { get; set; } = true;
		public UIEntity Parent { get; protected set; } = null;
		public List<UIEntity> Children { get; protected set; } = new List<UIEntity>();

		/// <summary>
		/// Position and Size of UIEntity relative to its parent.
		/// </summary>
		public Rectangle RelativeRect { 
			get { return _relativeRect; }
			set { 
				_relativeRect = value;
				UIEntity p = Parent;
				while (p is not null) {
					_absoluteRect.x += p.RelativeRect.x;
					_absoluteRect.y += p.RelativeRect.y;
					p = p.Parent;
				}	
			} 
		}	

		public Alignment VerticalAlign { 
			get { return _verticalAlign; } 
			set { 
				_verticalAlign = value;

				// When not set to none, position will ignore relative rects
				switch (_verticalAlign) {
					case Alignment.None:
						UIEntity p = Parent;
						while (p is not null) {
							_absoluteRect.x += p.RelativeRect.x;
							_absoluteRect.y += p.RelativeRect.y;
							p = p.Parent;
						}	
						break;
					case Alignment.Center:
						if (Parent is null)
							_absoluteRect.y = (Globals.SCREEN_HEIGHT - _absoluteRect.height) / 2;
						else
							_absoluteRect.y = (Parent._absoluteRect.height - _absoluteRect.height) / 2;
						break;
					default:
						break;
				}
			} 
		}
		public Alignment HorizontalAlign { 
			get { return _horizontalAlign; } 
			set {
				_horizontalAlign = value;

				// When not set to none, position will ignore relative rects
				switch (_horizontalAlign) {
					case Alignment.None:
						UIEntity p = Parent;
						while (p is not null) {
							_absoluteRect.x += p.RelativeRect.x;
							_absoluteRect.y += p.RelativeRect.y;
							p = p.Parent;
						}	
						break;
					case Alignment.Center:
						if (Parent is null)
							_absoluteRect.x = (Globals.SCREEN_WIDTH - _absoluteRect.width) / 2;
						else
							_absoluteRect.x = (Parent._absoluteRect.width - _absoluteRect.width) / 2;
						break;
					default:
						break;
				}
			} 
		} 

		public Color? BGColor { get; protected set; } = null;

		/// <summary>
		/// Regular Constructor
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="bgColor"></param>
		public UIEntity(Vector2 pos, Vector2 size, Color? bgColor) {
			_absoluteRect = RelativeRect = new Rectangle(pos.X, pos.Y, size.X, size.Y);
			BGColor = bgColor;

            UIEntitySystem.Register(this);
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public virtual void Render() { 
			if (BGColor == null) return; // null coalescing operator at the end will never trigger anyway


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
				_absoluteRect.x += p.RelativeRect.x;
				_absoluteRect.y += p.RelativeRect.y;
				p = p.Parent;
			}
		}

		/// <summary>
		/// Moves UI Element to the top of the render list among its siblings
		/// </summary>
		public void MoveToTop() {
			if (Parent is null) {
				UIEntitySystem.MoveEntityOnList(this, 0);
			} else {
				Parent.Children.Remove(this);
				Parent.Children.Insert(0, this);
			}
		}
    }
}