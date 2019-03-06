using Ludo.API.Models;
using System.Collections.Generic;

namespace Ludo.API.Service.Components
{
    public interface IListGames
    {
        // returns null if no such game exists.
        GameListEntry TryGetGame(string gameId);

        // (never returns null)
        IEnumerable<GameListEntry> ListGames(ShowGame show);

        // returns null if no such user exists.
        IEnumerable<GameListEntry> TryListGames(ShowGame show, string userId);
    }
}