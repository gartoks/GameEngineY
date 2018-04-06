using System;
using System.Collections.Generic;
using System.Threading;
using GameEngine.Logging;
using Log = GameApp.Logging.Log;
using System.Linq;
using GameEngine.Game;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Math;

namespace GameApp.Game {
    public sealed class Scene : IScene {
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

                    gO.Update();
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
                    gO.Render();
            }
        }

        public GameObject CreateGameObject(string name, GameObject parent, Vector2 position, float rotation, Vector2 scale) {
            return new GameObject(name, parent, position, rotation, scale, this);
        }

        IGameObject IScene.CreateGameObject(string name, IGameObject parent, Vector2 position, float rotation, Vector2 scale) {
            return CreateGameObject(name, (GameObject)parent, position, rotation, scale);
        }

        internal void AddGameObject(GameObject gameObject) {
            if (!Thread.CurrentThread.Equals(Application.Application.Instance.UpdateThread)) {
                Log.Instance.WriteLine($"A GameObject can only be created from the update thread.", LogType.Error);
                return;
            }

            this.gameObjectsBuffer.Add(gameObject); // TODO maybe lock
            this.bufferModified = true;
        }

        public IEnumerable<GameObject> FindGameObjectsByName(string name, bool activeOnly = true) {
            return FindGameObjects(gO => gO.Name == name && (gO.IsEnabled || !activeOnly));
        }

        IEnumerable<IGameObject> IScene.FindGameObjectsByName(string name, bool activeOnly) => FindGameObjectsByName(name, activeOnly);

        public IEnumerable<GameObject> FindGameObjects(Func<GameObject, bool> selector) => GameObjects.Where(selector);

        IEnumerable<IGameObject> IScene.FindGameObjects(Func<IGameObject, bool> selector) => FindGameObjects(selector);

        public IEnumerable<GameObject> GameObjects => this.gameObjects;

        IEnumerable<IGameObject> IScene.GameObjects => GameObjects;

        public IEnumerable<T> FindComponentsByType<T>(bool activeOnly = true) where T : GOC => GameObjects.Where(gO => gO.IsEnabled || !activeOnly).SelectMany(gO => gO.GetComponents<T>());
    }
}