using System.Drawing;
using GameApp.Logging;
using GameEngine.Graphics.RenderSettings;

namespace GameApp.Graphics.Textures {
    internal class Texture2D : Texture {
        protected readonly int textureID;

        protected Bitmap bitmap;
        protected bool hasDirtyData;

        protected float[][] textureCoordinates;

        protected TextureWrapMode wrapS;
        protected TextureWrapMode wrapT;
        protected bool hasDirtyWrap;

        protected TextureFilterMode minFilter;
        protected TextureFilterMode magFilter;
        protected bool hasDirtyFilter;

        internal Texture2D(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter, int textureID) {
            this.textureID = textureID;

            this.bitmap = bitmap;

            this.wrapS = wrapS;
            this.wrapT = wrapT;
            this.hasDirtyWrap = false;

            this.minFilter = minFilter;
            this.magFilter = magFilter;

            this.textureCoordinates = Texture.GetDefaultTextureCoordinates();

            this.hasDirtyFilter = false;

            this.hasDirtyData = false;
        }

        ~Texture2D() {
            GraphicsHandler.Instance.DisposeTexture(this);
        }

        internal override void Bind() {
            Clean();

            GLHandler.Instance.AssignTexture(this);
        }

        /*internal override void Bind(int textureUnit) {
            GLHandler.Instance.BindTexture(this, textureUnit);

            TextureUnit = textureUnit;
        }*/

        internal override void Release() {
            GLHandler.Instance.UnassignTexture(this);
        }

        internal override void Clean() {
            if (this.hasDirtyWrap) {
                GLHandler.Instance.UpdateTextureWrapMode(this);
                this.hasDirtyWrap = false;
            }

            if (this.hasDirtyFilter) {
                GLHandler.Instance.UpdateTextureFilterMode(this);
                this.hasDirtyFilter = false;
            }

            if (this.hasDirtyData) {
                GLHandler.Instance.UpdateTextureData(this, 0, 0, this.bitmap);
                this.hasDirtyData = false;
            }
        }

        internal override int TextureID => this.textureID;

        internal override bool IsBound => GLHandler.Instance.IsTextureAssigned(this);

        public override int Width => this.bitmap.Width;

        public override int Height => this.bitmap.Height;

        public override float[][] TextureCoordinates => this.textureCoordinates;    // TODO separate between internal get and external get (copy array for 2nd)

        public override void UpdateData(Bitmap bitmap) {
            if (bitmap.Width != this.bitmap.Width || bitmap.Height != this.bitmap.Height) {
                Log.Instance.WriteLine("Cannot update texture data. Dimensions do not match.");
                return;
            }

            this.bitmap = bitmap;
            this.hasDirtyData = true;
        }

        public void UpdateData(Bitmap bitmap, int x, int y) {
            if (x + bitmap.Width > this.bitmap.Width || y + bitmap.Height > this.bitmap.Height) {
                Log.Instance.WriteLine("Cannot update texture data. Dimensions do not match.");
                return;
            }

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(this.bitmap)) {
                g.DrawImage(bitmap, x, y);
            }

            this.hasDirtyData = true;
        }

        public override TextureWrapMode WrapS {
            get => this.wrapS;
            set {
                this.wrapS = value;
                this.hasDirtyWrap = true;
            }
        }

        public override TextureWrapMode WrapT {
            get => this.wrapT;
            set {
                this.wrapT = value;
                this.hasDirtyWrap = true;
            }
        }

        public override TextureFilterMode MinFilter {
            get => this.minFilter;
            set {
                this.minFilter = value;
                this.hasDirtyFilter = true;
            }
        }

        public override TextureFilterMode MagFilter {
            get => this.magFilter;
            set {
                this.magFilter = value;
                this.hasDirtyFilter = true;
            }
        }

        public override bool IsDisposed => this.textureID <= 0;

        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            return obj is Texture2D tex && textureID == tex.textureID;
        }

        public override int GetHashCode() {
            return textureID;
        }

    }
}