namespace GameEngine.Application {
    public interface ITimeManager {
        /// <summary>
        /// Gets the time between updates.
        /// </summary>
        /// <value>
        /// The time between updates.
        /// </value>
        float DeltaTime { get; }

        /// <summary>
        /// Gets the time since the application started.
        /// </summary>
        /// <value>
        /// The time since the application started.
        /// </value>
        float TimeSinceStart { get; }

        int UpdatesPerSecond { get; }
    }
}