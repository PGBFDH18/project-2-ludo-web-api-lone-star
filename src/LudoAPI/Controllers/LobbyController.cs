using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

// Placeholder: TODO
// Proper Code: TODO
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/[controller]/{gameId:required:minLength(2)}")]
    [ApiController]
    public class LobbyController : LudoControllerBase
    {
        // operationId: ludoLobbyInfo
        // 200 response: TODO
        // 404 response: Done
        [HttpGet] public ActionResult<LobbyInfo> Get (
            [FromRoute]string gameId)
        {
            if (TryGetLobby(gameId, out LobbyInfo lobbyInfo))
                return lobbyInfo;
            return Status(404);
        }

        // operationId: ludoLobbyAddPlayer
        // 200 response: Done
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpPatch] public ActionResult<int> Patch (
            [FromRoute]string gameId, [FromHeader]string player)
        {
            if (TryJoinLobby(gameId, player, out int slot))
                return slot;
            // else:
            if (string.IsNullOrEmpty(player))
                return Status(400);
            if (!IsValidGameId(gameId) || !IsValidPlayerId(player))
                return Status(404);
            return Status(409);
        }

        // operationId: ludoGameStart
        // 204 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpPost] public void Post (
            [FromRoute]string gameId)
        {
            if (TryStartGame(gameId))
                Status(204);
            else if (!IsValidGameId(gameId))
                Status(404);
            else
                Status(409);
        }

        // operationId: ludoLobbyLeave
        // 204 response: Done
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpDelete] public void Delete (
            [FromRoute]string gameId, [FromHeader]string player)
        {   
            if (TryLeaveLobby(gameId, player))
                Status(204);
            else if (string.IsNullOrEmpty(player))
                Status(400);
            else if (!IsValidGameId(gameId) || !IsValidPlayerId(player))
                Status(404);
            else
                Status(409);
        }

        // -------------------------------------------------------------------

        // operationId: ludoLobbyGetPlayerReady
        // 200 response: Done
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpGet("{slotStr:required}")] public ActionResult<PlayerReady> Get (
            [FromRoute]string gameId, [FromRoute]string slotStr)
        {
            if (!uint.TryParse(slotStr, out uint slot) || slot > 3u)
                return Status(400);
            if (TryGetPlayerReady(gameId, unchecked((int)slot), out PlayerReady playerReady))
                return playerReady;
            if (!TryGetInfo(gameId, out int slotCount, out GameState state))
                return Status(404); // ^implied !IsValidGameId
            if (unchecked((int)slot) >= slotCount)
                return Status(400);
            if (state != GameState.setup)
                return Status(409);
            return Status(500);
        }

        // operationId: ludoLobbySetPlayer
        // 204 response: Done
        // 400 response: Done
        // 404 response: Done
        // 409 response: Done
        [HttpPut("{slotStr:required}")] public void Put (
            [FromRoute]string gameId, [FromRoute]string slotStr, [FromBody]PlayerReady playerReady)
        {
            if (!int.TryParse(slotStr, out int slot) || slot < -1 | slot > 3)
                Status(400);
            else if (TryPerform())
                Status(204);
            else if (!IsValidPlayerId(playerReady.Player) || !TryGetInfo(gameId, out int slotCount, out _))
                Status(404); // ^implied !IsValidGameId
            else if (slot >= slotCount)
                Status(400);
            else
                Status(409);

            bool TryPerform()
            => slot == -1
                ? TryUnSetPlayer(gameId, playerReady.Player)
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
        private bool TryGetInfo(string gameId, out int slotCount, out GameState state)
        {
            //TODO
            slotCount = 4;
            state = (GameState)(gameId.Length % 3);
            return gameId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private bool TryGetLobby(string gameId, out LobbyInfo lobbyInfo)
        {
            //TODO
            lobbyInfo = new LobbyInfo {
                Access = LobbyAccess.@public,
                State = GameState.setup,
                Slots = new PlayerReady[] {
                    new PlayerReady{
                        Player = $"placeholder1 ({gameId})",
                        Ready = true,
                    },
                    new PlayerReady{
                        Player = $"placeholder2 ({gameId})",
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
        private bool TryJoinLobby(string gameId, string player, out int slot)
        {
            //TODO
            slot = 0;
            return gameId != "test"; // just for experimentation
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
            playerReady = new PlayerReady { Player = "Placeholder", Ready = slot % 2 == 0 };
            return gameId != "test"; // just for experimentation
        }
    }
}
