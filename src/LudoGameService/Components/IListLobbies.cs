using System.Collections.Generic;
using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IListLobbies
    {
        IEnumerable<LobbyListEntry> ListLobbies(ShowLobby show, string[] users);
    }
}