using System;
using GameEngine.Graphics.Utility;
using GameEngine.Input;
using GameEngine.Logging;
using GameEngine.Math;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public class Viewport : GOC {
        private float width;
        private float height;
        private float depth;
        private float zoom;

        private Matrix4 viewMatrix;
        private bool isViewMatrixDirty;

        private Matrix4 projectionMatrix;
        private Matrix4 reverseProjectionMatrix;
        private bool isProjectionMatrixDirty;

        public override void Initialize(object[] parameters) => Initialize();

        public override void Initialize() {
            this.viewMatrix = Matrix4.CreateIdentity();
            this.projectionMatrix = Matrix4.CreateIdentity();
            this.reverseProjectionMatrix = Matrix4.CreateIdentity();

            this.isViewMatrixDirty = true;
            this.isProjectionMatrixDirty = true;

            GameObject.Transform.OnGlobalPositionChanged += MakeViewMatrixDirty;

            Width = 1 * Window.Window.AspectRatio;
            Height = 1;
            Depth = 1;
            Zoom = 1;
        }

        public override void Death() {
            GameObject.Transform.OnGlobalPositionChanged -= MakeViewMatrixDirty;
        }

        public Vector2 ScreenToViewport(Vector2 p) {
            float x = (p.x / Window.Window.Width - 0.5f) * 2f;
            float y = -(p.y / Window.Window.Height - 0.5f) * 2f;

            p.x = x;
            p.y = y;

            return p;
        }

        public Vector2 ViewportToScreen(Vector2 p) {
            float x = (p.x / 2f + 0.5f) * Window.Window.Width;
            float y = (-p.y / 2f + 0.5f) * Window.Window.Height;

            p.x = x;
            p.y = y;

            return p;
        }

        public Vector2 WorldToViewport(Vector2 p) {
            Vector4 pp = p.MakePoint();
            pp = ViewProjectionMatrix.Multiply(pp);
            p.Set(pp.x, pp.y);

            return p;
        }

        public Vector2 ViewportToWorld(Vector2 p) {
            Vector4 pp = p.MakePoint();
            pp = ReverseViewProjectionMatrix.Multiply(pp);
            p.Set(pp.x, pp.y);

            return p;
        }

        public Vector2 WorldToScreen(Vector2 p) {
            return ViewportToScreen(WorldToViewport(p));
        }

        public Vector2 ScreenToWorld(Vector2 p) {
            return ViewportToWorld(ScreenToViewport(p));
        }

        public float Width {
            get => this.width;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.width = value;
                this.isProjectionMatrixDirty = true;
            }
        }

        public float Height {
            get => this.height;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.height = value;
                this.isProjectionMatrixDirty = true;
            }
        }

        public float Depth {
            get => this.depth;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.depth = value;
                this.isProjectionMatrixDirty = true;
            }
        }

        public float Zoom {
            get => this.zoom;
            set {
                if (System.Math.Abs(value) < 0.0000001)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.zoom = value;
                this.isProjectionMatrixDirty = true;
            }
        }

        public float Left => -Width / 2f;

        public float Right => Width / 2f;

        public float Bottom => -Height / 2f;

        public float Top => Height / 2f;

        public float Far => Depth;

        public float Near => 0;

        internal Matrix4 RawViewMatrix {
            get {
                //if (isViewMatrixDirty) {
                    this.viewMatrix.Set(GameObject.Transform.RawGlobalTransformationMatrix);
                    this.viewMatrix.Inverse();
                    this.isViewMatrixDirty = false;
                //}

                return this.viewMatrix;
            }
        }

        internal Matrix4 ViewMatrix => RawViewMatrix.Clone();

        private Matrix4 RawProjectionMatrix {
            get {
                if (isProjectionMatrixDirty) {
                    this.projectionMatrix.MakeOrthographicProjection(Left * Zoom, Right * Zoom, Bottom * Zoom, Top * Zoom, Near, Far);
                    this.reverseProjectionMatrix.MakeReverseOrthographicProjection(Left * Zoom, Right * Zoom, Bottom * Zoom, Top * Zoom, Near, Far);
                    this.isProjectionMatrixDirty = false;
                }

                return this.projectionMatrix;
            }
        }

        private Matrix4 RawReverseProjectionMatrix {
            get {
                if (isProjectionMatrixDirty) {
                    this.projectionMatrix.MakeOrthographicProjection(Left * Zoom, Right * Zoom, Bottom * Zoom, Top * Zoom, Near, Far);
                    this.reverseProjectionMatrix.MakeReverseOrthographicProjection(Left * Zoom, Right * Zoom, Bottom * Zoom, Top * Zoom, Near, Far);
                    this.isProjectionMatrixDirty = false;
                }

                return this.reverseProjectionMatrix;
            }
        }

        internal Matrix4 ProjectionMatrix => RawProjectionMatrix.Clone();

        internal Matrix4 ViewProjectionMatrix => RawProjectionMatrix * RawViewMatrix;

        internal Matrix4 ReverseViewProjectionMatrix => RawReverseProjectionMatrix * RawViewMatrix.GetInverse();

        private void MakeViewMatrixDirty(IGameObject gameObject) => this.isViewMatrixDirty = true;
    }
}