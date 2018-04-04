using System;
using GameEngine.Graphics.RenderSettings;

namespace GameApp.Graphics.Utility {
    internal static class AntiAliasModeUtils {
        internal static OpenTK.Graphics.OpenGL.HintMode ToHint(AntiAliasMode aaMode) {
            switch (aaMode) {
                case AntiAliasMode.DontCare:
                    return OpenTK.Graphics.OpenGL.HintMode.DontCare;
                case AntiAliasMode.Fastest:
                    return OpenTK.Graphics.OpenGL.HintMode.Fastest;
                case AntiAliasMode.Nicest:
                    return OpenTK.Graphics.OpenGL.HintMode.Nicest;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
