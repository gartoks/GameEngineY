using System;
using GameEngine.Graphics.Utility;

namespace GameApp.Graphics.Utility {
    internal sealed class ShaderUniform : IUniform {
        public string Name { get; }
        public UniformType Type { get; }
        public int UniformIndex { get; }
        public int ComponentCount { get; }

        public ShaderUniform(string name, UniformType type, int uniformIndex, int componentCount) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            UniformIndex = uniformIndex;
            ComponentCount = componentCount;
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            ShaderUniform other = obj as ShaderUniform;

            return ComponentCount == other.ComponentCount && Name.Equals(other.Name);
        }

        public override int GetHashCode() {
            return 31 * Name.GetHashCode() + ComponentCount.GetHashCode();
        }

        public override string ToString() {
            return $"{Name}:{Type}:{ComponentCount}";
        }
    }
}