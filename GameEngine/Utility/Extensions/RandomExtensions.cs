using System;
using GameEngine.Math;

namespace GameEngine.Utility.Extensions {
    public static class RandomExtensions {
        public static float NextFloat(this Random rand) {
            return (float)rand.NextDouble();
        }

        public static float NextFloat(this Random rand, float min, float max) {
            return min + (rand.NextFloat() * (max - min));
        }

        public static float NextGaussian(this Random rand, float mean = 0, float stdDev = 1) {
            float u1 = rand.NextFloat();
            float u2 = rand.NextFloat();
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        public static float NextAngle(this Random rand) {
            return rand.NextFloat() * 2.0f * Mathf.PI;
        }

        public static void NextRandomInCircleUniformly(this Random rand, float radius, out float x, out float y) {
            float angle = rand.NextAngle();
            float r = Mathf.Sqrt(rand.NextFloat()) * radius;
            x = r * Mathf.Cos(angle);
            y = r * Mathf.Sin(angle);
        }

        public static (float x, float y) NextRandomInCircleCentered(this Random rand, float radius) {
            float angle = rand.NextAngle();
            float r = rand.NextFloat() * radius;
            float x = r * Mathf.Cos(angle);
            float y = r * Mathf.Sin(angle);

            return (x, y);
        }
    }
}