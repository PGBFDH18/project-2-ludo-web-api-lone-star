using Ludo.GameService;
using Ludo.WebAPI.Components;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/board/" + ROUTE_gameId)]
    [ApiController]
    public class BoardGameController : LudoControllerBase
    {
        private readonly Components.IBoardState boardState;

        public BoardGameController(IBoardState boardState) {
            this.boardState = boardState;
        }

        // operationId: ludoGetBoardState
        // 201 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpGet] public ActionResult<Models.BoardState> Get ([FromRoute]string gameId)
        {
            var err = boardState.TryGetBoardState(gameId, out Models.BoardState bstate);
            if (err == ErrorCodes.E00NoError)
                return bstate;
            if (err == ErrorCodes.E01GameNotFound)
                return NotFound();
            return Conflict();
        }
    }
}