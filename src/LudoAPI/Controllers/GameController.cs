using Ludo.API.Models;
using Ludo.API.Service.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/game")]
    [ApiController]
    public class GameController : LudoControllerBase
    {
        private readonly IListGames listGames;

        public GameController(IListGames listGames) {
            this.listGames = listGames;
        }

        // operationId: ludoListGames
        [HttpGet] public ActionResult<IEnumerable<GameListEntry>> Get (
            [FromQuery(Name = "show")]string showStr, [FromQuery]string gameId, [FromQuery]string userId)
        {
            ShowGame show;
            if (showStr == null)
                show = ShowGame.ingame;
            else if (!Enum.TryParse(showStr, true, out show))
                return BadRequest();
            if (!string.IsNullOrEmpty(gameId))
            {
                var game = listGames.TryGetGame(gameId);
                if (game == null)
                    return NotFound();
                return new ActionResult<IEnumerable<GameListEntry>>(game);
            }
            if (!string.IsNullOrEmpty(userId))
            {
                var games = listGames.TryListGames(show, userId);
                if (games == null)
                    return NotFound();
                return new ActionResult<IEnumerable<GameListEntry>>(games);
            }
            else
            {
                var games = listGames.ListGames(show);
                return new ActionResult<IEnumerable<GameListEntry>>(games);
            }
        }
    }
}