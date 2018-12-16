using System.Collections.Generic;
using GameEngine.Graphics.Utility;

namespace GameEngine.Graphics {
    public interface IShader {

        bool HasUniform(string uniformName);
        bool HasAttribute(string attributeName);

        IEnumerable<IUniform> Uniforms { get; }
        IEnumerable<VertexAttribute> Attributes { get; }

        bool IsCompiled { get; }

        bool Equals(object obj);
        int GetHashCode();
    }
}