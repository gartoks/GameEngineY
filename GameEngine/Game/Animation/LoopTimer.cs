﻿using GameEngine.Logging;

namespace GameEngine.Game.Animation {
    public sealed class LoopTimer : IAnimationTimer {

        public LoopTimer() { }

        public float Value(float time, float max) {
            if (max < 0) {
                Log.WriteLine("The loop timer maximum must be bigger than zero.", LogType.Warning);
                return float.NaN;
            }

            float div = time / max;
            int loops = (int)div;
            float frac = div - loops;

            return frac * max;
        }

    }
}