using System.Collections.Generic;
using System.Linq;
using GameEngine.Logging;

namespace GameEngine.Math.Shapes {
    public class Polygon : Shape {

        private Vector2 center;
        private readonly Vector2[] points;
        private readonly Vector2[] edges;

        private Rectangle boundingRect;
        private float area;

        public Polygon(bool clockwise, params Vector2[] points) {
            if (clockwise) {
                IEnumerable<Vector2> rev = points.Reverse();
                this.points = new Vector2[points.Length];
                for (int i = 0; i < points.Length; i++)
                    this.points[i] = rev.ElementAt(i);
            } else
                this.points = points;

            edges = new Vector2[points.Length];

            RecalculateData();

            if (!IsConvex()) {
                Log.WriteLine("Polygon is not convex.");
                return;
            }
        }

        public override void Translate(float dx, float dy) {
            foreach (Vector2 v in points)
                v.Add(dx, dy);

            RecalculateData();
        }

        public override void Scale(float sx, float sy) {
            Vector2 center = Center;

            for (int i = 0; i < points.Length; i++) {
                Vector2 p = points[i];

                Vector2 dir = p - center;
                dir.Scale(sx, sy);

                p = center + dir;

                points[i] = p;
            }

            RecalculateData();
        }

        public override bool Contains(float x, float y) {
            if (!BoundingRectangle.Contains(x, y))
                return false;

            int numPoints = points.Length;

            bool contains = false;
            for (int i = 0; i < numPoints; i++) {
                Vector2 p = points[i];
                Vector2 p1 = points[(i + 1) % numPoints];

                if ((p.y <= y && y < p1.y || p1.y <= y && y < p.y) && x < (p1.x - p.x) / (p1.y - p.y) * (y - p.y) + p.x)
                    contains = !contains;
            }

            return contains;
        }

        public override bool Contains(Shape s) {
            if (!Contains(s.BoundingRectangle))
                return false;

            if (s is Polygon p) {
                for (int i = 0; i < p.Points.Count(); i++)
                    if (!Contains(p.Points.ElementAt(i)))
                        return false;

                return true;
            }/* else if (s is Ellipse e) {
                for (int i = 0; i < points.Length; i++) {
                    Vector2 p0 = points[i];
                    Vector2 p1 = points[(i + 1) % points.Length];

                    float dst = MathUtility.PointLineDistance(e.Center, p0, p1);
                    if ()
                }   TODO
            }*/

            return false;
        }

        public override bool Intersects(Shape s, bool includeContains) {
            return s.BoundingRectangle.Intersects(BoundingRectangle, includeContains);    // TODO
        }

        public IEnumerable<Vector2> Points => points;

        public IEnumerable<Vector2> Edges => edges;

        public override Rectangle BoundingRectangle => this.boundingRect;

        public override Vector2 Center => new Vector2(center);

        public float Area => area;

        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            if (!(obj is Polygon p))
                return false;

            if (Points.Count() != p.Points.Count())
                return false;

            int startIndex = 0;
            while (startIndex < this.points.Length && !p.points[startIndex].Equals(this.points[0]))
                startIndex++;

            if (startIndex == this.points.Length)
                return false;

            for (int i = 0; i < this.points.Length; i++) {
                Vector2 tp = this.points[i];
                Vector2 op = p.points[(startIndex + i) % this.points.Length];

                if (!tp.Equals(op))
                    return false;
            }

            return true;
        }

        public override int GetHashCode() => -501195594 + EqualityComparer<Vector2[]>.Default.GetHashCode(this.points);

        private void RecalculateData() {
            RecalculateEdges();
            RecalculateBoundingRect();
            RecalculateCenter();

            area = System.Math.Abs(Mathf.CalculateArea(points));
        }

        private void RecalculateEdges() {
            for (int i = 0; i < points.Length; i++) {
                Vector2 p0 = points[i];
                Vector2 p1 = points[(i + 1) % points.Length];

                edges[i] = new Vector2(p1.x - p0.x, p1.y - p0.y);
            }
        }

        private void RecalculateBoundingRect() {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = 0; i < points.Length; i++) {
                if (points[i].x < minX)
                    minX = points[i].x;
                if (points[i].x > maxX)
                    maxX = points[i].x;
                if (points[i].y < minY)
                    minY = points[i].y;
                if (points[i].y > maxY)
                    maxY = points[i].y;
            }

            if (points.Length != 0)
                boundingRect = new Rectangle(minX, maxY, maxX - minX, maxY - minY);
            else
                boundingRect = new Rectangle(0, 0, 0, 0);
        }

        private void RecalculateCenter() {
            float totalX = 0;
            float totalY = 0;
            foreach (Vector2 p in points) {
                totalX += p.x;
                totalY += p.y;
            }

            center = new Vector2(totalX / points.Length, totalY / points.Length);
        }

        private bool IsConvex() {
            int numPoints = points.Length;

            if (numPoints < 4)
                return true;

            bool hasNegTurn = false;
            bool hasPosTurn = false;
            for (int i = 0; i < numPoints; i++) {
                int i1 = (i + 1) % numPoints;
                int i2 = (i1 + 1) % numPoints;

                Vector2 p = points[i];
                Vector2 p1 = points[i1];
                Vector2 p2 = points[i2];

                float xProdLen = (p.x - p1.x) * (p.y - p1.y) - (p2.x - p1.x) * (p2.y - p1.y);

                if (xProdLen < 0)
                    hasNegTurn = true;
                else if (xProdLen > 0)
                    hasPosTurn = true;

                if (hasNegTurn && hasPosTurn)
                    return false;
            }

            return true;
        }
    }
}