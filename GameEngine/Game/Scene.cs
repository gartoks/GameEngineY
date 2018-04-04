using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Modding;

namespace GameEngine.Game {
    public static class Scene {

        private static MethodInfo AddGameObjectMethodInfo;
        private static PropertyInfo UpdateThreadPropertyInfo;

        public static IEnumerable<GameObject> GameObjects => ModBase.SceneManager.ActiveScene.GameObjects;

        public static IEnumerable<GameObject> FindGameObjectsByName(string name, bool activeOnly = true) {
            return ModBase.SceneManager.ActiveScene.FindGameObjectsByName(name, activeOnly);
        }

        public static IEnumerable<GameObject> FindGameObjects(Func<GameObject, bool> selector) {
            return ModBase.SceneManager.ActiveScene.FindGameObjects(selector);
        }

        public static IEnumerable<T> FindComponents<T>(bool activeOnly = true) where T : GOC {
            return ModBase.SceneManager.ActiveScene.FindComponentsByType<T>(activeOnly);
        }

        public static Viewport MainViewport {
            get => ModBase.SceneManager.ActiveScene.MainViewport;
            set => ModBase.SceneManager.ActiveScene.MainViewport = value;
        }

        internal static void AddGameObject(IScene scene, GameObject gameObject) {
            if (AddGameObjectMethodInfo == null) {
                Type smType = ModBase.SceneManager.ActiveScene.GetType();    // TODO make sure this works
                AddGameObjectMethodInfo = smType.GetMethod("AddGameObject", BindingFlags.Instance | BindingFlags.NonPublic, null, new []{ typeof(GameObject) }, null);
            }

            AddGameObjectMethodInfo.Invoke(scene, new[] { gameObject });
        }

        internal static Thread UpdateThread {
            get {
                if (UpdateThreadPropertyInfo == null) {
                    Type smType = ModBase.App.GetType();    // TODO make sure this works
                    UpdateThreadPropertyInfo = smType.GetProperty("UpdateThread", BindingFlags.Instance | BindingFlags.Public);
                }

                return (Thread)UpdateThreadPropertyInfo.GetValue(ModBase.SceneManager);
            }
        }
    }
}