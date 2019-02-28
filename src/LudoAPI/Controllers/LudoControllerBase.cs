using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ludo.WebAPI.Controllers
{
    public abstract class LudoControllerBase : ControllerBase
    {
        protected internal const string ROUTE_gameId = "{gameId:required:minLength(2)}";
        protected internal const string ROUTE_slotStr = "{slotStr:required}";
        
        protected LudoControllerBase() { }

        // ...warning, use with care...
        protected string AppBaseUrl
        => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected dynamic Status(int statusCode)
        {
            Response.StatusCode = statusCode;
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool TryParseSlot(string slotStr, out int slot, bool allowNegative = false)
            => int.TryParse(slotStr, allowNegative ? NumberStyles.AllowLeadingSign : NumberStyles.None
                , CultureInfo.InvariantCulture, out slot);
    }
}
