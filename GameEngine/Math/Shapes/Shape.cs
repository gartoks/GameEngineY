namespace GameEngine.Math.Shapes {
    public abstract class Shape {

        public void Translate(Vector2 t) => Translate(t.x, t.y);

        public abstract void Translate(float dx, float dy);

        public void Scale(Vector2 s) => Scale(s.x, s.y);

        public abstract void Scale(float sx, float sy);

        public bool Contains(Vector2 p) => Contains(p.x, p.y);

        public abstract bool Contains(float x, float y);

        public abstract bool Contains(Shape s);

        public abstract bool Intersects(Shape s, bool includeContains);

        public abstract Vector2 Center { get; }

        public abstract Rectangle BoundingRectangle { get; }
    }
}