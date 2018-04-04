namespace GameEngine.Application {
    public interface IApplication {
        void Shutdown();

        string Name { get; }
    }
}