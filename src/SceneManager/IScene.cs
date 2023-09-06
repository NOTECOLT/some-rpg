//------------------------------------------------------------------------------------------
/*  ISCENE
*/
//------------------------------------------------------------------------------------------
namespace Topdown.SceneManager {
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