//------------------------------------------------------------------------------------------
/* PLAYER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Topdown.ECS;

namespace Topdown {
    public class Signpost : Entity, IInteractable {
        
        public Signpost(Vector2 tilePos, Dialogue dialogue, Sprite sprite) {
            ETransform transform = new ETransform(tilePos, 0, 0, Globals.TILE_SIZE);

            AddComponent(transform);
            AddComponent(new ESprite(sprite, 0));
			AddComponent(new EDialogue(dialogue));
        }

		public void OnInteract() {
			GetComponent<EDialogue>().StartDialogue();				
		}
    }
}