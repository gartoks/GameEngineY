using System.Collections.Generic;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;

namespace GameEngine.Graphics {
    public class MeshBuilder {
        public static MeshBuilder Create(params VertexAttribute[] supportedVertexAttributes) {
            if (supportedVertexAttributes.Length == 0) {
                Log.WriteLine($"There must be at least one vertex attribute in a mesh.", LogType.Error);
                return null;
            }

            return new MeshBuilder(supportedVertexAttributes);
        }

        private readonly VertexAttribute[] supportedAttributes;

        private readonly List<float[][]> vertexData;

        internal MeshBuilder(params VertexAttribute[] supportedAttributes) {
            this.supportedAttributes = supportedAttributes;
            this.vertexData = new List<float[][]>();
        }

        public Mesh Build() {
            if (VertexCount == 0) {
                Log.WriteLine($"No vertices in MeshBuilder.", LogType.Error);
                return null;
            }

            return new Mesh(this.supportedAttributes, this.vertexData.ToArray());
        }

        public void AddVertexaData(params float[][] vertexData) {
            if (vertexData == null) {
                Log.WriteLine($"Vertex data is null.", LogType.Error);
                return;
            }

            if (vertexData.Length != AttributeCount) {
                Log.WriteLine($"Invalid attribute count {vertexData.Length}. {AttributeCount} attributes expected.", LogType.Error);
                return;
            }

            for (int a = 0; a < vertexData.Length; a++) {
                if (vertexData[a] == null) {
                    Log.WriteLine($"Attribute vertex data {a} is null.", LogType.Error);
                    return;
                }

                if (vertexData[a].Length != this.supportedAttributes[a].ComponentCount) {
                    Log.WriteLine($"Invalid attribute vertex data {a} length {vertexData[a].Length}. {this.supportedAttributes[a].ComponentCount} expected.", LogType.Error);
                    return;
                }
            }

            this.vertexData.Add(vertexData);
        }

        public void SetVertexData(int vertexIndex, params float[][] vertexData) {
            if (vertexData == null) {
                Log.WriteLine($"Vertex data is null.", LogType.Error);
                return;
            }

            if (vertexIndex < 0 || vertexIndex >= VertexCount) {
                Log.WriteLine($"Invalid vertex index {vertexIndex}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                return;
            }

            if (vertexData.Length != AttributeCount) {
                Log.WriteLine($"Invalid attribute count {vertexData.Length}. {AttributeCount} attributes expected.", LogType.Error);
                return;
            }

            for (int a = 0; a < vertexData.Length; a++) {
                if (vertexData[a] == null) {
                    Log.WriteLine($"Attribute vertex data {a} is null.", LogType.Error);
                    return;
                }

                if (vertexData[a].Length != this.supportedAttributes[a].ComponentCount) {
                    Log.WriteLine($"Invalid attribute vertex data {a} length {vertexData[a].Length}. {this.supportedAttributes[a].ComponentCount} expected.", LogType.Error);
                    return;
                }
            }

            this.vertexData[vertexIndex] = vertexData;
        }

        public void ClearVertexData() {
            this.vertexData.Clear();
        }

        public int VertexCount => this.vertexData.Count;

        public IEnumerable<VertexAttribute> Attributes => this.supportedAttributes;

        public int AttributeCount => this.supportedAttributes.Length;
    }
}