using System;
using System.Collections.Generic;

namespace Ludo.API.Service.Extensions
{
    public static class TLoopExtensions
    {
        /// <summary>
        /// Expression syntax for-loop. Loops <paramref name="count"/> times.
        /// </summary>
        public static IEnumerable<(T Source, int Index)> Loop<T>(this T source, int count)
        {
            for (int i = 0; i < count; ++i)
                yield return (source, i);
        }

        /// <summary>
        /// Expression syntax for-loop with projection. Loops <paramref name="count"/> times.
        /// </summary>
        public static IEnumerable<TOut> Loop<TIn, TOut>(this TIn source, int count, Func<int, TOut> map)
        {
            for (int i = 0; i < count; ++i)
                yield return map(i);
        }

        /// <summary>
        /// Expression syntax for-loop with projection. Loops <paramref name="count"/> times.
        /// </summary>
        public static IEnumerable<TOut> Loop<TIn, TOut>(this TIn source, int count, Func<TIn, int, TOut> map)
        {
            for (int i = 0; i < count; ++i)
                yield return map(source, i);
        }

        /// <summary>
        /// Expression syntax for-loop. Both start and end are inclusive!
        /// </summary>
        public static IEnumerable<(T Source, int Index)> Loop<T>(this T source, int start, int end)
        {
            for (int i = start; i <= end; ++i)
                yield return (source, i);
        }

        /// <summary>
        /// Expression syntax for-loop with projection. Both start and end are inclusive!
        /// </summary>
        public static IEnumerable<TOut> Loop<TIn, TOut>(this TIn source, int start, int end, Func<int, TOut> map)
        {
            for (int i = start; i <= end; ++i)
                yield return map(i);
        }

        /// <summary>
        /// Expression syntax for-loop with projection. Both start and end are inclusive!
        /// </summary>
        public static IEnumerable<TOut> Loop<TIn, TOut>(this TIn source, int start, int end, Func<TIn, int, TOut> map)
        {
            for (int i = start; i <= end; ++i)
                yield return map(source, i);
        }

        /// <summary>
        /// Expression syntax descending for-loop. Both start and end are inclusive!
        /// </summary>
        public static IEnumerable<(T Source, int Index)> LoopReverse<T>(this T source, int start, int end = 0)
        {
            for (int i = start; i >= end; --i)
                yield return (source, i);
        }

        /// <summary>
        /// Expression syntax descending for-loop with projection. Both start and end are inclusive!
        /// </summary>
        public static IEnumerable<TOut> LoopReverse<TIn, TOut>(this TIn source, int start, Func<int, TOut> map, int end = 0)
        {
            for (int i = start; i >= end; --i)
                yield return map(i);
        }

        /// <summary>
        /// Expression syntax descending for-loop with projection. Both start and end are inclusive!
        /// </summary>
        public static IEnumerable<TOut> LoopReverse<TIn, TOut>(this TIn source, int start, Func<TIn, int, TOut> map, int end = 0)
        {
            for (int i = start; i >= end; --i)
                yield return map(source, i);
        }
    }
}
