using Ludo.API.Models;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/lobby")]
    [ApiController]
    public partial class LobbyController : LudoControllerBase
    {
        private readonly IListLobbies listLobby;
        private readonly ICreateLobby createLobby;
        private readonly IIsKnown isKnown;

        public LobbyController(
            IListLobbies listLobby,
            ICreateLobby createLobby,
            IIsKnown isKnown)
        {
            this.listLobby = listLobby;
            this.createLobby = createLobby;
            this.isKnown = isKnown;
        }

        // operationId: ludoListLobbies
        [ProducesResponseType(200, Type = typeof(LobbyListEntry))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
            if (!err)
                return Created(gameId, null);
            if (err == Error.Codes.E02UserNotFound)
                return NotFound(err);
            if (err == Error.Codes.E05InvalidSlotCount)
                return UnprocessableEntity(err);
            return Status(500); // (should never happen)
        }

        // operationId: ludoCreateLobby
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [HttpPost]
        public IActionResult Post([FromBody]LoadLobby lobby)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // TODO/FIXME: loading a saved game
            throw new NotImplementedException();
        }
    }
}