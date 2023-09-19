//------------------------------------------------------------------------------------------
/* PLAYER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Topdown.ECS;
using Raylib_cs;
using System.Runtime.CompilerServices;
using Topdown.Renderer;

namespace Topdown {
    public class Player : Entity, IControllable {
        private const int PlayerWalkSpeed = 200;
        private const int PlayerRunSpeed = 300;
        
        public Player(Vector2 tile) {
            TileTransform transform = new TileTransform(tile, PlayerWalkSpeed, PlayerRunSpeed);
            // ESprite sprite = new ESprite(, new Vector2(18, 22), new Vector2(13, 21), 1);
			Spritesheet playerSheet = new Spritesheet("resources/sprites/characters/player.png", 
												new Vector2(17, 20), 
												new Vector2(14, 23), 
												new Vector2(34, 25),
												1, 0);
			playerSheet.SetFPS(6);
			playerSheet.AnimDictionary["IdleSouth"] = new Animation {
				StartFrame = 0, FrameCount = 5 };
			playerSheet.AnimDictionary["IdleSide"] = new Animation {
				StartFrame = 6, FrameCount = 5 };
			playerSheet.AnimDictionary["IdleNorth"] = new Animation {
				StartFrame = 12, FrameCount = 5 };
			playerSheet.AnimDictionary["WalkSouth"] = new Animation {
				StartFrame = 18, FrameCount = 5 };
			playerSheet.AnimDictionary["WalkSide"] = new Animation {
				StartFrame = 24, FrameCount = 5 };
			playerSheet.AnimDictionary["WalkNorth"] = new Animation {
				StartFrame = 30, FrameCount = 5 };


			// playerSheet.SetAnimation("IdleSouth");
			EntityRender sprite = new EntityRender(playerSheet, 1);
            AddComponent(transform);
            AddComponent(sprite);
        }
		
        public void OnKeyInput() {
            TileTransform transform = GetComponent<TileTransform>();

			if (!transform.IsMoving) {
				CheckKeys();
			}

			// This shouldn't be here but i'll keep this here for now
			// TODO: MOVE THIS SOMEWHERE BETTER
			if (GetComponent<EntityRender>().RenderObject is Spritesheet) {
				Spritesheet ss = GetComponent<EntityRender>().RenderObject as Spritesheet;

				if (transform.IsRunning && transform.IsMoving)
					ss.SetFPS(15);
				else
					ss.SetFPS(6);

				if (!transform.IsMoving) {
					switch(GetComponent<TileTransform>().Facing) {
						case Direction.North:
							ss.SetAnimation("IdleNorth");
							break;
						case Direction.South:
							ss.SetAnimation("IdleSouth");	
							break;
						case Direction.East:
							ss.SetAnimation("IdleSide");
							ss.FlipX = false;
							break;
						case Direction.West:
							ss.SetAnimation("IdleSide");
							ss.FlipX = true;
							break;
					}
				} else {
					switch(GetComponent<TileTransform>().Facing) {
						case Direction.North:
							ss.SetAnimation("WalkNorth");
							break;
						case Direction.South:
							ss.SetAnimation("WalkSouth");
							break;
						case Direction.East:
							ss.SetAnimation("WalkSide");
							ss.FlipX = false;
							break;
						case Direction.West:
							ss.SetAnimation("WalkSide");
							ss.FlipX = true;
							break;
					}
				}
			}

        }

		public bool CheckKeys() {
			TileTransform transform = GetComponent<TileTransform>();

			transform.IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

			if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) {
				transform.ChangeDirection(Direction.East);
				transform.TargetTile = transform.Tile + Vector2.UnitX;
				return true;
			} else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) {
				transform.ChangeDirection(Direction.West);
				transform.TargetTile = transform.Tile - Vector2.UnitX;
				return true;
			} else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) {
				transform.ChangeDirection(Direction.South);
				transform.TargetTile = transform.Tile + Vector2.UnitY;	
				return true;	
			} else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) {
				transform.ChangeDirection(Direction.North);
				transform.TargetTile = transform.Tile - Vector2.UnitY;
				return true;
			}
			return false;
		}
    }
}