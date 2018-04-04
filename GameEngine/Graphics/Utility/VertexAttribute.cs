using System;

namespace GameEngine.Graphics.Utility {
    public class VertexAttribute {
        public readonly string Name;
        public readonly int ComponentCount;

        public VertexAttribute(string name, int componentCount) {
            Name = name ?? throw new ArgumentNullException("VertexAttribute name must not be null.");
            ComponentCount = componentCount;
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            VertexAttribute other = obj as VertexAttribute;

            return ComponentCount == other.ComponentCount && Name.Equals(other.Name);
        }

        public override int GetHashCode() {
            return 31 * Name.GetHashCode() + ComponentCount.GetHashCode();
        }
    }
}