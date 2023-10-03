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
		

		/// <summary>
		/// Battle Scene must be initialized with ____
		/// </summary>
        public BattleScene() {
			// if (!Map.MapDictionary.ContainsKey(name)) {
			// 	throw new Exception($"Map {name} not found in Map Dictionary!");
			// }

            RenderQueue.Camera = new Camera2D() {
				rotation = 0,
				zoom = 1
			};
			// _loadedMaps.Add(new Map(name, Vector2.Zero));
			// _startingTile = startingTile;
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
			// LoadMap(_loadedMaps[0]);

            // 3 - PLAYER LOADING
            //--------------------------------------------------
			// _player = new Player(_startingTile);			
		}

        public void Update() {
            // 1 - INPUT
            //--------------------------------------------------

		
			// 2 - PHYSICS
			//--------------------------------------------------


			TileTransformSystem.Update();

			// 3 - RENDERING
			//--------------------------------------------------

			Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.BLACK);
            	// RenderQueue.Camera.target = new Vector2(playerT.Position.X + (Globals.ScaledTileSize), playerT.Position.Y + (Globals.ScaledTileSize));
				// RenderQueue.Camera.offset = new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2);
				Raylib.BeginMode2D(RenderQueue.Camera);
					// RenderQueue.RenderAllLayers(_loadedMaps, ESpriteSystem.Components);
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
			TileTransformSystem.Unload();

			UIEntitySystem.Unload();

			Map.UnloadAllTextures();
        }

		// FUNCTIONS
        //------------------------------------------------------------------------------------------

    }
}