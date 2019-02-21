using System;
using System.Collections.Generic;

namespace Ludo.GameService
{
    internal static class TExtension
    {
        // Q: when would you need this?
        // A: When you need to create a local copy of a variable in an expression body for thread safety.
        //
        //    E.g. in cases where this would be a race condition:
        //      => t == null ? someValue : Process(t.OtherValue);
        //
        //    And because OtherValue needs "processing" but someValue does not we can't use:
        //      t?.OtherValue ?? someValue
        //
        //    OtherValue and someValue might not even be of the same type.
        //    Having an out parameter allows us to declare a local in the expression body.
        //    Thus with this extension we can write:
        //      => t.IsNull(out var tLocal) ? someValue : Process(tLocal.OtherValue);
        //
        public static bool IsNull<T>(this T tIn, out T tLocal)
            => (tLocal = tIn) == null;

        // Makes a copy of the array, optionally resizing it in the process
        public static T[] CopyResize<T>(this T[] source, int sizeDiff = 0)
        {
            T[] copy = new T[source.Length + sizeDiff];
            Array.Copy(source, copy, (sizeDiff < 0 ? copy : source).Length);
            return copy;
        }

        // Makes a copy of the array.
        public static T[] Copy<T>(this T[] source)
        {
            T[] copy = new T[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }

        // Modifies one array element and returns the (same) array instance.
        public static T[] Modify<T>(this T[] source, int index, T value)
        {
            source[index] = value;
            return source;
        }

        // Makes a copy of the array that contains one more value than the source.
        public static T[] CopyAppend<T>(this T[] source, T value)
            => source.CopyResize(1).Modify(source.Length, value);

        // Make a copy from which the i'th element has been excluded.
        public static T[] CopyRemoveAt<T>(this T[] source, int index)
        {
            T[] copy = new T[source.Length - 1];
            Array.Copy(source, copy, index);
            Array.Copy(source, index + 1, copy, index, copy.Length - index);
            return copy;
        }

        // Makes a copy from which the first occurence of the specified value has been excluded.
        // (Optimized for the asumption that the value exists at least once - slow if that's not the case.)
        public static T[] CopyRemoveFirst<T>(this T[] source, T value, IEqualityComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;
            T[] copy = new T[source.Length - 1];
            int iv = -1;
            for (int i = 0; i < copy.Length; ++i)
            {
                if (comparer.Equals(source[i], value)) // search...
                {
                    iv = i;
                    break;
                }
                copy[i] = source[i]; // ...and copy
            }
            if (iv == -1) // the value was never found.... :(
            {
                copy = null; // (allow memory to be freed)
                return source.Copy();
            }
            Array.Copy(source, iv + 1, copy, iv, copy.Length - iv);
            return copy;
        }
    }
}
