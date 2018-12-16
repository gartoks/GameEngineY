using GameEngine.Graphics.Utility;

namespace GameApp.Graphics.Utility {
    internal sealed class ShaderVertexAttribute : VertexAttribute {
        public readonly int AttributeIndex;
        public int ByteOffset;

        internal ShaderVertexAttribute(string name, int attributeIndex, VertexAttributeType type)
            : base(name, type) {

            AttributeIndex = attributeIndex;
            ByteOffset = -1;
        }

        internal void Enable() {
            GLHandler.Instance.EnableVertexAttributeArray(AttributeIndex);
        }

        internal void Disable() {
            GLHandler.Instance.DisableVertexAttributeArray(AttributeIndex);
        }
    }
}