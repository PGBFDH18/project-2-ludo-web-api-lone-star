using Ludo.API.Models;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/board/" + ROUTE_gameId)]
    [ApiController]
    public class BoardGameController : LudoControllerBase
    {
        private readonly IBoardState boardState;

        public BoardGameController(IBoardState boardState) {
            this.boardState = boardState;
        }

        // operationId: ludoGetBoardState
        [ProducesResponseType(201, Type = typeof(BoardState))]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpGet] public ActionResult<BoardState> Get ([FromRoute]string gameId)
        {
            var err = boardState.TryGetBoardState(gameId, out Models.BoardState bstate);
            if (!err)
                return bstate;
            if (err == Error.Codes.E01GameNotFound)
                return NotFound();
            return Conflict();
        }

        // operationId: ludoSetBoardState
        // TODO!
    }
}