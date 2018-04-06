using System;
using System.Drawing;
using GameEngine.Graphics.RenderSettings;

namespace GameEngine.Graphics.Textures {
    public abstract class Texture : IDisposable {
        private static Texture defaultTexture;
        internal static Texture DEFAULT_TEXTURE {
            get {
                if (defaultTexture == null) {
                    Bitmap bmp = new Bitmap(1, 1);
                    bmp.SetPixel(0, 0, Color.Magenta);
                    defaultTexture = new Texture2D(bmp, TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureFilterMode.Nearest, TextureFilterMode.Nearest);
                }

                return defaultTexture;
            }
        }

        internal Texture() {
        }

        ~Texture() {
            Dispose();
        }

        internal abstract bool Activate();

        internal abstract void Bind();

        internal abstract void Bind(int textureUnit);

        internal abstract void Release();

        public abstract void Dispose();

        internal abstract void Clean();

        internal abstract int TextureID { get; }

        internal abstract int TextureUnit { get; set; }

        public abstract bool IsBound { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract float[][] TextureCoordinates { get; }

        public abstract TextureWrapMode WrapS { get; set; }

        public abstract TextureWrapMode WrapT { get; set; }

        public abstract TextureFilterMode MinFilter { get; set; }

        public abstract TextureFilterMode MagFilter { get; set; }

        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();
    }
}