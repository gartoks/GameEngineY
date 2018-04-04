using System.Drawing;
using GameEngine.Graphics.RenderSettings;

namespace GameEngine.Graphics.Textures {
    public class Texture2D : Texture {
        private static readonly float[][] DEFAULT_TEXTURE_COORDINATES = {
            new[] { 0f, 0f },
            new[] { 1f, 0f },
            new[] { 0f, 1f },
            new[] { 1f, 1f }
        };

        private int textureUnit;

        protected int texID;    // set via reflection in GLHandler.InitializeTexture

        protected readonly Bitmap bitmap;
        protected float[][] textureCoordinates;

        protected TextureWrapMode wrapS;
        protected TextureWrapMode wrapT;
        protected bool hasDirtyWrap;

        protected TextureFilterMode minFilter;
        protected TextureFilterMode magFilter;
        protected bool hasDirtyFilter;

        internal Texture2D(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter) {
            this.bitmap = bitmap;

            this.wrapS = wrapS;
            this.wrapT = wrapT;
            this.hasDirtyWrap = true;

            this.minFilter = minFilter;
            this.magFilter = magFilter;

            GLHandler.InitializeTexture(this);

            this.textureCoordinates = DEFAULT_TEXTURE_COORDINATES;

            this.hasDirtyFilter = true;
        }

        //internal Texture2D(TextureAtlas2D mainAtlas)

        internal override bool Activate() {
            if (TextureUnit < 0)
                return false;

            GLHandler.ActivateTexture(this);
            Clean();

            return true;
        }

        internal override void Bind() {
            TextureUnit = GLHandler.BindTexture(this);
        }

        internal override void Bind(int textureUnit) {
            GLHandler.BindTexture(this, textureUnit);

            TextureUnit = textureUnit;
        }

        internal override void Release() {
            TextureUnit = -1;
            GLHandler.ReleaseTexture(this);
        }

        public override void Dispose() {
            GLHandler.DisposeTexture(this);
        }

        internal override void Clean() {
            if (!this.hasDirtyWrap && !this.hasDirtyFilter)
                return;

            //int previouslyActiveTextureUnit = RenderHandler.ActiveTextureUnit;
            GLHandler.ActivateTexture(this);

            if (this.hasDirtyWrap)
                GLHandler.UpdateTextureWrapMode(this);

            if (this.hasDirtyFilter)
                GLHandler.UpdateTextureFilterMode(this);

            //RenderHandler.ActivateTextureUnit(previouslyActiveTextureUnit);
        }

        internal override int TextureID => this.texID;

        internal override int TextureUnit { get => this.textureUnit; set => this.textureUnit = value; }

        public override bool IsBound => GLHandler.IsTextureBound(this);

        public override int Width => this.bitmap.Width;

        public override int Height => this.bitmap.Height;

        public override float[][] TextureCoordinates => this.textureCoordinates;

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

        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            return obj is Texture2D tex && texID == tex.texID;
        }

        public override int GetHashCode() {
            return texID;
        }

    }
}