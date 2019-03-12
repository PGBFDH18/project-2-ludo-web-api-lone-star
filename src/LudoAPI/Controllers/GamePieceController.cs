using Ludo.API.Models;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/game/" + ROUTE_gameId + "/" + ROUTE_slotStr + "/" + ROUTE_pieceIndex)]
    [ApiController]
    public class GamePieceController : LudoControllerBase
    {
        private readonly IGetPieceInfo getPieceInfo;
        private readonly IMovePiece movePiece;

        public GamePieceController(
            IGetPieceInfo getPieceInfo,
            IMovePiece movePiece)
        {
            this.getPieceInfo = getPieceInfo;
            this.movePiece = movePiece;
        }

        // operationId: ludoGetPieceInfo
        [ProducesResponseType(200, Type = typeof(PieceInfo))]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpGet] public ActionResult<PieceInfo> Get (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromRoute]int pieceIndex)
        {
            if (!TryParse(slotStr, out int slot))
                return BadRequest();
            var err = getPieceInfo.GetPieceInfo(gameId, slot, pieceIndex, out var pieceInfo);
            return OkOrNotFoundOrConflict(pieceInfo, err);
        }

        // operationId: ludoMovePiece
        [ProducesResponseType(204)]
        [HttpPost] public IActionResult Post (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromRoute]int pieceIndex)
        {
            if (!TryParse(slotStr, out int slot))
                return BadRequest();
            var err = movePiece.MovePiece(gameId, slot, pieceIndex);
            return NoContentOrNotFoundOrConflict(err);
        }
    }
}