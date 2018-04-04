using System;

namespace GameEngine.Math {
    public class Vector2 {

        public static Vector2 ZERO => new Vector2(0, 0);
        public static Vector2 IDENTITY => new Vector2(1, 1);

        public delegate void VectorChangedEventHandler(Vector2 v, float oldX, float oldY);
        public event VectorChangedEventHandler OnChanged;

        private float _x;
        private float _y;

        private float? length;

        public Vector2()
            : this(0, 0) {
        }

        public Vector2(float x, float y) {
            Data = (x, y);
        }

        public Vector2(Vector2 v)
            : this(v.x, v.y) {
        }

        public Vector2 Set(Vector2 v) {
            Data = (v.x, v.y);
            return this;
        }

        public Vector2 Set(float x, float y) {
            Data = (x, y);
            return this;
        }

        public Vector2 Normalize() {
            float l = Length;

            if (l == 0)
                return this;

            Data = (x / l, y / l);

            return this;
        }

        public Vector2 Add(float s) {
            return Add(s, s);
        }

        public Vector2 Add(float x, float y) {
            Data = (this.x + x, this.y + y);

            return this;
        }

        public Vector2 Add(Vector2 v) {
            return Add(v.x, v.y);
        }

        public Vector2 Subtract(float x, float y) {
            Data = (this.x - x, this.y - y);

            return this;
        }

        public Vector2 Subtract(Vector2 v) {
            return Subtract(v.x, v.y);
        }

        public Vector2 Scale(float s) {
            return Scale(s, s);
        }

        public Vector2 Scale(float x, float y) {
            Data = (this.x * x, this.y * y);

            return this;
        }

        public Vector2 Scale(Vector2 v) {
            return Scale(v.x, v.y);
        }

        public Vector2 Apply(Func<int, float, float> f) {
            Data = (f(0, x), f(1, y));

            return this;
        }

        public Vector2 Normalized {
            get {
                Vector2 v = new Vector2(this);
                v.Normalize();
                return v;
            }
        }

        public Vector2 Normal(bool up) {
            float x = this.y;
            float y = this.x;

            if (up)
                x *= -1;
            else
                y *= -1;

            return new Vector2(x, y);
        }

        public static float AngleBetween(Vector2 v1, Vector2 v2) {
            return AngleBetween(v1.x, v1.y, v2.x, v2._y);
        }

        public static float AngleBetween(float x1, float y1, float x2, float y2) {
            return (float)(System.Math.Atan2(y2, x2) - System.Math.Atan2(y1, x1));
        }

        public static float Dot(Vector2 v1, Vector2 v2) {
            return v1.x * v2.x + v1.y * v2.y;
        }

        public static float Distance(Vector2 v1, Vector2 v2) {
            float dx = v2.x - v1.x;
            float dy = v2.y - v1.y;

            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }

        public static float Distance(Vector2 v1, float x, float y) {
            float dx = x - v1.x;
            float dy = y - v1.y;

            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }

        public (float x, float y) Data {
            get => (x, y);
            set {
                float oldX = x;
                float oldY = y;

                _x = value.x;
                _y = value.y;

                length = null;

                OnChanged?.Invoke(this, oldX, oldY);
            }
        }

        public float x {
            get => _x;
            set => Data = (value, y);
        }

        public float y {
            get => _y;
            set => Data = (x, value);
        }

        public float this[int i] {
            get {
                switch (i) {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set {
                switch (i) {
                    case 0: {
                            x = value;
                            return;
                        }
                    case 1: {
                            y = value;
                            return;
                        }
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public float LengthSqr => x * x + y * y;

        public float Length {
            get {
                if (length == null)
                    length = (float)System.Math.Sqrt(LengthSqr);

                return (float)length;
            }
        }

        public override string ToString() {
            return $"({x}, {y})";
        }

        public override bool Equals(object obj) {
            if (obj == null || !(obj is Vector2))
                return false;

            Vector2 v = obj as Vector2;

            return Equals(v.x, v.y);
        }

        public bool Equals(float x, float y) {
            return this.x == x && this.y == y;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            return hash * 23 + y.GetHashCode();
        }

        public static Vector2 operator +(Vector2 v0, Vector2 v1) {
            return new Vector2(v0.x + v1.x, v0.y + v1.y);
        }

        public static Vector2 operator -(Vector2 v0, Vector2 v1) {
            return new Vector2(v0.x - v1.x, v0.y - v1.y);
        }

        public static Vector2 operator *(float s, Vector2 v) {
            return v * s;
        }

        public static Vector2 operator *(Vector2 v, float s) {
            return new Vector2(v.x * s, v.y * s);
        }

        public static Vector2 operator /(Vector2 v, float s) {
            return new Vector2(v.x / s, v.y / s);
        }

        public static bool operator ==(Vector2 v1, Vector2 v2) {
            return ReferenceEquals(null, v1) ? ReferenceEquals(null, v2) : v1.Equals(v2);
        }

        public static bool operator !=(Vector2 v1, Vector2 v2) {
            return !(v1 == v2);
        }

        //public XMLElement ToXML(string tag) {
        //    XMLElement e = new XMLElement(tag);
        //    e.SetAttribute("x", X.ToString());
        //    e.SetAttribute("y", Y.ToString());

        //    return e;
        //}

        //public static Vector2 FromXML(XMLElement e) {
        //    float x = float.Parse(e.GetAttribute("x"));
        //    float y = float.Parse(e.GetAttribute("y"));

        //    return new Vector2(x, y);
        //}

    }
}
