using System;

namespace GameEngine.Utility {
    public struct Color {
        public static readonly Color WHITE = new Color(1f, 1f, 1f);
        public static readonly Color LIGHT_GRAY = new Color(0.75f, 0.75f, 0.75f);
        public static readonly Color GRAY = new Color(0.5f, 0.5f, 0.5f);
        public static readonly Color DARK_GRAY = new Color(0.25f, 0.25f, 0.25f);
        public static readonly Color BLACK = new Color(0f, 0f, 0f);
        public static readonly Color RED = new Color(1f, 0f, 0f);
        public static readonly Color DARK_RED = new Color(0.5f, 0f, 0f);
        public static readonly Color GREEN = new Color(0f, 1f, 0f);
        public static readonly Color DARK_GREEN = new Color(0f, 0.5f, 0f);
        public static readonly Color BLUE = new Color(0f, 0f, 1f);
        public static readonly Color DARK_BLUE = new Color(0f, 0f, 0.5f);
        public static readonly Color YELLOW = new Color(1f, 1f, 0f);
        public static readonly Color ORANGE = new Color(1f, 0.5f, 0f);
        public static readonly Color LIME = new Color(0.5f, 1f, 0f);
        public static readonly Color MAGENTA = new Color(1f, 0f, 1f);
        public static readonly Color PURPLE = new Color(0.5f, 0f, 1f);
        public static readonly Color PINK = new Color(1f, 0f, 0.5f);
        public static readonly Color CYAN = new Color(0f, 1f, 1f);
        public static readonly Color LIGHT_GREEN = new Color(0f, 1f, 0.5f);
        public static readonly Color MEDIUM_BLUE = new Color(0f, 0.5f, 1f);
        public static readonly Color CLEAR = new Color(0f, 0f, 0f, 0f);

        private float _r;
        private float _g;
        private float _b;
        private float _a;

        public Color(string hex)
            : this(HexToColor(hex)) { }

        public Color(System.Drawing.Color c)
            : this(c.R, c.G, c.B, c.A) { }

        public Color(Color c)
            : this(c.r, c.g, c.b, c.a) { }

        public Color(byte r, byte g, byte b, byte a = 255)
            : this(r / 255f, g / 255f, b / 255f, a / 255f) { }

        public Color(float r, float g, float b, float a = 1f) {
            _r = Mathf.Clamp01(r);
            _g = Mathf.Clamp01(g);
            _b = Mathf.Clamp01(b);
            _a = Mathf.Clamp01(a);
        }

        //publiasdasdc Color(int rgba8888Color) {
        //    setColor(rgba8888Color);
        //}

        public Color Invert() {
            rgbData = (1f - r, 1f - g, 1f - b);

            return this;
        }

        public Color Clone() {
            return new Color(this);
        }

        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            if (obj is Color c) {
                return R == c.R && G == c.G && B == c.B && A == c.A;
            } else
                return false;
        }

        public override int GetHashCode() {
            int hash = 23;
            hash = (hash * 31) + R.GetHashCode();
            hash = (hash * 31) + G.GetHashCode();
            hash = (hash * 31) + B.GetHashCode();
            hash = (hash * 31) + A.GetHashCode();

            return hash;
        }

        public override string ToString() {
            return $"({R}, {G}, {B}, {A})";
        }

        public Color ColorData {
            set => RGBAData = value.RGBAData;
        }

        public (float r, float g, float b) rgbData {
            get => (r, g, b);
            set => rgbaData = (value.r, value.g, value.b, a);
        }

        public (float r, float g, float b, float a) rgbaData {
            get => (r, g, b, a);
            set {
                this._r = value.r;
                this._g = value.g;
                this._b = value.b;
                this._a = value.a;

                // TODO event ?
            }
        }

        public (byte R, byte G, byte B) RGBData {
            get => (R, G, B);
            set => RGBAData = (value.R, value.G, value.B, A);
        }

