using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using GameEngine.Game.GameObjects.GameObjectComponents;
using GameEngine.Game.Utility;
using GameEngine.Graphics;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Modding;

namespace GameEngine.Game.GameObjects {
    public sealed class GameObject {
        public delegate void GameObjectComponentModificationEventHandler(GameObject gameObject, GOC component);

        internal IScene Scene { get; }

        private readonly Guid ID;

        private bool isAlive;

        private string name;

        public bool IsEnabled;
        private GameObject parent;
        private readonly HashSet<GameObject> children;

        public Transform Transform { get; }

        private readonly List<GOC> components;
        private readonly List<GOC> componentsBuffer;
        private bool bufferModified;

        public event GameObjectComponentModificationEventHandler OnComponentAdd;
        public event GameObjectComponentModificationEventHandler OnComponentRemove;

        public GameObject(string name)
            : this(name, null, 0) { }

        public GameObject(string name, Vector2 position = null, float rotation = 0, Vector2 scale = null)
            : this(name, null, position, rotation, scale) { }

        public GameObject(string name, GameObject parent = null, Vector2 position = null, float rotation = 0, Vector2 scale = null)
            : this(name, parent, position, rotation, scale, ModBase.SceneManager.ActiveScene) {
        }

        internal GameObject(string name, GameObject parent = null, Vector2 position = null, float rotation = 0, Vector2 scale = null, IScene scene = null) {
            if (ModBase.SceneManager.ActiveScene == null) {
                Log.WriteLine($"Cannote create GameObject ({name}) when no scene is active.", LogType.Error);
                return;
            }

            if (scene == null)
                scene = ModBase.SceneManager.ActiveScene;

            Scene = scene;

            if (position == null)
                position = new Vector2();

            if (scale == null)
                scale = new Vector2(1, 1);

            this.children = new HashSet<GameObject>();
            this.components = new List<GOC>();
            this.componentsBuffer = new List<GOC>();
            this.bufferModified = false;

            ID = Guid.NewGuid();

            Name = name;

            Transform = new Transform(this, position, rotation, scale);

            IsEnabled = true;

            Game.Scene.AddGameObject(Scene, this);

            Parent = parent;

            this.isAlive = true;
        }

        private readonly List<GameObject> updatingChildren = new List<GameObject>();
        internal void Update() {
            if (!IsEnabled)
                return;

            this.updatingChildren.Clear();
            this.updatingChildren.AddRange(Children);

            foreach (GOC goc in this.components) {
                if (!goc.IsActive)
                    continue;
                
                goc.Update();
            }

            if (this.bufferModified) {
                lock (this.components) {
                    this.components.Clear();
                    this.components.AddRange(this.componentsBuffer);
                    this.bufferModified = false;
                }
            }

            foreach (GameObject child in this.updatingChildren) {
                child.Update();
            }
        }

        private readonly List<GameObject> renderingChildren = new List<GameObject>();
        internal void Render() {
            if (!IsEnabled)
                return;

            GLHandler.ApplyTransformation(Transform);

            foreach (GOC goc in this.components) {
                if (!goc.IsActive)
                    return;

                goc.Renderable?.Render();
            }

            this.renderingChildren.Clear();
            this.renderingChildren.AddRange(Children);

            foreach (GameObject child in this.renderingChildren) {
                child.Render();
            }

            GLHandler.RevertTransform();
        }

        public T AddComponent<T>(params object[] initializationParameters) where T : GOC {
            return (T)AddComponent(typeof(T), true, initializationParameters, null);
        }

