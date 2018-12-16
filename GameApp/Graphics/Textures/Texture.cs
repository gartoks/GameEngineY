using System.Drawing;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;

namespace GameApp.Graphics.Textures {
    internal abstract class Texture : ITexture {

        internal static float[][] GetDefaultTextureCoordinates() {
            float[][] texCoods = new float[4][];
            texCoods[0] = new[] { 0f, 0f };
            texCoods[1] = new[] { 1f, 0f };
            texCoods[2] = new[] { 1f, 1f };
            texCoods[3] = new[] { 0f, 1f };

            return texCoods;
        }

        internal abstract void Bind();

        //internal abstract void Bind(int textureUnit);

        internal abstract void Release();

        internal abstract void Clean();

        internal abstract int TextureID { get; }

        internal abstract bool IsBound { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract float[][] TextureCoordinates { get; }

        public abstract bool IsDisposed { get; }

        public abstract void UpdateData(Bitmap bitmap);

        public abstract TextureWrapMode WrapS { get; set; }

        public abstract TextureWrapMode WrapT { get; set; }

        public abstract TextureFilterMode MinFilter { get; set; }

        public abstract TextureFilterMode MagFilter { get; set; }

        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();
    }
}