using System;
using System.Collections.Generic;
using System.Threading;
using GameEngine.Logging;
using Log = GameApp.Logging.Log;
using System.Linq;
using GameApp.Application;
using GameEngine.Game;
using GameEngine.Game.GameObjects;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Game.Utility;
using GameEngine.Math;
using GameEngine.Utility.DataStructures;

namespace GameApp.Game {
    public sealed class Scene : IScene {
        public string Name { get; }

        public Viewport MainViewport { get; set; }

        private readonly List<GameObject> gameObjects;
        private readonly QuadTree<GameObject> positionTree;

        internal Scene(string name) {
            this.Name = name;

            this.gameObjects = new List<GameObject>();
            this.positionTree = new QuadTree<GameObject>(AppConstants.Internals.SCENE_QUADTREE_SPLIT_MARGIN, AppConstants.Internals.SCENE_QUADTREE_MERGE_MARGIN);
        }

        internal void Update() {
            foreach (GameObject gO in this.gameObjects.ToArray()) {
                if (gO.IsAlive && gO.IsEnabled) {
                    if (gO.Parent == null)
                        gO.Update();
                } else if (!gO.IsAlive)
                    RemoveGameObject(gO);
            }
        }

        internal void Render() {
            if (MainViewport == null)
                return;

            foreach (GameObject gO in this.gameObjects.ToArray()) {
                if (gO.IsAlive && gO.IsEnabled && gO.Parent == null)
                    gO.Render();
            }
        }

        public GameObject CreateGameObject(string name, GameObject parent, Vector2 position, float rotation, Vector2 scale) {
            if (!Thread.CurrentThread.Equals(Application.Application.Instance.UpdateThread)) {
                Log.Instance.WriteLine($"A GameObject can only be created from the update thread.", LogType.Error);
                return null;
            }

            GameObject gameObject = new GameObject(name, parent, position, rotation, scale, this);

            this.gameObjects.Add(gameObject);
            Vector2 gOgP = gameObject.Transform.GlobalPosition;
            this.positionTree.Add(gOgP.x, gOgP.y, gameObject);    // TODO may need to lock and in remove and findByRange too
            gameObject.Transform.OnGlobalPositionChanged += GameObjectTransformOnGlobalPositionChanged;

            return gameObject;
        }

        IGameObject IScene.CreateGameObject(string name, IGameObject parent, Vector2 position, float rotation, Vector2 scale) => CreateGameObject(name, (GameObject)parent, position, rotation, scale);

        private void RemoveGameObject(GameObject gameObject) {
            this.gameObjects.Remove(gameObject);
            this.positionTree.Remove(gameObject);
            gameObject.Transform.OnGlobalPositionChanged -= GameObjectTransformOnGlobalPositionChanged;
        }

        public IEnumerable<GameObject> FindGameObjectsByName(string name, bool activeOnly = true) => FindGameObjects(gO => gO.Name == name && (gO.IsEnabled || !activeOnly));

        IEnumerable<IGameObject> IScene.FindGameObjectsByName(string name, bool activeOnly) => FindGameObjectsByName(name, activeOnly);

        public IEnumerable<GameObject> FindGameObjectsInRange(Vector2 p, float range) => this.positionTree.ItemsIn(p.x, p.y, range);

        IEnumerable<IGameObject> IScene.FindGameObjectsInRange(Vector2 p, float range) => FindGameObjectsInRange(p, range);

        public IEnumerable<GameObject> FindGameObjects(Func<GameObject, bool> selector) => GameObjects.Where(selector);

        IEnumerable<IGameObject> IScene.FindGameObjects(Func<IGameObject, bool> selector) => FindGameObjects(selector);

        public IEnumerable<GameObject> GameObjects => this.gameObjects;

        IEnumerable<IGameObject> IScene.GameObjects => GameObjects;

        public IEnumerable<T> FindComponentsByType<T>(bool activeOnly = true) where T : GOC => GameObjects.Where(gO => gO.IsEnabled || !activeOnly).SelectMany(gO => gO.GetComponents<T>());

        internal IEnumerable<GOC> FindComponentsByType(Type t, bool activeOnly = true) => GameObjects.Where(gO => gO.IsAlive || !activeOnly).SelectMany(gO => gO.GetComponents(t, GOCSearchMode.This, true));

        public T FindComponentByType<T>(bool activeOnly = true) where T : GOC => GameObjects.Where(gO => gO.IsAlive || !activeOnly).Select(gO => gO.GetComponent<T>()).FirstOrDefault();

        private void GameObjectTransformOnGlobalPositionChanged(IGameObject gameObject) {
            Vector2 gOgP = gameObject.Transform.GlobalPosition;
            this.positionTree.Move(gOgP.x, gOgP.y, (GameObject)gameObject);    // TODO may need to lock and in remove and findByRange too
        }
    }
}