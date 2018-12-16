using System;
using GameEngine.Game.Utility;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Math.Shapes;

namespace GameEngine.Game.GameObjects.GameObjectComponents.GUIComponents {
    public delegate void GUIInteractionHandler(GUIComponent component, float x, float y);

    [RequiredGOCs(true, typeof(GUIHandler))]
    public abstract class GUIComponent : GOC {

        public GUIHandler Handler { get; private set; }

        public GUIComponentState InteractionState { get; internal set; }

        private Rectangle bounds;
        private Rectangle worldBounds;

        private GUIDock dock;

        public event GUIInteractionHandler OnMouseEntered;
        public event GUIInteractionHandler OnMouseHovering;
        public event GUIInteractionHandler OnMouseExited;
        public event GUIInteractionHandler OnMouseClicked;
        public event GUIInteractionHandler OnMouseDown;
        public event GUIInteractionHandler OnMouseReleased;

        protected event Action OnSizeChanged;
        public Action<GUIComponent> OnFocusGained;
        public Action<GUIComponent> OnFocusLost;

        public override void Initialize() {
            Handler = Scene.FindComponentByType<GUIHandler>();

            bounds = new Rectangle(-0.5f, -0.5f, 1f, 1f);
            UpdateWorldBounds();

            InteractionState = GUIComponentState.None;
            dock = GUIDock.Centered;
        }

        public sealed override void Death() {
        }

        internal void InvokeMouseEntered(float mouseX, float mouseY) {
            OnMouseEntered?.Invoke(this, mouseX - Transform.Position.x, mouseY - Transform.Position.y);
        }

        internal void InvokeMouseHovering(float mouseX, float mouseY) {
            OnMouseHovering?.Invoke(this, mouseX - Transform.Position.x, mouseY - Transform.Position.y);
        }

        internal void InvokeMouseExited(float mouseX, float mouseY) {
            OnMouseExited?.Invoke(this, mouseX - Transform.Position.x, mouseY - Transform.Position.y);
        }

        internal void InvokeMouseClicked(float mouseX, float mouseY) {
            OnMouseClicked?.Invoke(this, mouseX - Transform.Position.x, mouseY - Transform.Position.y);
        }

        internal void InvokeMouseDown(float mouseX, float mouseY) {
            OnMouseDown?.Invoke(this, mouseX - Transform.Position.x, mouseY - Transform.Position.y);
        }

        internal void InvokeMouseReleased(float mouseX, float mouseY) {
            OnMouseReleased?.Invoke(this, mouseX - Transform.Position.x, mouseY - Transform.Position.y);
        }

        public GUIDock Dock {
            get => this.dock;
            set {
                this.dock = value;

                ResolveDockingCoordinates(Dock, Width, Height, out float x, out float y);
                this.bounds.X = -Width / 2f + x;
                this.bounds.Y = -Height / 2f + y;
                UpdateWorldBounds();

                OnSizeChanged?.Invoke();
            }
        }

        public float X => bounds.X;

        public float Y => bounds.Y;

        public float Width {
            get => this.bounds.Width;
            set {
                if (value <= 0) {
                    Log.WriteLine("Width of a GUIComponent must be bigger than zero.");
                    return;
                }

                ResolveDockingCoordinates(Dock, value, Height, out float x, out float _);
                this.bounds.X = -value / 2f + x;
                this.bounds.Width = value;
                UpdateWorldBounds();

                OnSizeChanged?.Invoke();
            }
        }

        public float Height {
            get => this.bounds.Height;
            set {
                if (value <= 0) {
                    Log.WriteLine("Height of a GUIComponent must be bigger than zero.");
                    return;
                }

                ResolveDockingCoordinates(Dock, Width, value, out float _, out float y);
                this.bounds.Y = -value / 2f + y;
                this.bounds.Height = value;
                UpdateWorldBounds();

                OnSizeChanged?.Invoke();
            }
        }

        public Rectangle Bounds => new Rectangle(this.bounds);

        public float WorldWidth => worldBounds.Width;

        public float WorldHeight => worldBounds.Height;

        public Rectangle WorldBounds => new Rectangle(worldBounds.X + Transform.Position.x, worldBounds.Y + Transform.Position.y, worldBounds.Width, worldBounds.Height);

        public bool HasFocus => Handler.Focus == this;

        private void UpdateWorldBounds() {
            Vector2 v0 = Scene.MainViewport.ViewportToWorld(bounds.BottomLeft);
            Vector2 v1 = Scene.MainViewport.ViewportToWorld(bounds.TopRight);

            worldBounds = new Rectangle(v0.x, v0.y, v1.x - v0.x, v1.y - v0.y);
        }

        internal void InvokeOnFocusGained() {
            OnFocusGained?.Invoke(this);
        }

        internal void InvokeOnFocusLost() {
            OnFocusLost?.Invoke(this);
        }

        protected static void ResolveDockingCoordinates(GUIDock dock, float w, float h, out float x, out float y) {
            switch (dock) {
                case GUIDock.Centered:
                    x = 0;
                    y = 0;
                    break;
                case GUIDock.TopLeft:
                    x = +w / 2f;
                    y = -h / 2f;
                    break;
                case GUIDock.TopRight:
                    x = -w / 2f;
                    y = -h / 2f;
                    break;
                case GUIDock.BottomLeft:
                    x = +w / 2f;
                    y = +h / 2f;
                    break;
                case GUIDock.BottomRight:
                    x = -w / 2f;
                    y = +h / 2f;
                    break;
                case GUIDock.TopCenter:
                    x = 0;
                    y = -h / 2f;
                    break;
                case GUIDock.BottomCenter:
                    x = 0;
                    y = +h / 2f;
                    break;
                case GUIDock.LeftCenter:
                    x = +w / 2f;
                    y = 0;
                    break;
                case GUIDock.RightCenter:
                    x = -w / 2f;
                    y = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dock), dock, null);
            }
        }

    }
}