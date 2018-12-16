using GameEngine.Logging;

namespace GameEngine.Game.Animation {
    public sealed class PingPongTimer : IAnimationTimer {

        public PingPongTimer() { }

        public float Value(float time, float max) {
            if (max <= 0) {
                Log.WriteLine("The ping pong timer maximum must be bigger than zero.", LogType.Warning);
                return float.NaN;
            }

            float div = time / max;
            int loops = (int)div;
            float frac = div - loops;

            float value;
            if (loops % 2 == 0)
                value = frac * max;
            else
                value = max * (1f - frac);

            return value;
        }
    }
}