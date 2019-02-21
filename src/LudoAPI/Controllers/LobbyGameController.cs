using Ludo.GameService;
using Ludo.WebAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

// Placeholder: TODO
// Proper Code: TODO
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/lobby/" + ROUTE_gameId)]
    [ApiController]
    public class LobbyGameController : LudoControllerBase
    {
        private readonly ILudoService ludoService;
        private readonly Components.IIsKnown isKnown;
        public LobbyGameController(ILudoService ludoService, Components.IIsKnown isKnown)
        {
            this.ludoService = ludoService;
            this.isKnown = isKnown;
        }

        // operationId: ludoGetLobby
        // 200 response: Done
        // 404 response: Done
        [HttpGet] public ActionResult<LobbyInfo> Get (
            [FromRoute]string gameId)
        {
            if (TryGetLobby(gameId, out LobbyInfo lobbyInfo))
                return lobbyInfo;
            return NotFound();
        }

        // operationId: ludoJoinLobby
        // 200 response: DoneD:\Source\LudoLoneStar\src\LudoAPI\Controllers\LobbyGameController.cs
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpPatch] public ActionResult<int> Patch (
            [FromRoute]string gameId, [FromHeader]string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            var err = TryJoinLobby(gameId, userId, out int slot);
            if (err == null)
                return slot;
            if (err.Code == ErrorCodes.Err1GameNotFound || err.Code == ErrorCodes.Err2UserNotFound)
                return NotFound();
            return Conflict();
            //if (!IsKnownGameId(gameId) || !IsKnownUserId(userId))
            //    return NotFound(); //TODO: add ErrorCode
            //return Conflict(); //TODO: add ErrorCode
        }

        // operationId: ludoStartGame
        // 201 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpPost] public IActionResult Post([FromRoute]string gameId)
        {
            if (TryStartGame(gameId))
                return Created($"../game/{gameId}", null);
            if (!isKnown.GameId(gameId))
                return NotFound();
            else
                return Conflict(); //TODO: ErrorCode
        }

        // operationId: ludoLeaveLobby
        // 204 response: Done
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpDelete] public IActionResult Delete (
            [FromRoute]string gameId, [FromHeader]string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            if (!TryLeaveLobby(gameId, userId) && !(isKnown.GameId(gameId) && isKnown.UserId(userId)))
                return NotFound(); //TODO: ErrorCode
            return NoContent();
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
            if (!uint.TryParse(slotStr, out uint slot) || slot > 3u)
                return BadRequest();
            if (TryGetPlayerReady(gameId, unchecked((int)slot), out PlayerReady playerReady))
                return playerReady;
            if (!TryGetInfo(gameId, out int slotCount, out Models.GameState state))
                return NotFound(); // ^implied !IsKnownGameId
            if (unchecked((int)slot) >= slotCount)
                return BadRequest();
            if (state != Models.GameState.setup)
                return Conflict();
            return Status(500);
        }

        // operationId: ludoSetPlayerReady
        // 204 response: Done
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpPut(ROUTE_slotStr)] public IActionResult Put (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromBody]PlayerReady playerReady)
        {
            if (!int.TryParse(slotStr, out int slot))
                return BadRequest();
            if (slot < -1 | slot > 3)
                return NotFound(); //TODO: ErrorCode
            if (Try())
                return NoContent();
            if (!isKnown.UserId(playerReady.UserId) ||
                !TryGetInfo(gameId, out int slotCount, out _) || // !TryGetInfo implies !IsKnownGameId
                slot >= slotCount)
                return NotFound(); //TODO: ErrorCode
            return Conflict(); //TODO: ErrorCode

            bool Try()
            => slot == -1
                ? TryUnSetPlayer(gameId, playerReady.UserId)
                : TrySetPlayer(gameId, slot, playerReady);
        }

        // ===================================================================

        //TODO: refactor out to a dependency injected component
        private bool TrySetPlayer(string gameId, int slot, PlayerReady pr)
        {
            //TODO
            return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private bool TryUnSetPlayer(string gameId, string player)
        {
            //TODO
            return player != "test"; // just for experimentation
        }
        
        //TODO: refactor out to a dependency injected component
        private bool TryGetInfo(string gameId, out int slotCount, out Models.GameState state)
        {
            //TODO
            slotCount = 4;
            state = (Models.GameState)(gameId.Length % 3);
            return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private bool TryGetLobby(string gameId, out LobbyInfo lobbyInfo)
        {
            //TODO
            lobbyInfo = new LobbyInfo {
                Access = LobbyAccess.@public,
                State = Models.GameState.setup,
                Slots = new PlayerReady[] {
                    new PlayerReady{
                        UserId = $"placeholder1 ({gameId})",
                        Ready = true,
                    },
                    new PlayerReady{
                        UserId = $"placeholder2 ({gameId})",
                        Ready = false,
                    },
                },
                Reservations = new LobbyReservation[] {
                    new LobbyReservation { Player = "olle", Slot = 0, Strict = false },
                    new LobbyReservation { Player = "pelle", Slot = 0, Strict = false },
                },
            };
            return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private ErrorCode TryJoinLobby(string gameId, string userId, out int slot)
        {
            slot = -1;
            if (!ludoService.Users.ContainsId(Id.Partial(userId)))
                return new ErrorCode { Code = ErrorCodes.Err2UserNotFound };
            var game = ludoService.Games.FirstOrDefault((kvp) => kvp.Key.Encoded == gameId).Value;
            if (game == null)
                return new ErrorCode { Code = ErrorCodes.Err1GameNotFound };
            return game.TryAddUser(userId, out slot);
            //TODO
            //slot = 0;
            //return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private bool TryStartGame(string gameId)
        {
            //TODO
            return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private bool TryLeaveLobby(string gameId, string player)
        {
            //TODO
            return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private bool TryGetPlayerReady(string gameId, int slot, out PlayerReady playerReady)
        {
            //TODO
            playerReady = new PlayerReady { UserId = "Placeholder", Ready = slot % 2 == 0 };
            return gameId != "test"; // just for experimentation
        }
    }
}
