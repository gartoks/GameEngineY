using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Logging;
using GameEngine.Math;

namespace GameEngine.Resources.Loaders {
    public class FontLoadingParameters : ResourceLoadingParameters<ITextureAtlas> {

        //public const string DEFAULT_CHARACTERS = "A";
        public const string DEFAULT_CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?.()&*-+<>{}[]%$\"';:@#~/|\\=_ ";
        public const int MIN_FONT_TEXTURE_HEIGHT = 16;

        public readonly FontFamily Font;
        public readonly int TextureHeight;
        public readonly string Characters;

        public FontLoadingParameters(FontFamily font, int textureHeight, string characters = DEFAULT_CHARACTERS)
            : base(new string[0]) {

            if (font == null) {
                Log.WriteLine("FontFamily for a Font resource cannot be null.", LogType.Error);
                return;
            }

            if (textureHeight <= MIN_FONT_TEXTURE_HEIGHT) {
                Log.WriteLine($"A font texture height must be at least {MIN_FONT_TEXTURE_HEIGHT}.");
            }

            if (string.IsNullOrWhiteSpace(characters))
                characters = DEFAULT_CHARACTERS;

            Font = font;
            TextureHeight = textureHeight;
            Characters = characters;
        }
    }

    public class FontLoader : ResourceLoader<ITextureAtlas, FontLoadingParameters> {
        public override ITextureAtlas Load(IEnumerable<string> filePaths, FontLoadingParameters loadingParameters) {

            (Bitmap bmp, string[] data) atlasData = FontToTextureAtlas(loadingParameters.Font.Name, loadingParameters.TextureHeight, loadingParameters.Characters);
            List<(string, int x, int y, int width, int height)> textureAtlasData = TextureAtlasLoader.ParseTextureAtlasData(atlasData.bmp, atlasData.data);

            //atlasData.bmp.Save("Test.png");

            //for (int i = 0; i < atlasData.data.Length; i++) {
            //    Log.WriteLine((i + 1) + " " + atlasData.data[i]);
            //}

            return GraphicsHandler.CreateTextureAtlas(atlasData.bmp, TextureWrapMode.Clamp, TextureWrapMode.Clamp, TextureFilterMode.Nearest, TextureFilterMode.Nearest, textureAtlasData);
        }

        private static (Bitmap bmp, string[] data) FontToTextureAtlas(string fontName, int fontHeight, string characters) {
            Bitmap[] textures = new Bitmap[characters.Length];
            int height = 0;
            int totalWidth = 0;
            for (int i = 0; i < characters.Length; i++) {
                char c = characters[i];

                Bitmap bmp = CharacterToImage(fontName, fontHeight, c);

                if (height == 0)
                    height = bmp.Height;

                totalWidth += bmp.Width;

                textures[i] = bmp;
            }

            float averageWidth = totalWidth / (float)characters.Length;

            //int totalHeight = characters.Length * height;
            //float sqrt = characters.Length * Mathf.Sqrt(height);
            int size = NextHighestPowerOfTwo(Mathf.Sqrt(characters.Length) * System.Math.Max(averageWidth, height));

            string[] atlasData = new string[textures.Length];
            Bitmap res = new Bitmap(size, size);
            res.MakeTransparent();
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(res)) {
                //g.Clear(Color.Aquamarine);
                int line = 0;
                int width = 0;
                for (int i = 0; i < textures.Length; i++) {
                    Bitmap texture = textures[i];

                    if (width + texture.Width > size) {
                        line++;
                        width = 0;
                    }

                    int x = width;
                    int y = line * height;
                    g.DrawImage(texture, x, y);
                    width += texture.Width;

                    StringBuilder sB = new StringBuilder();
                    sB.Append(characters[i]).Append(",");
                    sB.Append(x).Append(",").Append(y).Append(",");
                    sB.Append(texture.Width).Append(",").Append(texture.Height);

                    atlasData[i] = sB.ToString();
                }
            }

            //res.Save($"{fontName}_{fontHeight}.png");
            return (res, atlasData);
        }

        private static int NextHighestPowerOfTwo(float v) {
            return Mathf.RoundToInt(Mathf.Pow(2, Mathf.CeilToInt(Mathf.Log(v, 2))));
        }

        private static Bitmap CharacterToImage(string fontName, int height, char character) {
            Bitmap bmp = new Bitmap(height, height);
            bmp.MakeTransparent();

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp)) {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                (int fontSize, float width, float height) data = GetMaximumFontSizeFitInRectangle(fontName, character.ToString(), (height, height));

                Font font = new Font(fontName, data.fontSize);
                g.DrawString(character != ' ' ? character.ToString() : "-", font, new SolidBrush(Color.White), 0, 0);
            }

            bmp = TrimImage(bmp);

            if (character == ' ') {
                Color transparent = Color.FromArgb(0, 0, 0, 0);
                for (int y = 0; y < bmp.Height; y++) {
                    for (int x = 0; x < bmp.Width; x++) {
                        bmp.SetPixel(x, y, transparent);
                    }
                }
            }

            return bmp;
        }

        private static Bitmap TrimImage(Bitmap bmp) {
            int minX = 0;
            int maxX = bmp.Width - 1;

            while (IsColumnEmpty(bmp, minX))
                minX++;

            while (IsColumnEmpty(bmp, maxX))
                maxX--;

            minX--;
            maxX++;

            int newWidth = bmp.Width - (bmp.Width - maxX) - minX;
            Bitmap nBmp = new Bitmap(newWidth, bmp.Height);

            for (int y = 0; y < bmp.Height; y++) {
                for (int x = 0; x < newWidth; x++) {
                    nBmp.SetPixel(x, y, bmp.GetPixel(x + minX, y));
                }
            }

            return nBmp;
        }

        private static bool IsColumnEmpty(Bitmap bmp, int x) {
            for (int y = 0; y < bmp.Height; y++) {
                if (bmp.GetPixel(x, y).A != 0)
                    return false;
            }

            return true;
        }

        private static (int fontSize, float width, float height) GetMaximumFontSizeFitInRectangle(string fontName, string text, (int width, int height) container, int minimumFontSize = 3, int maximumFontSize = 1000) {
            float prevWidth = 0;
            float prevHeight = 0;
            for (int newFontSize = minimumFontSize; ; newFontSize++) {
                Font newFont = new Font(fontName, newFontSize);

                Size size = MeasureDrawTextSize(text, newFont, container.width, container.height);

                if (size.Width > container.width || size.Height > container.height || newFontSize > maximumFontSize)
                    return (newFontSize - 1, prevWidth, prevHeight);

                prevWidth = size.Width;
                prevHeight = size.Height;
            }
        }

        private static Size MeasureDrawTextSize(string text, Font font, int width, int height) {
            return TextRenderer.MeasureText(text, font, new Size(width, height), TextFormatFlags.NoPadding);
        }
    }
}