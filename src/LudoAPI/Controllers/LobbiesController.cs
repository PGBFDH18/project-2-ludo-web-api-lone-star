using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LudoWebAPI.Controllers
{
    [Route("ludo/[controller]")]
    [ApiController]
    public class LobbiesController : ControllerBase
    {
        //TODO; just testing stuff...
        [HttpGet]
        public ActionResult<IEnumerable<LobbyListEntry>> Get([FromQuery]Show show, [FromQuery]string[] playerId)
        {
            //TODO
            return Placeholder();

            ActionResult<IEnumerable<LobbyListEntry>> Placeholder()
                => new LobbyListEntry[] {
                    new LobbyListEntry {
                        GameId = show.ToString(),
                        Access = ModelState.IsValid ? LobbyAccess.@public : LobbyAccess.unlisted,
                        Slots = new LobbySlot[] {
                            new LobbySlot { Occupant = playerId?.Length >= 1 ? playerId[0] : null },
                            new LobbySlot { Occupant = playerId?.Length >= 2 ? playerId[1] : null },
                }   }   };
        }

        [HttpPost]
        public ActionResult<string> Post([FromHeader]string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return null;
            }
            else if (!IsValidPlayerId(playerId))
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return null;
            }
            else
            {
                Response.StatusCode = StatusCodes.Status201Created;
                //TODO
                return $"placeholder ({playerId})";
            }
        }

        //TODO: refactor out as a dependency injected component
        private bool IsValidPlayerId(string player)
        {
            //TODO
            return true;
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