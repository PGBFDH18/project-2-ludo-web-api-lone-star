using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/board")]
    [ApiController]
    public class BoardController : LudoControllerBase
    {
        private readonly Components.IBoardInfo boardInfo;

        public BoardController(Components.IBoardInfo boardInfo) {
            this.boardInfo = boardInfo;
        }

        // operationId: ludoGetBoardInfo
        // 200 response: Done
        // 400 response: Done
        [HttpGet] public ActionResult<BoardInfo> Get ([FromQuery]int length)
        {
            if (boardInfo.TryGetBoardInfo(length, out BoardInfo bi))
                return bi;
            return BadRequest();
        }
    }
}