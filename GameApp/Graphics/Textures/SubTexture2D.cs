using System.Drawing;
using GameApp.Logging;
using GameEngine.Graphics.RenderSettings;

namespace GameApp.Graphics.Textures {
    internal class SubTexture2D : Texture {
        private readonly TextureAtlas atlas;

        private readonly int x;
        private readonly int y;
        public override int Width { get; }
        public override int Height { get; }

        internal SubTexture2D(TextureAtlas atlas, int offsetX, int offsetY, int width, int height) {
            this.atlas = atlas;
            this.x = offsetX;
            this.y = offsetY;
            Width = width;
            Height = height;

            this.TextureCoordinates = new [] {
                new []{x / (float)atlas.Width, y / (float)atlas.Height},
                new []{(x + width) / (float)atlas.Width, y / (float)atlas.Height},
                new []{(x + width) / (float)atlas.Width, (y + height) / (float)atlas.Height},
                new []{x / (float)atlas.Width, (y + height) / (float)atlas.Height}
            };
        }

        internal override void Bind() => this.atlas.Bind();

        //internal override void Bind(int textureUnit) => this.atlas.Bind(textureUnit);

        internal override void Release() => this.atlas.Release();

        internal override void Clean() => this.atlas.Clean();

        internal override int TextureID => this.atlas.TextureID;

        internal override bool IsBound => this.atlas.IsBound;

        public override float[][] TextureCoordinates { get; }

        public override bool IsDisposed => atlas.IsDisposed;

        public override void UpdateData(Bitmap bitmap) {
            if (bitmap.Width != Width || bitmap.Height != Height) {
                Log.Instance.WriteLine("Cannot update texture data. Dimensions do not match.");
                return;
            }

            this.atlas.UpdateData(bitmap, x, y);
        }

        public override TextureWrapMode WrapS {
            get => this.atlas.WrapS;
            set => this.atlas.WrapS = value;
        }

        public override TextureWrapMode WrapT {
            get => this.atlas.WrapT;
            set => this.atlas.WrapT = value;
        }

        public override TextureFilterMode MinFilter {
            get => this.atlas.MinFilter;
            set => this.atlas.MinFilter = value;
        }

        public override TextureFilterMode MagFilter {
            get => this.atlas.MagFilter;
            set => this.atlas.MagFilter = value;
        }

        public override bool Equals(object obj) => this.atlas.Equals(obj);

        public override int GetHashCode() => this.atlas.GetHashCode();
    }
}