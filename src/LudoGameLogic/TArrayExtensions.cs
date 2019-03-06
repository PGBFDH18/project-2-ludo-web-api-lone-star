using System;

namespace Ludo.GameLogic.Extensions
{
    public static class TArrayExtensions
    {
        public static T[] Copy<T>(this T[] source)
        {
            T[] copy = new T[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }

        public static T[][] JaggedCopy<T>(this T[,] source)
        {
            var tt = new T[source.GetLength(0)][];
            int yMax = source.GetLength(1);
            for (int x = 0; x < tt.Length; ++x)
            {
                var t = new T[yMax];
                for (int y = 0; y < t.Length; ++y)
                    t[y] = source[x, y];
                tt[x] = t;
            }
            return tt;
        }

        public static T[][] JaggedCopy<T>(this T[][] source)
        {
            var tt = new T[source.Length][];
            for (int x = 0; x < tt.Length; ++x)
            {
                var s = source[x];
                if (s != null)
                    tt[x] = Copy(s);
            }
            return tt;
        }
    }
}
