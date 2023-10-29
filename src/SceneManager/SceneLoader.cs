//------------------------------------------------------------------------------------------
/*  SCENELOADER
*/
//------------------------------------------------------------------------------------------
namespace Topdown.SceneManager {
    public class SceneLoader {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		private IScene _currentScene; 

        private IScene _queuedScene = null;

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
        public bool isSceneQueued() {
            return _queuedScene is not null;
        } 
        
        /// <summary>
        /// Loads the next scene. Calls Unload() of previous scene and Load() of next scene
        /// </summary>
        /// <param name="nextScene"></param>
        public void LoadScene(IScene nextScene, PlayerData playerData) {
			if (_currentScene != null)
				_currentScene.Unload();
            _currentScene = nextScene;
			_currentScene.Load(playerData, this);
        }

        /// <summary>
        /// Loads scene from queue. Calls Unload() of previous scene and Load() of next scene
        /// </summary>
        public void LoadSceneFromQueue(PlayerData playerData) {
			if (_currentScene != null)
				_currentScene.Unload();
            _currentScene = _queuedScene;
            _queuedScene = null;

			_currentScene.Load(playerData, this);
        }

        public void QueueScene(IScene nextScene) {
            _queuedScene = nextScene;
        }

        public void UpdateCurrentScene() {
            if (_currentScene != null)
                _currentScene.Update();
        }
    }
}