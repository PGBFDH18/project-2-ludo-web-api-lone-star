using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IJoinLobby
    {
        Error TryJoinLobby(string gameId, string userId, out int slot);
    }
}