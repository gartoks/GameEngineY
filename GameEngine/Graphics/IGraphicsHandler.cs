using System.Collections.Generic;
using System.Drawing;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Utility;
using GameEngine.Math;
using Color = GameEngine.Utility.Color;

namespace GameEngine.Graphics {
    public interface IGraphicsHandler {

        Matrix4 CurrentTransformationMatrix { get; }

        IMesh CreateMesh(int vertexCount, VertexAttribute[] vertexAttributes, (uint idx0, uint idx1, uint idx2)[] clockwiseTriangles);

        IMesh CreateDefaultMesh();

        ITextureAtlas CreateTextureAtlas(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter, IEnumerable<(string regionName, int x, int y, int width, int height)> regionData);

        ITexture CreateTexture(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter);

        ITexture CreateDefaultTexture(int width, int height, Color color);

        IShader CreateShader(string vertexShaderSource, string fragmentShaderSource);

        IShader CreateDefaultShader(int textureCount = 0);

        IRenderable CreateRenderable(ShaderVertexAttributeResolver attributeResolver, ShaderUniformAssignmentHandler shaderUniformAssignmentHandler, IShader shader, IMesh mesh);
    }
}