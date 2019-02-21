using Ludo.GameService;
using Ludo.WebAPI.Components;
using Microsoft.AspNetCore.Mvc;

// Placeholder: Done
// Proper Code: Done?
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/board/" + ROUTE_gameId)]
    [ApiController]
    public class BoardGameController : LudoControllerBase
    {
        private readonly Components.IBoardState boardState;

        public BoardGameController(IBoardState boardState)
        {
            this.boardState = boardState;
        }

        // operationId: ludoGetBoardState
        // 201 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpGet] public ActionResult<Models.BoardState> Get ([FromRoute]string gameId)
        {
            var err = boardState.TryGetBoardState(gameId, out Models.BoardState bstate);
            if (err == null)
                return bstate;
            if (err.Code == ErrorCodes.Err1GameNotFound)
                return NotFound();
            return Conflict();
            //if (IsKnownGameId(gameId))
            //    return Conflict();
            //return NotFound();
        }

        //TODO: refactor out to a dependency injected component
        
    }
}