//------------------------------------------------------------------------------------------
/* PLAYER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Topdown.ECS;
using Raylib_cs;

namespace Topdown {
    public class Player : Entity, IControllable {
        private const int PlayerWalkSpeed = 200;
        private const int PlayerRunSpeed = 300;
        
        public Player(Vector2 tile) {
            ETransform transform = new ETransform(tile, PlayerWalkSpeed, PlayerRunSpeed);
            ESprite sprite = new ESprite("resources/sprites/characters/player.png", 1);

            AddComponent(transform);
            AddComponent(sprite);
        }

        public void OnKeyInput() {
            ETransform transform = GetComponent<ETransform>();

			if (!transform.IsMoving) {
				transform.IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

				if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) {
					transform.ChangeDirection(Direction.East);
					transform.TargetTile = transform.Tile + Vector2.UnitX;
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) {
					transform.ChangeDirection(Direction.West);
					transform.TargetTile = transform.Tile - Vector2.UnitX;
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) {
					transform.ChangeDirection(Direction.South);
					transform.TargetTile = transform.Tile + Vector2.UnitY;
				} else if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) {
					transform.ChangeDirection(Direction.North);
					transform.TargetTile = transform.Tile - Vector2.UnitY;
				} else {
					// playerT.TargetTile = new Vector2(playerT.Tile.X, playerT.Tile.Y);
				}	
			}
        }
    }
}