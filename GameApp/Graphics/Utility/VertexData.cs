using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;

namespace GameApp.Graphics.Utility {
    public class VertexData : IVertexData {
        public delegate void VertexDataChangedEventHandler(VertexData vertexData);

        private readonly Dictionary<VertexAttribute, float[]> vertexAttributeData;
        private readonly VertexDataChangedEventHandler dataChangedCallback;

        private readonly int interleavedVertexDataLength;

        internal VertexData(VertexAttribute[] vertexattributes, VertexDataChangedEventHandler dataChangedCallback) {
            this.vertexAttributeData = new Dictionary<VertexAttribute, float[]>();
            this.dataChangedCallback = dataChangedCallback;

            this.interleavedVertexDataLength = 0;
            foreach (VertexAttribute va in vertexattributes) {
                this.vertexAttributeData[va] = new float[va.ComponentCount];
                interleavedVertexDataLength += va.ComponentCount;
            }
        }

        public void SetAttributeData(string vertexAttributeName, params float[] data) {
            IEnumerable<VertexAttribute> vas = vertexAttributeData.Keys.Where(va => va.Name.Equals(vertexAttributeName));

            int matchingVAs = vas.Count();

            if (matchingVAs == 0) {
                Log.WriteLine($"Vertex attribute of name {vertexAttributeName} does not exist for this vertex.", LogType.Error);
                return;
            }

            if (matchingVAs != 1) {
                Log.WriteLine($"Ambiguous attribute of name {vertexAttributeName}.", LogType.Error);
                return;
            }

            SetAttributeData(vas.Single(), data);
        }


        public void SetAttributeData(VertexAttribute vertexAttribute, params float[] data) {
            if (data == null || vertexAttribute == null) {
                Log.WriteLine($"Arguments null.", LogType.Error);
                return;
            }

            if (!vertexAttributeData.ContainsKey(vertexAttribute)) {
                Log.WriteLine($"VertexAttribute ({vertexAttribute.Name}:{vertexAttribute.ComponentCount}) does not exist for this vertex.", LogType.Error);
                return;
            }

            if (vertexAttribute.ComponentCount != data.Length) {
                Log.WriteLine($"Vertex data component count does not match required vertex attribute ({vertexAttribute.Name}:{vertexAttribute.Type}:{vertexAttribute.ComponentCount}) component count.", LogType.Error);
                return;
            }

            Array.Copy(data, this.vertexAttributeData[vertexAttribute], data.Length);

            dataChangedCallback(this);
        }

        internal float[] InterleavedVertexData(IEnumerable<VertexAttribute> attributesInOrder) {
            float[] interleavedVertexData = new float[InterleavedVertexDataLength];
            int interleavedIndex = 0;
            foreach (VertexAttribute vertexAttribute in attributesInOrder) {
                if (!this.vertexAttributeData.TryGetValue(vertexAttribute, out float[] attributeData)) {
                    Log.WriteLine($"Cannot interleave vertex data, vertex attribute {vertexAttribute.Name}:{vertexAttribute.ComponentCount} is not present in this vertex data object.", LogType.Error);
                    return null;
                }

                Array.Copy(attributeData, 0, interleavedVertexData, interleavedIndex, vertexAttribute.ComponentCount);
                interleavedIndex += vertexAttribute.ComponentCount;
            }

            return interleavedVertexData;
        }

        public IEnumerable<VertexAttribute> VertexAttributes => this.vertexAttributeData.Keys;

        internal int InterleavedVertexDataLength => this.interleavedVertexDataLength;
    }
}