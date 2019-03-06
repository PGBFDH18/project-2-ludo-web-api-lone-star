using Ludo.API.Models;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/board")]
    [ApiController]
    public class BoardController : LudoControllerBase
    {
        private readonly IBoardInfo boardInfo;

        public BoardController(IBoardInfo boardInfo) {
            this.boardInfo = boardInfo;
        }

        // operationId: ludoGetBoardInfo
        [ProducesResponseType(200, Type = typeof(BoardInfo))]
        [ProducesResponseType(400)]
        [HttpGet] public ActionResult<BoardInfo> Get ([FromQuery]int length)
        {
            if (boardInfo.TryGetBoardInfo(length, out BoardInfo bi))
                return bi;
            return BadRequest();
        }
    }
}