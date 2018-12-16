using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Logging;

namespace GameEngine.Resources.Loaders {
    public class TextureLoadingParameters : ResourceLoadingParameters<ITexture> {
        public TextureWrapMode WrapS;
        public TextureWrapMode WrapT;

        public TextureFilterMode MinFilter;
        public TextureFilterMode MagFilter;

        public TextureLoadingParameters(IEnumerable<string> filePaths, TextureWrapMode wrapMode, TextureFilterMode filterMode)
            : this(filePaths, wrapMode, wrapMode, filterMode, filterMode) { }

        protected TextureLoadingParameters(IEnumerable<string> filePaths, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter)
            : base(filePaths) {

            if (filePaths.Count() != 1) {
                Log.WriteLine("A texture resource must have exactly one file.", LogType.Error);
                return;
            }

            WrapS = wrapS;
            WrapT = wrapT;

            MinFilter = minFilter;
            MagFilter = magFilter;
        }
    }

    public class TextureLoader : ResourceLoader<ITexture, TextureLoadingParameters> {
        public override ITexture Load(IEnumerable<string> filePaths, TextureLoadingParameters loadingParameters) {
            string file = filePaths.Single();

            Bitmap bmp = new Bitmap(file);

            return GraphicsHandler.CreateTexture(bmp, loadingParameters.WrapS, loadingParameters.WrapT, loadingParameters.MinFilter, loadingParameters.MagFilter);
        }
    }
}