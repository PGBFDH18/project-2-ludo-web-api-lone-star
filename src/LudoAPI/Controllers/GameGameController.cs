using Ludo.API.Models;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/game/" + ROUTE_gameId)]
    [ApiController]
    public class GameGameController : LudoControllerBase
    {
        private readonly IGetCurrent getCurrent;
        private readonly IGetTurnInfo getTurnInfo;
        private readonly IPassTurn passTurn;
        private readonly IConcede concede;

        public GameGameController(
            IGetCurrent getCurrent,
            IGetTurnInfo getTurnInfo,
            IPassTurn passTurn,
            IConcede concede) {
            this.getCurrent = getCurrent;
            this.getTurnInfo = getTurnInfo;
            this.passTurn = passTurn;
            this.concede = concede;
        }

        // operationId: ludoGetCurrent
        [ProducesResponseType(200, Type = typeof(TurnSlotDie))]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [HttpGet] public ActionResult<TurnSlotDie> Get ([FromRoute]string gameId)
        {
            var err = getCurrent.GetCurrent(gameId, out TurnSlotDie turnSlotDie);
            if (err == 0)
                return turnSlotDie;
            if (err == Error.Codes.E01GameNotFound)
                return NotFound();
            return Conflict();
        }

        // operationId: ludoConcede
        [ProducesResponseType(204)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409)]
        [HttpDelete] public IActionResult Delete(
            [FromRoute]string gameId, [FromHeader]string userId)
        {
            var err = concede.Concede(gameId, userId);
            if (!err)
                return NoContent();
            if (err == Error.Codes.E07NotInGamePhase)
                return Conflict();
            return NotFound(err);
        }

        // -------------------------------------------------------------------

        // operationId: ludoGetTurnInfo
        [ProducesResponseType(200, Type = typeof(TurnInfo))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpGet(ROUTE_slotStr)] public ActionResult<TurnInfo> Get (
            [FromRoute]string gameId, [FromRoute]string slotStr)
        {
            if (!TryParse(slotStr, out int slot))
                return BadRequest();
            var err = getTurnInfo.GetTurnInfo(gameId, slot, out TurnInfo turnInfo);
            return OkOrNotFoundOrConflict(turnInfo, err);
        }

        // operationId: ludoPassTurn
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpPost(ROUTE_slotStr)] public IActionResult Post (
            [FromRoute]string gameId, [FromRoute]string slotStr)
        {
            if (!TryParse(slotStr, out int slot))
                return BadRequest();
            var err = passTurn.PassTurn(gameId, slot);
            return NoContentOrNotFoundOrConflict(err);
        }
    }
}