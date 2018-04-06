using System;
using GameEngine.Graphics.Utility;
using GameEngine.Math;

namespace GameEngine.Game.GameObjects {
    public sealed class Transform {
        public IGameObject GameObject { get; }

        private readonly Vector2 position;
        private readonly Vector2 scale;
        private float rotation;

        private readonly Matrix4 transformationMatrix;
        private bool isMatrixDirty;

        internal Transform(IGameObject gameObject, Vector2 position, float rotation = 0, Vector2 scale = null) {
            GameObject = gameObject;

            this.position = new Vector2();
            this.scale = new Vector2();
            this.rotation = 0;

            Position = position == null ? new Vector2() : position;
            Rotation = rotation;
            Scale = scale == null ? new Vector2(1, 1) : scale;
            
            this.transformationMatrix = Matrix4.CreateIdentity();
            this.isMatrixDirty = false;
        }

        public Vector2 Position {
            get => this.position.Clone();
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                this.position.Set(value);
                this.isMatrixDirty = true;
            }
        }

        public float Rotation {
            get => this.rotation;
            set {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.rotation = value;
                while (this.rotation < 0)
                    this.rotation += Mathf.TWO_PI;
                this.rotation %= Mathf.TWO_PI;

                this.isMatrixDirty = true;
            }
        }

        public Vector2 Scale {
            get => this.scale.Clone();
            set {
                if (value == null || value.Equals(0, 0))
                    throw new ArgumentNullException(nameof(value));

                this.scale.Set(value);
                this.isMatrixDirty = true;
            }
        }

        public Vector2 GlobalPosition => GlobalTransformationMatrix.Multiply(Vector4.ZERO).ToVector2();

        public Matrix4 GlobalTransformationMatrix => CalculateGlobalTransformationMatrix(Matrix4.CreateIdentity());

        private Matrix4 CalculateGlobalTransformationMatrix(Matrix4 m) {
            GameObject.Parent?.Transform.CalculateGlobalTransformationMatrix(m);

            m.MultiplyLeft(LocalTransformationMatrix);

            return m;
        }

        public Matrix4 LocalTransformationMatrix {
            get {
                if (isMatrixDirty) {
                    this.transformationMatrix.MakeTransformation(this.position, this.rotation, false, this.scale);
                    this.isMatrixDirty = false;
                }

                return this.transformationMatrix.Clone();
            }
        }

    }
}