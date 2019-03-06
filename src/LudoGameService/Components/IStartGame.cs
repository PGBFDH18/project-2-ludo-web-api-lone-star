using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IStartGame
    {
        Error TryStartGame(string gameId);
    }
}