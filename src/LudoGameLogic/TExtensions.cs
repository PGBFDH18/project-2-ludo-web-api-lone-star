using System;

namespace Ludo.GameLogic.Extensions
{
    public static class TExtension
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

        // Runs the action if the condition evaluates to true, and passes through the condition value.
        public static bool OnTrue(this bool condition, Action action)
        {
            if (condition)
                action();
            return condition;
        }

        // Runs the action if the condition evaluates to false, and passes through the condition value.
        public static bool OnFalse(this bool condition, Action action)
        {
            if (!condition)
                action();
            return condition;
        }
    }
}