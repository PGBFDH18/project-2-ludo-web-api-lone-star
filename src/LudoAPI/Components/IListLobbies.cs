using System.Collections.Generic;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IListLobbies
    {
        IEnumerable<LobbyListEntry> ListLobbies(ShowLobby show, string[] users);
    }
}