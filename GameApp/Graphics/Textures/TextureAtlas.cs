using System.Collections.Generic;
using System.Drawing;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;

namespace GameApp.Graphics.Textures {
    internal class TextureAtlas : Texture2D, ITextureAtlas {
        private readonly Dictionary<string, SubTexture2D> atlas;

        public TextureAtlas(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter, int textureID)
            : base(bitmap, wrapS, wrapT, minFilter, magFilter, textureID) {

            this.atlas = new Dictionary<string, SubTexture2D>();
        }

        internal void SetRegion(string name, int offsetX, int offsetY, int width, int height) {
            this.atlas[name] = new SubTexture2D(this, offsetX, offsetY, width, height);
        }

        public SubTexture2D GetSubTexture(string name) {
            if (atlas.TryGetValue(name, out SubTexture2D tex))
                return tex;
            return null;
        }

        ITexture ITextureAtlas.GetSubTexture(string name) {
            return GetSubTexture(name);
        }
    }
}