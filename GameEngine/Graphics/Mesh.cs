using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;

namespace GameEngine.Graphics {
    public class Mesh {
        private readonly VertexAttribute[] vertexAttributes;
        private readonly float[][][] vertexData;
        private readonly List<uint> indices;

        internal Mesh(VertexAttribute[] vertexAttributes, float[][][] vertexData) {
            this.vertexAttributes = vertexAttributes;
            this.vertexData = vertexData;
            this.indices = new List<uint>();
        }

        public void AddTriangle(uint index1, uint index2, uint index3) {
            if (index1 >= VertexCount) {
                Log.WriteLine($"Invalid index {index1}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                return;
            }

            if (index2 >= VertexCount) {
                Log.WriteLine($"Invalid index {index2}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                return;
            }

            if (index3 >= VertexCount) {
                Log.WriteLine($"Invalid index {index3}. Must be in range of [0, {VertexCount - 1}]", LogType.Error);
                return;
            }

            this.indices.Add(index1);
            this.indices.Add(index2);
            this.indices.Add(index3);
        }

        public void ClearIndices() {
            this.indices.Clear();
        }

        public float[] GetInterleavedVertexData(IEnumerable<VertexAttribute> attributesInOrder) {
            if (attributesInOrder.Count() != this.vertexAttributes.Length) {
                Log.WriteLine($"Invalid attributes to interleave. Count does not match.", LogType.Error);
                return null;
            }

            int interleavedArrayLength = 0;
            foreach (VertexAttribute att in attributesInOrder) {
                if (!this.vertexAttributes.Contains(att)) {
                    Log.WriteLine($"Invalid attribute '{att.Name}' to interleave. Attribute not found in mesh.", LogType.Error);
                    return null;
                }

                interleavedArrayLength += att.ComponentCount;
            }
            interleavedArrayLength *= VertexCount;

            float[] interleavedVertexData = new float[interleavedArrayLength];

            int i = 0;
            for (int v = 0; v < VertexCount; v++) {
                foreach (VertexAttribute att in attributesInOrder) {
                    int a = Array.IndexOf(this.vertexAttributes, att);
                    for (int ac = 0; ac < AttributeComponents(a); ac++) {
                        interleavedVertexData[i] = this.vertexData[v][a][ac];
                        i++;
                    }
                }
            }

            return interleavedVertexData;
        }

        public IEnumerable<VertexAttribute> VertexAttributes => this.vertexAttributes;

        public IEnumerable<uint> Indices => this.indices;

        public int VertexCount => this.vertexData.Length;

        public int AttributeCount => this.vertexData[0].Length;

        public int AttributeComponents(int attributeIndex) {
            if (attributeIndex < 0 || attributeIndex >= AttributeCount) {
                Log.WriteLine($"Invalid attribute index. Must be in range of [0, {AttributeCount - 1}]", LogType.Error);
                return -1;
            }

            return this.vertexData[0][attributeIndex].Length;
        }
    }
}