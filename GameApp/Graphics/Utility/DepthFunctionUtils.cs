using System;
using GameEngine.Graphics.RenderSettings;

namespace GameApp.Graphics.Utility {
    internal static class DepthFunctionUtils {
        internal static OpenTK.Graphics.OpenGL.DepthFunction ToDepthFunctions(DepthFunction function) {
            switch (function) {
                case DepthFunction.Always:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Always;
                case DepthFunction.Equal:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Equal;
                case DepthFunction.Gequal:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Gequal;
                case DepthFunction.Greater:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Greater;
                case DepthFunction.Lequal:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Lequal;
                case DepthFunction.Less:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Less;
                case DepthFunction.Never:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Never;
                case DepthFunction.Notequal:
                    return OpenTK.Graphics.OpenGL.DepthFunction.Notequal;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
