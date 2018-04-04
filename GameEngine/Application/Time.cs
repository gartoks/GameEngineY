using GameEngine.Modding;

namespace GameEngine.Application {
    public static class Time {

        /// <summary>
        /// Gets the time between updates.
        /// </summary>
        /// <value>
        /// The time between updates.
        /// </value>
        public static float DeltaTime => ModBase.TimeManager.DeltaTime;

        /// <summary>
        /// Gets the time since the application started.
        /// </summary>
        /// <value>
        /// The time since the application started.
        /// </value>
        public static float TimeSinceStart => ModBase.TimeManager.TimeSinceStart;

    }
}