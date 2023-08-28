//------------------------------------------------------------------------------------------
/*  SCENELOADER
*/
//------------------------------------------------------------------------------------------


namespace Topdown {
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

    /// <summary>
    /// Generic Scene Interface. Is called upon by the Scene Loader
    /// </summary>
    public interface IScene {
        /// <summary>
        /// Runs once at the start of a scene.
        /// </summary>
        void Load();

        /// <summary>
        /// Continuously runs at every frame
        /// </summary>
        void Update();

        /// <summary>
        /// Runs once at the end of a scene
        /// </summary>
        void Unload();
    }
}