using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface ILeaveLobby
    {
        Error TryLeaveLobby(string userId, string gameId);
    }
}