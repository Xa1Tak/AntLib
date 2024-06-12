using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AntLib.Tools
{
    public static class StaticRandom
    {
        private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() => new Random());
        public static decimal GetNextDeccimal() => (decimal)randomWrapper.Value.NextDouble();
        public static int GetNextInt(int min, int max) => randomWrapper.Value.Next(min, max);
        public static int GetNextInt() => randomWrapper.Value.Next();
        public static double GetNextDouble() => randomWrapper.Value.NextDouble();
        public static float GetNextFloat() => randomWrapper.Value.NextSingle();
        public static double GetNextDouble(double from, double to) => randomWrapper.Value.NextDouble() * (to - from) + from;
        public static float GetNextFloat(float from, float to) => randomWrapper.Value.NextSingle() * (to - from) + from;
        public static Random GetRandom() => randomWrapper.Value;
    }
}
