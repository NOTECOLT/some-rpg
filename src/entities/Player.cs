//------------------------------------------------------------------------------------------
/* PLAYER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Topdown.ECS;

namespace Topdown {
    public class Player : Entity {
        private const int PLAYER_WALKSPEED = 200;
        private const int PLAYER_RUNSPEED = 300;
        
        public Player(Vector2 tile) {
            ETransform transform = new ETransform(tile, PLAYER_WALKSPEED, PLAYER_RUNSPEED, Globals.ScaledTileSize);
            ESprite sprite = new ESprite("resources/sprites/characters/player.png", 1);

            AddComponent(transform);
            AddComponent(sprite);
        }
    }
}