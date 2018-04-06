using System;

namespace GameEngine.Math {
    public class Vector3 {

        public static Vector3 ZERO => new Vector3(0, 0, 0);
        public static Vector3 IDENTITY => new Vector3(1, 1, 1);

        private float _x;
        private float _y;
        private float _z;

        private float? length;

        public Vector3()
            : this(0, 0, 0) {
        }

        public Vector3(float x, float y, float z) {
            Data = (x, y, z);
        }

        public Vector3(Vector2 v)
            : this(v.x, v.y, 0) { }

        public Vector3(Vector3 v)
            : this(v.x, v.y, v.z) { }

        public Vector3(Vector4 v)
            : this(v.x, v.y, v.z) { }

        public Vector3 Set(Vector2 v) {
            Data = (v.x, v.y, 0);
            return this;
        }

        public Vector3 Set(Vector3 v) {
            Data = (v.x, v.y, v.z);
            return this;
        }

        public Vector3 Set(float x, float y, float z) {
            Data = (x, y, z);
            return this;
        }

        public Vector3 Normalize() {
            float l = Length;

            if (l == 0)
                return this;

            Data = (x / l, y / l, z / l);

            return this;
        }

        public Vector3 Add(float s) {
            return Add(s, s, s);
        }

        public Vector3 Add(float x, float y, float z) {
            Data = (this.x + x, this.y + y, this.z + z);

            return this;
        }

        public Vector3 Add(Vector3 v) {
            return Add(v.x, v.y, v.z);
        }

        public Vector3 Subtract(float x, float y, float z) {
            Data = (this.x - x, this.y - y, this.z - z);

            return this;
        }

        public Vector3 Subtract(Vector3 v) {
            return Subtract(v.x, v.y, v.z);
        }

        public Vector3 Scale(float s) {
            return Scale(s, s, s);
        }

        public Vector3 Scale(float x, float y, float z) {
            Data = (this.x * x, this.y * y, this.z * z);

            return this;
        }

        public Vector3 Scale(Vector3 v) {
            return Scale(v.x, v.y, v.z);
        }

        public Vector3 Apply(Func<int, float, float> f) {
            Data = (f(0, x), f(1, y), f(2, z));

            return this;
        }

        public Vector3 Normalized {
            get {
                Vector3 v = new Vector3(this);
                v.Normalize();
                return v;
            }
        }

        public Vector2 ToVector2() => new Vector2(this);

        public Vector4 ToVector4() => new Vector4(this);

        public Vector3 Clone() => new Vector3(this);

        public static float Dot(Vector3 v1, Vector3 v2) {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2) {
            float x = v1.y * v2.z - v1.z * v2.y;
            float y = v1.z * v2.x - v1.x * v2.z;
            float z = v1.x * v2.y - v1.y * v2.x;

            return new Vector3(x, y, z);
        }

        public static float Distance(Vector3 v1, Vector3 v2) {
            return Distance(v1, v2.x, v2.y, v2.z);
        }

        public static float Distance(Vector3 v1, float x, float y, float z) {
            float dx = x - v1.x;
            float dy = y - v1.y;
            float dz = z - v1.z;

            return (float)System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public (float x, float y, float z) Data {
            get => (x, y, z);
            set {
                float oldX = x;
                float oldY = y;
                float oldZ = z;

                this._x = value.x;
                this._y = value.y;
                this._z = value.z;

                this.length = null;
            }
        }

        public float x {
            get => this._x;
            set => Data = (value, y, z);
        }

        public float y {
            get => this._y;
            set => Data = (x, value, z);
        }

        public float z {
            get => this._z;
            set => Data = (x, y, value);
        }

        public float this[int i] {
            get {
                switch (i) {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set {
                switch (i) {
                    case 0: x = value; return;
                    case 1: y = value; return;
                    case 2: z = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public float LengthSqr => x * x + y * y + z * z;

        public float Length {
            get {
                if (this.length == null)
                    this.length = (float)System.Math.Sqrt(LengthSqr);

                return (float)this.length;
            }
        }

        public override string ToString() {
            return $"({x}, {y}. {z})";
        }

        public override bool Equals(object obj) {
            if (obj == null || !(obj is Vector3))
                return false;

            Vector3 v = obj as Vector3;

            return Equals(v.x, v.y, v.z);
        }

        public bool Equals(float x, float y, float z) {
            return this.x == x && this.y == y && this.z == z;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash * 23 + z.GetHashCode();
        }

        public static Vector3 operator +(Vector3 v0, Vector3 v1) {
            return new Vector3(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
        }

        public static Vector3 operator -(Vector3 v0, Vector3 v1) {
            return new Vector3(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
        }

        public static Vector3 operator *(float s, Vector3 v) {
            return v * s;
        }

        public static Vector3 operator *(Vector3 v, float s) {
            return new Vector3(v.x * s, v.y * s, v.z * s);
        }

        public static Vector3 operator /(Vector3 v, float s) {
            return new Vector3(v.x / s, v.y / s, v.z / s);
        }

        public static bool operator ==(Vector3 v1, Vector3 v2) {
            return ReferenceEquals(null, v1) ? ReferenceEquals(null, v2) : v1.Equals(v2);
        }

        public static bool operator !=(Vector3 v1, Vector3 v2) {
            return !(v1 == v2);
        }

        //public XMLElement ToXML(string tag) {
        //    XMLElement e = new XMLElement(tag);
        //    e.SetAttribute("x", X.ToString());
        //    e.SetAttribute("y", Y.ToString());

        //    return e;
        //}

        //public static Vector3 FromXML(XMLElement e) {
        //    float x = float.Parse(e.GetAttribute("x"));
        //    float y = float.Parse(e.GetAttribute("y"));

        //    return new Vector3(x, y);
        //}

    }
}
