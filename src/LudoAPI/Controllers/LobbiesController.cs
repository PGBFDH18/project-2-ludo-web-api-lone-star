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
        public ActionResult<IEnumerable<LobbyListEntry>> Get([FromQuery]Show show)
        {
            return new LobbyListEntry[] {
                new LobbyListEntry {
                    GameId = show.ToString(),
                    Access = LobbyAccess.@public,
                    Slots = new LobbySlot[] {
                        new LobbySlot(),
                        new LobbySlot(),
            }   }   };
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