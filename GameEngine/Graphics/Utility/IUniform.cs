namespace GameEngine.Graphics.Utility {
    public interface IUniform {
        string Name { get; }
        UniformType Type { get; }
        int ComponentCount { get; }
    }
}