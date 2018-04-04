using System;
using GameEngine.Graphics.RenderSettings;
using OpenTK.Graphics.OpenGL;

namespace GameApp.Graphics.Utility {
    internal static class TextureFilterModeUtils {
        internal static TextureMinFilter ToMinFilter(TextureFilterMode filterMode) {
            switch (filterMode) {
                case TextureFilterMode.Nearest:
                    return TextureMinFilter.Nearest;
                case TextureFilterMode.Linear:
                    return TextureMinFilter.Linear;
                default:
                    throw new ArgumentException();
            }
        }

        internal static TextureMagFilter ToMagFilter(TextureFilterMode filterMode) {
            switch (filterMode) {
                case TextureFilterMode.Nearest:
                    return TextureMagFilter.Nearest;
                case TextureFilterMode.Linear:
                    return TextureMagFilter.Linear;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
