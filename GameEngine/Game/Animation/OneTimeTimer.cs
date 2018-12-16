using GameEngine.Logging;
using GameEngine.Math;

namespace GameEngine.Game.Animation {
    public sealed class OneTimeTimer : IAnimationTimer {

        public OneTimeTimer() { }

        public float Value(float time, float max) {
            if (max < 0) {
                Log.WriteLine("The oen time timer maximum must be bigger than zero.", LogType.Warning);
                return float.NaN;
            }

            time = Mathf.Clamp(time, 0, max);

            return time;
        }
    }
}