using GameEngine.Graphics;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public abstract class GOC {
        public GameObject GameObject { get; internal set; }

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

        internal virtual void Update() { }

        public virtual Renderable Renderable => null;

        /// <summary>
        /// Returns true if this GOC is enabled, and its GameObject is enabled and alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        internal bool IsActive => IsEnabled && GameObject.IsEnabled && GameObject.IsAlive;

        public Transform Transform => GameObject.Transform;
    }
}