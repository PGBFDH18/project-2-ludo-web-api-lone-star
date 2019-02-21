using Ludo.GameService;
using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// Placeholder: Done
// Proper Code: Done?
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/lobby")]
    [ApiController]
    public class LobbyController : LudoControllerBase
    {
        private readonly ILudoService ludoService;
        private readonly Components.IIsKnown isKnown;

        public LobbyController(ILudoService ludoService, Components.IIsKnown isKnown)
        {
            this.ludoService = ludoService;
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
            if (userId?.All(isKnown.UserId) == false)
                return NotFound();
            var result = ListLobbies(eShow, userId);
            return new ActionResult<IEnumerable<LobbyListEntry>>(result);
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
            var err = TryCreateLobby(userId, out string gameId);
            if (err == null)
                return Created(gameId, null);
            if (err.Code == ErrorCodes.Err02UserNotFound)
                return NotFound();
            return Status(500);
        }

        // ===================================================================

        //TODO: refactor out to a dependency injected component
        private ErrorCode TryCreateLobby(string userId, out string gameId)
        {
            int slots = GameLogic.SessionFactory.MaxPlayers; // <-- TODO
            return ludoService.TryCreateLobby(userId, slots, out gameId);
            //TODO
            //gameId = $"placeholder({userId})";
            //return userId != "test"; // just for experimentation
        }

        //TODO: refactor out to a dependency injected component
        private IEnumerable<LobbyListEntry> ListLobbies(Show show, string[] users)//, out IEnumerable<LobbyListEntry> result)
        {
            // a bit of a Linq mess here...
            return ludoService.Games
                .Where(kvp => kvp.Value.Phase.State <= GameService.GameLifecycle.setup
                && kvp.Value.Phase.Setup != null
                && (show == Show.all
                || (show == Show.full && kvp.Value.Phase.Setup.Data.OpenCount == 0)
                || (show == Show.open && kvp.Value.Phase.Setup.Data.OpenCount > 0)
                || (show == Show.penultimate && kvp.Value.Phase.Setup.Data.OpenCount == 1))
                && (users == null || users.Length == 0 || kvp.Value.Phase.Setup.Data.Any(u => users.Contains(u))))
                .Select(kvp => new LobbyListEntry
                {
                    GameId = kvp.Key.Encoded,
                    Slots = kvp.Value.Phase.Setup.Data.ToArray(),
                    Others = kvp.Value.Phase.Setup.Data.Others,
                    Access = LobbyAccess.@public // TODO: LobbyAccess
                });
            //TODO
            //result = new LobbyListEntry[] {
            //    new LobbyListEntry {
            //        GameId = show.ToString(),
            //        Access = ModelState.IsValid ? LobbyAccess.@public : LobbyAccess.unlisted,
            //        Slots = new string[] {
            //            "placeholder",
            //            users?.Length >= 1 ? users[0] : null, // for experimentation
            //            users?.Length >= 2 ? users[1] : null,
            //            users?.Length >= 3 ? users[1] : null,
            //}   }   };
            //return !users.Contains("test");
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