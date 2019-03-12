using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Runtime.CompilerServices;
using Codes = Ludo.API.Models.Error.Codes;

namespace Ludo.API.Web.Controllers
{
    public abstract class LudoControllerBase : ControllerBase
    {
        protected internal const string ROUTE_gameId = "{gameId:required:minLength(3)}";
        protected internal const string ROUTE_slotStr = "{slotStr:required:maxLength(2)}";
        protected internal const string ROUTE_pieceIndex = "{pieceIndex:required:int:range(0,3)}";
        /* Warning
         * Don't use constraints for input validation.
         * If constraints are used for input validation, invalid input results
         * in a 404 - Not Found response instead of a 400 - Bad Request with an
         * appropriate error message. Route constraints are used to
         * disambiguate similar routes, not to validate the inputs for a
         * particular route.
         */ //MSDN


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

        // -------------------------------------------------------------------
        // Realized I was doing these boilerplate patterns over and over...

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool TryParse(string str, out int result, bool allowNegative = false)
            => int.TryParse(str, allowNegative ? NumberStyles.AllowLeadingSign : NumberStyles.None
                , CultureInfo.InvariantCulture, out result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ActionResult<T> OkOrNotFoundOrConflict<T>(in T value, in Models.Error err)
            => err.Code == Codes.E00NoError
            ? new ActionResult<T>(value)
            : NotFoundOrConflict(in err);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ActionResult NoContentOrNotFoundOrConflict(in Models.Error err)
            => err.Code == Codes.E00NoError
            ? NoContent()
            : NotFoundOrConflict(in err);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ActionResult NotFoundOrConflict(in Models.Error err)
            => err.Code == Codes.E01GameNotFound
            || err.Code == Codes.E02UserNotFound
            || err.Code == Codes.E10InvalidSlotIndex
            || err.Code == Codes.E18InvalidPieceIndex
            ? (ActionResult)NotFound(err)
            : Conflict(err);
    }
}
