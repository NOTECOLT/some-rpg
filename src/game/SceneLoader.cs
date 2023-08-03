//------------------------------------------------------------------------------------------
/*  SCENELOADER
*/
//------------------------------------------------------------------------------------------


namespace Topdown.Engine {
    public class SceneLoader {
		// FIELDS
		//------------------------------------------------------------------------------------------
        private IScene _currentScene = null;

		// PROPERTIES
		//------------------------------------------------------------------------------------------
        
		public IScene CurrentScene { get { return _currentScene; } } 

		// STATIC FUNCTIONS
		//------------------------------------------------------------------------------------------
        /// <summary>
        /// Loads the next scene. Calls Unload() of previous scene and Load() of next scene
        /// </summary>
        /// <param name="nextScene"></param>
        public void LoadScene(IScene nextScene) {
			if (_currentScene != null)
				_currentScene.Unload();
			_currentScene = nextScene;
			_currentScene.Load();
        }

        public void UpdateCurrentScene() {
            if (_currentScene != null)
                _currentScene.Update();
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