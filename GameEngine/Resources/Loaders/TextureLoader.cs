using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Textures;
using GameEngine.Logging;

namespace GameEngine.Resources.Loaders {
    public class Texture2DLoaderParameters : ResourceLoadingParameters<Texture2D> {
        public TextureWrapMode WrapS;
        public TextureWrapMode WrapT;

        public TextureFilterMode MinFilter;
        public TextureFilterMode MagFilter;

        public Texture2DLoaderParameters(IEnumerable<string> filePaths, TextureWrapMode wrapMode, TextureFilterMode filterMode)
            : this(filePaths, wrapMode, wrapMode, filterMode, filterMode) { }

        protected Texture2DLoaderParameters(IEnumerable<string> filePaths, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter)
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

    public class Texture2DLoader : ResourceLoader<Texture2D, Texture2DLoaderParameters> {
        public override Texture2D Load(IEnumerable<string> filePaths, Texture2DLoaderParameters loadingParameters) {
            string file = filePaths.Single();

            Bitmap bmp = new Bitmap(file);

            return new Texture2D(bmp, loadingParameters.WrapS, loadingParameters.WrapT, loadingParameters.MinFilter, loadingParameters.MagFilter);
        }
    }
}