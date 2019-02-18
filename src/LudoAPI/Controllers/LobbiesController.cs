using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// Placeholder: Done
// Proper Code: TODO
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/[controller]")]
    [ApiController]
    public class LobbiesController : LudoControllerBase
    {
        // operationId: ludoListLobbies
        // 200 response: TODO
        // 400 response: Done
        // 404 response: Done
        [HttpGet] public ActionResult<IEnumerable<LobbyListEntry>> Get (
            [FromQuery]string show, [FromQuery]string[] player)
        {
            Show eShow = 0;
            if (!(string.IsNullOrEmpty(show) || Enum.TryParse(show, true, out eShow)))
                return Status(400);
            if (player?.All(IsValidPlayerId) == false)
                return Status(404);
            
            return Placeholder(); //TODO
            
            ActionResult<IEnumerable<LobbyListEntry>> Placeholder()
            => new LobbyListEntry[] {
                new LobbyListEntry {
                    GameId = eShow.ToString(),
                    Access = ModelState.IsValid ? LobbyAccess.@public : LobbyAccess.unlisted,
                    Slots = new string[] {
                        "placeholder",
                        player?.Length >= 1 ? player[0] : null, // for experimentation
                        player?.Length >= 2 ? player[1] : null,
                        player?.Length >= 3 ? player[1] : null,
            }   }   };
        }

        // operationId: ludoCreateLobby
        // 201 response: Done
        // 400 response: Done
        // 404 response: Done
        [HttpPost] public ActionResult<string> Post (
            [FromHeader]string player)
        {
            if (TryCreateLobby(player, out string gameId))
                return Status(201, gameId);
            // else:
            if (string.IsNullOrEmpty(player))
                return Status(400);
            if (!IsValidPlayerId(player))
                return Status(404);
            return Status(500);
        }

        // ===================================================================

        //TODO: refactor out to a dependency injected component
        private bool TryCreateLobby(string player, out string gameId)
        {
            //TODO
            gameId = $"placeholder ({player})";
            return player != "test"; // just for experimentation
        }

        public enum Show
        {
            open = 0, //default
            full,
            all,
            penultimate,
        }
    }
}