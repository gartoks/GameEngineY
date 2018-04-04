namespace GameEngine.Game {
    public interface ISceneManager {
        /// <summary>
        /// Loads the scene.
        /// </summary>
        /// <param name="sceneName">Name of the scene.</param>
        void LoadScene(string sceneName);

        /// <summary>
        /// Gets or sets the active scene. Must not be null.
        /// </summary>
        /// <value>
        /// The active scene.
        /// </value>
        IScene ActiveScene { get; }
    }
}