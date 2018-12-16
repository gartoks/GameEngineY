using System.Collections.Generic;

namespace GameEngine.Graphics.Utility {
    public interface IVertexData {

        void SetAttributeData(string vertexAttributeName, params float[] data);

        void SetAttributeData(VertexAttribute vertexAttribute, params float[] data);

        IEnumerable<VertexAttribute> VertexAttributes { get; }
    }
}