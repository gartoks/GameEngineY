using System;
using GameEngine.Graphics.Utility;
using GameEngine.Math;

namespace GameEngine.Game.GameObjects {
    public sealed class Transform {

        public delegate void GlobalPositionChangedEventHandler(IGameObject gameObject);

        public IGameObject GameObject { get; }

        private readonly Vector2 position;
        private readonly Vector2 scale;
        private float rotation;

        private readonly Matrix4 localTransformationMatrix;
        private bool isLocalTransformationMatrixDirty;

        private readonly Matrix4 globalTransformationMatrix;
        private bool isGlobalTransformationMatrixDirty;

        public event GlobalPositionChangedEventHandler OnGlobalPositionChanged;

        internal Transform(IGameObject gameObject, Vector2 position, float rotation = 0, Vector2 scale = null) {
            GameObject = gameObject;

            this.position = new Vector2();
            this.scale = new Vector2();
            this.rotation = 0;

            this.localTransformationMatrix = Matrix4.CreateIdentity();
            this.isLocalTransformationMatrixDirty = false;
            this.globalTransformationMatrix = Matrix4.CreateIdentity();
            this.isGlobalTransformationMatrixDirty = false;

            Position = position == null ? new Vector2() : position;
            Rotation = rotation;
            Scale = scale == null ? new Vector2(1, 1) : scale;
        }

        public Vector2 Position {
            get => this.position.Clone();
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (position.Equals(value))
                    return;

                this.position.Set(value);
                MakeLocalTransformationMatrixDirty();
            }
        }

        public float Rotation {
            get => this.rotation;
            set {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (System.Math.Abs(rotation - value) < 0.00001f)
                    return;

                this.rotation = value;
                while (this.rotation < 0)
                    this.rotation += Mathf.TWO_PI;
                this.rotation %= Mathf.TWO_PI;

                MakeLocalTransformationMatrixDirty();
            }
        }

        public Vector2 Scale {
            get => this.scale.Clone();
            set {
                if (value == null || value.Equals(0, 0))
                    throw new ArgumentNullException(nameof(value));

                if (scale.Equals(value))
                    return;

                this.scale.Set(value);
                MakeLocalTransformationMatrixDirty();
            }
        }

        public Vector2 GlobalPosition => RawGlobalTransformationMatrix.Multiply(Vector4.ZERO).ToVector2();

        public Matrix4 GlobalTransformationMatrix => RawGlobalTransformationMatrix.Clone();

        private Matrix4 RawGlobalTransformationMatrix {
            get {
                if (isGlobalTransformationMatrixDirty) {
                    if (GameObject.Parent == null)
                        this.globalTransformationMatrix.MakeIdentity();
                    else
                        this.globalTransformationMatrix.Set(GameObject.Parent.Transform.RawGlobalTransformationMatrix);

                    this.globalTransformationMatrix.MultiplyLeft(RawLocalTransformationMatrix);

                    this.isGlobalTransformationMatrixDirty = false;
                }

                return this.globalTransformationMatrix;
            }
        }

        public Matrix4 LocalTransformationMatrix => RawLocalTransformationMatrix.Clone();

        private Matrix4 RawLocalTransformationMatrix {
            get {
                if (isLocalTransformationMatrixDirty) {
                    this.localTransformationMatrix.MakeTransformation(this.position, this.rotation, false, this.scale);
                    this.isLocalTransformationMatrixDirty = false;
                }

                return this.localTransformationMatrix;
            }
        }

        private void MakeLocalTransformationMatrixDirty() {
            this.isLocalTransformationMatrixDirty = true;
            MakeGlobalTransformationMatrixDirty();
        }

        private void MakeGlobalTransformationMatrixDirty() {
            this.isGlobalTransformationMatrixDirty = true;

            OnGlobalPositionChanged?.Invoke(GameObject);

            foreach (IGameObject child in GameObject.Children) {
                child.Transform.MakeGlobalTransformationMatrixDirty();
            }
        }

    }
}