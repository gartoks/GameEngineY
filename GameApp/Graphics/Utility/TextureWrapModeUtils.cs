using System;
using GameEngine.Graphics.RenderSettings;

namespace GameApp.Graphics.Utility {
    internal static class TextureWrapModeUtils {
        internal static OpenTK.Graphics.OpenGL.TextureWrapMode ToWrapMode(TextureWrapMode wrapMode) {
            switch (wrapMode) {
                case TextureWrapMode.Repeat:
                    return OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
                case TextureWrapMode.Clamp:
                    return OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp;
                case TextureWrapMode.ClampToEdge:
                    return OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