        public (byte R, byte G, byte B, byte A) RGBAData {
            get => (R, G, B, A);
            set => rgbaData = (value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
        }

        public float r {
            get => this._r;
            set => rgbaData = (value, g, b, a);
        }

        public float g {
            get => this._g;
            set => rgbaData = (r, value, b, a);
        }

        public float b {
            get => this._b;
            set => rgbaData = (r, g, value, a);
        }

        public float a {
            get => this._a;
            set => rgbaData = (r, g, b, value);
        }

        public byte R {
            get => (byte)(r * 255f);
            set => RGBAData = (value, G, B, A);
        }

        public byte G {
            get => (byte)(g * 255f);
            set => RGBAData = (R, value, B, A);
        }

        public byte B {
            get => (byte)(b * 255f);
            set => RGBAData = (R, G, value, A);
        }

        public byte A {
            get => (byte)(a * 255f);
            set => RGBAData = (R, G, B, value);
        }

        public bool HasAlpha => A < 255;

        //public Color setColor(int rgba8888Color) {
        //    return setColor((rgba8888Color & 0xff000000) >>> 24, (rgba8888Color & 0x00ff0000) >>> 16, (rgba8888Color & 0x0000ff00) >>> 8, rgba8888Color & 0x0000ff);
        //}

        //public int getRGBA8888Encoded() {
        //    return (getRed() << 24) | (getGreen() << 16) | (getBlue() << 8) | getAlpha();
        //}

        public float[] WriteVertices(float[] array, int offset) {
            int neededLength = offset + 4;
            if (array == null || array.Length < neededLength)
                throw new IndexOutOfRangeException();

            array[offset] = r;
            array[offset + 1] = g;
            array[offset + 2] = b;
            array[offset + 3] = a;

            return array;
        }

        public float[] ToArray() {
            return new float[] { r, g, b, a };
        }

        //public FloatBuffer getAsFloatBuffer() {
        //    return BufferUtils.createColorBuffer(this);
        //}

        //public FloatBuffer writeToFloatBuffer(FloatBuffer floatBuffer) {
        //    return BufferUtils.fillColorBuffer(floatBuffer, this);
        //}

        //public static int ARGB8ToRGBA8(int argb8) {
        //    int a = (argb8 >> 24) & 0xFF;
        //    int c = (argb8 << 8) | a;

        //    return c;
        //}

        //private float clampColorValue(float v) {
        //    return v < 0 ? 0 : (v > 1 ? 1 : v);
        //}

        public static Color Lerp(Color c1, Color c2, float t) {
            float a = c1.A * (1 - t) + c2.A * t;
            float r = c1.R * (1 - t) + c2.R * t;
            float g = c1.G * (1 - t) + c2.G * t;
            float b = c1.B * (1 - t) + c2.B * t;

            return new Color(r, g, b, a);
        }

        public static Color CalculateGradientColor(Color[] colors, float[] gradients, float t) {
            if (colors.Length != gradients.Length)
                throw new ArgumentException("GetColorGradient: color array length does not equal gradients array length.");

            t = Mathf.Clamp01(t);

            Color c = new Color();
            for (int i = 1; i < colors.Length; i++) {
                if (t <= gradients[i]) {
                    float ratio = gradients[i] - gradients[i - 1];
                    float x1 = t - gradients[i - 1];
                    float div = x1 / ratio;
                    c.rgbaData = (colors[i].r * div + colors[i - 1].r * (1f - div),
                                    colors[i].g * div + colors[i - 1].g * (1f - div),
                                    colors[i].b * div + colors[i - 1].b * (1f - div),
                                    colors[i].a * div + colors[i - 1].a * (1f - div));
                }
            }

            return c;
        }

        //public static void RemoveTransparentColor(int[] pixels, Color transparentColor) {
        //    Color c = new Color();
        //    for (int i = 0; i < pixels.length; i++) {
        //        c.setColor(pixels[i]);
        //        if (!c.equals(transparentColor))
        //            continue;

        //        c.setAlpha(0);
        //        pixels[i] = c.getRGBA8888Encoded();
        //    }
        //}

        public static string ColorToHex(Color color) {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + color.A.ToString("X2");
        }

        public static Color HexToColor(string hex) {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            byte a = 255;
            if (hex.Length == 8)
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color(r, g, b, a);
        }
    }
}