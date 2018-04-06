using System;

namespace GameEngine.Math {
    public class Vector4 {

        public static Vector4 ZERO => new Vector4(0, 0, 0, 0);
        public static Vector4 IDENTITY => new Vector4(1, 1, 1, 1);

        private float _x;
        private float _y;
        private float _z;
        private float _w;

        private float? length;

        public Vector4()
            : this(0, 0, 0, 0) {
        }

        public Vector4(float x, float y, float z, float w) {
            Data = (x, y, z, w);
        }

        public Vector4(Vector2 v)
            : this(v.x, v.y, 0, 0) { }

        public Vector4(Vector3 v)
            : this(v.x, v.y, v.z, 0) { }


        public Vector4(Vector4 v)
            : this(v.x, v.y, v.z, v.w) { }

        public Vector4 Set(Vector2 v) {
            Data = (v.x, v.y, 0, 0);
            return this;
        }

        public Vector4 Set(Vector3 v) {
            Data = (v.x, v.y, v.z, 0);
            return this;
        }

        public Vector4 Set(Vector4 v) {
            Data = (v.x, v.y, v.z, v.w);
            return this;
        }

        public Vector4 Set(float x, float y, float z, float w) {
            Data = (x, y, z, w);
            return this;
        }

        public Vector4 Normalize() {
            float l = Length;

            if (l == 0)
                return this;

            Data = (x / l, y / l, z / l, w / l);

            return this;
        }

        public Vector4 Add(float s) {
            return Add(s, s, s, s);
        }

        public Vector4 Add(float x, float y, float z, float w) {
            Data = (this.x + x, this.y + y, this.z + z, this.w + w);

            return this;
        }

        public Vector4 Add(Vector4 v) {
            return Add(v.x, v.y, v.z, v.w);
        }

        public Vector4 Subtract(float x, float y, float z, float w) {
            Data = (this.x - x, this.y - y, this.z - z, this.w - w);

            return this;
        }

        public Vector4 Subtract(Vector4 v) {
            return Subtract(v.x, v.y, v.z, v.w);
        }

        public Vector4 Scale(float s) {
            return Scale(s, s, s, s);
        }

        public Vector4 Scale(float x, float y, float z, float w) {
            Data = (this.x * x, this.y * y, this.z * z, this.w * w);

            return this;
        }

        public Vector4 Scale(Vector4 v) {
            return Scale(v.x, v.y, v.z, v.w);
        }

        public Vector4 Apply(Func<int, float, float> f) {
            Data = (f(0, x), f(1, y), f(2, z), f(3, w));

            return this;
        }

        public Vector4 Normalized {
            get {
                Vector4 v = new Vector4(this);
                v.Normalize();
                return v;
            }
        }

        public Vector2 ToVector2() => new Vector2(this);

        public Vector3 ToVector3() => new Vector3(this);

        public Vector4 Clone() => new Vector4(this);

        public static float Dot(Vector4 v1, Vector4 v2) {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z + v1.w * v2.w;
        }

        public static float Distance(Vector4 v1, Vector4 v2) {
            return Distance(v1, v2.x, v2.y, v2.z, v2.w);
        }

        public static float Distance(Vector4 v1, float x, float y, float z, float w) {
            float dx = x - v1.x;
            float dy = y - v1.y;
            float dz = z - v1.z;
            float dw = w - v1.w;

            return (float)System.Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
        }

        public (float x, float y, float z, float w) Data {
            get => (x, y, z, w);
            set {
                float oldX = x;
                float oldY = y;
                float oldZ = z;
                float oldW = w;

                this._x = value.x;
                this._y = value.y;
                this._z = value.z;
                this._w = value.w;

                this.length = null;
            }
        }

        public float x {
            get => this._x;
            set => Data = (value, y, z, w);
        }

        public float y {
            get => this._y;
            set => Data = (x, value, z, w);
        }

        public float z {
            get => this._z;
            set => Data = (x, y, value, w);
        }

        public float w {
            get => this._w;
            set => Data = (x, y, z, value);
        }

        public float this[int i] {
            get {
                switch (i) {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set {
                switch (i) {
                    case 0: x = value; return;
                    case 1: y = value; return;
                    case 2: z = value; return;
                    case 3: w = value; return;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public float LengthSqr => x * x + y * y + z * z + w * w;

        public float Length {
            get {
                if (this.length == null)
                    this.length = (float)System.Math.Sqrt(LengthSqr);

                return (float)this.length;
            }
        }

        public override string ToString() {
            return $"({x}, {y}, {z}, {w})";
        }

        public override bool Equals(object obj) {
            if (!(obj is Vector4))
                return false;

            Vector4 v = (Vector4)obj;

            return Equals(v.x, v.y, v.z, v.w);
        }

        public bool Equals(float x, float y, float z, float w) {
            return this.x == x && this.y == y && this.z == z && this.w == w;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            hash = hash * 23 + z.GetHashCode();
            return hash * 23 + w.GetHashCode();
        }

        public static Vector4 operator +(Vector4 v0, Vector4 v1) {
            return new Vector4(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z, v0.w + v1.w);
        }

        public static Vector4 operator -(Vector4 v0, Vector4 v1) {
            return new Vector4(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z, v0.w - v1.w);
        }

        public static Vector4 operator *(float s, Vector4 v) {
            return v * s;
        }

        public static Vector4 operator *(Vector4 v, float s) {
            return new Vector4(v.x * s, v.y * s, v.z * s, v.w * s);
        }

        public static Vector4 operator /(Vector4 v, float s) {
            return new Vector4(v.x / s, v.y / s, v.z / s, v.w / s);
        }

        public static bool operator ==(Vector4 v1, Vector4 v2) {
            return object.ReferenceEquals(null, v1) ? object.ReferenceEquals(null, v2) : v1.Equals(v2);
        }

        public static bool operator !=(Vector4 v1, Vector4 v2) {
            return !(v1 == v2);
        }

        //public XMLElement ToXML(string tag) {
        //    XMLElement e = new XMLElement(tag);
        //    e.SetAttribute("x", X.ToString());
        //    e.SetAttribute("y", Y.ToString());

        //    return e;
        //}

        //public static Vector4 FromXML(XMLElement e) {
        //    float x = float.Parse(e.GetAttribute("x"));
        //    float y = float.Parse(e.GetAttribute("y"));

        //    return new Vector4(x, y);
        //}

    }
}
