using System;
using System.Collections.Generic;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Modding;

namespace GameEngine.Game {
    public static class Scene {

        /// <summary>
        /// Creates and loads the scene.
        /// </summary>
        /// <param name="sceneName">Name of the scene.</param>
        public static void LoadScene(string sceneName) {
            if (!ModBase.IsBaseMod) {
                Log.WriteLine("Loading a scene is only permitted by the base mod.");
                return;
            }

            ModBase.SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Creates a new GameObject and adds it to the scene.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IGameObject CreateGameObject(string name)
            => ModBase.SceneManager.ActiveScene.CreateGameObject(name, null, null, 0, null);


        /// <summary>
        /// Creates a new GameObject and adds it to the scene.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public static IGameObject CreateGameObject(string name, Vector2 position = null, float rotation = 0, Vector2 scale = null)
            => ModBase.SceneManager.ActiveScene.CreateGameObject(name, null, position, rotation, scale);

        /// <summary>
        /// Creates a new GameObject and adds it to the scene.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public static IGameObject CreateGameObject(string name, IGameObject parent = null, Vector2 position = null, float rotation = 0, Vector2 scale = null)
            => ModBase.SceneManager.ActiveScene.CreateGameObject(name, parent, position, rotation, scale);

        public static IEnumerable<IGameObject> GameObjects => ModBase.SceneManager.ActiveScene.GameObjects;

        public static IEnumerable<IGameObject> FindGameObjectsByName(string name, bool activeOnly = true) => ModBase.SceneManager.ActiveScene.FindGameObjectsByName(name, activeOnly);

        public static IEnumerable<IGameObject> FindGameObjects(Func<IGameObject, bool> selector) => ModBase.SceneManager.ActiveScene.FindGameObjects(selector);

        public static IEnumerable<T> FindComponents<T>(bool activeOnly = true) where T : GOC => ModBase.SceneManager.ActiveScene.FindComponentsByType<T>(activeOnly);

        public static T FindComponentByType<T>(bool activeOnly = true) where T : GOC => ModBase.SceneManager.ActiveScene.FindComponentByType<T>();

        public static Viewport MainViewport {
            get => ModBase.SceneManager.ActiveScene.MainViewport;
            set => ModBase.SceneManager.ActiveScene.MainViewport = value;
        }
    }
}