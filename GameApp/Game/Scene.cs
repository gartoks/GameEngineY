using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using GameEngine.Logging;
using Log = GameApp.Logging.Log;
using System.Linq;
using GameEngine.Game;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;

namespace GameApp.Game {
    public sealed class Scene : IScene {
        static Scene() {
            GOUpdateMI = typeof(GameObject).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
            GORenderMI = typeof(GameObject).GetMethod("Render", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static readonly MethodInfo GOUpdateMI;
        private static readonly MethodInfo GORenderMI;
        private static readonly object[] EMPTY_OBJECT_ARRAY = new object[0];

        public string Name { get; }

        public Viewport MainViewport { get; set; }

        private readonly List<GameObject> gameObjects;
        private readonly List<GameObject> gameObjectsBuffer;
        private bool bufferModified;

        public Scene(string name) {
            this.Name = name;

            this.gameObjects = new List<GameObject>();
            this.gameObjectsBuffer = new List<GameObject>();
            this.bufferModified = false;
        }

        public void Update() {
            foreach (GameObject gO in this.gameObjects) {
                if (gO.IsAlive && gO.IsEnabled) {
                    if (gO.Parent != null)
                        continue;

                    GOUpdateMI.Invoke(gO, EMPTY_OBJECT_ARRAY);  // TODO performance?
                } else if (!gO.IsAlive) {
                    this.gameObjectsBuffer.Remove(gO);
                    this.bufferModified = true;
                }
            }

            if (!this.bufferModified)
                return;

            lock (this.gameObjects) {
                this.gameObjects.Clear();
                this.gameObjects.AddRange(this.gameObjectsBuffer);
                this.bufferModified = false;
            }
        }

        public void Render() {
            if (MainViewport == null)
                return;

            foreach (GameObject gO in this.gameObjects) {
                if (gO.IsAlive && gO.IsEnabled)
                    GORenderMI.Invoke(gO, EMPTY_OBJECT_ARRAY);  // TODO performance?
            }
        }

        internal void AddGameObject(GameObject gameObject) {
            if (!Thread.CurrentThread.Equals(Application.Application.Instance.UpdateThread)) {
                Log.Instance.WriteLine($"A GameObject can only be created from the update thread.", LogType.Error);
                return;
            }

            this.gameObjectsBuffer.Add(gameObject); // TODO maybe lock
            this.bufferModified = true;
        }

        public IEnumerable<GameObject> GameObjects => this.gameObjects;

        public IEnumerable<GameObject> FindGameObjectsByName(string name, bool activeOnly = true) {
            return FindGameObjects(gO => gO.Name == name && (gO.IsEnabled || !activeOnly));
        }

        public IEnumerable<GameObject> FindGameObjects(Func<GameObject, bool> selector) {
            return GameObjects.Where(selector);
        }

        public IEnumerable<T> FindComponentsByType<T>(bool activeOnly = true) where T : GOC {
            return GameObjects.Where(gO => gO.IsEnabled || !activeOnly).SelectMany(gO => gO.GetComponents<T>());
        }
    }
}