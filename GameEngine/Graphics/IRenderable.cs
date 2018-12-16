using System.Collections.Generic;
using GameEngine.Graphics.Utility;

namespace GameEngine.Graphics {
    public delegate VertexAttribute ShaderVertexAttributeResolver(VertexAttribute shaderAttribute, IEnumerable<VertexAttribute> meshAttributes);
    public delegate void ShaderUniformAssignmentHandler(IShaderUniformAssigner uniformAssigner, IUniform shaderUniform);
    public interface IRenderable {
        IMesh Mesh { get; set; }

        IShader Shader { get; set; }
    }
}