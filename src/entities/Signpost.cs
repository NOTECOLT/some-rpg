//------------------------------------------------------------------------------------------
/* PLAYER
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using Topdown.ECS;
using Topdown.Renderer;
using Topdown.DialogueSystem;

namespace Topdown {
    public class Signpost : Entity, IInteractable {
        
        public Signpost(Vector2 tile, Dialogue dialogue, Sprite sprite) {
            TileTransform transform = new TileTransform(tile);

            AddComponent(transform);
            AddComponent(new EntityRender(sprite, 0));
			AddComponent(new EDialogue(dialogue));
        }

		public void OnInteract() {
			GetComponent<EDialogue>().StartDialogue();				
		}
    }
}