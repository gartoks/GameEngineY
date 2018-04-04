using System.Collections.Generic;
using System.Drawing;
using GameEngine.Graphics.RenderSettings;
using Rectangle = GameEngine.Math.Shapes.Rectangle;

namespace GameEngine.Graphics.Textures {
    public class TextureAtlas2D : Texture2D {
        private readonly Dictionary<string, Rectangle> atlas;

        public TextureAtlas2D(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter)
            : base(bitmap, wrapS, wrapT, minFilter, magFilter) {

            this.atlas = new Dictionary<string, Rectangle>();
        }

        internal void SetRegion(string name, Rectangle rectangle) {
            this.atlas[name] = new Rectangle(rectangle);
        }
    }
}