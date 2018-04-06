using System;
using GameEngine.Graphics.Utility;
using GameEngine.Math;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public class Viewport : GOC {

        private float width;
        private float height;
        private float depth;
        private float zoom;

        private Matrix4 projectionMatrix;
        private bool isMatrixDirty;

        public override void Initialize(object[] parameters) => Initialize();

        public override void Initialize() {
            this.projectionMatrix = Matrix4.CreateIdentity();

            Width = 1;
            Height = 1;
            Depth = 1;
            Zoom = 1;
        }

        public void ScreenToViewport(ref Vector2 p) {
            //m4_0.MakeTransformation(-Width / 2f, Height / 2f, 0, false, 1f / Window.Window.Width, -1f / Window.Window.Height);
            float x = (p.x / Window.Window.Width - 0.5f) * Width;
            float y = -(p.y / Window.Window.Height - 0.5f) * Height;

            p.x = x;
            p.y = y;
        }

        public void ViewportToScreen(ref Vector2 p) {
            float x = (p.x / Width + 0.5f) * Window.Window.Width;
            float y = (-p.y / Height + 0.5f) * Window.Window.Height;

            p.x = x;
            p.y = y;
        }

        public void WorldToViewport(ref Vector2 p) {
            //float sin = Mathf.Sin(Transform.GlobalRotation);
            //float cos = Mathf.Cos(Transform.GlobalRotation);

            //float x = cos * Transform.GlobalScale.x * (p.x - Transform.GlobalPosition.x) - sin * Transform.GlobalScale.y * (p.y - Transform.GlobalPosition.y);
            //x /= Zoom;
            //float y = sin * Transform.GlobalScale.x * (p.x - Transform.GlobalPosition.x) + cos * Transform.GlobalScale.y * (p.y - Transform.GlobalPosition.y);
            //y /= Zoom;

            //p.x = x;
            //p.y = y;

            Vector4 pp = p.MakePoint();
            pp = ViewMatrix.MultiplyLeft(RawProjectionMatrix).Multiply(pp);
            p.Set(pp.x, pp.y);
        }

        public void ViewportToWorld(ref Vector2 p) {
            //float sin = Mathf.Sin(Transform.GlobalRotation);
            //float cos = Mathf.Cos(Transform.GlobalRotation);
            //float tan = sin / cos;
            //float tanSin = tan * sin;

            //float y = (p.y - tan * p.x) * Zoom / Transform.GlobalScale.y + (tanSin - cos) * Transform.GlobalPosition.y;
            //y /= tanSin + cos;

            //float x = p.x * Zoom + sin * Transform.GlobalScale.y * (y - Transform.GlobalPosition.y);
            //x /= cos * Transform.GlobalScale.x;
            //x += Transform.GlobalPosition.x;

            //p.x = x;
            //p.y = y;

            Vector4 pp = p.MakePoint();
            pp = ViewMatrix.MultiplyLeft(RawProjectionMatrix).Inverse().Multiply(pp);
            p.Set(pp.x, pp.y);
        }

        public float Width {
            get => this.width;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.width = value;
                this.isMatrixDirty = true;
            }
        }

        public float Height {
            get => this.height;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.height = value;
                this.isMatrixDirty = true;
            }
        }

        public float Depth {
            get => this.depth;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.depth = value;
                this.isMatrixDirty = true;
            }
        }

        public float Zoom {
            get => this.zoom;
            set {
                if (System.Math.Abs(value) < 0.0000001)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.zoom = value;
                this.isMatrixDirty = true;
            }
        }

        public float Left => -Width / 2f;

        public float Right => Width / 2f;

        public float Bottom => -Height / 2f;

        public float Top => Height / 2f;

        public float Far => Depth;

        public float Near => 0;

        internal Matrix4 ViewMatrix => GameObject.Transform.GlobalTransformationMatrix.Inverse();

        private Matrix4 RawProjectionMatrix {
            get {
                if (isMatrixDirty) {
                    this.projectionMatrix.MakeOrthographicProjection(Left, Right, Bottom, Top, Near, Far);
                    this.isMatrixDirty = false;
                }

                return this.projectionMatrix;
            }
        }

        internal Matrix4 ProjectionMatrix => RawProjectionMatrix.Clone();
    }
}