        internal GOC AddComponent(Type t, bool isEnabled, object[] initializationParameters, IEnumerable<(string fieldName, object fielValue)> fields) {
            if (!Thread.CurrentThread.Equals(Game.Scene.UpdateThread)) {
                Log.WriteLine($"A GOC can only be added to a GameObject ({Name}) from the update thread.", LogType.Error);
                return null;
            }

            if (t.IsAbstract) {
                Log.WriteLine($"Cannot add an abstract component to a GameObject ({Name}).", LogType.Error);
                return null;
            }

            IEnumerable<RequiredGOCs> requiredComponentCollection = t.GetCustomAttributes<RequiredGOCs>(true);
            foreach (RequiredGOCs requiredComponents in requiredComponentCollection) {
                GOCSearchMode searchMode = requiredComponents.InHierarchy ? GOCSearchMode.ParentalHierarchy : GOCSearchMode.This;
                foreach (Type requiredComponent in requiredComponents.Required) {
                    if (!GetComponents(requiredComponent, searchMode, true).Any())
                        AddComponent(requiredComponent, true, new object[0], new(string fieldName, object fielValue)[0]);
                }
            }

            ConstructorInfo ctor = t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            GOC component = ctor.Invoke(new object[0]) as GOC;
            component.GameObject = this;

            this.componentsBuffer.Add(component);
            this.bufferModified = true;

            component.IsEnabled = isEnabled;

            if (fields != null && fields.Any()) {
                foreach ((string fieldName, object fielValue) fieldData in fields) {
                    Type tmpT = t;
                    FieldInfo fI = null;
                    while (tmpT != null) {
                        fI = tmpT.GetField(fieldData.fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                        if (fI != null)
                            break;

                        tmpT = tmpT.BaseType;
                    }


                    if (fI == null)
                        throw new SerializationException("Cannot initialize component field.");

                    fI.SetValue(component, fieldData.fielValue);
                }
            }

            if (initializationParameters != null && initializationParameters.Length > 0)
                component.Initialize(initializationParameters);
            else
                component.Initialize();

            component.Initialize();

            OnComponentAdd?.Invoke(this, component);

            return component;
        }

        internal void RemoveComponent(GOC component) {
            if (!Thread.CurrentThread.Equals(Game.Scene.UpdateThread)) {
                Log.WriteLine($"A GOC can only be removed from a GameObject ({Name}) from the update thread.", LogType.Error);
                return;
            }

            if (!this.components.Contains(component))
                return;

            component.Death();

            OnComponentRemove?.Invoke(this, component);

            this.componentsBuffer.Remove(component);
            this.bufferModified = true;
        }

        public T GetComponent<T>(GOCSearchMode searchMode = GOCSearchMode.This, bool includeDerivations = true) where T : GOC {
            return GetComponents<T>(searchMode, includeDerivations).FirstOrDefault();
        }

        public IEnumerable<T> GetComponents<T>(GOCSearchMode searchMode = GOCSearchMode.This, bool includeDerivations = true) where T : GOC {
            return GetComponents(typeof(T), searchMode, includeDerivations).Cast<T>();
        }

        private IEnumerable<GOC> GetComponents(Type t, GOCSearchMode searchMode, bool includeDerivations) {
            if (!t.IsSubclassOf(typeof(GOC)))
                throw new ArgumentException(nameof(t));

            IEnumerable<GOC> comps;
            if (includeDerivations)
                comps = FindComponents(c => c.GetType() == t || c.GetType().IsSubclassOf(t));
            else
                comps = FindComponents(c => c.GetType() == t);

            if (searchMode == GOCSearchMode.This)
                return comps;

            if ((searchMode & GOCSearchMode.ChildHierarchy) > 0) {
                foreach (GameObject child in this.children) {
                    comps = comps.Concat(child.GetComponents(t, GOCSearchMode.ChildHierarchy, includeDerivations));
                }
            }

            if ((searchMode & GOCSearchMode.ParentalHierarchy) > 0 && Parent != null) {
                comps = comps.Concat(Parent.GetComponents(t, GOCSearchMode.ParentalHierarchy, includeDerivations));
            }

            return comps;
        }

        public IEnumerable<GOC> FindComponents(Func<GOC, bool> selector, GOCSearchMode searchMode = GOCSearchMode.This) {
            IEnumerable<GOC> comps = this.components.Where(selector);

            if (searchMode == GOCSearchMode.This)
                return comps;

            if ((searchMode & GOCSearchMode.ChildHierarchy) > 0) {
                foreach (GameObject child in this.children) {
                    comps = comps.Concat(child.FindComponents(selector, GOCSearchMode.ChildHierarchy));
                }
            }

            if ((searchMode & GOCSearchMode.ParentalHierarchy) > 0 && Parent != null) {
                comps = comps.Concat(Parent.FindComponents(selector, GOCSearchMode.ParentalHierarchy));
            }

            return comps;
        }

        public IEnumerable<GameObject> GetParentalHierarchy(bool includeCurrent = true) {
            GameObject gO = includeCurrent ? this : this.parent;
            while (gO != null) {
                yield return gO;
                gO = gO.Parent;
            }
        }

        public GameObject GetRootGameObject() {
            return Parent == null ? this : Parent.GetRootGameObject();
        }

        public GameObject Parent {
            get => this.parent;
            set {
                if (Equals(Parent, value))
                    return;

                if (value != null)
                    value.MakeChild(this);
                else
                    Parent?.UnmakeChild(this);
            }
        }

        private void MakeChild(GameObject child) {
            if (child == null)
                throw new ArgumentNullException();

            if (Children.Contains(child))
                return;

            Parent?.UnmakeChild(child);

            this.children.Add(child);
            child.parent = this;
        }

        private void UnmakeChild(GameObject child) {
            if (child == null) {
                Log.WriteLine($"Invalid child to unmake.", LogType.Error);
                return;
            }

            if (!Children.Contains(child))
                return;

            this.children.Remove(child);
            child.parent = null;
        }

        public IEnumerable<GameObject> Children => this.children;

        public IEnumerable<GameObject> FindChildren(Func<GameObject, bool> selector, bool includeChildrensChildren = false) {
            IEnumerable<GameObject> chs = this.children.Where(selector);

            if (!includeChildrensChildren)
                return chs;

            foreach (GameObject child in this.children) {
                chs = chs.Concat(child.FindChildren(selector, true));
            }

            return chs;
        }

        public void Kill() {
            Destroy();
        }

        public void Destroy() {
            this.isAlive = false;

            for (int i = this.components.Count - 1; i >= 0; i--) {
                this.components[i].Destroy();
            }
        }

        public bool IsAlive => this.isAlive;

        public string Name {
            get => this.name;
            set => this.name = string.IsNullOrWhiteSpace(value) ? "GameObject" : value;
        }

        public override bool Equals(object obj) {
            if (obj == null || !(obj is GameObject gO))
                return false;

            return ID.Equals(gO.ID);
        }

        public override int GetHashCode() {
            return ID.GetHashCode();
        }

        public override string ToString() {
            return $"GO [{Name}]";
        }
    }
}