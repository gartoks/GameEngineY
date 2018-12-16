using System;
using GameEngine.Graphics;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public abstract class GOC {

        private readonly IGameObject gameObject;    // set via reflection

        public bool IsEnabled;

        public IRenderable Renderable { get; private set; }

        protected GOC() {
            IsEnabled = true;
        }

        public void Destroy() {
            GameObject.RemoveComponent(this);
        }

        public virtual void Initialize() { }

        public virtual void Initialize(object[] parameters) {
            Initialize();
        }

        public virtual void Death() { }

        protected virtual void Update() { }

        protected void CreateRenderable(ShaderVertexAttributeResolver attributeResolver, ShaderUniformAssignmentHandler shaderUniformAssignmentHandler, IShader shader, IMesh mesh) {
            Renderable = GraphicsHandler.CreateRenderable(attributeResolver, shaderUniformAssignmentHandler, shader, mesh);
        }

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
    }
}