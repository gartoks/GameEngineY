using GameEngine.Logging;

namespace GameEngine.Graphics.Utility {
    public class VertexAttribute {
        public readonly string Name;
        public readonly VertexAttributeType Type;

        public VertexAttribute(string name, VertexAttributeType type) {
            if (string.IsNullOrWhiteSpace(name)) {
                Log.WriteLine("VertexAttribute name must not be null or empty.", LogType.Error);
                return;
            }


            Name = name;
            Type = type;
        }

        public int ComponentCount => (int)Type;

        public override string ToString() {
            return $"{Name}:{Type}";
        }

        public override bool Equals(object obj) {
            return obj != null && obj is VertexAttribute va && Type == va.Type && Name.Equals(va.Name);
        }

        public override int GetHashCode() {
            return 31 * Name.GetHashCode() + Type.GetHashCode();
        }
    }
}