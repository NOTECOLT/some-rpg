//------------------------------------------------------------------------------------------
/*  SCENELOADER
*/
//------------------------------------------------------------------------------------------
namespace Topdown.Scene {
    public class SceneLoader {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public IScene CurrentScene { get; private set; } 

		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
        /// <summary>
        /// Loads the next scene. Calls Unload() of previous scene and Load() of next scene
        /// </summary>
        /// <param name="nextScene"></param>
        public void LoadScene(IScene nextScene) {
			if (CurrentScene != null)
				CurrentScene.Unload();
            CurrentScene = nextScene;
			CurrentScene.Load();
        }

        public void UpdateCurrentScene() {
            if (CurrentScene != null)
                CurrentScene.Update();
        }
    }

}