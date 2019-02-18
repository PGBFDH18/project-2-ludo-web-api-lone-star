using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Ludo.WebAPI
{
    public abstract class ExtendedControllerBase : ControllerBase
    {
        protected ExtendedControllerBase() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected dynamic Status(int statusCode)
        {
            Response.StatusCode = statusCode;
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T Status<T>(int statusCode, T value = default(T))
        {
            Response.StatusCode = statusCode;
            return value;
        }
    }
}
