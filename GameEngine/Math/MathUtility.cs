namespace GameEngine.Math {
    public class MathUtility {
        public static float PointLineDistance(Vector2 p, Vector2 p0, Vector2 p1) {
            return PointLineDistance(p.x, p.y, p0.x, p0.y, p1.x, p1.y);
        }

        private static float PointLineDistance(float pX, float pY, float p0X, float p0Y, float p1X, float p1Y) {
            float dx = p1X - p0X;
            float dy = p1Y - p0Y;
            return (float)System.Math.Abs(dy * pX - dx * pY + p1X * p0Y - p1Y * p0X) / Mathf.Sqrt(dx * dx + dy * dy);
        }
    }
}