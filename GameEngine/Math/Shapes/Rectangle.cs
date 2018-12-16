using System;

namespace GameEngine.Math.Shapes {
    public class Rectangle : Shape {
        private float x;
        private float y;
        private float width;
        private float height;

        internal Action<Rectangle> OnChange;

        public Rectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

        public Rectangle(Vector2 position, Vector2 size)
            : this(position.x, position.y, size.x, size.y) { }

        public Rectangle(float x, float y, float width, float height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public override void Translate(float dx, float dy) {

            if (!float.IsInfinity(dx) && !float.IsNaN(dx))
                this.x += dx;
            if (!float.IsInfinity(dy) && !float.IsNaN(dy))
                this.y += dy;

            OnChange?.Invoke(this);
        }

        public override void Scale(float sx, float sy) {
            float w = Width * sx;
            float h = Height * sy;

            this.x += (Width - w) / 2f;
            this.y += (Height - h) / 2f;

            this.width = w;
            this.height = h;

            OnChange?.Invoke(this);
        }

        public override bool Contains(float x, float y) {
            return x >= Left && x <= Right && y >= Bottom && y <= Top;
        }

        public override bool Contains(Shape s) {
            return s.BoundingRectangle.Contains(BoundingRectangle);    // TODO
        }

        public override bool Intersects(Shape s, bool includeContains) {
            return s.BoundingRectangle.Intersects(BoundingRectangle, includeContains);    // TODO
        }

        public float X {
            get => this.x;
            set {
                if (float.IsInfinity(value) || float.IsNaN(value))
                    return;

                this.x = value;

                OnChange?.Invoke(this);
            }
        }

        public float Y {
            get => this.y;
            set {
                if (float.IsInfinity(value) || float.IsNaN(value))
                    return;

                this.y = value;

                OnChange?.Invoke(this);
            }
        }

        public Vector2 Position {
            get => new Vector2(X, Y);
            set {
                if (value == null)
                    return;

                if (float.IsInfinity(value.x) || float.IsNaN(value.x))
                    return;
                if (float.IsInfinity(value.y) || float.IsNaN(value.y))
                    return;

                this.x = value.x;
                this.y = value.y;

                OnChange?.Invoke(this);
            }
        }

        public float Width {
            get => this.width;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "A rectangle width must be bigger than zero.");

                this.width = value;
            }
        }

        public float Height {
            get => this.height;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "A rectangle height must be bigger than zero.");

                this.height = value;
            }
        }

        public Vector2 Size {
            get => new Vector2(Width, Height);
            set {
                if (value == null)
                    return;

                if (float.IsInfinity(value.x) || float.IsNaN(value.x))
                    return;
                if (float.IsInfinity(value.y) || float.IsNaN(value.y))
                    return;

                if (value.x < 0)
                    return;

                if (value.y < 0)
                    return;

                this.width = value.x;
                this.height = value.y;
                
                OnChange?.Invoke(this);
            }
        }

        public float Top => Y + Height;

        public float Bottom => Y;

        public float Left => X;

        public float Right => X + Width;

        public float CenterX => X + Width / 2f;

        public float CenterY => Y + Height / 2f;

        public override Vector2 Center => new Vector2(CenterX, CenterY);

        public override Rectangle BoundingRectangle => this;

        public Vector2 TopLeft => new Vector2(Left, Top);

        public Vector2 TopRight => new Vector2(Right, Top);

        public Vector2 BottomLeft => new Vector2(Left, Bottom);

        public Vector2 BottomRight => new Vector2(Right, Bottom);

        //public bool Contains(Rectangle r) { TODO
        //    return Contains(TopLeft) && Contains(TopRight) && Contains(BottomLeft) && Contains(BottomRight);
        //}

        //public bool Intersects(Rectangle r, bool includeContains) { TODO
        //    bool ctl = Contains(TopLeft);
        //    bool ctr = Contains(TopRight);
        //    bool cbl = Contains(BottomLeft);
        //    bool cbr = Contains(BottomRight);

        //    int containedPoints = 0;
        //    if (ctl) containedPoints++;
        //    if (ctr) containedPoints++;
        //    if (cbl) containedPoints++;
        //    if (cbr) containedPoints++;

        //    return containedPoints == 1 || containedPoints == 2 || (includeContains && containedPoints == 4);
        //}

        public override string ToString() {
            return $"({Left}, {Bottom}, {Right}, {Top})";
        }

        public string ToStringSize() {
            return $"({Left}, {Bottom}, {Width}, {Height})";
        }
    }
}