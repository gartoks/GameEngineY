using System.Collections.Generic;
using GameEngine.Graphics.Utility;

namespace GameEngine.Graphics {
    public interface IMesh {

        IVertexData GetVertexData(int vertexIndex);

        int TriangleCount { get; }

        int VertexCount { get; }

        IEnumerable<VertexAttribute> VertexAttributes { get; }

        int AttributeCount { get; }
    }
}