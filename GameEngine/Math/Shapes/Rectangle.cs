using System;

namespace GameEngine.Math.Shapes {
    public class Rectangle : Shape {
        public float X;
        public float Y;
        private float width;
        private float height;

        public Rectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

        public Rectangle(Vector2 position, Vector2 size)
            : this(position.x, position.y, size.x, size.y) { }

        public Rectangle(float x, float y, float width, float height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public override void Translate(float dx, float dy) {
            X += dx;
            Y += dy;
        }

        public override void Scale(float sx, float sy) {
            float w = Width * sx;
            float h = Height * sy;

            X += (Width - w) / 2f;
            Y += (Height - h) / 2f;

            Width = w;
            Height = h;
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

        public float Top => Y + Height;

        public float Bottom => Y;

        public float Left => X;

        public float Right => X + Width;

        public float CenterX => X + Width / 2f;

        public float CenterY => Y + Height / 2f;

        public override Vector2 Center => new Vector2(X + Width / 2f, Y + Height / 2f);

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
            return $"({Left}, {Top}, {Right}, {Bottom})";
        }
    }
}