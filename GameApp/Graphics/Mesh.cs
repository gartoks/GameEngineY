using System;
using System.Collections.Generic;
using System.Linq;
using GameApp.Graphics.Utility;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;

namespace GameApp.Graphics {
    internal class Mesh : IMesh {
        public delegate void MeshVertexDataChangedEventHandler(Mesh mesh);
        public delegate void MeshIndicesChangedEventHandler(Mesh mesh);

        private readonly VertexAttribute[] vertexAttributes;
        private readonly VertexData[] vertexData;
        private readonly int interleavedVertexDataLength;

        private readonly int[] indices;

        internal event MeshVertexDataChangedEventHandler OnMeshVertexDataChanged;

        internal Mesh(int vertexCount, VertexAttribute[] vertexAttributes, (uint idx0, uint idx1, uint idx2)[] clockwiseIndices) {
            if (vertexCount <= 0) {
                Log.WriteLine($"Cannot create mesh. Invalid vertex count ({vertexCount}). Must be at least one.", LogType.Error);
                return;
            }

            this.vertexAttributes = vertexAttributes;

            this.vertexData = new VertexData[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                this.vertexData[i] = new VertexData(vertexAttributes, vertexData => OnMeshVertexDataChanged?.Invoke(this));

            this.interleavedVertexDataLength = vertexCount * this.vertexData[0].InterleavedVertexDataLength;

            this.indices = new int[clockwiseIndices.Length * 3];
            for (int i = 0; i < clockwiseIndices.Length; i++) {
                (uint idx0, uint idx1, uint idx2) triangle = clockwiseIndices[i];
                if (triangle.idx0 >= VertexCount) {
                    Log.WriteLine($"Invalid index {triangle.idx0}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                    return;
                }

                if (triangle.idx1 >= VertexCount) {
                    Log.WriteLine($"Invalid index {triangle.idx1}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                    return;
                }

                if (triangle.idx2 >= VertexCount) {
                    Log.WriteLine($"Invalid index {triangle.idx2}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                    return;
                }

                this.indices[i * 3 + 0] = (int)triangle.idx0;
                this.indices[i * 3 + 1] = (int)triangle.idx1;
                this.indices[i * 3 + 2] = (int)triangle.idx2;
            }
        }

        public VertexData GetVertexData(int vertexIndex) {
            if (vertexIndex < 0 || vertexIndex >= this.vertexData.Length) {
                Log.WriteLine($"Invalid vertex index {vertexIndex}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                return null;
            }

            return this.vertexData[vertexIndex];
        }

        IVertexData IMesh.GetVertexData(int vertexIndex) => GetVertexData(vertexIndex);

        internal float[] GetInterleavedVertexData(IEnumerable<VertexAttribute> attributesInOrder) {
            if (attributesInOrder.Count() != this.vertexAttributes.Length) {
                Log.WriteLine($"Invalid attributes to interleave. Count does not match.", LogType.Error);
                return null;
            }

            foreach (VertexAttribute att in attributesInOrder) {
                if (!this.vertexAttributes.Contains(att)) {
                    Log.WriteLine($"Invalid attribute '{att.Name}' to interleave. Attribute not found in mesh.", LogType.Error);
                    return null;
                }
            }

            float[] interleavedVertexData = new float[this.interleavedVertexDataLength];

            int interleavedIndex = 0;
            for (int v = 0; v < VertexCount; v++) {
                int interleavedLength = this.vertexData[v].InterleavedVertexDataLength;
                Array.Copy(this.vertexData[v].InterleavedVertexData(attributesInOrder), 0, interleavedVertexData, interleavedIndex, interleavedLength);
                interleavedIndex += interleavedLength;
            }

            return interleavedVertexData;
        }

        public IEnumerable<VertexAttribute> VertexAttributes => this.vertexAttributes;

        internal int[] Indices => this.indices.ToArray();

        public int IndiceCount => this.indices.Length;

        public int VertexCount => this.vertexData.Length;

        public int AttributeCount => this.vertexAttributes.Length;

        public int TriangleCount => this.indices.Length / 3;
    }
}