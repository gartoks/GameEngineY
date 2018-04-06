using System;
using GameEngine.Graphics;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public abstract class GOC {

        private readonly IGameObject gameObject;

        public bool IsEnabled;

        protected GOC() {
            IsEnabled = true;
        }

        public void Destroy() {
            GameObject.RemoveComponent(this);
        }

        public virtual void Initialize() { }

        public virtual void Initialize(object[] parameters) { }

        public virtual void Death() { }

        protected virtual void Update() { }

        private void Render() {
            Renderable?.Render();
        }

        protected virtual Renderable Renderable => null;

        /// <summary>
        /// Returns true if this GOC is enabled, and its GameObject is enabled and alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive => IsEnabled && GameObject.IsEnabled && GameObject.IsAlive;

        public IGameObject GameObject => this.gameObject;

        public Transform Transform => GameObject.Transform;

        private Action GetUpdateMethod => Update;

        private Action GetRenderMethod => Render;
    }
}