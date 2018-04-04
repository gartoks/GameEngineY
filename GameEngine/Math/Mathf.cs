using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.Math {
    public static class Mathf {
        public const float PI = (float)System.Math.PI;
        public const float TWO_PI = (float)(2.0 * System.Math.PI);
        public const float HALF_PI = (float)(System.Math.PI / 2.0);
        public const float QUARTER_PI = (float)(System.Math.PI / 4.0);

        public const float DEG2RAD = PI / 180f;
        public const float RAD2DEG = 180f / PI;

        public static float Sqrt(float v) {
            return (float)System.Math.Sqrt(v);
        }

        public static float Log(float v) {
            return (float)System.Math.Log(v);
        }

        public static float Log(float v, float newBase) {
            return (float)System.Math.Log(v, newBase);
        }

        public static float NthRt(float v, float n) {
            return (float)System.Math.Pow(v, 1f / n);
        }

        public static float Pow(float v, float n) {
            return (float)System.Math.Pow(v, n);
        }

        public static float Exp(float v) {
            return (float)System.Math.Exp(v);
        }

        public static float Sin(float v) {
            return (float)System.Math.Sin(v);
        }

        public static float Cos(float v) {
            return (float)System.Math.Cos(v);
        }

        public static float Tan(float v) {
            return (float)System.Math.Tan(v);
        }

        public static float Asin(float v) {
            return (float)System.Math.Asin(v);
        }

        public static float Acos(float v) {
            return (float)System.Math.Acos(v);
        }

        public static float Atan(float v) {
            return (float)System.Math.Atan(v);
        }

        public static float Atan2(float y, float x) {
            return (float)System.Math.Atan2(y, x);
        }

        public static float Clamp01(float value) {
            return Clamp(value, 0f, 1f);
        }

        public static float Clamp(float value, float min, float max) {
            return value > max ? max : (value < min ? min : value);
        }

        public static int Clamp(int v, int min, int max) {
            return v > max ? max : (v < min ? min : v);
        }

        public static int FloorToInt(float v) {
            return (v > 0) ? ((int)v) : (((int)v) - 1);
        }

        public static int CeilToInt(float v) {
            return v > 0 ? ((int)v) + 1 : (int)v;
        }

        public static int RoundToInt(float v) {
            return FloorToInt(v + 0.5f);
        }

        public static int Mod(int v, int m) {
            int a = v % m;
            return a < 0 ? a + m : a;
        }

        public static float NormalizeAngle(this float a) {
            while (a < 0)
                a += TWO_PI;

            if (a >= TWO_PI)
                a = a % TWO_PI;

            return a;
        }

        public static void NormalizeAngle(ref float a) {
            while (a < 0)
                a += TWO_PI;

            if (a >= TWO_PI)
                a = a % TWO_PI;
        }

        public static float NormalDistributionProbabilityDensity(float x, float mean, float stdDeviation) {
            float a = (x - mean) * (x - mean);
            float b = 2 * stdDeviation * stdDeviation;
            return (1f / Sqrt(b * PI)) * Exp(-a / b);
        }

        public static (float x, float y) PolarToCarthesianCoordinates(float radius, float angle) {
            return (radius * Cos(angle), radius * Sin(angle));
        }

        public static (float radius, float angle) CarthesianToPolarCoordinates(float y, float x) {
            return (Sqrt(x * x + y * y), Atan2(y, x));
        }

        public static float CalculateArea(IEnumerable<Vector2> points) {
            int numPoints = points.Count();

            float area = 0;
            for (int i = 0; i < numPoints; i++) {
                int j = (i + 1) % numPoints;

                Vector2 p0 = points.ElementAt(i);
                Vector2 p1 = points.ElementAt(j);

                area += p0.x * p1.y;
                area -= p0.x * p1.y;
            }

            area /= 2f;
            return area;
        }

        public static string ToRoman(int number) {
            if ((number < 0) || (number > 3999))
                throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1)
                return string.Empty;
            if (number >= 1000)
                return "M" + ToRoman(number - 1000);
            if (number >= 900)
                return "CM" + ToRoman(number - 900); //EDIT: i've typed 400 instead 900
            if (number >= 500)
                return "D" + ToRoman(number - 500);
            if (number >= 400)
                return "CD" + ToRoman(number - 400);
            if (number >= 100)
                return "C" + ToRoman(number - 100);
            if (number >= 90)
                return "XC" + ToRoman(number - 90);
            if (number >= 50)
                return "L" + ToRoman(number - 50);
            if (number >= 40)
                return "XL" + ToRoman(number - 40);
            if (number >= 10)
                return "X" + ToRoman(number - 10);
            if (number >= 9)
                return "IX" + ToRoman(number - 9);
            if (number >= 5)
                return "V" + ToRoman(number - 5);
            if (number >= 4)
                return "IV" + ToRoman(number - 4);
            if (number >= 1)
                return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

    }
}