using Ludo.GameService;
using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/lobby")]
    [ApiController]
    public partial class LobbyController : LudoControllerBase
    {
        private readonly Components.IListLobbies listLobby;
        private readonly Components.ICreateLobby createLobby;
        private readonly Components.IIsKnown isKnown;

        public LobbyController(
            Components.IListLobbies listLobby,
            Components.ICreateLobby createLobby,
            Components.IIsKnown isKnown)
        {
            this.listLobby = listLobby;
            this.createLobby = createLobby;
            this.isKnown = isKnown;
        }

        // operationId: ludoListLobbies
        // 200 response: Done
        // 400 response: Done
        // 404 response: Done
        [HttpGet] public ActionResult<IEnumerable<LobbyListEntry>> Get (
            [FromQuery(Name = "show")]string showStr, [FromQuery]string[] userId)
        {
            ShowLobby show;
            if (showStr == null)
                show = ShowLobby.open;
            else if (!Enum.TryParse(showStr, true, out show))
                return BadRequest();
            if (userId?.All(isKnown.UserId) == false)
                return NotFound();
            var result = listLobby.ListLobbies(show, userId);
            return new ActionResult<IEnumerable<LobbyListEntry>>(result);
        }

        // operationId: ludoCreateLobby
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [HttpPost] public IActionResult Post ([FromBody]CreateLobby lobby)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            // TODO/FIXME: reservations
            var err = createLobby.TryCreateLobby(lobby.UserId, lobby.Slots, lobby.Access, out string gameId);
            if (err == ErrorCodes.E00NoError)
                return Created(gameId, null);
            if (err == ErrorCodes.E02UserNotFound)
                return NotFound();
            if (err == ErrorCodes.E05InvalidSlotCount)
                return UnprocessableEntity();
            return Status(500); // (should never happen)
        }
    }
}