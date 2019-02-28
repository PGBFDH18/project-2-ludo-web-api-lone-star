using Ludo.GameService;
using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

// Placeholder: Done
// Proper Code: TODO
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/lobby/" + ROUTE_gameId)]
    [ApiController]
    public class LobbyGameController : LudoControllerBase
    {
        private readonly ILudoService ludoService;
        private readonly Components.IIsKnown isKnown;
        private readonly Components.IGetLobby getLobby;
        private readonly Components.IJoinLobby joinLobby;
        private readonly Components.IStartGame startGame;
        private readonly Components.ILeaveLobby leaveLobby;
        private readonly Components.IGetPlayerReady getPlayerReady;
        private readonly Components.ISlotUser slotUser;

        public LobbyGameController(
            ILudoService ludoService,
            Components.IIsKnown isKnown,
            Components.IGetLobby getLobby,
            Components.IJoinLobby joinLobby,
            Components.IStartGame startGame,
            Components.ILeaveLobby leaveLobby,
            Components.IGetPlayerReady getPlayerReady,
            Components.ISlotUser slotUser)
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
        // 200 response: Done
        // 404 response: Done
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
        [ProducesResponseType(404, Type = typeof(ErrorCode))]
        [ProducesResponseType(409, Type = typeof(ErrorCode))]
        [HttpPatch] public ActionResult<int> Patch (
            [FromRoute]string gameId, [FromHeader]string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            var err = joinLobby.TryJoinLobby(gameId, userId, out int slot);
            if (err == ErrorCodes.E00NoError)
                return slot;
            if (err == ErrorCodes.E01GameNotFound || err == ErrorCodes.E02UserNotFound)
                return NotFound(err);
            else
                return Conflict(err);
        }

        // operationId: ludoStartGame
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409, Type = typeof(ErrorCode))]
        [HttpPost] public IActionResult Post([FromRoute]string gameId)
        {
            var err = startGame.TryStartGame(gameId);
            if (err == ErrorCodes.E00NoError)
                return Created($"../game/{gameId}", null);
            if (err == ErrorCodes.E01GameNotFound)
                return NotFound();
            return Conflict(err);
        }

        // operationId: ludoLeaveLobby
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(ErrorCode))]
        [ProducesResponseType(409, Type = typeof(ErrorCode))]
        [HttpDelete] public IActionResult Delete (
            [FromRoute]string gameId, [FromHeader]string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            var err = leaveLobby.TryLeaveLobby(userId: userId, gameId: gameId);
            if (err == ErrorCodes.E00NoError)
                return NoContent();
            if (err == ErrorCodes.E01GameNotFound || err == ErrorCodes.E02UserNotFound)
                return NotFound(err);
            else
                return Conflict(err); // Not in setup phase //TODO: change this so it calls a concede?
        }

        // -------------------------------------------------------------------

        // operationId: ludoGetPlayerReady
        // 200 response: Done
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpGet(ROUTE_slotStr)] public ActionResult<PlayerReady> Get (
            [FromRoute]string gameId, [FromRoute]string slotStr)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = getPlayerReady.TryGetPlayerReady(gameId, slot, out PlayerReady playerReady);
            if (err == ErrorCodes.E00NoError)
                return playerReady;
            if (err == ErrorCodes.E01GameNotFound || err == ErrorCodes.E10InvalidSlotIndex)
                return NotFound(err);
            return Conflict();
        }

        // operationId: ludoSetSlotPlayer
        [HttpGet(ROUTE_slotStr)] public IActionResult Post (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromHeader]string userId)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = slotUser.TryClaimSlot(gameId, slot, userId);
            if (err == ErrorCodes.E00NoError)
                return NoContent();
            if (err == ErrorCodes.E01GameNotFound || err == ErrorCodes.E02UserNotFound || err == ErrorCodes.E10InvalidSlotIndex)
                return NotFound(err);
            return Conflict(err);
        }

        // operationId: ludoSetSlotReady
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404, Type = typeof(ErrorCode))]
        [ProducesResponseType(409, Type = typeof(ErrorCode))]
        [HttpPut(ROUTE_slotStr)] public IActionResult Put (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromBody]PlayerReady playerReady)
        {
            if (!TryParseSlot(slotStr, out int slot))
                return BadRequest();
            var err = slotUser.TrySetSlotReady(gameId, slot, playerReady);
            if (err == ErrorCodes.E00NoError)
                return NoContent();
            if (err == ErrorCodes.E01GameNotFound || err == ErrorCodes.E02UserNotFound || err == ErrorCodes.E10InvalidSlotIndex)
                return NotFound(err);
            return Conflict(err);
        }
    }
}
