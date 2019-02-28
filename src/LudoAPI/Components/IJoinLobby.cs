using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IJoinLobby
    {
        ErrorCode TryJoinLobby(string gameId, string userId, out int slot);
    }
}