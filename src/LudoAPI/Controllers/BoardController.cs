using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

// Placeholder: Done
// Proper Code: Done
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/board")]
    [ApiController]
    public class BoardController : LudoControllerBase
    {
        // operationId: ludoGetBoardInfo
        // 200 response: Done
        // 400 response: Done
        [HttpGet] public ActionResult<BoardInfo> Get (
            [FromQuery]int length)
        {
            if (TryGetBoardInfo(length, out BoardInfo bi))
                return bi;
            return BadRequest();
        }

        //TODO: refactor out to a dependency injected component
        private bool TryGetBoardInfo(int length, out Models.BoardInfo info)
        {
            if (GameLogic.BoardInfo.IsValidLength(length))
            {
                info = new GameLogic.BoardInfo(length); // implicit-cast
                return true;
            }
            else
            {
                info = default;
                return false;
            }
        }
    }
}