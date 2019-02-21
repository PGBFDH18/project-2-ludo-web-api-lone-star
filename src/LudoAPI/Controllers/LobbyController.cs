using Ludo.GameService;
using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// Placeholder: Done
// Proper Code: TODO
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/lobby")]
    [ApiController]
    public class LobbyController : LudoControllerBase
    {
        private readonly Components.IIsKnown isKnown;

        public LobbyController(ILudoService ludoService, Components.IIsKnown isKnown)
        {
            this.isKnown = isKnown;
        }

        // operationId: ludoListLobbies
        // 200 response: Done
        // 400 response: Done
        // 404 response: Done
        [HttpGet] public ActionResult<IEnumerable<LobbyListEntry>> Get (
            [FromQuery]string show, [FromQuery]string[] userId)
        {
            Show eShow = 0;
            if (!(string.IsNullOrEmpty(show) || Enum.TryParse(show, true, out eShow)))
                return BadRequest();
            if (TryListLobbies(eShow, userId, out var result))
                return new ActionResult<IEnumerable<LobbyListEntry>>(result);
            if (userId?.All(isKnown.UserId) == false)
                return NotFound();
            return Status(500);
        }

        // operationId: ludoCreateLobby
        // 201 response: Done
        // 400 response: Done
        // 404 response: Done
        [HttpPost] public IActionResult Post (
            [FromHeader]string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            if (TryCreateLobby(userId, out string gameId))
                return Created(gameId, null);
            if (!isKnown.UserId(userId))
                return NotFound();
            return Status(500);
        }

        // ===================================================================

        //TODO: refactor out to a dependency injected component
        private bool TryCreateLobby(string userId, out string gameId)
        {
            //TODO
            gameId = $"placeholder({userId})";
            return userId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private bool TryListLobbies(Show show, string[] users, out IEnumerable<LobbyListEntry> result)
        {
            //TODO
            result = new LobbyListEntry[] {
                new LobbyListEntry {
                    GameId = show.ToString(),
                    Access = ModelState.IsValid ? LobbyAccess.@public : LobbyAccess.unlisted,
                    Slots = new string[] {
                        "placeholder",
                        users?.Length >= 1 ? users[0] : null, // for experimentation
                        users?.Length >= 2 ? users[1] : null,
                        users?.Length >= 3 ? users[1] : null,
            }   }   };
            return !users.Contains("test");
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