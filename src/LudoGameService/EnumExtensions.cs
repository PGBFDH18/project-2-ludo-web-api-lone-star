using System;

namespace Ludo.API.Service.Extensions
{
    public static class EnumExtensions
    {
        // Returns true if the value is a named / declared.
        // Remark: Does not work for flags, same as System.Enum.IsDefined.
        public static bool IsDefined<TEnum>(this TEnum value) where TEnum : struct, Enum
            => value.GetType().IsEnumDefined(value);
    }
}
