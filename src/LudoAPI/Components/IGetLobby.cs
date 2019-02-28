using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IGetLobby
    {
        bool TryGetLobby(string gameId, out LobbyInfo lobbyInfo);
    }
}