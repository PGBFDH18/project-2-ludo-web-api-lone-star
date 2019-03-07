using System;
using System.Collections.Generic;
using Ludo.API.Service;
using Ludo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ludo.API.Service.Components;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/game/" + ROUTE_gameId)]
    [ApiController]
    public class GameGameController : LudoControllerBase
    {
        private readonly IGetCurrent getCurrent;
        private readonly IGetTurnInfo getTurnInfo;
        private readonly IPassTurn passTurn;

        public GameGameController(
            IGetCurrent getCurrent,
            IGetTurnInfo getTurnInfo,
            IPassTurn passTurn) {
            this.getCurrent = getCurrent;
            this.getTurnInfo = getTurnInfo;
            this.passTurn = passTurn;
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

        // -------------------------------------------------------------------

        // operationId: ludoGetTurnInfo
        [ProducesResponseType(200, Type = typeof(TurnInfo))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpGet(ROUTE_slotStr)] public ActionResult<TurnInfo> Get (
            [FromRoute]string gameId, [FromRoute]string slotStr)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = getTurnInfo.GetTurnInfo(gameId, slot, out TurnInfo turnInfo);
            if (err == Error.Codes.E00NoError)
                return turnInfo;
            if (err == Error.Codes.E01GameNotFound || err == Error.Codes.E05InvalidSlotCount)
                return NotFound(err);
            return Conflict(err);
        }

        // operationId: ludoPassTurn
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpPost(ROUTE_slotStr)] public IActionResult Post (
            [FromRoute]string gameId, [FromRoute]string slotStr)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = passTurn.PassTurn(gameId, slot);
            if (err == Error.Codes.E00NoError)
                return NoContent();
            if (err == Error.Codes.E01GameNotFound || err == Error.Codes.E10InvalidSlotIndex)
                return NotFound(err);
            return Conflict(err);
        }

        //TODO:  delete
    }
}