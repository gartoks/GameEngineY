using GameEngine.Graphics.RenderSettings;

namespace GameEngine.Graphics.Textures {
    public class SubTexture2D : Texture {
        private readonly TextureAtlas2D atlas;

        internal SubTexture2D(TextureAtlas2D atlas, float[][] textureCoordinates) {
            this.atlas = atlas;
            this.TextureCoordinates = textureCoordinates;
        }

        internal override bool Activate() => this.atlas.Activate();

        internal override void Bind() => this.atlas.Bind();

        internal override void Bind(int textureUnit) => this.atlas.Bind(textureUnit);

        internal override void Release() => this.atlas.Release();

        public override void Dispose() => this.atlas.Dispose();

        internal override void Clean() => this.atlas.Clean();

        internal override int TextureID => this.atlas.TextureID;

        internal override int TextureUnit { get => this.atlas.TextureUnit; set => this.atlas.TextureUnit = value; }

        public override bool IsBound => this.atlas.IsBound;

        public override int Width => this.atlas.Width;

        public override int Height => this.atlas.Height;

        public override float[][] TextureCoordinates { get; }

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