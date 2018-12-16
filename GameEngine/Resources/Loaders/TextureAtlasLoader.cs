using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Logging;

namespace GameEngine.Resources.Loaders {
    public class TextureAtlasLoadingParameters : ResourceLoadingParameters<ITextureAtlas> {
        public TextureWrapMode WrapS;
        public TextureWrapMode WrapT;

        public TextureFilterMode MinFilter;
        public TextureFilterMode MagFilter;

        public TextureAtlasLoadingParameters(IEnumerable<string> filePaths, TextureWrapMode wrapMode, TextureFilterMode filterMode)
            : this(filePaths, wrapMode, wrapMode, filterMode, filterMode) { }

        protected TextureAtlasLoadingParameters(IEnumerable<string> filePaths, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter)
            : base(filePaths) {

            if (filePaths.Count() != 2) {
                Log.WriteLine("A texture resource must have exactly one file.", LogType.Error);
                return;
            }

            WrapS = wrapS;
            WrapT = wrapT;

            MinFilter = minFilter;
            MagFilter = magFilter;
        }
    }

    public class TextureAtlasLoader : ResourceLoader<ITextureAtlas, TextureAtlasLoadingParameters> {
        public override ITextureAtlas Load(IEnumerable<string> filePaths, TextureAtlasLoadingParameters loadingParameters) {
            string textureFile = filePaths.First();
            string regionFile = filePaths.Last();

            Bitmap bmp = new Bitmap(textureFile);
            string[] rawRegionData;
            try {
                rawRegionData = File.ReadAllLines(regionFile);
            } catch (Exception) {
                Log.WriteLine("Cannot load texture atlas, region file not readable.", LogType.Error);
                return null;
            }

            List<(string, int x, int y, int width, int height)> regionData = ParseTextureAtlasData(bmp, rawRegionData);

            return GraphicsHandler.CreateTextureAtlas(bmp, loadingParameters.WrapS, loadingParameters.WrapT, loadingParameters.MinFilter, loadingParameters.MagFilter, regionData);
        }

        internal static List<(string, int x, int y, int width, int height)> ParseTextureAtlasData(Bitmap bmp, string[] rawRegionData) {
            List<(string, int x, int y, int width, int height)> regionData = new List<(string, int x, int y, int width, int height)>();
            for (int i = 0; i < rawRegionData.Length; i++) {
                string regiondataLine = rawRegionData[i];

                string[] values = regiondataLine.Split(',');

                if (values.Length != 5) {   // 5 = 1 for name, 4 for x, y, width, height
                    Log.WriteLine($"Cannot load texture atlas, invalid number of arguments on line {i}.", LogType.Error);
                    return null;
                }

                if (values[0].Length == 0) {
                    Log.WriteLine($"Cannot load texture atlas, region name of line {i} is not valid.", LogType.Error);
                    return null;
                }

                string regionName = values[0];

                if (!int.TryParse(values[1], out int x)) {
                    Log.WriteLine($"Cannot load texture atlas, 1st argument (x) on line {i}({regionName}) is not a number.", LogType.Error);
                    return null;
                }

                if (x < 0 || x >= bmp.Width) {
                    Log.WriteLine($"Cannot load texture atlas, 1st argument (x) on line {i}({regionName}) is not in range [0, {bmp.Width - 1}].", LogType.Error);
                    return null;
                }

                if (!int.TryParse(values[2], out int y)) {
                    Log.WriteLine($"Cannot load texture atlas, 2nd argument (y) on line {i}({regionName}) is not a number.", LogType.Error);
                    return null;
                }

                if (y < 0 || y >= bmp.Height) {
                    Log.WriteLine($"Cannot load texture atlas, 2nd argument (y) on line {i}({regionName}) is not in range [0, {bmp.Height - 1}].", LogType.Error);
                    return null;
                }


                if (!int.TryParse(values[3], out int w)) {
                    Log.WriteLine($"Cannot load texture atlas, 3rd argument (width) on line {i}({regionName}) is not a number.", LogType.Error);
                    return null;
                }

                if (w < 1 || x + w > bmp.Width) {
                    Log.WriteLine($"Cannot load texture atlas, 3rd argument (width) on line {i}({regionName}) is not in range [1, {bmp.Width - x}].", LogType.Error);
                    return null;
                }

                if (!int.TryParse(values[4], out int h)) {
                    Log.WriteLine($"Cannot load texture atlas, 4th argument (height) on line {i}({regionName}) is not a number.", LogType.Error);
                    return null;
                }

                if (h < 1 || y + h > bmp.Height) {
                    Log.WriteLine($"Cannot load texture atlas, 4th argument (height) on line {i}({regionName}) is not in range [1, {bmp.Height - y}].", LogType.Error);
                    return null;
                }

                regionData.Add((regionName, x, y, w, h));
            }

            return regionData;
        }
    }
}