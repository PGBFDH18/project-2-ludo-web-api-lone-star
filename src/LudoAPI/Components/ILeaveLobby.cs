using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface ILeaveLobby
    {
        ErrorCode TryLeaveLobby(string userId, string gameId);
    }
}