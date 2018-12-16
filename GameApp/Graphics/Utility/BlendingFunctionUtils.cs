using System;
using GameEngine.Graphics.RenderSettings;
using OpenTK.Graphics.OpenGL;

namespace GameApp.Graphics.Utility {
    internal static class BlendingModeUtils {
        internal static (BlendFunction source, BlendFunction destination)? ModeToFunctions(BlendMode blendMode) {
            switch (blendMode) {
                case BlendMode.Default:
                    return (BlendFunction.SourceAlpha, BlendFunction.OneMinusSourceAlpha);
                case BlendMode.Replace:
                    return (BlendFunction.One, BlendFunction.Zero);
                case BlendMode.Additive:
                    return (BlendFunction.SourceAlpha, BlendFunction.One);
                case BlendMode.Overlay:
                    return (BlendFunction.DestinationColor, BlendFunction.Zero);
                case BlendMode.Premultiplied:
                    return (BlendFunction.One, BlendFunction.OneMinusSourceAlpha);
                default:
                    return null;
            }
        }

        internal static (BlendingFactorSrc source, BlendingFactorDest destination) ToBlendFunctions((BlendFunction source, BlendFunction destination) functions) {
            BlendingFactorSrc source;
            BlendingFactorDest destination;

            switch (functions.source) {
                case BlendFunction.Zero:
                    source = BlendingFactorSrc.Zero;
                    break;
                case BlendFunction.One:
                    source = BlendingFactorSrc.One;
                    break;
                case BlendFunction.SourceAlpha:
                    source = BlendingFactorSrc.SrcAlpha;
                    break;
                case BlendFunction.OneMinusSourceAlpha:
                    source = BlendingFactorSrc.OneMinusSrcAlpha;
                    break;
                case BlendFunction.SourceColor:
                    source = BlendingFactorSrc.SrcColor;
                    break;
                case BlendFunction.OneMinusSourceColor:
                    source = BlendingFactorSrc.OneMinusSrcColor;
                    break;
                case BlendFunction.DestinationAlpha:
                    source = BlendingFactorSrc.DstAlpha;
                    break;
                case BlendFunction.OneMinusDestinationAlpha:
                    source = BlendingFactorSrc.OneMinusDstAlpha;
                    break;
                case BlendFunction.DestinationColor:
                    source = BlendingFactorSrc.DstColor;
                    break;
                case BlendFunction.OneMinusDestinationColor:
                    source = BlendingFactorSrc.OneMinusDstColor;
                    break;
                default:
                    throw new ArgumentException();
            }

            switch (functions.destination) {
                case BlendFunction.Zero:
                    destination = BlendingFactorDest.Zero;
                    break;
                case BlendFunction.One:
                    destination = BlendingFactorDest.One;
                    break;
                case BlendFunction.SourceAlpha:
                    destination = BlendingFactorDest.SrcAlpha;
                    break;
                case BlendFunction.OneMinusSourceAlpha:
                    destination = BlendingFactorDest.OneMinusSrcAlpha;
                    break;
                case BlendFunction.SourceColor:
                    destination = BlendingFactorDest.SrcColor;
                    break;
                case BlendFunction.OneMinusSourceColor:
                    destination = BlendingFactorDest.OneMinusSrcColor;
                    break;
                case BlendFunction.DestinationAlpha:
                    destination = BlendingFactorDest.DstAlpha;
                    break;
                case BlendFunction.OneMinusDestinationAlpha:
                    destination = BlendingFactorDest.OneMinusDstAlpha;
                    break;
                case BlendFunction.DestinationColor:
                    destination = BlendingFactorDest.DstColor;
                    break;
                case BlendFunction.OneMinusDestinationColor:
                    destination = BlendingFactorDest.OneMinusDstColor;
                    break;
                default:
                    throw new ArgumentException();
            }

            return (source, destination);
        }
    }
}
