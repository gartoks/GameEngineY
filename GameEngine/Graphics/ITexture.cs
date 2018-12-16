using System.Drawing;
using GameEngine.Graphics.RenderSettings;

namespace GameEngine.Graphics {
    public interface ITexture {
        int Width { get; }

        int Height { get; }

        void UpdateData(Bitmap bitmap);

        TextureFilterMode MagFilter { get; set; }

        TextureFilterMode MinFilter { get; set; }

        TextureWrapMode WrapS { get; set; }

        TextureWrapMode WrapT { get; set; }

        float[][] TextureCoordinates { get; }

        bool IsDisposed { get; }
    }
}