using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IGetLobby
    {
        bool TryGetLobby(string gameId, out LobbyInfo lobbyInfo);
    }
}