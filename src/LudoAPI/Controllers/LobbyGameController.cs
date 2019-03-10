﻿using Ludo.API.Models;
using Ludo.API.Service;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/lobby/" + ROUTE_gameId)]
    [ApiController]
    public class LobbyGameController : LudoControllerBase
    {
        private readonly ILudoService ludoService;
        private readonly IIsKnown isKnown;
        private readonly IGetLobby getLobby;
        private readonly IJoinLobby joinLobby;
        private readonly IStartGame startGame;
        private readonly ILeaveLobby leaveLobby;
        private readonly IGetPlayerReady getPlayerReady;
        private readonly ISlotUser slotUser;

        public LobbyGameController(
            ILudoService ludoService,
            IIsKnown isKnown,
            IGetLobby getLobby,
            IJoinLobby joinLobby,
            IStartGame startGame,
            ILeaveLobby leaveLobby,
            IGetPlayerReady getPlayerReady,
            ISlotUser slotUser)
        {
            this.ludoService = ludoService;
            this.isKnown = isKnown;
            this.getLobby = getLobby;
            this.joinLobby = joinLobby;
            this.startGame = startGame;
            this.leaveLobby = leaveLobby;
            this.getPlayerReady = getPlayerReady;
            this.slotUser = slotUser;
        }

        // operationId: ludoGetLobby
        [ProducesResponseType(200, Type = typeof(LobbyInfo))]
        [ProducesResponseType(404)]
        [HttpGet] public ActionResult<LobbyInfo> Get (
            [FromRoute]string gameId)
        {
            if (getLobby.TryGetLobby(gameId, out LobbyInfo lobbyInfo))
                return lobbyInfo;
            return NotFound();
        }

        // operationId: ludoJoinLobby
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpPatch] public ActionResult<int> Patch (
            [FromRoute]string gameId, [FromHeader]string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            var err = joinLobby.TryJoinLobby(gameId, userId, out int slot);
            if (err == Error.Codes.E00NoError)
                return slot;
            if (err == Error.Codes.E01GameNotFound || err == Error.Codes.E02UserNotFound)
                return NotFound(err);
            else
                return Conflict(err);
        }

        // operationId: ludoStartGame
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpPost] public IActionResult Post([FromRoute]string gameId)
        {
            var err = startGame.TryStartGame(gameId);
            if (err == Error.Codes.E00NoError)
                return Created($"../game/{gameId}", null);
            if (err == Error.Codes.E01GameNotFound)
                return NotFound();
            return Conflict(err);
        }

        // operationId: ludoLeaveLobby
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpDelete] public IActionResult Delete (
            [FromRoute]string gameId, [FromHeader]string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            var err = leaveLobby.TryLeaveLobby(userId: userId, gameId: gameId);
            if (err == Error.Codes.E00NoError)
                return NoContent();
            if (err == Error.Codes.E01GameNotFound || err == Error.Codes.E02UserNotFound)
                return NotFound(err);
            else
                return Conflict(err); // Not in setup phase //TODO: change this so it calls a concede?
        }

        // -------------------------------------------------------------------

        // operationId: ludoGetPlayerReady
        [ProducesResponseType(200, Type = typeof(PlayerReady))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409)]
        [HttpGet(ROUTE_slotStr)] public ActionResult<PlayerReady> Get (
            [FromRoute]string gameId, [FromRoute]string slotStr)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = getPlayerReady.TryGetPlayerReady(gameId, slot, out PlayerReady playerReady);
            if (err == Error.Codes.E00NoError)
                return playerReady;
            if (err == Error.Codes.E01GameNotFound || err == Error.Codes.E10InvalidSlotIndex)
                return NotFound(err);
            return Conflict();
        }

        // operationId: ludoSetSlotPlayer
        [HttpPut(ROUTE_slotStr)] public IActionResult Put (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromHeader]string userId)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = slotUser.TryClaimSlot(gameId, slot, userId);
            if (err == Error.Codes.E00NoError)
                return NoContent();
            if (err == Error.Codes.E01GameNotFound || err == Error.Codes.E02UserNotFound || err == Error.Codes.E10InvalidSlotIndex)
                return NotFound(err);
            return Conflict(err);
        }

        // operationId: ludoSetSlotReady
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(Error))]
        [ProducesResponseType(409, Type = typeof(Error))]
        [HttpPatch(ROUTE_slotStr)] public IActionResult Patch (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromBody]PlayerReady playerReady)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = slotUser.TrySetSlotReady(gameId, slot, playerReady);
            if (err == Error.Codes.E00NoError)
                return NoContent();
            if (err == Error.Codes.E01GameNotFound || err == Error.Codes.E02UserNotFound || err == Error.Codes.E10InvalidSlotIndex)
                return NotFound(err);
            return Conflict(err);
        }
    }
}
