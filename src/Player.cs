//------------------------------------------------------------------------------------------
/* PLAYER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Topdown.ECS;

namespace Topdown {
    public class Player : Entity {
        public Player(ETransform Transform, ESprite Sprite) {
            AddComponent(Transform);
            AddComponent(Sprite);
        }
    }
}