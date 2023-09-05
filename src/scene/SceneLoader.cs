//------------------------------------------------------------------------------------------
/*  SCENELOADER
*/
//------------------------------------------------------------------------------------------
namespace Topdown.Scene {
    public class SceneLoader {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public static IScene CurrentScene { get; private set; } 

        public static IScene QueuedScene { get; private set; } = null;

		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
        /// <summary>
        /// Loads the next scene. Calls Unload() of previous scene and Load() of next scene
        /// </summary>
        /// <param name="nextScene"></param>
        public static void LoadScene(IScene nextScene) {
			if (CurrentScene != null)
				CurrentScene.Unload();
            CurrentScene = nextScene;
			CurrentScene.Load();
        }

        /// <summary>
        /// Loads scene from queue. Calls Unload() of previous scene and Load() of next scene
        /// </summary>
        public static void LoadSceneFromQueue() {
			if (CurrentScene != null)
				CurrentScene.Unload();
            CurrentScene = QueuedScene;
            QueuedScene = null;

			CurrentScene.Load();
        }

        public static void QueueScene(IScene nextScene) {
            QueuedScene = nextScene;
        }

        public static void UpdateCurrentScene() {
            if (CurrentScene != null)
                CurrentScene.Update();
        }
    }
}