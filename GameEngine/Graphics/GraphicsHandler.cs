using System.Collections.Generic;
using System.Drawing;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Utility;
using GameEngine.Math;
using GameEngine.Modding;
using Color = GameEngine.Utility.Color;

namespace GameEngine.Graphics {
    public static class GraphicsHandler {
        public static Matrix4 CurrentTransformationMatrix => ModBase.GraphicsHandler.CurrentTransformationMatrix;

        public static IMesh CreateMesh(int vertexCount, VertexAttribute[] vertexAttributes, (uint idx0, uint idx1, uint idx2)[] clockwiseTriangles)
            => ModBase.GraphicsHandler.CreateMesh(vertexCount, vertexAttributes, clockwiseTriangles);

        public static ITextureAtlas CreateTextureAtlas(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter, IEnumerable<(string regionName, int x, int y, int width, int height)> regionData)
            => ModBase.GraphicsHandler.CreateTextureAtlas(bitmap, wrapS, wrapT, minFiler, magFilter, regionData);

        public static ITexture CreateTexture(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter)
            => ModBase.GraphicsHandler.CreateTexture(bitmap, wrapS, wrapT, minFiler, magFilter);

        public static ITexture CreateDefaultTexture(int width, int height, Color color)
            => ModBase.GraphicsHandler.CreateDefaultTexture(width, height, color);

        public static IShader CreateShader(string vertexShaderSource, string fragmentShaderSource)
            => ModBase.GraphicsHandler.CreateShader(vertexShaderSource, fragmentShaderSource);

        public static IShader CreateDefaultShader(int textureCount = 0)
            => ModBase.GraphicsHandler.CreateDefaultShader(textureCount);

        public static IRenderable CreateRenderable(ShaderVertexAttributeResolver attributeResolver, ShaderUniformAssignmentHandler shaderUniformAssignmentHandler, IShader shader, IMesh mesh)
            => ModBase.GraphicsHandler.CreateRenderable(attributeResolver, shaderUniformAssignmentHandler, shader, mesh);

    }
}