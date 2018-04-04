namespace GameEngine.Math.Shapes {
    public class Ellipse : Shape {

        private readonly Vector2 center;
        private readonly Vector2 radius;

        private Rectangle boundingRectangle;

        public Ellipse(Vector2 center, Vector2 radius) : this(center.x, center.y, radius.x, radius.y) { }

        public Ellipse(float centerX, float centerY, float radiusX, float radiusY) {
            this.center = new Vector2(centerX, centerY);
            this.radius = new Vector2(radiusX, radiusY);

            RecalculateBoundingRectangle();
        }

        public override void Translate(float dx, float dy) {
            this.center.Add(dx, dy);
            RecalculateBoundingRectangle();
        }

        public override void Scale(float sx, float sy) {
            this.radius.Scale(sx, sy);
            RecalculateBoundingRectangle();
        }

        public override bool Contains(float x, float y) {
            float dx = x - Center.x;
            float dy = y - Center.y;

            return dx * dx / (radius.x * radius.x) + dy * dy / (radius.y * radius.y) <= 1;
        }

        public override bool Contains(Shape s) {
            return s.BoundingRectangle.Contains(BoundingRectangle);
        }

        public override bool Intersects(Shape s, bool includeContains) {
            return s.BoundingRectangle.Intersects(BoundingRectangle, includeContains);
        }

        public override Vector2 Center => new Vector2(center);

        public override Rectangle BoundingRectangle => this.boundingRectangle;

        private void RecalculateBoundingRectangle() {
            this.boundingRectangle = new Rectangle(center.x - radius.x, center.y - radius.y, 2f * radius.x, 2f * radius.y);
        }
    }
}