using System;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;

namespace GameEngine.Game.GameObjects {
    public sealed class Transform {
        public GameObject GameObject { get; }

        private readonly Vector2 position;
        private readonly Vector2 scale;
        private float rotation;

        private Matrix4 translationMatrix;
        private Matrix4 scalingMatrix;    // TODO reuse matrices ?
        private Matrix4 rotationMatrix;

        internal Transform(GameObject gameObject)
            : this(gameObject, new Vector2(), 0, new Vector2(1, 1)) { }

        internal Transform(GameObject gameObject, Vector2 position, float rotation = 0, Vector2 scale = null) {

            GameObject = gameObject;

            this.position = new Vector2();
            this.scale = new Vector2();
            this.rotation = 0;

            this.translationMatrix = Matrix4.CreateIdentity();
            this.scalingMatrix = Matrix4.CreateIdentity();
            this.rotationMatrix = Matrix4.CreateIdentity();

            Position = position == null ? new Vector2() : position;
            Rotation = rotation;
            Scale = scale == null ? new Vector2(1, 1) : scale;
        }

        public Vector2 LocalPosition {
            get {
                if (GameObject.Parent == null)
                    return Position;

                return Position - GameObject.Parent.Transform.Position;
            }

            set {
                if (GameObject.Parent == null)
                    Position = value;
                else
                    Position = GameObject.Parent.Transform.Position + value;
            }
        }

        public Vector2 Position {
            get => this.position;
            set {
                if (value == null) {
                    Log.WriteLine($"[{GameObject.Name}] Position vector cannot be set if null.", LogType.Error);
                    return;
                }

                this.position.Set(value);
            }
        }

        public float Rotation {
            get => this.rotation;
            set {
                this.rotation = value;

                while (this.rotation < 0)
                    this.rotation += Mathf.TWO_PI;

                this.rotation = this.rotation % Mathf.TWO_PI;
            }
        }

        public Vector2 Scale {
            get => this.scale;
            set {
                if (value == null) {
                    Log.WriteLine($"[{GameObject.Name}] Scaling vector cannot be set if null.", LogType.Error);
                    return;
                }

                if (value.LengthSqr == 0)
                    throw new ArgumentException();

                this.scale.Set(value);
            }
        }

        public Matrix4 GlobalTransformationMatrix => CalculateGlobalTransformationMatrix(Matrix4.CreateIdentity());

        public Matrix4 CalculateGlobalTransformationMatrix(Matrix4 m) {
            GameObject.Parent?.Transform.CalculateGlobalTransformationMatrix(m);

            m.MultiplyLeft(LocalTransformationMatrix);  // TODO make sure it's left multiplication

            return m;
        }

        public Matrix4 LocalTransformationMatrix {
            get {
                this.rotationMatrix = MatrixTransformationHelper.SetTo3DZAxisCounterClockwiseRotation(Rotation, this.rotationMatrix);
                this.scalingMatrix = MatrixTransformationHelper.SetTo3DScaling(Scale.x, Scale.y, 1, this.scalingMatrix);
                this.translationMatrix = MatrixTransformationHelper.SetTo3DTranslation(Position.x, Position.y, 0, this.translationMatrix);

                return this.translationMatrix * this.rotationMatrix * this.scalingMatrix; // TODO make sure order is'nt reversed
            }
        }
    }
}