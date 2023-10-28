//------------------------------------------------------------------------------------------
/* OVERWORLD SCENE
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
using Topdown.ECS;
using Topdown.GUI;
using Topdown.Renderer;
using Topdown.SceneManager;
using Topdown.DialogueSystem;

namespace Topdown {
	/// <summary>
	/// Scene for when the player is in battle.
	/// </summary>
    public class BattleScene : IScene {
		private DialogueManager _dialogueManager;
		
		private UICard _testEntity;
		private List<Card> _cardDeck = new List<Card>();
		

		/// <summary>
		/// Battle Scene must be initialized with ____
		/// </summary>
        public BattleScene() {
            RenderQueue.Camera = new Camera2D() {
				rotation = 0,
				zoom = 1
			}; 
        }

		// SCENE FUNCTIONS
        //------------------------------------------------------------------------------------------
        public void Load() {
			// 1 - SYSTEM LOADING
			//--------------------------------------------------
			// most DialogueManager functions uses static objects, but an object is needed for the UI elements
			_dialogueManager = new DialogueManager();

			// 2 - MAP LOADING
			//--------------------------------------------------

            // 3 - PLAYER LOADING
            //--------------------------------------------------
			
			_testEntity = new UICard(new Vector2(10, 270), PlayerData.Cards["Test Card"]);	
		}

        public void Update() {
            // 1 - INPUT
            //--------------------------------------------------

			if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT) ||
				Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) ||
				Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
				UIEntitySystem.DoMouseInputAll(Raylib.GetMousePosition());
			}
			
			// 2 - PHYSICS
			//--------------------------------------------------

			EntityTransformSystem.Update();

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.BLACK);
            	RenderQueue.Camera.target = Vector2.Zero;
				RenderQueue.Camera.offset = new Vector2(Globals.SCREEN_WIDTH / 2, Globals.SCREEN_HEIGHT / 2);
				Raylib.BeginMode2D(RenderQueue.Camera);
					RenderQueue.RenderAllLayers(null, EntityRenderSystem.Components);
				Raylib.EndMode2D();

				// UI
				//--------------------------------------------------

				Raylib.DrawText($"fps: {Raylib.GetFPS()}; Frame Time:{Raylib.GetFrameTime()}", 5, 5, 30, Color.RAYWHITE);
				Raylib.DrawText($"Mode: BATTLE", 5, 40, 30, Color.RAYWHITE);

				UIEntitySystem.RenderAll();
				_dialogueManager.UpdateDialogueBox();

			Raylib.EndDrawing();
        }

        public void Unload() {
			// Unloading Component Systems once the scene ends because the list is static
			EntityRenderSystem.Unload();
			EntityTransformSystem.Unload();

			UIEntitySystem.Unload();

			Map.UnloadAllTextures();
        }

		// FUNCTIONS
        //------------------------------------------------------------------------------------------

    }
}